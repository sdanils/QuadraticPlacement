using System.Text.Json.Serialization;
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Data;

/// <summary>
/// Контракт данных для сериализации графа в JSON
/// </summary>
public class GraphDataContract
{
    /// <summary>Количество вершин</summary>
    [JsonPropertyName("vertexCount")]
    public int VertexCount { get; set; }

    /// <summary>Рёбра графа</summary>
    [JsonPropertyName("edges")]
    public List<EdgeData> Edges { get; set; } = new();

    /// <summary>Фиксированные вершины</summary>
    [JsonPropertyName("fixedVertices")]
    public List<FixedVertexData> FixedVertices { get; set; } = new();

    /// <summary>
    /// Данные ребра для сериализации
    /// </summary>
    public class EdgeData
    {
        [JsonPropertyName("from")]
        public int From { get; set; }

        [JsonPropertyName("to")]
        public int To { get; set; }

        [JsonPropertyName("weight")]
        public double Weight { get; set; }

        public EdgeData() { }

        public EdgeData(Edge edge)
        {
            From = edge.From;
            To = edge.To;
            Weight = edge.Weight;
        }
    }

    /// <summary>
    /// Данные фиксированной вершины для сериализации
    /// </summary>
    public class FixedVertexData
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }

        public FixedVertexData() { }

        public FixedVertexData(FixedVertex fv)
        {
            Index = fv.Index;
            X = fv.X;
            Y = fv.Y;
        }
    }

    /// <summary>
    /// Создаёт контракт из графа
    /// </summary>
    public static GraphDataContract FromGraph(Graph graph)
    {
        if (graph == null)
            throw new ArgumentNullException(nameof(graph));

        return new GraphDataContract
        {
            VertexCount = graph.VertexCount,
            Edges = graph.Edges.Select(e => new EdgeData(e)).ToList(),
            FixedVertices = graph.FixedVertices.Values.Select(fv => new FixedVertexData(fv)).ToList()
        };
    }

    /// <summary>
    /// Преобразует контракт в граф
    /// </summary>
    public Graph ToGraph()
    {
        var edges = Edges.Select(e => new Edge(e.From, e.To, e.Weight));
        var fixedVertices = FixedVertices.ToDictionary(fv => fv.Index, fv => new FixedVertex(fv.Index, fv.X, fv.Y));

        return new Graph(VertexCount, edges, fixedVertices);
    }
}
