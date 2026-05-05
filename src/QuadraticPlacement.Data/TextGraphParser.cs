using System.Globalization;
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Data;

/// <summary>
/// Парсер текстового формата графа из спецификации задачи
/// </summary>
public class TextGraphParser
{
    /// <summary>
    /// Парсит граф из текстового формата
    /// Формат: первая строка - "<n> <m> <f>", затем m строк рёбер "u v", затем f строк фиксированных вершин "idx x y"
    /// </summary>
    public static Graph Parse(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Входная строка пуста", nameof(text));

        var lines = text.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0)
            throw new FormatException("Файл не содержит данных");

        // Парсим заголовок: <n> <m> <f>
        var headerParts = lines[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (headerParts.Length != 3)
            throw new FormatException("Первая строка должна содержать 3 числа: количество вершин, рёбер и фиксированных вершин");

        if (!int.TryParse(headerParts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int vertexCount))
            throw new FormatException($"Некорректное количество вершин: {headerParts[0]}");

        if (!int.TryParse(headerParts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int edgeCount))
            throw new FormatException($"Некорректное количество рёбер: {headerParts[1]}");

        if (!int.TryParse(headerParts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int fixedVertexCount))
            throw new FormatException($"Некорректное количество фиксированных вершин: {headerParts[2]}");

        if (vertexCount < 1)
            throw new FormatException($"Граф должен содержать хотя бы одну вершину, получено: {vertexCount}");

        // Проверяем количество строк
        int expectedLines = 1 + edgeCount + fixedVertexCount;
        if (lines.Length < expectedLines)
            throw new FormatException($"Ожидается {expectedLines} строк, но найдено только {lines.Length}");

        // Парсим рёбра
        var edges = new List<Edge>(edgeCount);
        for (int i = 0; i < edgeCount; i++)
        {
            var edgeParts = lines[1 + i].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (edgeParts.Length < 2)
                throw new FormatException($"Ребро {i + 1}: строка должна содержать хотя бы 2 числа (индексы вершин)");

            if (!int.TryParse(edgeParts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int from))
                throw new FormatException($"Ребро {i + 1}: некорректный индекс первой вершины: {edgeParts[0]}");

            if (!int.TryParse(edgeParts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int to))
                throw new FormatException($"Ребро {i + 1}: некорректный индекс второй вершины: {edgeParts[1]}");

            // Опциональный вес
            double weight = 1.0;
            if (edgeParts.Length >= 3)
            {
                if (!double.TryParse(edgeParts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out weight))
                    throw new FormatException($"Ребро {i + 1}: некорректный вес: {edgeParts[2]}");
            }

            edges.Add(new Edge(from, to, weight));
        }

        // Парсим фиксированные вершины
        var fixedVertices = new Dictionary<int, FixedVertex>(fixedVertexCount);
        for (int i = 0; i < fixedVertexCount; i++)
        {
            var fvParts = lines[1 + edgeCount + i].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (fvParts.Length != 3)
                throw new FormatException($"Фиксированная вершина {i + 1}: строка должна содержать 3 числа (индекс, x, y)");

            if (!int.TryParse(fvParts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
                throw new FormatException($"Фиксированная вершина {i + 1}: некорректный индекс: {fvParts[0]}");

            if (!double.TryParse(fvParts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double x))
                throw new FormatException($"Фиксированная вершина {i + 1}: некорректная координата X: {fvParts[1]}");

            if (!double.TryParse(fvParts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
                throw new FormatException($"Фиксированная вершина {i + 1}: некорректная координата Y: {fvParts[2]}");

            if (fixedVertices.ContainsKey(index))
                throw new FormatException($"Фиксированная вершина с индексом {index} уже определена");

            fixedVertices[index] = new FixedVertex(index, x, y);
        }

        return new Graph(vertexCount, edges, fixedVertices);
    }

    /// <summary>
    /// Парсит граф из текстового файла
    /// </summary>
    public static Graph ParseFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл не найден: {filePath}");

        string text = File.ReadAllText(filePath);
        return Parse(text);
    }

    /// <summary>
    /// Преобразует граф в текстовый формат
    /// </summary>
    public static string ToText(Graph graph)
    {
        if (graph == null)
            throw new ArgumentNullException(nameof(graph));

        var lines = new List<string>();

        // Заголовок
        lines.Add($"{graph.VertexCount} {graph.EdgeCount} {graph.FixedVertices.Count}");

        // Рёбра
        foreach (var edge in graph.Edges)
        {
            lines.Add($"{edge.From} {edge.To} {edge.Weight.ToString(CultureInfo.InvariantCulture)}");
        }

        // Фиксированные вершины (сортируем по индексу для детерминированности)
        foreach (var fv in graph.FixedVertices.Values.OrderBy(v => v.Index))
        {
            lines.Add($"{fv.Index} {fv.X.ToString(CultureInfo.InvariantCulture)} {fv.Y.ToString(CultureInfo.InvariantCulture)}");
        }

        return string.Join("\n", lines);
    }

    /// <summary>
    /// Сохраняет граф в текстовый файл
    /// </summary>
    public static void SaveToFile(Graph graph, string filePath)
    {
        if (graph == null)
            throw new ArgumentNullException(nameof(graph));

        string text = ToText(graph);
        File.WriteAllText(filePath, text);
    }
}
