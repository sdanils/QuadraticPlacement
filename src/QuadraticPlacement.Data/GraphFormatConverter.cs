namespace QuadraticPlacement.Data;

/// <summary>
/// Конвертер между текстовым и JSON форматами графа
/// </summary>
public static class GraphFormatConverter
{
    /// <summary>
    /// Конвертирует текстовый формат в JSON
    /// </summary>
    public static string TextToJson(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Входная строка пуста", nameof(text));

        var graph = TextGraphParser.Parse(text);
        return JsonGraphParser.ToJson(graph);
    }

    /// <summary>
    /// Конвертирует текстовый файл в JSON файл
    /// </summary>
    public static void TextFileToJsonFile(string textFilePath, string jsonFilePath)
    {
        if (string.IsNullOrWhiteSpace(textFilePath))
            throw new ArgumentException("Путь к текстовому файлу пуст", nameof(textFilePath));

        if (string.IsNullOrWhiteSpace(jsonFilePath))
            throw new ArgumentException("Путь к JSON файлу пуст", nameof(jsonFilePath));

        var graph = TextGraphParser.ParseFromFile(textFilePath);
        JsonGraphParser.SaveToFile(graph, jsonFilePath);
    }

    /// <summary>
    /// Конвертирует JSON формат в текстовый
    /// </summary>
    public static string JsonToText(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON строка пуста", nameof(json));

        var graph = JsonGraphParser.Parse(json);
        return TextGraphParser.ToText(graph);
    }

    /// <summary>
    /// Конвертирует JSON файл в текстовый файл
    /// </summary>
    public static void JsonFileToTextFile(string jsonFilePath, string textFilePath)
    {
        if (string.IsNullOrWhiteSpace(jsonFilePath))
            throw new ArgumentException("Путь к JSON файлу пуст", nameof(jsonFilePath));

        if (string.IsNullOrWhiteSpace(textFilePath))
            throw new ArgumentException("Путь к текстовому файлу пуст", nameof(textFilePath));

        var graph = JsonGraphParser.ParseFromFile(jsonFilePath);
        TextGraphParser.SaveToFile(graph, textFilePath);
    }
}
