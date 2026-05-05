using QuadraticPlacement.Core;

namespace QuadraticPlacement.Tests;

/// <summary>
/// Хелпер для создания тестовых графов
/// </summary>
public static class TestDataHelper
{
    /// <summary>
    /// Создаёт простой граф-треугольник для тестирования
    /// </summary>
    public static Graph CreateSimpleTriangle()
    {
        var edges = new List<Edge>
        {
            new Edge(1, 2, 1.0),
            new Edge(2, 3, 1.0),
            new Edge(3, 1, 1.0)
        };

        var fixedVertices = new Dictionary<int, FixedVertex>
        {
            [1] = new FixedVertex(1, 0, 0),
            [2] = new FixedVertex(2, 1, 0)
        };

        return new Graph(3, edges, fixedVertices);
    }

    /// <summary>
    /// Создаёт граф-линию из 3 вершин
    /// </summary>
    public static Graph CreateLineGraph()
    {
        var edges = new List<Edge>
        {
            new Edge(1, 2),
            new Edge(2, 3)
        };

        var fixedVertices = new Dictionary<int, FixedVertex>
        {
            [1] = new FixedVertex(1, 0, 0),
            [3] = new FixedVertex(3, 2, 0)
        };

        return new Graph(3, edges, fixedVertices);
    }
}
