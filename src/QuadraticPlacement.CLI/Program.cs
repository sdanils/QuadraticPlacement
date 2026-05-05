using System.Reflection;

namespace QuadraticPlacement.CLI;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return;
        }

        string command = args[0].ToLower();

        try
        {
            switch (command)
            {
                case "generate":
                    HandleGenerateCommand(args);
                    break;

                case "convert":
                    HandleConvertCommand(args);
                    break;

                case "solve":
                    HandleSolveCommand(args);
                    break;

                case "report":
                    HandleReportCommand(args);
                    break;

                default:
                    Console.WriteLine($"Неизвестная команда: {command}");
                    PrintUsage();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Environment.Exit(1);
        }
    }

    static void PrintUsage()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

        Console.WriteLine($"QuadraticPlacement v{version}");
        Console.WriteLine("Система квадратичного размещения элементов");
        Console.WriteLine();
        Console.WriteLine("Использование:");
        Console.WriteLine("  generate --output <file> --vertices <n> --edges <m> --fixed <f> [--format text|json]");
        Console.WriteLine("  convert --input <file> --output <file> --to-format text|json");
        Console.WriteLine("  solve --input <file> --algorithm basic|heuristic --output <file>");
        Console.WriteLine("  report --input <graph> --output <html> [--no-viz]");
        Console.WriteLine();
        Console.WriteLine("Примеры:");
        Console.WriteLine("  QuadraticPlacement.exe generate --output graph.txt --vertices 100 --edges 500 --fixed 10");
        Console.WriteLine("  QuadraticPlacement.exe report --input graph.txt --output report.html");
    }

    static void HandleGenerateCommand(string[] args)
    {
        string output = GetArgument(args, "--output") ?? throw new Exception("Не указан --output");
        string verticesStr = GetArgument(args, "--vertices") ?? throw new Exception("Не указан --vertices");
        string edgesStr = GetArgument(args, "--edges") ?? throw new Exception("Не указан --edges");
        string fixedStr = GetArgument(args, "--fixed") ?? throw new Exception("Не указан --fixed");
        string format = GetArgument(args, "--format") ?? "text";

        if (!int.TryParse(verticesStr, out int vertices))
            throw new Exception("Неверный формат --vertices");
        if (!int.TryParse(edgesStr, out int edges))
            throw new Exception("Неверный формат --edges");
        if (!int.TryParse(fixedStr, out int fixedCount))
            throw new Exception("Неверный формат --fixed");

        Console.WriteLine($"Генерация графа: {vertices} вершин, {edges} рёбер, {fixedCount} фиксированных");

        var graph = Data.GraphGenerator.GenerateRandom(vertices, edges, fixedCount);

        if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
        {
            var json = System.Text.Json.JsonSerializer.Serialize(
                Data.GraphDataContract.FromGraph(graph),
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(output, json);
        }
        else
        {
            var lines = new List<string>();
            lines.Add($"{graph.VertexCount} {graph.EdgeCount} {graph.FixedVertices.Count}");
            foreach (var edge in graph.Edges)
                lines.Add($"{edge.From} {edge.To}");
            foreach (var fv in graph.FixedVertices.Values)
                lines.Add($"{fv.Index} {fv.X.ToString(System.Globalization.CultureInfo.InvariantCulture)} {fv.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            File.WriteAllLines(output, lines);
        }

        Console.WriteLine($"Граф сохранён в: {output}");
    }

    static void HandleConvertCommand(string[] args)
    {
        string input = GetArgument(args, "--input") ?? throw new Exception("Не указан --input");
        string output = GetArgument(args, "--output") ?? throw new Exception("Не указан --output");
        string toFormat = GetArgument(args, "--to-format") ?? throw new Exception("Не указан --to-format");

        Console.WriteLine($"Конвертация {input} в {toFormat} формат...");

        if (toFormat.Equals("json", StringComparison.OrdinalIgnoreCase))
        {
            Data.GraphFormatConverter.TextFileToJsonFile(input, output);
        }
        else if (toFormat.Equals("text", StringComparison.OrdinalIgnoreCase))
        {
            Data.GraphFormatConverter.JsonFileToTextFile(input, output);
        }
        else
        {
            throw new Exception($"Неподдерживаемый формат: {toFormat}");
        }

        Console.WriteLine($"Конвертация завершена: {output}");
    }

    static void HandleSolveCommand(string[] args)
    {
        string input = GetArgument(args, "--input") ?? throw new Exception("Не указан --input");
        string algorithmStr = GetArgument(args, "--algorithm") ?? throw new Exception("Не указан --algorithm");
        string output = GetArgument(args, "--output") ?? throw new Exception("Не указан --output");

        Console.WriteLine($"Загрузка графа из {input}...");

        var graphContent = File.ReadAllText(input);
        Core.Graph graph;

        if (input.EndsWith(".json"))
        {
            graph = Data.JsonGraphParser.Parse(graphContent);
        }
        else
        {
            graph = Data.TextGraphParser.Parse(graphContent);
        }

        Console.WriteLine($"Граф загружен: {graph.VertexCount} вершин, {graph.EdgeCount} рёбер");

        Core.IPlacementSolver solver = algorithmStr.ToLower() switch
        {
            "basic" => new Algorithms.BasicSolver(),
            "heuristic" => new Algorithms.HeuristicSolver(),
            _ => throw new Exception($"Неизвестный алгоритм: {algorithmStr}")
        };

        Console.WriteLine($"Запуск алгоритма: {solver.Name}");

        var result = solver.Solve(graph);

        Console.WriteLine($"Алгоритм завершён за {result.ComputationTime.TotalSeconds:F2} сек");
        Console.WriteLine($"Метрики:");
        Console.WriteLine($"  Суммарная длина: {result.Metrics.TotalWeightedLength:F2}");
        Console.WriteLine($"  Макс. ребро: {result.Metrics.MaxEdgeLength:F2}");
        Console.WriteLine($"  Среднее: {result.Metrics.AverageEdgeLength:F2}");

        // Сохраняем результат в JSON
        var resultData = new
        {
            algorithm = solver.Name,
            vertexCount = graph.VertexCount,
            edgeCount = graph.EdgeCount,
            xCoordinates = result.XCoordinates,
            yCoordinates = result.YCoordinates,
            metrics = new
            {
                totalLength = result.Metrics.TotalWeightedLength,
                maxLength = result.Metrics.MaxEdgeLength,
                minLength = result.Metrics.MinEdgeLength,
                avgLength = result.Metrics.AverageEdgeLength
            },
            computationTimeMs = result.ComputationTime.TotalMilliseconds
        };

        var json = System.Text.Json.JsonSerializer.Serialize(resultData, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(output, json);
        Console.WriteLine($"\nРезультат сохранён в: {output}");
    }

    static void HandleReportCommand(string[] args)
    {
        string input = GetArgument(args, "--input") ?? throw new Exception("Не указан --input");
        string output = GetArgument(args, "--output") ?? throw new Exception("Не указан --output");
        bool noViz = HasArgument(args, "--no-viz");

        var orchestrator = new ReportOrchestrator();
        orchestrator.GenerateFullReport(input, output, !noViz);
    }

    static string? GetArgument(string[] args, string name)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i].Equals(name, StringComparison.OrdinalIgnoreCase))
                return args[i + 1];
        }
        return null;
    }

    static bool HasArgument(string[] args, string name)
    {
        return args.Any(a => a.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
