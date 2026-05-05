namespace QuadraticPlacement.Core;

/// <summary>
/// Граф связей между элементами
/// </summary>
public class Graph
{
    /// <summary>Общее количество вершин</summary>
    public int VertexCount { get; }

    /// <summary>Количество рёбер</summary>
    public int EdgeCount { get; }

    /// <summary>Список рёбер графа</summary>
    public IReadOnlyList<Edge> Edges { get; }

    /// <summary>Словарь фиксированных вершин (индекс → вершина)</summary>
    public IReadOnlyDictionary<int, FixedVertex> FixedVertices { get; }

    public Graph(
        int vertexCount,
        IEnumerable<Edge> edges,
        IDictionary<int, FixedVertex> fixedVertices)
    {
        if (vertexCount < 1)
            throw new ArgumentOutOfRangeException(nameof(vertexCount), "Граф должен содержать хотя бы одну вершину");

        VertexCount = vertexCount;
        Edges = edges?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(edges));
        EdgeCount = Edges.Count;
        FixedVertices = new Dictionary<int, FixedVertex>(fixedVertices ?? throw new ArgumentNullException(nameof(fixedVertices)));

        // Валидация
        ValidateEdges();
        ValidateFixedVertices();
    }

    private void ValidateEdges()
    {
        foreach (var edge in Edges)
        {
            if (edge.From > VertexCount)
                throw new ArgumentException($"Ребро ссылается на вершину {edge.From}, но всего вершин: {VertexCount}");
            if (edge.To > VertexCount)
                throw new ArgumentException($"Ребро ссылается на вершину {edge.To}, но всего вершин: {VertexCount}");
        }
    }

    private void ValidateFixedVertices()
    {
        foreach (var (index, _) in FixedVertices)
        {
            if (index > VertexCount)
                throw new ArgumentException($"Фиксированная вершина {index}, но всего вершин: {VertexCount}");
        }
    }
}
