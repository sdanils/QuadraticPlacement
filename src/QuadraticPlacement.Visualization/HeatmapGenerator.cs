using ScottPlot;
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Visualization;

/// <summary>
/// Генератор тепловых карт для визуализации плотности вершин в больших графах.
/// Вычисляет 2D гистограмму и отображает её с использованием цветовой карты.
/// </summary>
public class HeatmapGenerator
{
    /// <summary>
    /// Создает тепловую карту распределения вершин.
    /// </summary>
    /// <param name="placement">Результат размещения вершин</param>
    /// <param name="gridSize">Размер сетки для гистограммы (gridSize x gridSize)</param>
    /// <param name="width">Ширина изображения в пикселях</param>
    /// <param name="height">Высота изображения в пикселях</param>
    /// <returns>Массив байт PNG изображения</returns>
    public byte[] GenerateHeatmap(
        PlacementResult placement,
        int gridSize = 50,
        int width = 800,
        int height = 600)
    {
        var plot = new Plot();
        plot.Title("Тепловая карта плотности вершин");
        plot.XLabel("X координата");
        plot.YLabel("Y координата");

        // Вычисляем 2D гистограмму
        var (heatmap, xEdges, yEdges) = Compute2DHistogram(placement, gridSize);

        // Добавляем тепловую карту на график
        var heatmapPlot = plot.Add.Heatmap(heatmap);
        heatmapPlot.Colormap = new ScottPlot.Colormaps.Turbo();

        return plot.GetImageBytes(width, height);
    }

    /// <summary>
    /// Вычисляет 2D гистограмму распределения вершин.
    /// </summary>
    private (double[,] heatmap, double[] xEdges, double[] yEdges) Compute2DHistogram(
        PlacementResult placement,
        int gridSize)
    {
        if (placement.XCoordinates.Length == 0)
        {
            var empty = new double[gridSize, gridSize];
            var emptyEdges = new double[gridSize + 1];
            return (empty, emptyEdges, emptyEdges);
        }

        // Находим границы
        var xValues = placement.XCoordinates;
        var yValues = placement.YCoordinates;

        var minX = xValues.Min();
        var maxX = xValues.Max();
        var minY = yValues.Min();
        var maxY = yValues.Max();

        // Добавляем небольшой отступ
        var padding = 0.1;
        var rangeX = maxX - minX;
        var rangeY = maxY - minY;

        if (rangeX == 0) rangeX = 1.0;
        if (rangeY == 0) rangeY = 1.0;

        var paddedMinX = minX - rangeX * padding;
        var paddedMaxX = maxX + rangeX * padding;
        var paddedMinY = minY - rangeY * padding;
        var paddedMaxY = maxY + rangeY * padding;

        // Создаем границы ячеек
        var xEdges = new double[gridSize + 1];
        var yEdges = new double[gridSize + 1];

        for (int i = 0; i <= gridSize; i++)
        {
            xEdges[i] = paddedMinX + (paddedMaxX - paddedMinX) * i / gridSize;
            yEdges[i] = paddedMinY + (paddedMaxY - paddedMinY) * i / gridSize;
        }

        // Вычисляем гистограмму
        var heatmap = new double[gridSize, gridSize];

        for (int i = 0; i < placement.XCoordinates.Length; i++)
        {
            var x = placement.XCoordinates[i];
            var y = placement.YCoordinates[i];

            // Находим индекс ячейки
            var xIdx = (int)((x - paddedMinX) / (paddedMaxX - paddedMinX) * gridSize);
            var yIdx = (int)((y - paddedMinY) / (paddedMaxY - paddedMinY) * gridSize);

            // Ограничиваем индексы
            xIdx = Math.Max(0, Math.Min(gridSize - 1, xIdx));
            yIdx = Math.Max(0, Math.Min(gridSize - 1, yIdx));

            heatmap[yIdx, xIdx]++;
        }

        return (heatmap, xEdges, yEdges);
    }

    /// <summary>
    /// Создает тепловую карту с отдельными слоями для фиксированных и свободных вершин.
    /// </summary>
    /// <param name="graph">Граф с информацией о фиксированных вершинах</param>
    /// <param name="placement">Результат размещения вершин</param>
    /// <param name="gridSize">Размер сетки для гистограммы</param>
    /// <param name="width">Ширина изображения</param>
    /// <param name="height">Высота изображения</param>
    /// <returns>Массив байт PNG изображения</returns>
    public byte[] GenerateLayeredHeatmap(
        Graph graph,
        PlacementResult placement,
        int gridSize = 50,
        int width = 800,
        int height = 600)
    {
        var plot = new Plot();
        plot.Title("Тепловая карта (синие - фиксированные, цвета - свободные)");
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

            if (graph.FixedVertices.ContainsKey(vertexId))
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

        // Создаем PlacementResult только для свободных вершин
        if (freeX.Count > 0)
        {
            var freePlacement = new PlacementResult(
                freeX.ToArray(),
                freeY.ToArray(),
                placement.Metrics,
                placement.ComputationTime);

            var (heatmap, _, _) = Compute2DHistogram(freePlacement, gridSize);

            var heatmapPlot = plot.Add.Heatmap(heatmap);
            heatmapPlot.Colormap = new ScottPlot.Colormaps.Turbo();
        }

        // Добавляем фиксированные вершины как синие точки
        if (fixedX.Count > 0)
        {
            var scatter = plot.Add.Scatter(fixedX.ToArray(), fixedY.ToArray(), color: ScottPlot.Colors.Blue);
            scatter.LineStyle = LineStyle.None;
            scatter.MarkerSize = 10;
            scatter.LegendText = "Фиксированные вершины";
            plot.ShowLegend();
        }

        return plot.GetImageBytes(width, height);
    }
}
