namespace QuadraticPlacement.Core;

/// <summary>
/// Интерфейс алгоритма размещения
/// </summary>
public interface IPlacementSolver
{
    /// <summary>Название алгоритма</summary>
    string Name { get; }

    /// <summary>
    /// Решает задачу размещения для заданного графа
    /// </summary>
    PlacementResult Solve(Graph graph);
}
