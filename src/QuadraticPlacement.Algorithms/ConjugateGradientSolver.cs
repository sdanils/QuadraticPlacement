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

    /// <summary>
    /// Решает систему Ax = b методом сопряжённых градиентов
    /// </summary>
    public static double[] Solve(SparseMatrixCSR A, double[] b, double tolerance = DefaultTolerance)
    {
        int n = A.RowCount;
        double[] x = new double[n];  // Начальное приближение = 0
        double[] r = new double[n];  // Остаток
        double[] p = new double[n];  // Направление поиска
        double[] Ap = new double[n]; // A * p

        // r = b - A * x (при x=0: r = b)
        Array.Copy(b, r, n);
        Array.Copy(r, p, n);

        double rsOld = DotProduct(r, r);

        for (int iter = 0; iter < MaxIterations; iter++)
        {
            // Ap = A * p
            MultiplyMatrixVector(A, p, Ap);

            double alpha = rsOld / DotProduct(p, Ap);

            // x = x + alpha * p
            AddVectors(x, p, alpha, x);

            // r = r - alpha * Ap
            AddVectors(r, Ap, -alpha, r);

            double rsNew = DotProduct(r, r);

            if (Math.Sqrt(rsNew) < tolerance)
                break;

            double beta = rsNew / rsOld;

            // p = r + beta * p
            AddVectors(r, p, beta, p);

            rsOld = rsNew;
        }

        return x;
    }
}
