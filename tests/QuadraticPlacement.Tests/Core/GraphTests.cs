using FluentAssertions;
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Tests.Core;

public class GraphTests
{
    [Fact]
    public void Graph_CreatesWithBasicProperties()
    {
        var edges = new List<Edge>
        {
            new Edge(1, 2),
            new Edge(2, 3)
        };

        var fixedVertices = new Dictionary<int, FixedVertex>
        {
            [1] = new FixedVertex(1, 0, 0)
        };

        var graph = new Graph(3, edges, fixedVertices);

        graph.VertexCount.Should().Be(3);
        graph.EdgeCount.Should().Be(2);
        graph.Edges.Should().HaveCount(2);
        graph.FixedVertices.Should().HaveCount(1);
    }

    [Fact]
    public void Graph_EdgesAreReadOnly()
    {
        var edges = new List<Edge> { new Edge(1, 2) };
        var graph = new Graph(2, edges, new Dictionary<int, FixedVertex>());

        // Проверка, что это IReadOnlyList
        IReadOnlyList<Edge> readOnlyEdges = graph.Edges;
        readOnlyEdges.Should().NotBeNull();
    }
}
