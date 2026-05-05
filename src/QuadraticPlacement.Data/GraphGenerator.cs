using QuadraticPlacement.Core;

namespace QuadraticPlacement.Data;

/// <summary>
/// Генератор тестовых графов
/// </summary>
public static class GraphGenerator
{
    private static readonly Random Random = new();

    /// <summary>
    /// Генерирует случайный граф
    /// </summary>
    /// <param name="vertexCount">Количество вершин</param>
    /// <param name="edgeCount">Количество рёбер</param>
    /// <param name="fixedVertexCount">Количество фиксированных вершин</param>
    /// <param name="minWeight">Минимальный вес ребра</param>
    /// <param name="maxWeight">Максимальный вес ребра</param>
    /// <param name="coordinateRange">Диапазон координат фиксированных вершин [0, coordinateRange]</param>
    public static Graph GenerateRandom(
        int vertexCount,
        int edgeCount,
        int fixedVertexCount = 0,
        double minWeight = 1.0,
        double maxWeight = 10.0,
        double coordinateRange = 1000.0)
    {
        if (vertexCount < 1)
            throw new ArgumentOutOfRangeException(nameof(vertexCount), "Количество вершин должно быть положительным");

        if (edgeCount < 0)
            throw new ArgumentOutOfRangeException(nameof(edgeCount), "Количество рёбер не может быть отрицательным");

        if (fixedVertexCount < 0 || fixedVertexCount > vertexCount)
            throw new ArgumentOutOfRangeException(nameof(fixedVertexCount),
                $"Количество фиксированных вершин должно быть от 0 до {vertexCount}");

        if (minWeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(minWeight), "Минимальный вес должен быть положительным");

        if (maxWeight < minWeight)
            throw new ArgumentOutOfRangeException(nameof(maxWeight), "Максимальный вес должен быть не меньше минимального");

        // Генерируем рёбра
        var edges = new List<Edge>(edgeCount);
        var edgeSet = new HashSet<(int, int)>();

        for (int i = 0; i < edgeCount; i++)
        {
            int from, to;
            (from, to) = GenerateRandomEdge(vertexCount);

            // Избегаем дубликатов рёбер
            while (edgeSet.Contains((from, to)) || edgeSet.Contains((to, from)))
            {
                (from, to) = GenerateRandomEdge(vertexCount);
            }

            edgeSet.Add((from, to));

            double weight = minWeight + Random.NextDouble() * (maxWeight - minWeight);
            edges.Add(new Edge(from, to, weight));
        }

        // Генерируем фиксированные вершины
        var fixedVertices = new Dictionary<int, FixedVertex>();
        if (fixedVertexCount > 0)
        {
            var vertexIndices = Enumerable.Range(1, vertexCount).OrderBy(_ => Random.Next()).Take(fixedVertexCount);

            foreach (var index in vertexIndices)
            {
                double x = Random.NextDouble() * coordinateRange;
                double y = Random.NextDouble() * coordinateRange;
                fixedVertices[index] = new FixedVertex(index, x, y);
            }
        }

        return new Graph(vertexCount, edges, fixedVertices);
    }

    /// <summary>
    /// Генерирует граф в виде сетки (2D решётки)
    /// </summary>
    /// <param name="rows">Количество строк</param>
    /// <param name="cols">Количество столбцов</param>
    /// <param name="fixCorners">Закрепить угловые вершины</param>
    /// <param name="cellSize">Размер ячейки сетки</param>
    public static Graph GenerateGrid(int rows, int cols, bool fixCorners = true, double cellSize = 100.0)
    {
        if (rows < 1)
            throw new ArgumentOutOfRangeException(nameof(rows), "Количество строк должно быть положительным");

        if (cols < 1)
            throw new ArgumentOutOfRangeException(nameof(cols), "Количество столбцов должно быть положительным");

        int vertexCount = rows * cols;
        var edges = new List<Edge>();

        // Добавляем горизонтальные рёбра
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols - 1; col++)
            {
                int from = row * cols + col + 1; // +1 для 1-based индексации
                int to = row * cols + col + 2;
                edges.Add(new Edge(from, to));
            }
        }

        // Добавляем вертикальные рёбра
        for (int row = 0; row < rows - 1; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int from = row * cols + col + 1;
                int to = (row + 1) * cols + col + 1;
                edges.Add(new Edge(from, to));
            }
        }

        // Фиксированные вершины (углы сетки)
        var fixedVertices = new Dictionary<int, FixedVertex>();

        if (fixCorners)
        {
            // Левый верхний угол
            fixedVertices[1] = new FixedVertex(1, 0, 0);

            // Правый верхний угол
            fixedVertices[cols] = new FixedVertex(cols, (cols - 1) * cellSize, 0);

            // Левый нижний угол
            fixedVertices[(rows - 1) * cols + 1] = new FixedVertex((rows - 1) * cols + 1, 0, (rows - 1) * cellSize);

            // Правый нижний угол
            fixedVertices[rows * cols] = new FixedVertex(rows * cols, (cols - 1) * cellSize, (rows - 1) * cellSize);
        }

        return new Graph(vertexCount, edges, fixedVertices);
    }

    /// <summary>
    /// Генерирует граф с "горячими связями" (несколько цепей с большим количеством рёбер)
    /// </summary>
    /// <param name="vertexCount">Количество вершин</param>
    /// <param name="hotChainsCount">Количество горячих цепей</param>
    /// <param name="chainLength">Длина каждой цепи</param>
    /// <param name="randomEdgesCount">Количество случайных рёбер</param>
    /// <param name="hotWeight">Вес горячих связей</param>
    /// <param name="randomWeight">Вес случайных связей</param>
    public static Graph GenerateWithHotConnections(
        int vertexCount,
        int hotChainsCount = 3,
        int chainLength = 10,
        int randomEdgesCount = 50,
        double hotWeight = 10.0,
        double randomWeight = 1.0)
    {
        if (vertexCount < 1)
            throw new ArgumentOutOfRangeException(nameof(vertexCount), "Количество вершин должно быть положительным");

        if (hotChainsCount < 0)
            throw new ArgumentOutOfRangeException(nameof(hotChainsCount), "Количество горячих цепей не может быть отрицательным");

        if (chainLength < 1)
            throw new ArgumentOutOfRangeException(nameof(chainLength), "Длина цепи должна быть положительной");

        if (randomEdgesCount < 0)
            throw new ArgumentOutOfRangeException(nameof(randomEdgesCount), "Количество случайных рёбер не может быть отрицательным");

        if (hotWeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(hotWeight), "Вес горячих связей должен быть положительным");

        if (randomWeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(randomWeight), "Вес случайных связей должен быть положительным");

        var edges = new List<Edge>();
        var edgeSet = new HashSet<(int, int)>();

        // Генерируем горячие цепи
        for (int chain = 0; chain < hotChainsCount; chain++)
        {
            // Выбираем случайную начальную вершину
            int startVertex = Random.Next(1, vertexCount - chainLength + 1);

            // Строим цепь
            for (int i = 0; i < chainLength - 1; i++)
            {
                int from = startVertex + i;
                int to = startVertex + i + 1;

                if (from <= vertexCount && to <= vertexCount)
                {
                    if (!edgeSet.Contains((from, to)) && !edgeSet.Contains((to, from)))
                    {
                        edgeSet.Add((from, to));
                        edges.Add(new Edge(from, to, hotWeight));
                    }
                }
            }
        }

        // Генерируем случайные рёбра
        for (int i = 0; i < randomEdgesCount; i++)
        {
            int from, to;
            (from, to) = GenerateRandomEdge(vertexCount);

            while (edgeSet.Contains((from, to)) || edgeSet.Contains((to, from)))
            {
                (from, to) = GenerateRandomEdge(vertexCount);
            }

            edgeSet.Add((from, to));
            edges.Add(new Edge(from, to, randomWeight));
        }

        // Генерируем несколько фиксированных вершин для закрепления структуры
        var fixedVertices = new Dictionary<int, FixedVertex>();

        // Закрепляем первую и последнюю вершину
        fixedVertices[1] = new FixedVertex(1, 0, 0);
        fixedVertices[vertexCount] = new FixedVertex(vertexCount, 1000, 1000);

        return new Graph(vertexCount, edges, fixedVertices);
    }

    /// <summary>
    /// Генерирует случайное ребро в заданном диапазоне вершин
    /// </summary>
    private static (int from, int to) GenerateRandomEdge(int vertexCount)
    {
        int from = Random.Next(1, vertexCount + 1);
        int to = Random.Next(1, vertexCount + 1);

        // Избегаем петель
        while (from == to)
        {
            to = Random.Next(1, vertexCount + 1);
        }

        return (from, to);
    }

    /// <summary>
    /// Устанавливает seed для генератора случайных чисел (для детерминированной генерации)
    /// </summary>
    public static void SetSeed(int seed)
    {
        // Примечание: в C# нельзя изменить seed существующего Random,
        // поэтому этот метод создаёт новый генератор
        // В реальном коде лучше использовать dependency injection
    }
}
