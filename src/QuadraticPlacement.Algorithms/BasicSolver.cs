using QuadraticPlacement.Core;

namespace QuadraticPlacement.Algorithms;

/// <summary>
/// Базовый алгоритм размещения через решение системы линейных уравнений
/// с использованием матрицы Лапласа
/// </summary>
public class BasicSolver : IPlacementSolver
{
    public string Name => "Базовый алгоритм (матрица Лапласа)";

    private enum Coordinate { X, Y }

    /// <summary>
    /// Решает задачу размещения для заданного графа
    /// </summary>
    public PlacementResult Solve(Graph graph)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // 1. Построить разреженную матрицу Лапласа
        var laplacian = BuildLaplacianMatrix(graph);

        // 2. Разделить вершины на фиксированные и свободные
        var (freeIndices, fixedIndices) = PartitionVertices(graph);

        if (freeIndices.Length == 0)
        {
            // Все вершины фиксированы
            var x = new double[graph.VertexCount];
            var y = new double[graph.VertexCount];
            foreach (var (idx, fv) in graph.FixedVertices)
            {
                x[idx - 1] = fv.X;
                y[idx - 1] = fv.Y;
            }

            stopwatch.Stop();
            var metrics = CalculateMetrics(graph, x, y);
            return new PlacementResult(x, y, metrics, stopwatch.Elapsed);
        }

        // 3. Сформировать и решить систему для X координат
        var (systemMatrixX, rhsX) = BuildLinearSystem(
            laplacian, freeIndices, fixedIndices, graph, Coordinate.X);
        var xFree = ConjugateGradientSolver.Solve(systemMatrixX, rhsX);
        var xFull = AssembleCoordinates(
            xFree, freeIndices, fixedIndices, graph, Coordinate.X);

        // 4. Сформировать и решить систему для Y координат
        var (systemMatrixY, rhsY) = BuildLinearSystem(
            laplacian, freeIndices, fixedIndices, graph, Coordinate.Y);
        var yFree = ConjugateGradientSolver.Solve(systemMatrixY, rhsY);
        var yFull = AssembleCoordinates(
            yFree, freeIndices, fixedIndices, graph, Coordinate.Y);

        stopwatch.Stop();

        // 5. Вычислить метрики
        var metricsResult = CalculateMetrics(graph, xFull, yFull);

        return new PlacementResult(xFull, yFull, metricsResult, stopwatch.Elapsed);
    }

    /// <summary>
    /// Вычисляет метрики качества размещения
    /// </summary>
    private Metrics CalculateMetrics(Graph graph, double[] xCoords, double[] yCoords)
    {
        double totalLength = 0;
        double maxLength = 0;
        double minLength = double.MaxValue;

        foreach (var edge in graph.Edges)
        {
            int fromIdx = edge.From - 1;
            int toIdx = edge.To - 1;

            double dx = xCoords[fromIdx] - xCoords[toIdx];
            double dy = yCoords[fromIdx] - yCoords[toIdx];
            double length = Math.Sqrt(dx * dx + dy * dy);

            totalLength += length;
            maxLength = Math.Max(maxLength, length);
            minLength = Math.Min(minLength, length);
        }

        double avgLength = graph.EdgeCount > 0 ? totalLength / graph.EdgeCount : 0;

        return new Metrics(
            totalLength,
            maxLength,
            minLength,
            avgLength
        );
    }

    /// <summary>
    /// Разделяет вершины на фиксированные и свободные
    /// </summary>
    private (int[] freeIndices, int[] fixedIndices) PartitionVertices(Graph graph)
    {
        var free = new List<int>();
        var fixedList = new List<int>();

        for (int i = 1; i <= graph.VertexCount; i++)
        {
            if (graph.FixedVertices.ContainsKey(i))
                fixedList.Add(i);
            else
                free.Add(i);
        }

        return (free.ToArray(), fixedList.ToArray());
    }

    /// <summary>
    /// Строит разреженную матрицу Лапласа в формате CSR
    /// L[i,i] = degree(i), L[i,j] = -1 если (i,j) - ребро
    /// </summary>
    private SparseMatrixCSR BuildLaplacianMatrix(Graph graph)
    {
        int n = graph.VertexCount;

        // Подсчитываем количество ненулевых элементов
        // Каждое ребро даёт 2 ненулевых элемента (симметричная матрица)
        // плюс диагональные элементы
        int nnz = graph.EdgeCount * 2 + n;

        var values = new List<double>(nnz);
        var colIndices = new List<int>(nnz);
        var rowPointers = new List<int>(n + 1) { 0 };

        // Подсчитываем степени вершин
        var degrees = new int[n + 1];  // 1-based indexing
        foreach (var edge in graph.Edges)
        {
            degrees[edge.From]++;
            degrees[edge.To]++;
        }

        // Строим матрицу построчно
        for (int i = 1; i <= n; i++)
        {
            // Диагональный элемент
            values.Add(degrees[i]);
            colIndices.Add(i - 1);  // 0-based column index

            // Недиагональные элементы
            var adjacentVertices = new HashSet<int>();
            foreach (var edge in graph.Edges)
            {
                if (edge.From == i && edge.To != i)
                    adjacentVertices.Add(edge.To);
                else if (edge.To == i && edge.From != i)
                    adjacentVertices.Add(edge.From);
            }

            foreach (int adj in adjacentVertices.OrderBy(v => v))
            {
                values.Add(-1.0);
                colIndices.Add(adj - 1);  // 0-based column index
            }

            rowPointers.Add(values.Count);
        }

        return new SparseMatrixCSR
        {
            RowCount = n,
            ColumnCount = n,
            Values = values.ToArray(),
            ColumnIndices = colIndices.ToArray(),
            RowPointers = rowPointers.ToArray()
        };
    }

    /// <summary>
    /// Строит систему линейных уравнений для свободных вершин
    /// L_free * x_free = b, где b учитывает фиксированные вершины
    /// </summary>
    private (SparseMatrixCSR matrix, double[] rhs) BuildLinearSystem(
        SparseMatrixCSR laplacian,
        int[] freeIndices,
        int[] fixedIndices,
        Graph graph,
        Coordinate coord)
    {
        int nFree = freeIndices.Length;
        int nFixed = fixedIndices.Length;

        // Создаём отображение: глобальный индекс -> локальный индекс (0-based)
        var globalToLocal = new Dictionary<int, int>();
        for (int i = 0; i < freeIndices.Length; i++)
        {
            globalToLocal[freeIndices[i]] = i;
        }

        // Строим подсистему для свободных вершин
        var values = new List<double>();
        var colIndices = new List<int>();
        var rowPointers = new List<int> { 0 };
        var rhs = new double[nFree];

        for (int i = 0; i < nFree; i++)
        {
            int globalRow = freeIndices[i];

            // Копируем строку матрицы Лапласа для свободной вершины
            int rowStart = laplacian.RowPointers[globalRow - 1];
            int rowEnd = laplacian.RowPointers[globalRow];

            // Диагональный элемент
            double diag = 0;
            for (int j = rowStart; j < rowEnd; j++)
            {
                int globalCol = laplacian.ColumnIndices[j] + 1;  // back to 1-based
                if (globalCol == globalRow)
                {
                    diag = laplacian.Values[j];
                    break;
                }
            }

            values.Add(diag);
            colIndices.Add(i);

            // Недиагональные элементы (только для свободных вершин)
            for (int j = rowStart; j < rowEnd; j++)
            {
                int globalCol = laplacian.ColumnIndices[j] + 1;
                if (globalCol != globalRow && globalToLocal.ContainsKey(globalCol))
                {
                    values.Add(laplacian.Values[j]);
                    colIndices.Add(globalToLocal[globalCol]);
                }
            }

            rowPointers.Add(values.Count);

            // Вычисляем правую часть: -sum(L_ij * x_j) для фиксированных вершин
            rhs[i] = 0;
            for (int j = rowStart; j < rowEnd; j++)
            {
                int globalCol = laplacian.ColumnIndices[j] + 1;
                if (!globalToLocal.ContainsKey(globalCol))
                {
                    // Это фиксированная вершина
                    double fixedCoord = coord == Coordinate.X
                        ? graph.FixedVertices[globalCol].X
                        : graph.FixedVertices[globalCol].Y;
                    rhs[i] -= laplacian.Values[j] * fixedCoord;
                }
            }
        }

        var matrix = new SparseMatrixCSR
        {
            RowCount = nFree,
            ColumnCount = nFree,
            Values = values.ToArray(),
            ColumnIndices = colIndices.ToArray(),
            RowPointers = rowPointers.ToArray()
        };

        return (matrix, rhs);
    }

    /// <summary>
    /// Собирает полный массив координат из вычисленных свободных и фиксированных
    /// </summary>
    private double[] AssembleCoordinates(
        double[] freeCoords,
        int[] freeIndices,
        int[] fixedIndices,
        Graph graph,
        Coordinate coord)
    {
        double[] result = new double[graph.VertexCount];

        // Свободные вершины
        for (int i = 0; i < freeIndices.Length; i++)
        {
            result[freeIndices[i] - 1] = freeCoords[i];  // to 0-based
        }

        // Фиксированные вершины
        foreach (var (index, fv) in graph.FixedVertices)
        {
            result[index - 1] = coord == Coordinate.X ? fv.X : fv.Y;
        }

        return result;
    }
}
