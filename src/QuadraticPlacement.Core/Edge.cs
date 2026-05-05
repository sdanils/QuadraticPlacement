namespace QuadraticPlacement.Core;

/// <summary>
/// Ребро графа с весом
/// </summary>
public class Edge
{
    /// <summary>Индекс начальной вершины (нумерация с 1)</summary>
    public int From { get; }

    /// <summary>Индекс конечной вершины (нумерация с 1)</summary>
    public int To { get; }

    /// <summary>Вес ребра (по умолчанию 1.0)</summary>
    public double Weight { get; }

    public Edge(int from, int to, double weight = 1.0)
    {
        if (from < 1)
            throw new ArgumentOutOfRangeException(nameof(from), "Нумерация вершин начинается с 1");
        if (to < 1)
            throw new ArgumentOutOfRangeException(nameof(to), "Нумерация вершин начинается с 1");
        if (weight <= 0)
            throw new ArgumentOutOfRangeException(nameof(weight), "Вес должен быть положительным");

        From = from;
        To = to;
        Weight = weight;
    }
}
