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
    /// Вычисляет силы для всех вершин
    /// </summary>
    private (double[] fx, double[] fy) ComputeForces(Graph graph, double[] x, double[] y)
    {
        int n = graph.VertexCount;
        var fx = new double[n];
        var fy = new double[n];

        // 1. Силы притяжения (пружины) вдоль рёбер
        foreach (var edge in graph.Edges)
        {
            int u = edge.From - 1;
            int v = edge.To - 1;

            double dx = x[v] - x[u];
            double dy = y[v] - y[u];
            double dist = Math.Sqrt(dx * dx + dy * dy) + 1e-10;

            double force = SpringConstant * (dist - IdealLength);

            double fx_val = force * dx / dist;
            double fy_val = force * dy / dist;

            if (!graph.FixedVertices.ContainsKey(edge.From))
            {
                fx[u] += fx_val;
                fy[u] += fy_val;
            }

            if (!graph.FixedVertices.ContainsKey(edge.To))
            {
                fx[v] -= fx_val;
                fy[v] -= fy_val;
            }
        }

        // 2. Силы отталкивания (между всеми парами вершин)
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                double dx = x[j] - x[i];
                double dy = y[j] - y[i];
                double distSq = dx * dx + dy * dy + 1e-10;
                double dist = Math.Sqrt(distSq);

                double force = RepulsionConstant / distSq;

                double fx_val = force * dx / dist;
                double fy_val = force * dy / dist;

                if (!graph.FixedVertices.ContainsKey(i + 1))
                {
                    fx[i] -= fx_val;
                    fy[i] -= fy_val;
                }

                if (!graph.FixedVertices.ContainsKey(j + 1))
                {
                    fx[j] += fx_val;
                    fy[j] += fy_val;
                }
            }
        }

        return (fx, fy);
    }

    /// <summary>
    /// Обновляет позиции вершин на основе вычисленных сил
    /// </summary>
    private void UpdatePositions(double[] x, double[] y, double[] fx, double[] fy, Graph graph, double temperature)
    {
        double maxForce = Math.Max(fx.Max(Math.Abs), fy.Max(Math.Abs));
        double scale = temperature / (maxForce + 1e-10);

        for (int i = 0; i < graph.VertexCount; i++)
        {
            if (!graph.FixedVertices.ContainsKey(i + 1))
            {
                double dx = fx[i] * scale;
                double dy = fy[i] * scale;

                double dist = Math.Sqrt(dx * dx + dy * dy);
                if (dist > temperature)
                {
                    dx = dx / dist * temperature;
                    dy = dy / dist * temperature;
                }

                x[i] += dx;
                y[i] += dy;

                x[i] = Math.Max(0, Math.Min(1000, x[i]));
                y[i] = Math.Max(0, Math.Min(1000, y[i]));
            }
        }
    }

    /// <summary>
    /// Вычисляет энергию системы
    /// </summary>
    private double ComputeSystemEnergy(Graph graph, double[] x, double[] y)
    {
        double energy = 0;
        foreach (var edge in graph.Edges)
        {
            int u = edge.From - 1;
            int v = edge.To - 1;
            double dx = x[v] - x[u];
            double dy = y[v] - y[u];
            double dist = Math.Sqrt(dx * dx + dy * dy);
            energy += SpringConstant * Math.Pow(dist - IdealLength, 2) / 2;
        }
        return energy;
    }

    /// <summary>
    /// Вычисляет метрики качества размещения
    /// </summary>
    private Metrics CalculateMetrics(Graph graph, double[] x, double[] y)
    {
        double totalLength = 0;
        double maxLen = 0;
        double minLen = double.MaxValue;

        foreach (var edge in graph.Edges)
        {
            int u = edge.From - 1;
            int v = edge.To - 1;
            double dx = x[u] - x[v];
            double dy = y[u] - y[v];
            double len = Math.Sqrt(dx * dx + dy * dy) * edge.Weight;

            totalLength += len;
            maxLen = Math.Max(maxLen, len);
            minLen = Math.Min(minLen, len);
        }

        double avgLen = totalLength / graph.EdgeCount;

        return new Metrics(totalLength, maxLen, minLen, avgLen);
    }

    /// <summary>
    /// Решает задачу размещения, используя силовой алгоритм
    /// </summary>
    public PlacementResult Solve(Graph graph)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var (x, y) = InitializePositions(graph);

        // Адаптивное число итераций в зависимости от размера графа
        int adaptiveMaxIterations = graph.VertexCount switch
        {
            < 100 => 500,
            < 1000 => 200,
            _ => 100  // Для больших графов меньше итераций
        };

        double temperature = InitialTemperature;
        double prevEnergy = double.MaxValue;
        int stagnationCount = 0;
        const int maxStagnation = 10;  // Остановиться если нет улучшений 10 итераций подряд

        // Отслеживаем историю энергии для отчётов
        EnergyHistory.Clear();
        double initialEnergy = ComputeSystemEnergy(graph, x, y);
        EnergyHistory.Add(initialEnergy);

        int actualIterations = 0;
        for (int iteration = 0; iteration < adaptiveMaxIterations; iteration++)
        {
            var (forcesX, forcesY) = ComputeForces(graph, x, y);

            UpdatePositions(x, y, forcesX, forcesY, graph, temperature);

            double currentEnergy = ComputeSystemEnergy(graph, x, y);
            EnergyHistory.Add(currentEnergy);
            actualIterations++;

            // Проверка на сходимость с учётом размера графа
            double energyThreshold = ConvergenceThreshold * Math.Max(1, graph.VertexCount / 1000.0);
            if (Math.Abs(prevEnergy - currentEnergy) < energyThreshold)
            {
                stagnationCount++;
                if (stagnationCount >= maxStagnation)
                    break;  // Ранняя остановка при стагнации
            }
            else
            {
                stagnationCount = 0;
            }

            prevEnergy = currentEnergy;
            temperature *= CoolingRate;
        }

        stopwatch.Stop();

        var metrics = CalculateMetrics(graph, x, y);

        return new PlacementResult(x, y, metrics, stopwatch.Elapsed);
    }

    /// <summary>
    /// История значений энергии системы на каждой итерации
    /// </summary>
    public List<double> EnergyHistory { get; } = new();
}

/// <summary>
/// Вспомогательные методы расширения
/// </summary>
internal static class ArrayExtensions
{
    public static double Max(this double[] array, Func<double, double> selector)
    {
        double max = double.MinValue;
        foreach (var val in array)
        {
            double selected = selector(val);
            if (selected > max) max = selected;
        }
        return max;
    }
}
