using FluentAssertions;
using QuadraticPlacement.Algorithms;

namespace QuadraticPlacement.Tests.Algorithms;

public class ConjugateGradientSolverTests
{
    [Fact]
    public void Solve_SimpleDiagonalSystem()
    {
        // Система:
        // 2x = 4
        // 3y = 9
        // Решение: x=2, y=3

        var A = new SparseMatrixCSR
        {
            RowCount = 2,
            ColumnCount = 2,
            Values = new double[] { 2, 3 },
            ColumnIndices = new int[] { 0, 1 },
            RowPointers = new int[] { 0, 1, 2 }
        };

        double[] b = { 4, 9 };

        double[] x = ConjugateGradientSolver.Solve(A, b);

        x[0].Should().BeApproximately(2.0, 1e-6);
        x[1].Should().BeApproximately(3.0, 1e-6);
    }
}
