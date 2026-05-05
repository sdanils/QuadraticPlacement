namespace QuadraticPlacement.Core;

/// <summary>
/// Фиксированная вершина с заданными координатами
/// </summary>
public class FixedVertex
{
    /// <summary>Индекс вершины</summary>
    public int Index { get; }

    /// <summary>Координата X</summary>
    public double X { get; }

    /// <summary>Координата Y</summary>
    public double Y { get; }

    public FixedVertex(int index, double x, double y)
    {
        if (index < 1)
            throw new ArgumentOutOfRangeException(nameof(index), "Нумерация вершин начинается с 1");

        Index = index;
        X = x;
        Y = y;
    }
}
