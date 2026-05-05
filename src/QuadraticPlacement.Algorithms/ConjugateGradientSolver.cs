namespace QuadraticPlacement.Algorithms;

/// <summary>
/// Сопряжённый градиент solver для разреженных матриц
/// </summary>
public static class ConjugateGradientSolver
{
    private const int MaxIterations = 1000;
    private const double DefaultTolerance = 1e-10;

    /// <summary>
    /// Вычисляет скалярное произведение векторов
    /// </summary>
    private static double DotProduct(double[] a, double[] b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Векторы должны иметь одинаковую длину");

        double sum = 0;
        for (int i = 0; i < a.Length; i++)
        {
            sum += a[i] * b[i];
        }
        return sum;
    }

    /// <summary>
    /// Умножает разреженную матрицу на вектор: result = A * x
    /// </summary>
    private static void MultiplyMatrixVector(SparseMatrixCSR A, double[] x, double[] result)
    {
        if (A.RowCount != result.Length)
            throw new ArgumentException("Неверная размерность результата");
        if (A.ColumnCount != x.Length)
            throw new ArgumentException("Неверная размерность вектора x");

        Array.Clear(result, 0, result.Length);

        for (int i = 0; i < A.RowCount; i++)
        {
            int rowStart = A.RowPointers[i];
            int rowEnd = A.RowPointers[i + 1];

            for (int j = rowStart; j < rowEnd; j++)
            {
                result[i] += A.Values[j] * x[A.ColumnIndices[j]];
            }
        }
    }

    /// <summary>
    /// result = a + scalar * b
    /// </summary>
    private static void AddVectors(double[] a, double[] b, double scalar, double[] result)
    {
        if (a.Length != b.Length || a.Length != result.Length)
            throw new ArgumentException("Векторы должны иметь одинаковую длину");

        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i] + scalar * b[i];
        }
    }
}
