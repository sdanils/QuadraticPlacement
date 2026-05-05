using FluentAssertions;
using QuadraticPlacement.Algorithms;

namespace QuadraticPlacement.Tests.Algorithms;

public class HeuristicSolverTests
{
    [Fact]
    public void Solve_SimpleTriangle_ReturnsValidResult()
    {
        var graph = TestDataHelper.CreateSimpleTriangle();
        var solver = new HeuristicSolver();

        var result = solver.Solve(graph);

        result.Should().NotBeNull();
        result.XCoordinates.Should().HaveCount(3);
        result.YCoordinates.Should().HaveCount(3);
        result.Metrics.Should().NotBeNull();
        result.ComputationTime.Should().BeGreaterThan(TimeSpan.Zero);

        // Проверяем фиксированные вершины
        result.XCoordinates[0].Should().Be(0);
        result.YCoordinates[0].Should().Be(0);
        result.XCoordinates[1].Should().Be(1);
        result.YCoordinates[1].Should().Be(0);
    }

    [Fact]
    public void Solver_Name_ReturnsCorrectName()
    {
        var solver = new HeuristicSolver();
        solver.Name.Should().Be("Эвристический алгоритм (силовой метод)");
    }

    [Fact]
    public void Solve_CompletesWithoutError()
    {
        var graph = TestDataHelper.CreateSimpleTriangle();
        var solver = new HeuristicSolver();

        Action act = () => solver.Solve(graph);

        act.Should().NotThrow();
    }
}
