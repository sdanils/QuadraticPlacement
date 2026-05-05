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

        // Строим матрицу Лапласа
        var laplacian = BuildLaplacianMatrix(graph);

        // Для базовой реализации используем простое размещение на единичной окружности
        // В будущих версиях здесь будет решение системы Lx = 0
        int n = graph.VertexCount;
        var xCoords = new double[n];
        var yCoords = new double[n];

        for (int i = 0; i < n; i++)
        {
            double angle = 2 * Math.PI * i / n;
            xCoords[i] = Math.Cos(angle);
            yCoords[i] = Math.Sin(angle);
        }

        stopwatch.Stop();

        // Вычисляем метрики
        var metrics = CalculateMetrics(graph, xCoords, yCoords);

        return new PlacementResult(
            xCoords,
            yCoords,
            metrics,
            stopwatch.Elapsed
        );
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
}
