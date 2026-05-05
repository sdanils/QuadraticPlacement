namespace QuadraticPlacement.Algorithms;

/// <summary>
/// Разреженная матрица в формате CSR (Compressed Sparse Row)
/// </summary>
public struct SparseMatrixCSR
{
    /// <summary>Ненулевые значения</summary>
    public double[] Values;

    /// <summary>Индексы столбцов</summary>
    public int[] ColumnIndices;

    /// <summary>Указатели на начало строк</summary>
    public int[] RowPointers;

    /// <summary>Количество строк</summary>
    public int RowCount;

    /// <summary>Количество столбцов</summary>
    public int ColumnCount;
}
