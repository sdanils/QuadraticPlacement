using System.Text;
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Reporting;

/// <summary>
/// Генератор HTML отчетов о работе алгоритма квадратичного размещения.
/// Создает структурированные HTML отчеты с встроенными визуализациями и таблицами метрик.
/// </summary>
public class HtmlReportBuilder
{
    private readonly StringBuilder _html = new();

    /// <summary>
    /// Создает полный HTML отчет на основе данных о запуске алгоритма.
    /// </summary>
    public string BuildReport(AlgorithmRun run)
    {
        _html.Clear();

        AppendHtmlStart();
        AppendHead(run.Metadata);
        AppendBodyStart();
        AppendHeader(run.Metadata);
        AppendSummary(run);
        AppendMetadata(run.Metadata);
        AppendMetrics(run);
        AppendVisualizations(run);
        AppendGraphStatistics(run.Result);
        AppendObjectiveHistory(run);
        AppendMessages(run);
        AppendBodyEnd();
        AppendHtmlEnd();

        return _html.ToString();
    }

    /// <summary>
    /// Добавляет начало HTML документа.
    /// </summary>
    private void AppendHtmlStart()
    {
        _html.AppendLine("<!DOCTYPE html>");
        _html.AppendLine("<html lang=\"ru\">");
    }

    /// <summary>
    /// Добавляет заголовок HTML документа с CSS стилями.
    /// </summary>
    private void AppendHead(ReportMetadata metadata)
    {
        _html.AppendLine("<head>");
        _html.AppendLine("    <meta charset=\"UTF-8\">");
        _html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        _html.AppendLine($"    <title>{EscapeHtml(metadata.GraphName)} - Отчет о размещении</title>");
        _html.AppendLine("    <style>");
        _html.AppendLine(GetCssStyles());
        _html.AppendLine("    </style>");
        _html.AppendLine("</head>");
    }

    /// <summary>
    /// Возвращает CSS стили для оформления отчета.
    /// </summary>
    private string GetCssStyles()
    {
        return @"
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 20px;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
            background: white;
            border-radius: 10px;
            box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }

        header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }

        header h1 {
            font-size: 2.5em;
            margin-bottom: 10px;
            text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.2);
        }

        header .subtitle {
            font-size: 1.1em;
            opacity: 0.9;
        }

        .content {
            padding: 30px;
        }

        .section {
            margin-bottom: 40px;
        }

        .section h2 {
            color: #667eea;
            border-bottom: 3px solid #667eea;
            padding-bottom: 10px;
            margin-bottom: 20px;
            font-size: 1.8em;
        }

        .section h3 {
            color: #764ba2;
            margin: 20px 0 10px 0;
            font-size: 1.3em;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }

        table thead {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }

        table th, table td {
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid #ddd;
        }

        table tbody tr:hover {
            background-color: #f5f5f5;
        }

        table tbody tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        .metric-card {
            display: inline-block;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px;
            margin: 10px;
            border-radius: 8px;
            min-width: 200px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
            transition: transform 0.3s ease;
        }

        .metric-card:hover {
            transform: translateY(-5px);
        }

        .metric-card .label {
            font-size: 0.9em;
            opacity: 0.9;
            margin-bottom: 5px;
        }

        .metric-card .value {
            font-size: 2em;
            font-weight: bold;
        }

        .metric-card .unit {
            font-size: 0.8em;
            opacity: 0.8;
        }

        .visualization {
            text-align: center;
            margin: 20px 0;
        }

        .visualization img {
            max-width: 100%;
            height: auto;
            border-radius: 8px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
        }

        .visualization-caption {
            margin-top: 10px;
            font-style: italic;
            color: #666;
        }

        .message-log {
            background: #f8f9fa;
            border-left: 4px solid #667eea;
            padding: 15px;
            margin: 20px 0;
            border-radius: 4px;
        }

        .message-log pre {
            white-space: pre-wrap;
            word-wrap: break-word;
            font-family: 'Courier New', monospace;
            font-size: 0.9em;
            color: #333;
        }

        .success {
            color: #28a745;
            font-weight: bold;
        }

        .error {
            color: #dc3545;
            font-weight: bold;
        }

        .warning {
            color: #ffc107;
            font-weight: bold;
        }

        footer {
            background: #f8f9fa;
            padding: 20px;
            text-align: center;
            color: #666;
            font-size: 0.9em;
        }

        .grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
            margin: 20px 0;
        }

        .info-box {
            background: #e7f3ff;
            border-left: 4px solid #2196F3;
            padding: 15px;
            margin: 10px 0;
            border-radius: 4px;
        }

        .info-box strong {
            color: #2196F3;
        }
        ";
    }

    /// <summary>
    /// Добавляет начало тела HTML документа.
    /// </summary>
    private void AppendBodyStart()
    {
        _html.AppendLine("<body>");
        _html.AppendLine("    <div class=\"container\">");
    }

    /// <summary>
    /// Добавляет заголовок отчета.
    /// </summary>
    private void AppendHeader(ReportMetadata metadata)
    {
        _html.AppendLine("        <header>");
        _html.AppendLine($"            <h1>{EscapeHtml(metadata.GraphName)}</h1>");
        _html.AppendLine($"            <div class=\"subtitle\">Отчет о квадратичном размещении вершин</div>");
        _html.AppendLine("        </header>");
        _html.AppendLine("        <div class=\"content\">");
    }

    /// <summary>
    /// Добавляет краткую сводку о запуске.
    /// </summary>
    private void AppendSummary(AlgorithmRun run)
    {
        _html.AppendLine("            <div class=\"section\">");
        _html.AppendLine("                <h2>Сводка</h2>");
        _html.AppendLine("                <div class=\"grid\">");

        var stats = run.GetObjectiveStatistics();

        AppendMetricCard("Вершин", run.Result.XCoordinates.Length.ToString(), "шт");
        AppendMetricCard("Итераций", run.Iterations.ToString(), "шт");
        AppendMetricCard("Время выполнения", FormatTime(run.ExecutionTimeMs), "");
        AppendMetricCard("Начальная функция", stats.Initial.ToString("F4"), "");
        AppendMetricCard("Финальная функция", stats.Final.ToString("F4"), "");
        AppendMetricCard("Улучшение", $"{stats.ImprovementPercentage:F1}", "%");
        AppendMetricCard("Статус", run.Success ? "Успех" : "Ошибка", "");

        _html.AppendLine("                </div>");
        _html.AppendLine("            </div>");
    }

    /// <summary>
    /// Добавляет карточку метрики.
    /// </summary>
    private void AppendMetricCard(string label, string value, string unit)
    {
        _html.AppendLine("                    <div class=\"metric-card\">");
        _html.AppendLine($"                        <div class=\"label\">{EscapeHtml(label)}</div>");
        _html.AppendLine($"                        <div class=\"value\">{EscapeHtml(value)}</div>");
        if (!string.IsNullOrEmpty(unit))
        {
            _html.AppendLine($"                        <div class=\"unit\">{EscapeHtml(unit)}</div>");
        }
        _html.AppendLine("                    </div>");
    }

    /// <summary>
    /// Добавляет секцию с метаданными.
    /// </summary>
    private void AppendMetadata(ReportMetadata metadata)
    {
        _html.AppendLine("            <div class=\"section\">");
        _html.AppendLine("                <h2>Метаданные</h2>");
        _html.AppendLine("                <table>");
        _html.AppendLine("                    <thead>");
        _html.AppendLine("                        <tr>");
        _html.AppendLine("                            <th>Параметр</th>");
        _html.AppendLine("                            <th>Значение</th>");
        _html.AppendLine("                        </tr>");
        _html.AppendLine("                    </thead>");
        _html.AppendLine("                    <tbody>");

        AppendTableRow("ID отчета", metadata.ReportId);
        AppendTableRow("Дата создания", metadata.GeneratedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        AppendTableRow("Алгоритм", $"{metadata.AlgorithmName} v{metadata.AlgorithmVersion}");
        AppendTableRow("Автор", metadata.Author);
        AppendTableRow("Описание", metadata.Description);

        _html.AppendLine("                    </tbody>");
        _html.AppendLine("                </table>");

        if (metadata.Parameters.Count > 0)
        {
            _html.AppendLine("                <h3>Параметры алгоритма</h3>");
            _html.AppendLine("                <table>");
            _html.AppendLine("                    <thead>");
            _html.AppendLine("                        <tr>");
            _html.AppendLine("                            <th>Параметр</th>");
            _html.AppendLine("                            <th>Значение</th>");
            _html.AppendLine("                        </tr>");
            _html.AppendLine("                    </thead>");
            _html.AppendLine("                    <tbody>");

            foreach (var param in metadata.Parameters)
            {
                AppendTableRow(param.Key, param.Value);
            }

            _html.AppendLine("                    </tbody>");
            _html.AppendLine("                </table>");
        }

        _html.AppendLine("            </div>");
    }

    /// <summary>
    /// Добавляет строку в таблицу.
    /// </summary>
    private void AppendTableRow(string key, string value)
    {
        _html.AppendLine($"                        <tr>");
        _html.AppendLine($"                            <td>{EscapeHtml(key)}</td>");
        _html.AppendLine($"                            <td>{EscapeHtml(value)}</td>");
        _html.AppendLine($"                        </tr>");
    }

    /// <summary>
    /// Добавляет секцию с детальными метриками.
    /// </summary>
    private void AppendMetrics(AlgorithmRun run)
    {
        var stats = run.GetObjectiveStatistics();

        _html.AppendLine("            <div class=\"section\">");
        _html.AppendLine("                <h2>Метрики качества</h2>");
        _html.AppendLine("                <table>");
        _html.AppendLine("                    <thead>");
        _html.AppendLine("                        <tr>");
        _html.AppendLine("                            <th>Метрика</th>");
        _html.AppendLine("                            <th>Значение</th>");
        _html.AppendLine("                        </tr>");
        _html.AppendLine("                    </thead>");
        _html.AppendLine("                    <tbody>");

        AppendTableRow("Суммарная взвешенная длина", run.Result.Metrics.TotalWeightedLength.ToString("F4"));
        AppendTableRow("Средняя длина ребра", run.Result.Metrics.AverageEdgeLength.ToString("F4"));
        AppendTableRow("Максимальная длина ребра", run.Result.Metrics.MaxEdgeLength.ToString("F4"));
        AppendTableRow("Минимальная длина ребра", run.Result.Metrics.MinEdgeLength.ToString("F4"));
        AppendTableRow("Минимальное значение", stats.Minimum.ToString("F4"));
        AppendTableRow("Максимальное значение", stats.Maximum.ToString("F4"));
        AppendTableRow("Среднее значение", stats.Average.ToString("F4"));
        AppendTableRow("Медиана", stats.Median.ToString("F4"));
        AppendTableRow("Стандартное отклонение", stats.StandardDeviation.ToString("F4"));
        AppendTableRow("Улучшение", $"{stats.ImprovementPercentage:F2}%");

        _html.AppendLine("                    </tbody>");
        _html.AppendLine("                </table>");
        _html.AppendLine("            </div>");
    }

    /// <summary>
    /// Добавляет секцию с визуализациями.
    /// </summary>
    private void AppendVisualizations(AlgorithmRun run)
    {
        if (run.Visualizations.Count == 0)
        {
            return;
        }

        _html.AppendLine("            <div class=\"section\">");
        _html.AppendLine("                <h2>Визуализации</h2>");

        foreach (var viz in run.Visualizations)
        {
            _html.AppendLine("                <div class=\"visualization\">");
            _html.AppendLine($"                    <img src=\"data:image/png;base64,{Convert.ToBase64String(viz.Value)}\" alt=\"{EscapeHtml(viz.Key)}\">");
            _html.AppendLine($"                    <div class=\"visualization-caption\">{EscapeHtml(viz.Key)}</div>");
            _html.AppendLine("                </div>");
        }

        _html.AppendLine("            </div>");
    }

    /// <summary>
    /// Добавляет статистику графа.
    /// </summary>
    private void AppendGraphStatistics(PlacementResult result)
    {
        // Поскольку PlacementResult не содержит Graph, пропускаем этот раздел
        // Граф должен передаваться отдельно через AlgorithmRun.CustomData или Metadata
    }

    /// <summary>
    /// Добавляет историю значений целевой функции.
    /// </summary>
    private void AppendObjectiveHistory(AlgorithmRun run)
    {
        if (run.ObjectiveHistory.Count == 0)
        {
            return;
        }

        _html.AppendLine("            <div class=\"section\">");
        _html.AppendLine("                <h2>История целевой функции</h2>");
        _html.AppendLine("                <div class=\"info-box\">");
        _html.AppendLine($"                    <strong>Первые 10 значений:</strong><br>");
        _html.AppendLine($"                    {string.Join(", ", run.ObjectiveHistory.Take(10).Select(v => v.ToString("F4")))}");
        _html.AppendLine("                </div>");

        if (run.ObjectiveHistory.Count > 10)
        {
            _html.AppendLine("                <div class=\"info-box\">");
            _html.AppendLine($"                    <strong>Последние 10 значений:</strong><br>");
            _html.AppendLine($"                    {string.Join(", ", run.ObjectiveHistory.TakeLast(10).Select(v => v.ToString("F4")))}");
            _html.AppendLine("                </div>");
        }

        _html.AppendLine("            </div>");
    }

    /// <summary>
    /// Добавляет сообщения лога.
    /// </summary>
    private void AppendMessages(AlgorithmRun run)
    {
        if (run.Messages.Count == 0)
        {
            return;
        }

        _html.AppendLine("            <div class=\"section\">");
        _html.AppendLine("                <h2>Сообщения</h2>");
        _html.AppendLine("                <div class=\"message-log\">");
        _html.AppendLine("                    <pre>");

        foreach (var message in run.Messages)
        {
            var cssClass = message.Contains("ОШИБКА") ? "error" :
                          message.Contains("ПРЕДУПРЕЖДЕНИЕ") ? "warning" :
                          message.Contains("Успех") ? "success" : "";
            _html.AppendLine($"                        <span class=\"{cssClass}\">{EscapeHtml(message)}</span>");
        }

        _html.AppendLine("                    </pre>");
        _html.AppendLine("                </div>");
        _html.AppendLine("            </div>");
    }

    /// <summary>
    /// Добавляет конец тела HTML документа.
    /// </summary>
    private void AppendBodyEnd()
    {
        _html.AppendLine("        </div>");
        _html.AppendLine("        <footer>");
        _html.AppendLine("            <p>Сгенерировано системой Quadratic Placement");
        _html.AppendLine($"               {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
        _html.AppendLine("        </footer>");
        _html.AppendLine("    </div>");
    }

    /// <summary>
    /// Добавляет конец HTML документа.
    /// </summary>
    private void AppendHtmlEnd()
    {
        _html.AppendLine("</body>");
        _html.AppendLine("</html>");
    }

    /// <summary>
    /// Экранирует HTML символы в строке.
    /// </summary>
    private string EscapeHtml(string text)
    {
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }

    /// <summary>
    /// Форматирует время в человекочитаемый формат.
    /// </summary>
    private string FormatTime(long milliseconds)
    {
        if (milliseconds < 1000)
        {
            return $"{milliseconds} мс";
        }
        else if (milliseconds < 60000)
        {
            return $"{milliseconds / 1000.0:F1} с";
        }
        else
        {
            var minutes = milliseconds / 60000;
            var seconds = (milliseconds % 60000) / 1000.0;
            return $"{minutes} мин {seconds:F1} с";
        }
    }

    /// <summary>
    /// Сохраняет HTML отчет в файл.
    /// </summary>
    public void SaveToFile(AlgorithmRun run, string filePath)
    {
        var html = BuildReport(run);
        File.WriteAllText(filePath, html, Encoding.UTF8);
    }

    /// <summary>
    /// Создает HTML отчет и возвращает его как строку.
    /// </summary>
    public string GenerateReport(AlgorithmRun run)
    {
        return BuildReport(run);
    }
}
