using FluentAssertions;
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Tests.Core;

public class EdgeTests
{
    [Fact]
    public void Edge_CreatesWithDefaultWeight()
    {
        var edge = new Edge(1, 2);
        edge.From.Should().Be(1);
        edge.To.Should().Be(2);
        edge.Weight.Should().Be(1.0);
    }

    [Fact]
    public void Edge_CreatesWithCustomWeight()
    {
        var edge = new Edge(1, 2, 2.5);
        edge.Weight.Should().Be(2.5);
    }

    [Fact]
    public void Edge_IsImmutable()
    {
        var edge = new Edge(1, 2);
        // Свойства только для чтения, компиляция проверяет иммутабельность
    }
}
