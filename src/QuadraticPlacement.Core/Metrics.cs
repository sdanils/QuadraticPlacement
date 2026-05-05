namespace QuadraticPlacement.Core;

/// <summary>
/// Метрики качества размещения
/// </summary>
public class Metrics
{
    /// <summary>Суммарная взвешенная длина всех рёбер</summary>
    public double TotalWeightedLength { get; }

    /// <summary>Максимальная длина ребра</summary>
    public double MaxEdgeLength { get; }

    /// <summary>Минимальная длина ребра</summary>
    public double MinEdgeLength { get; }

    /// <summary>Средняя длина ребра</summary>
    public double AverageEdgeLength { get; }

    public Metrics(
        double totalWeightedLength,
        double maxEdgeLength,
        double minEdgeLength,
        double averageEdgeLength)
    {
        TotalWeightedLength = totalWeightedLength;
        MaxEdgeLength = maxEdgeLength;
        MinEdgeLength = minEdgeLength;
        AverageEdgeLength = averageEdgeLength;
    }
}
