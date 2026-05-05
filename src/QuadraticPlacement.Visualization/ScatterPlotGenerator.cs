using ScottPlot;
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Visualization;

/// <summary>
/// Генератор точечных графиков для визуализации размещения вершин.
/// Отделяет фиксированные и свободные вершины, использует разные цвета.
/// </summary>
public class ScatterPlotGenerator
{
    /// <summary>
    /// Создает точечный график размещения вершин.
    /// Для фиксированных вершин использует синий цвет, для свободных - красный.
    /// </summary>
    /// <param name="graph">Граф с информацией о вершинах и ребрах</param>
    /// <param name="placement">Результат размещения вершин</param>
    /// <param name="width">Ширина изображения в пикселях</param>
    /// <param name="height">Высота изображения в пикселях</param>
    /// <returns>Массив байт PNG изображения</returns>
    public byte[] GenerateScatterPlot(
        Graph graph,
        PlacementResult placement,
        int width = 800,
        int height = 600)
    {
        var plot = new Plot();
        plot.Title("Размещение вершин графа");
        plot.XLabel("X координата");
        plot.YLabel("Y координата");

        // Разделяем вершины на фиксированные и свободные
        var fixedVertices = new List<(double x, double y, int id)>();
        var freeVertices = new List<(double x, double y, int id)>();

        for (int i = 0; i < placement.XCoordinates.Length; i++)
        {
            var vertexId = i + 1; // нумерация с 1
            var x = placement.XCoordinates[i];
            var y = placement.YCoordinates[i];
            var isFixed = graph.FixedVertices.ContainsKey(vertexId);

            if (isFixed)
            {
                fixedVertices.Add((x, y, vertexId));
            }
            else
            {
                freeVertices.Add((x, y, vertexId));
            }
        }

        // Добавляем фиксированные вершины (синие)
        if (fixedVertices.Count > 0)
        {
            var fixedX = fixedVertices.Select(v => v.x).ToArray();
            var fixedY = fixedVertices.Select(v => v.y).ToArray();
            var scatter = plot.Add.Scatter(fixedX, fixedY, color: ScottPlot.Colors.Blue);
            scatter.LegendText = "Фиксированные вершины";
            scatter.LineStyle = LineStyle.None;
            scatter.MarkerSize = 8;
        }

        // Добавляем свободные вершины (красные)
        if (freeVertices.Count > 0)
        {
            var freeX = freeVertices.Select(v => v.x).ToArray();
            var freeY = freeVertices.Select(v => v.y).ToArray();
            var scatter = plot.Add.Scatter(freeX, freeY, color: ScottPlot.Colors.Red);
            scatter.LegendText = "Свободные вершины";
            scatter.LineStyle = LineStyle.None;
            scatter.MarkerSize = 5;
        }

        // Добавляем ребра для небольших графов
        if (graph.VertexCount <= 100)
        {
            AddEdgesToPlot(plot, graph, placement);
        }

        plot.ShowLegend();

        // Сохраняем в память как PNG
        return plot.GetImageBytes(width, height);
    }

    /// <summary>
    /// Добавляет ребра графа на график (только для небольших графов).
    /// </summary>
    private void AddEdgesToPlot(Plot plot, Graph graph, PlacementResult placement)
    {
        // Добавляем линии для каждого ребра
        foreach (var edge in graph.Edges)
        {
            var sourceIdx = edge.From - 1; // нумерация с 0 в массиве
            var targetIdx = edge.To - 1;

            if (sourceIdx < 0 || sourceIdx >= placement.XCoordinates.Length ||
                targetIdx < 0 || targetIdx >= placement.XCoordinates.Length)
                continue;

            var startX = placement.XCoordinates[sourceIdx];
            var startY = placement.YCoordinates[sourceIdx];
            var endX = placement.XCoordinates[targetIdx];
            var endY = placement.YCoordinates[targetIdx];

            var xs = new[] { startX, endX };
            var ys = new[] { startY, endY };

            var line = plot.Add.ScatterLine(xs, ys, color: ScottPlot.Colors.Gray.WithAlpha(0.3f));
            line.LineWidth = 1;
        }
    }

    /// <summary>
    /// Создает простой график размещения без ребер.
    /// </summary>
    /// <param name="placement">Результат размещения вершин</param>
    /// <param name="fixedVertexIds">Множество идентификаторов фиксированных вершин</param>
    /// <param name="width">Ширина изображения в пикселях</param>
    /// <param name="height">Высота изображения в пикселях</param>
    /// <returns>Массив байт PNG изображения</returns>
    public byte[] GenerateSimpleScatterPlot(
        PlacementResult placement,
        HashSet<int> fixedVertexIds,
        int width = 800,
        int height = 600)
    {
        var plot = new Plot();
        plot.Title("Размещение вершин графа");
        plot.XLabel("X координата");
        plot.YLabel("Y координата");

        // Разделяем вершины на фиксированные и свободные
        var fixedX = new List<double>();
        var fixedY = new List<double>();
        var freeX = new List<double>();
        var freeY = new List<double>();

        for (int i = 0; i < placement.XCoordinates.Length; i++)
        {
            var vertexId = i + 1; // нумерация с 1
            var x = placement.XCoordinates[i];
            var y = placement.YCoordinates[i];

            if (fixedVertexIds.Contains(vertexId))
            {
                fixedX.Add(x);
                fixedY.Add(y);
            }
            else
            {
                freeX.Add(x);
                freeY.Add(y);
            }
        }

        // Добавляем фиксированные вершины (синие)
        if (fixedX.Count > 0)
        {
            var scatter = plot.Add.Scatter(fixedX.ToArray(), fixedY.ToArray(), color: ScottPlot.Colors.Blue);
            scatter.LegendText = "Фиксированные вершины";
            scatter.LineStyle = LineStyle.None;
            scatter.MarkerSize = 8;
        }

        // Добавляем свободные вершины (красные)
        if (freeX.Count > 0)
        {
            var scatter = plot.Add.Scatter(freeX.ToArray(), freeY.ToArray(), color: ScottPlot.Colors.Red);
            scatter.LegendText = "Свободные вершины";
            scatter.LineStyle = LineStyle.None;
            scatter.MarkerSize = 5;
        }

        plot.ShowLegend();

        // Сохраняем в память как PNG
        return plot.GetImageBytes(width, height);
    }
}
