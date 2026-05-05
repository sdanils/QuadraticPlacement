namespace QuadraticPlacement.Reporting;

/// <summary>
/// Метаданные отчета о работе алгоритма квадратичного размещения.
/// Содержит информацию о времени создания, параметрах алгоритма и т.д.
/// </summary>
public class ReportMetadata
{
    /// <summary>
    /// Уникальный идентификатор отчета.
    /// </summary>
    public string ReportId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Время и дата создания отчета.
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Название графа или задачи.
    /// </summary>
    public string GraphName { get; set; } = string.Empty;

    /// <summary>
    /// Описание задачи или графа.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Название использованного алгоритма.
    /// </summary>
    public string AlgorithmName { get; set; } = string.Empty;

    /// <summary>
    /// Версия алгоритма.
    /// </summary>
    public string AlgorithmVersion { get; set; } = "1.0";

    /// <summary>
    /// Имя пользователя или системы, запустившей алгоритм.
    /// </summary>
    public string Author { get; set; } = Environment.UserName;

    /// <summary>
    /// Дополнительные параметры алгоритма в формате ключ-значение.
    /// </summary>
    public Dictionary<string, string> Parameters { get; set; } = new();

    /// <summary>
    /// Теги или категории для группировки отчетов.
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Произвольные дополнительные метаданные.
    /// </summary>
    public Dictionary<string, object> CustomData { get; set; } = new();

    /// <summary>
    /// Создает копию текущего объекта метаданных.
    /// </summary>
    public ReportMetadata Clone()
    {
        return new ReportMetadata
        {
            ReportId = ReportId,
            GeneratedAt = GeneratedAt,
            GraphName = GraphName,
            Description = Description,
            AlgorithmName = AlgorithmName,
            AlgorithmVersion = AlgorithmVersion,
            Author = Author,
            Parameters = new Dictionary<string, string>(Parameters),
            Tags = new List<string>(Tags),
            CustomData = new Dictionary<string, object>(CustomData)
        };
    }

    /// <summary>
    /// Преобразует метаданные в строковое представление для включения в отчет.
    /// </summary>
    public override string ToString()
    {
        var parts = new List<string>
        {
            $"Отчет: {ReportId}",
            $"Создан: {GeneratedAt:yyyy-MM-dd HH:mm:ss}",
            $"Алгоритм: {AlgorithmName} v{AlgorithmVersion}",
            $"Автор: {Author}"
        };

        if (!string.IsNullOrEmpty(GraphName))
            parts.Add($"Граф: {GraphName}");

        if (Parameters.Count > 0)
            parts.Add($"Параметры: {string.Join(", ", Parameters.Select(kv => $"{kv.Key}={kv.Value}"))}");

        return string.Join(" | ", parts);
    }
}
