using System.Text.Json;
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Data;

/// <summary>
/// Парсер JSON формата графа
/// </summary>
public class JsonGraphParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Парсит граф из JSON строки
    /// </summary>
    public static Graph Parse(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON строка пуста", nameof(json));

        try
        {
            var contract = JsonSerializer.Deserialize<GraphDataContract>(json, Options);
            if (contract == null)
                throw new FormatException("Не удалось десериализовать JSON: получен null");

            return contract.ToGraph();
        }
        catch (JsonException ex)
        {
            throw new FormatException($"Ошибка парсинга JSON: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Парсит граф из JSON файла
    /// </summary>
    public static Graph ParseFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл не найден: {filePath}");

        string json = File.ReadAllText(filePath);
        return Parse(json);
    }

    /// <summary>
    /// Преобразует граф в JSON строку
    /// </summary>
    public static string ToJson(Graph graph)
    {
        if (graph == null)
            throw new ArgumentNullException(nameof(graph));

        var contract = GraphDataContract.FromGraph(graph);
        return JsonSerializer.Serialize(contract, Options);
    }

    /// <summary>
    /// Сохраняет граф в JSON файл
    /// </summary>
    public static void SaveToFile(Graph graph, string filePath)
    {
        if (graph == null)
            throw new ArgumentNullException(nameof(graph));

        string json = ToJson(graph);

        // Убеждаемся, что директория существует
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, json);
    }
}
