using QuadraticPlacement.Core;

namespace QuadraticPlacement.Algorithms;

/// <summary>
/// Эвристический силовой алгоритм размещения (force-directed)
/// </summary>
public class HeuristicSolver : IPlacementSolver
{
    public string Name => "Эвристический алгоритм (силовой метод)";

    private const int MaxIterations = 1000;
    private const double ConvergenceThreshold = 1e-6;
    private const double CoolingRate = 0.95;
    private const double InitialTemperature = 100.0;
    private const double RepulsionConstant = 1000.0;
    private const double SpringConstant = 1.0;
    private const double IdealLength = 50.0;

    private Random _random = new Random(42);  // фиксированный seed для воспроизводимости

    /// <summary>
    /// Инициализирует случайные позиции для свободных вершин
    /// </summary>
    private (double[] x, double[] y) InitializePositions(Graph graph)
    {
        var x = new double[graph.VertexCount];
        var y = new double[graph.VertexCount];

        for (int i = 0; i < graph.VertexCount; i++)
        {
            int vertexIdx = i + 1;
            if (graph.FixedVertices.ContainsKey(vertexIdx))
            {
                x[i] = graph.FixedVertices[vertexIdx].X;
                y[i] = graph.FixedVertices[vertexIdx].Y;
            }
            else
            {
                x[i] = _random.NextDouble() * 1000;
                y[i] = _random.NextDouble() * 1000;
            }
        }

        return (x, y);
    }

    /// <summary>
    /// Решает задачу размещения, используя силовой алгоритм
    /// </summary>
    public PlacementResult Solve(Graph graph)
    {
        var startTime = DateTime.UtcNow;

        var (x, y) = InitializePositions(graph);

        // TODO: Реализовать силовой алгоритм в следующих задачах
        // Сейчас возвращаем инициализированные позиции

        var metrics = new Metrics(0.0, 0.0, 0.0, 0.0);
        var computationTime = DateTime.UtcNow - startTime;

        return new PlacementResult(x, y, metrics, computationTime);
    }
}
