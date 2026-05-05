using QuadraticPlacement.Core;

namespace QuadraticPlacement.Reporting;

/// <summary>
/// Данные о запуске алгоритма квадратичного размещения для включения в отчеты.
/// Содержит результаты работы, метрики и визуализации.
/// </summary>
public class AlgorithmRun
{
    /// <summary>
    /// Метаданные запуска.
    /// </summary>
    public ReportMetadata Metadata { get; set; } = new();

    /// <summary>
    /// Результат размещения вершин.
    /// </summary>
    public PlacementResult Result { get; set; } = null!;

    /// <summary>
    /// Время выполнения алгоритма в миллисекундах.
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// Количество выполненных итераций.
    /// </summary>
    public int Iterations { get; set; }

    /// <summary>
    /// История значений целевой функции на каждой итерации.
    /// </summary>
    public List<double> ObjectiveHistory { get; set; } = new();

    /// <summary>
    /// История размещений на каждой итерации (для анимации).
    /// </summary>
    public List<PlacementResult> PlacementHistory { get; set; } = new();

    /// <summary>
    /// Сообщения об ошибках или предупреждения.
    /// </summary>
    public List<string> Messages { get; set; } = new();

    /// <summary>
    /// Успешно ли завершилась работа алгоритма.
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Визуализации в формате base64 (ключ - название изображения).
    /// </summary>
    public Dictionary<string, byte[]> Visualizations { get; set; } = new();

    /// <summary>
    /// Добавляет визуализацию в отчет.
    /// </summary>
    public void AddVisualization(string name, byte[] imageBytes)
    {
        Visualizations[name] = imageBytes;
    }

    /// <summary>
    /// Добавляет сообщение в лог.
    /// </summary>
    public void AddMessage(string message)
    {
        Messages.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
    }

    /// <summary>
    /// Добавляет предупреждение в лог.
    /// </summary>
    public void AddWarning(string warning)
    {
        Messages.Add($"[{DateTime.Now:HH:mm:ss}] ПРЕДУПРЕЖДЕНИЕ: {warning}");
    }

    /// <summary>
    /// Добавляет ошибку в лог и помечает запуск как неуспешный.
    /// </summary>
    public void AddError(string error)
    {
        Messages.Add($"[{DateTime.Now:HH:mm:ss}] ОШИБКА: {error}");
        Success = false;
    }

    /// <summary>
    /// Вычисляет статистику по истории целевой функции.
    /// </summary>
    public ObjectiveStatistics GetObjectiveStatistics()
    {
        if (ObjectiveHistory.Count == 0)
        {
            return new ObjectiveStatistics();
        }

        var values = ObjectiveHistory.ToArray();
        Array.Sort(values);

        return new ObjectiveStatistics
        {
            Initial = ObjectiveHistory[0],
            Final = ObjectiveHistory[^1],
            Minimum = values[0],
            Maximum = values[^1],
            Average = values.Average(),
            Median = values[values.Length / 2],
            StandardDeviation = CalculateStandardDeviation(values)
        };
    }

    /// <summary>
    /// Вычисляет стандартное отклонение.
    /// </summary>
    private double CalculateStandardDeviation(double[] values)
    {
        var avg = values.Average();
        var sumOfSquares = values.Sum(v => Math.Pow(v - avg, 2));
        return Math.Sqrt(sumOfSquares / values.Length);
    }

    /// <summary>
    /// Создает краткое резюме запуска.
    /// </summary>
    public string GetSummary()
    {
        var stats = GetObjectiveStatistics();
        return $"Алгоритм: {Metadata.AlgorithmName}, " +
               $"Вершин: {Result.XCoordinates.Length}, " +
               $"Итераций: {Iterations}, " +
               $"Время: {ExecutionTimeMs}мс, " +
               $"Целевая функция: {stats.Final:F4} (с {stats.Initial:F4})";
    }
}

/// <summary>
/// Статистика значений целевой функции.
/// </summary>
public class ObjectiveStatistics
{
    /// <summary>
    /// Начальное значение целевой функции.
    /// </summary>
    public double Initial { get; set; }

    /// <summary>
    /// Финальное значение целевой функции.
    /// </summary>
    public double Final { get; set; }

    /// <summary>
    /// Минимальное значение.
    /// </summary>
    public double Minimum { get; set; }

    /// <summary>
    /// Максимальное значение.
    /// </summary>
    public double Maximum { get; set; }

    /// <summary>
    /// Среднее значение.
    /// </summary>
    public double Average { get; set; }

    /// <summary>
    /// Медианное значение.
    /// </summary>
    public double Median { get; set; }

    /// <summary>
    /// Стандартное отклонение.
    /// </summary>
    public double StandardDeviation { get; set; }

    /// <summary>
    /// Улучшение в процентах.
    /// </summary>
    public double ImprovementPercentage => Initial > 0
        ? (Initial - Final) / Initial * 100
        : 0;
}
