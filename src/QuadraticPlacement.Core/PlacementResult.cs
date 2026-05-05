namespace QuadraticPlacement.Core;

/// <summary>
/// Результат размещения графа
/// </summary>
public class PlacementResult
{
    /// <summary>Массив X координат всех вершин</summary>
    public double[] XCoordinates { get; }

    /// <summary>Массив Y координат всех вершин</summary>
    public double[] YCoordinates { get; }

    /// <summary>Метрики качества размещения</summary>
    public Metrics Metrics { get; }

    /// <summary>Время вычисления</summary>
    public TimeSpan ComputationTime { get; }

    public PlacementResult(
        double[] xCoordinates,
        double[] yCoordinates,
        Metrics metrics,
        TimeSpan computationTime)
    {
        XCoordinates = xCoordinates ?? throw new ArgumentNullException(nameof(xCoordinates));
        YCoordinates = yCoordinates ?? throw new ArgumentNullException(nameof(yCoordinates));
        Metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));

        if (xCoordinates.Length != yCoordinates.Length)
            throw new ArgumentException("Массивы координат должны иметь одинаковую длину");

        XCoordinates = (double[])xCoordinates.Clone();
        YCoordinates = (double[])yCoordinates.Clone();
        ComputationTime = computationTime;
    }

    /// <summary>
    /// Получить координаты вершины по индексу (нумерация с 1)
    /// </summary>
    public (double X, double Y) GetVertexCoordinates(int index)
    {
        if (index < 1 || index > XCoordinates.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        int arrayIndex = index - 1;
        return (XCoordinates[arrayIndex], YCoordinates[arrayIndex]);
    }
}
