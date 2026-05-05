using FluentAssertions;
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Tests.Core;

public class FixedVertexTests
{
    [Fact]
    public void FixedVertex_CreatesWithCoordinates()
    {
        var vertex = new FixedVertex(5, 100.5, 200.7);
        vertex.Index.Should().Be(5);
        vertex.X.Should().Be(100.5);
        vertex.Y.Should().Be(200.7);
    }

    [Fact]
    public void FixedVertex_RequiresValidIndex()
    {
        Action act = () => new FixedVertex(0, 100, 200);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
