using Xunit;
using QuadraticPlacement.Core;
using QuadraticPlacement.Data;

namespace QuadraticPlacement.Tests.Data;

/// <summary>
/// Тесты для TextGraphParser
/// </summary>
public class TextGraphParserTests
{
    [Fact]
    public void Parse_ValidGraph_ReturnsCorrectGraph()
    {
        // Arrange
        string text = "5 4 2\n1 2 1.0\n2 3 2.0\n3 4 1.5\n4 5 1.0\n1 0.0 0.0\n5 100.0 100.0";

        // Act
        var graph = TextGraphParser.Parse(text);

        // Assert
        Assert.Equal(5, graph.VertexCount);
        Assert.Equal(4, graph.EdgeCount);
        Assert.Equal(2, graph.FixedVertices.Count);

        // Проверяем рёбра
        Assert.Equal(1, graph.Edges[0].From);
        Assert.Equal(2, graph.Edges[0].To);
        Assert.Equal(1.0, graph.Edges[0].Weight);

        Assert.Equal(2, graph.Edges[1].From);
        Assert.Equal(3, graph.Edges[1].To);
        Assert.Equal(2.0, graph.Edges[1].Weight);

        // Проверяем фиксированные вершины
        Assert.True(graph.FixedVertices.ContainsKey(1));
        Assert.Equal(0.0, graph.FixedVertices[1].X);
        Assert.Equal(0.0, graph.FixedVertices[1].Y);

        Assert.True(graph.FixedVertices.ContainsKey(5));
        Assert.Equal(100.0, graph.FixedVertices[5].X);
        Assert.Equal(100.0, graph.FixedVertices[5].Y);
    }

    [Fact]
    public void Parse_EmptyGraph_ReturnsGraphWithoutEdgesAndFixedVertices()
    {
        // Arrange
        string text = "3 0 0";

        // Act
        var graph = TextGraphParser.Parse(text);

        // Assert
        Assert.Equal(3, graph.VertexCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.Equal(0, graph.FixedVertices.Count);
    }

    [Fact]
    public void Parse_InvalidHeader_ThrowsFormatException()
    {
        // Arrange
        string text = "invalid header";

        // Act & Assert
        Assert.Throws<FormatException>(() => TextGraphParser.Parse(text));
    }

    [Fact]
    public void Parse_NegativeVertexCount_ThrowsFormatException()
    {
        // Arrange
        string text = "-1 0 0";

        // Act & Assert
        Assert.Throws<FormatException>(() => TextGraphParser.Parse(text));
    }

    [Fact]
    public void Parse_InsufficientLines_ThrowsFormatException()
    {
        // Arrange
        string text = "5 10 0\n1 2";

        // Act & Assert
        Assert.Throws<FormatException>(() => TextGraphParser.Parse(text));
    }

    [Fact]
    public void Parse_EdgeWithoutWeight_UsesDefaultWeight()
    {
        // Arrange
        string text = "2 1 0\n1 2";

        // Act
        var graph = TextGraphParser.Parse(text);

        // Assert
        Assert.Equal(1, graph.EdgeCount);
        Assert.Equal(1.0, graph.Edges[0].Weight);
    }

    [Fact]
    public void Parse_DuplicateFixedVertex_ThrowsFormatException()
    {
        // Arrange
        string text = "3 0 2\n1 0.0 0.0\n1 10.0 10.0";

        // Act & Assert
        Assert.Throws<FormatException>(() => TextGraphParser.Parse(text));
    }

    [Fact]
    public void ToText_ValidGraph_ReturnsCorrectFormat()
    {
        // Arrange
        var edges = new List<Edge>
        {
            new Edge(1, 2, 1.0),
            new Edge(2, 3, 2.0)
        };
        var fixedVertices = new Dictionary<int, FixedVertex>
        {
            { 1, new FixedVertex(1, 0.0, 0.0) },
            { 3, new FixedVertex(3, 100.0, 100.0) }
        };
        var graph = new Graph(3, edges, fixedVertices);

        // Act
        string text = TextGraphParser.ToText(graph);

        // Assert
        var lines = text.Split('\n');
        Assert.Equal("3 2 2", lines[0]);
        Assert.Contains("1 2 1", lines[1]);
        Assert.Contains("2 3 2", lines[2]);
        Assert.Contains("1 0 0", lines[3]);
        Assert.Contains("3 100 100", lines[4]);
    }

    [Fact]
    public void ToText_RoundTrip_PreservesData()
    {
        // Arrange
        var edges = new List<Edge>
        {
            new Edge(1, 2, 1.5),
            new Edge(2, 3, 2.3),
            new Edge(3, 4, 0.7)
        };
        var fixedVertices = new Dictionary<int, FixedVertex>
        {
            { 1, new FixedVertex(1, 10.5, 20.7) },
            { 4, new FixedVertex(4, 100.0, 200.0) }
        };
        var originalGraph = new Graph(5, edges, fixedVertices);

        // Act
        string text = TextGraphParser.ToText(originalGraph);
        var parsedGraph = TextGraphParser.Parse(text);

        // Assert
        Assert.Equal(originalGraph.VertexCount, parsedGraph.VertexCount);
        Assert.Equal(originalGraph.EdgeCount, parsedGraph.EdgeCount);
        Assert.Equal(originalGraph.FixedVertices.Count, parsedGraph.FixedVertices.Count);

        // Проверяем все рёбра
        for (int i = 0; i < originalGraph.EdgeCount; i++)
        {
            Assert.Equal(originalGraph.Edges[i].From, parsedGraph.Edges[i].From);
            Assert.Equal(originalGraph.Edges[i].To, parsedGraph.Edges[i].To);
            Assert.Equal(originalGraph.Edges[i].Weight, parsedGraph.Edges[i].Weight, 6);
        }

        // Проверяем все фиксированные вершины
        foreach (var (index, fv) in originalGraph.FixedVertices)
        {
            Assert.True(parsedGraph.FixedVertices.ContainsKey(index));
            Assert.Equal(fv.X, parsedGraph.FixedVertices[index].X, 6);
            Assert.Equal(fv.Y, parsedGraph.FixedVertices[index].Y, 6);
        }
    }

    [Fact]
    public void Parse_WithWhitespace_HandlesCorrectly()
    {
        // Arrange
        string text = "   3   2   1\n   1   2   1.5\n   2   3   2.0\n   2   50.0   75.0";

        // Act
        var graph = TextGraphParser.Parse(text);

        // Assert
        Assert.Equal(3, graph.VertexCount);
        Assert.Equal(2, graph.EdgeCount);
        Assert.Equal(1, graph.FixedVertices.Count);
        Assert.True(graph.FixedVertices.ContainsKey(2));
        Assert.Equal(50.0, graph.FixedVertices[2].X);
        Assert.Equal(75.0, graph.FixedVertices[2].Y);
    }
}
