using QuadraticPlacement.Algorithms;
using QuadraticPlacement.Core;
using QuadraticPlacement.Reporting;
using QuadraticPlacement.Visualization;

namespace QuadraticPlacement.CLI;

/// <summary>
/// Координирует выполнение всех шагов для создания отчёта
/// </summary>
public class ReportOrchestrator
{
    private readonly BasicSolver _basicSolver = new();
    private readonly HeuristicSolver _heuristicSolver = new();
    private readonly ScatterPlotGenerator _scatterGenerator = new();
    private readonly HeatmapGenerator _heatmapGenerator = new();
    private readonly HtmlReportBuilder _reportBuilder = new();

    /// <summary>
    /// Запускает оба алгоритма и создаёт полный HTML отчёт
    /// </summary>
    public void GenerateFullReport(
        string graphPath,
        string outputPath,
        bool generateVisualizations = true)
    {
        Console.WriteLine("Загрузка графа...");

        // Загружаем граф из файла
        var graphContent = File.ReadAllText(graphPath);
        Graph graph;

        if (graphPath.EndsWith(".json"))
        {
            graph = Data.JsonGraphParser.Parse(graphContent);
        }
        else
        {
            graph = Data.TextGraphParser.Parse(graphContent);
        }

        Console.WriteLine($"Граф загружен: {graph.VertexCount} вершин, {graph.EdgeCount} рёбер");

        // Запускаем базовый алгоритм
        Console.WriteLine("\nЗапуск базового алгоритма...");
        var basicResult = _basicSolver.Solve(graph);
        Console.WriteLine($"Базовый алгоритм завершён за {basicResult.ComputationTime.TotalSeconds:F2} сек");
        Console.WriteLine($"Суммарная длина: {basicResult.Metrics.TotalWeightedLength:F2}");

        // Создаём отчёт для базового алгоритма
        var basicRun = new AlgorithmRun
        {
            Metadata = new ReportMetadata
            {
                AlgorithmName = "Базовый алгоритм (матрица Лапласа)",
                GraphName = graphPath
            },
            Result = basicResult,
            ExecutionTimeMs = (long)basicResult.ComputationTime.TotalMilliseconds
        };

        // Генерируем визуализацию для базового алгоритма
        if (generateVisualizations)
        {
            Console.WriteLine("Генерация визуализации базового алгоритма...");
            var basicScatter = _scatterGenerator.GenerateScatterPlot(graph, basicResult);
            var basicHeatmap = _heatmapGenerator.GenerateHeatmap(basicResult);
            basicRun.AddVisualization("scatter", basicScatter);
            basicRun.AddVisualization("heatmap", basicHeatmap);
        }

        // Запускаем эвристический алгоритм
        Console.WriteLine("\nЗапуск эвристического алгоритма...");
        var heuristicResult = _heuristicSolver.Solve(graph);
        Console.WriteLine($"Эвристический алгоритм завершён за {heuristicResult.ComputationTime.TotalSeconds:F2} сек");
        Console.WriteLine($"Суммарная длина: {heuristicResult.Metrics.TotalWeightedLength:F2}");

        // Создаём отчёт для эвристического алгоритма
        var heuristicRun = new AlgorithmRun
        {
            Metadata = new ReportMetadata
            {
                AlgorithmName = "Эвристический алгоритм (силовой метод)",
                GraphName = graphPath
            },
            Result = heuristicResult,
            ExecutionTimeMs = (long)heuristicResult.ComputationTime.TotalMilliseconds
        };

        // Генерируем визуализацию для эвристического алгоритма
        if (generateVisualizations)
        {
            Console.WriteLine("Генерация визуализации эвристического алгоритма...");
            var heuristicScatter = _scatterGenerator.GenerateScatterPlot(graph, heuristicResult);
            var heuristicHeatmap = _heatmapGenerator.GenerateHeatmap(heuristicResult);
            heuristicRun.AddVisualization("scatter", heuristicScatter);
            heuristicRun.AddVisualization("heatmap", heuristicHeatmap);
        }

        // Создаём HTML отчёты
        Console.WriteLine("\nСоздание HTML отчётов...");

        var basicHtml = _reportBuilder.BuildReport(basicRun);
        var basicReportPath = outputPath.Replace(".html", "_basic.html");
        File.WriteAllText(basicReportPath, basicHtml, System.Text.Encoding.UTF8);
        Console.WriteLine($"Отчёт базового алгоритма сохранён в: {basicReportPath}");

        var heuristicHtml = _reportBuilder.BuildReport(heuristicRun);
        var heuristicReportPath = outputPath.Replace(".html", "_heuristic.html");
        File.WriteAllText(heuristicReportPath, heuristicHtml, System.Text.Encoding.UTF8);
        Console.WriteLine($"Отчёт эвристического алгоритма сохранён в: {heuristicReportPath}");

        // Создаём сравнительный отчёт
        var comparisonReportPath = outputPath;
        CreateComparisonReport(basicRun, heuristicRun, graphPath, comparisonReportPath);
        Console.WriteLine($"Сравнительный отчёт сохранён в: {comparisonReportPath}");

        // Выводим сводку
        Console.WriteLine("\n=== Сводка результатов ===");
        Console.WriteLine($"Базовый:   {basicResult.ComputationTime.TotalSeconds:F2} сек, длина = {basicResult.Metrics.TotalWeightedLength:F2}");
        Console.WriteLine($"Эвристика: {heuristicResult.ComputationTime.TotalSeconds:F2} сек, длина = {heuristicResult.Metrics.TotalWeightedLength:F2}");

        double diff = Math.Abs(basicResult.Metrics.TotalWeightedLength - heuristicResult.Metrics.TotalWeightedLength);
        double pct = (diff / basicResult.Metrics.TotalWeightedLength) * 100;
        Console.WriteLine($"\nОтличие: {pct:F2}%");
    }

    /// <summary>
    /// Создаёт простой сравнительный HTML отчёт
    /// </summary>
    private void CreateComparisonReport(AlgorithmRun basic, AlgorithmRun heuristic, string graphPath, string outputPath)
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang='ru'>");
        sb.AppendLine("<head>");
        sb.AppendLine("  <meta charset='UTF-8'>");
        sb.AppendLine("  <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine("  <title>Сравнительный отчёт - Квадратичное размещение</title>");
        sb.AppendLine(GetCssStyles());
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("  <div class='container'>");
        sb.AppendLine("    <h1>Сравнительный анализ алгоритмов размещения</h1>");

        // Метаданные
        sb.AppendLine($"    <div class='metadata-box'>");
        sb.AppendLine($"      <h3>Информация о графе</h3>");
        sb.AppendLine($"      <p><strong>Источник:</strong> {graphPath}</p>");
        sb.AppendLine($"      <p><strong>Вершин:</strong> {basic.Result.XCoordinates.Length}</p>");
        sb.AppendLine($"      <p><strong>Время генерации:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
        sb.AppendLine($"    </div>");

        // Сравнительная таблица
        bool basicFaster = basic.Result.ComputationTime < heuristic.Result.ComputationTime;
        bool basicBetter = basic.Result.Metrics.TotalWeightedLength < heuristic.Result.Metrics.TotalWeightedLength;

        sb.AppendLine("    <h2>Сравнение метрик</h2>");
        sb.AppendLine("    <table>");
        sb.AppendLine("      <tr>");
        sb.AppendLine("        <th>Метрика</th>");
        sb.AppendLine("        <th>Базовый алгоритм</th>");
        sb.AppendLine("        <th>Эвристический алгоритм</th>");
        sb.AppendLine("        <th>Лучший</th>");
        sb.AppendLine("      </tr>");
        sb.AppendLine("      <tr>");
        sb.AppendLine($"        <td>Время вычисления</td>");
        sb.AppendLine($"        <td class='metric-value {(basicFaster ? "best-value" : "")}'>{basic.Result.ComputationTime.TotalSeconds:F2} сек</td>");
        sb.AppendLine($"        <td class='metric-value {(!basicFaster ? "best-value" : "")}'>{heuristic.Result.ComputationTime.TotalSeconds:F2} сек</td>");
        sb.AppendLine($"        <td>{(basicFaster ? "Базовый" : "Эвристический")}</td>");
        sb.AppendLine("      </tr>");
        sb.AppendLine("      <tr>");
        sb.AppendLine($"        <td>Суммарная длина рёбер</td>");
        sb.AppendLine($"        <td class='metric-value {(basicBetter ? "best-value" : "")}'>{basic.Result.Metrics.TotalWeightedLength:F2}</td>");
        sb.AppendLine($"        <td class='metric-value {(!basicBetter ? "best-value" : "")}'>{heuristic.Result.Metrics.TotalWeightedLength:F2}</td>");
        sb.AppendLine($"        <td>{(basicBetter ? "Базовый" : "Эвристический")}</td>");
        sb.AppendLine("      </tr>");
        sb.AppendLine("      <tr>");
        sb.AppendLine($"        <td>Максимальная длина ребра</td>");
        sb.AppendLine($"        <td class='metric-value'>{basic.Result.Metrics.MaxEdgeLength:F2}</td>");
        sb.AppendLine($"        <td class='metric-value'>{heuristic.Result.Metrics.MaxEdgeLength:F2}</td>");
        sb.AppendLine($"        <td>-</td>");
        sb.AppendLine("      </tr>");
        sb.AppendLine("      <tr>");
        sb.AppendLine($"        <td>Средняя длина ребра</td>");
        sb.AppendLine($"        <td class='metric-value'>{basic.Result.Metrics.AverageEdgeLength:F2}</td>");
        sb.AppendLine($"        <td class='metric-value'>{heuristic.Result.Metrics.AverageEdgeLength:F2}</td>");
        sb.AppendLine($"        <td>-</td>");
        sb.AppendLine("      </tr>");
        sb.AppendLine("    </table>");

        // Ссылки на полные отчёты
        sb.AppendLine("    <div class='analysis-box'>");
        sb.AppendLine("      <h3>Полные отчёты</h3>");
        sb.AppendLine("      <p><a href='_basic.html'>Отчёт базового алгоритма</a></p>");
        sb.AppendLine("      <p><a href='_heuristic.html'>Отчёт эвристического алгоритма</a></p>");
        sb.AppendLine("    </div>");

        // Анализ
        double totalLengthDiff = Math.Abs(
            basic.Result.Metrics.TotalWeightedLength -
            heuristic.Result.Metrics.TotalWeightedLength);
        double totalLengthPct = (totalLengthDiff / basic.Result.Metrics.TotalWeightedLength) * 100;

        double timeDiff = (heuristic.Result.ComputationTime - basic.Result.ComputationTime).TotalSeconds;

        sb.AppendLine("    <div class='analysis-box'>");
        sb.AppendLine("      <h3>Анализ и выводы</h3>");
        sb.AppendLine($"      <p><strong>Отличие в суммарной длине:</strong> {totalLengthPct:F2}%</p>");
        sb.AppendLine($"      <p><strong>Разница во времени:</strong> {Math.Abs(timeDiff):F2} сек {(timeDiff > 0 ? "(эвристический медленнее)" : "(эвристический быстрее)")}</p>");
        sb.AppendLine($"      <p>Базовый алгоритм обеспечивает {(basicBetter ? "лучшую" : "худшую")} оптимизацию " +
                      $"и работает {(basicFaster ? "быстрее" : "медленнее")}.</p>");
        sb.AppendLine("    </div>");

        sb.AppendLine("  </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        File.WriteAllText(outputPath, sb.ToString(), System.Text.Encoding.UTF8);
    }

    private string GetCssStyles()
    {
        return @"
  <style>
    body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 20px; background: #f5f5f5; }
    .container { max-width: 1200px; margin: 0 auto; background: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
    h1 { color: #333; border-bottom: 3px solid #007acc; padding-bottom: 10px; }
    h2 { color: #555; margin-top: 30px; }
    table { width: 100%; border-collapse: collapse; margin: 20px 0; }
    th, td { padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }
    th { background: #007acc; color: white; }
    tr:hover { background: #f5f5f5; }
    .metric-value { font-weight: bold; color: #007acc; }
    .best-value { background: #e6ffe6; }
    .metadata-box { background: #f9f9f9; padding: 15px; border-radius: 4px; margin: 20px 0; }
    .analysis-box { background: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0; }
    a { color: #007acc; text-decoration: none; }
    a:hover { text-decoration: underline; }
  </style>";
    }
}
