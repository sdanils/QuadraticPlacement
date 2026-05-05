using FluentAssertions;
using QuadraticPlacement.Core;
using QuadraticPlacement.Algorithms;

namespace QuadraticPlacement.Tests.Algorithms;

public class BasicSolverTests
{
    [Fact]
    public void Solve_SimpleTriangle_ReturnsValidResult()
    {
        var graph = TestDataHelper.CreateSimpleTriangle();
        var solver = new BasicSolver();

        var result = solver.Solve(graph);

        result.Should().NotBeNull();
        result.XCoordinates.Should().HaveCount(3);
        result.YCoordinates.Should().HaveCount(3);
        result.Metrics.Should().NotBeNull();
        result.ComputationTime.Should().BeGreaterThan(TimeSpan.Zero);

        // Проверяем, что фиксированные вершины остались на месте
        result.XCoordinates[0].Should().Be(0);  // вершина 1
        result.YCoordinates[0].Should().Be(0);
        result.XCoordinates[1].Should().Be(1);  // вершина 2
        result.YCoordinates[1].Should().Be(0);
    }

    [Fact]
    public void Solve_LineGraph_PlacesVertexInMiddle()
    {
        var graph = TestDataHelper.CreateLineGraph();
        var solver = new BasicSolver();

        var result = solver.Solve(graph);

        // Вершина 2 должна быть примерно посередине между 1 и 3
        result.XCoordinates[1].Should().BeApproximately(1.0, 0.1);
    }
}
