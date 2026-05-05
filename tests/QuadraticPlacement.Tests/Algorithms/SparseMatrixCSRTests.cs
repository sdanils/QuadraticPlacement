using FluentAssertions;
using QuadraticPlacement.Algorithms;

namespace QuadraticPlacement.Tests.Algorithms;

public class SparseMatrixCSRTests
{
    [Fact]
    public void SparseMatrixCSR_CreatesCorrectly()
    {
        // Матрица 3x3:
        // 1 0 2
        // 0 3 0
        // 4 0 5

        var matrix = new SparseMatrixCSR
        {
            RowCount = 3,
            ColumnCount = 3,
            Values = new double[] { 1, 2, 3, 4, 5 },
            ColumnIndices = new int[] { 0, 2, 1, 0, 2 },
            RowPointers = new int[] { 0, 2, 3, 5 }
        };

        matrix.RowCount.Should().Be(3);
        matrix.ColumnCount.Should().Be(3);
        matrix.Values.Length.Should().Be(5);
    }
}
