# Дизайн: Система квадратичного размещения элементов

**Дата:** 2026-05-05  
**Автор:** Claude  
**Статус:** Утверждён

## 1. Обзор проекта

Система для решения задачи квадратичного размещения элементов с минимизацией взвешенной суммы квадратов длин связей. Включает генератор тестовых данных, два алгоритма решения (базовый и эвристический), средства визуализации и генерацию HTML отчётов.

### Цели

- Реализовать базовый алгоритм на основе решения системы линейных уравнений
- Реализовать эвристический силовой алгоритм
- Предоставить средства генерации тестовых данных
- Визуализировать результаты размещения
- Собирать и сравнивать метрики обоих алгоритмов

### Ограничения

- До 10^5 вершин
- До 5×10^5 рёбер
- До 10^5 фиксированных вершин
- Нумерация вершин с 1
- Координаты в диапазоне [0, 1000]×[0, 1000]

## 2. Архитектура

### 2.1 Структура решения

```
QuadraticPlacement.sln
├── src/
│   ├── QuadraticPlacement.Core/          (Доменный слой)
│   ├── QuadraticPlacement.Algorithms/    (Алгоритмы)
│   ├── QuadraticPlacement.Data/          (Ввод/вывод данных)
│   ├── QuadraticPlacement.Visualization/ (Визуализация)
│   ├── QuadraticPlacement.Reporting/     (Генерация отчётов)
│   └── QuadraticPlacement.CLI/           (Консольный интерфейс)
└── tests/
    └── QuadraticPlacement.Tests/         (Юнит-тесты)
```

### 2.2 Слоистая архитектура

```
┌─────────────────────────────────────────┐
│         CLI (Console Interface)         │
├─────────────────────────────────────────┤
│            Reporting Layer              │
├─────────────────────────────────────────┤
│         Visualization Layer             │
├─────────────────────────────────────────┤
│           Data Layer                    │
├─────────────────────────────────────────┤
│         Algorithm Layer                 │
├─────────────────────────────────────────┤
│           Core Layer                    │
└─────────────────────────────────────────┘
```

**Принципы:**
- Все слои ссылаются на Core
- Алгоритмы не зависят от I/O
- Данные форматов агностик
- Визуализация генерирует изображения в памяти
- Отчёты оркестрируют все компоненты

## 3. Доменная модель (Core Layer)

### 3.1 Основные сущности

```csharp
/// <summary>
/// Граф связей между элементами
/// </summary>
public class Graph
{
    /// <summary>Общее количество вершин</summary>
    public int VertexCount { get; }
    
    /// <summary>Количество рёбер</summary>
    public int EdgeCount { get; }
    
    /// <summary>Список рёбер графа</summary>
    public IReadOnlyList<Edge> Edges { get; }
    
    /// <summary>Словарь фиксированных вершин (индекс → вершина)</summary>
    public IReadOnlyDictionary<int, FixedVertex> FixedVertices { get; }
}

/// <summary>
/// Ребро графа с весом
/// </summary>
public class Edge
{
    /// <summary>Индекс начальной вершины (нумерация с 1)</summary>
    public int From { get; }
    
    /// <summary>Индекс конечной вершины (нумерация с 1)</summary>
    public int To { get; }
    
    /// <summary>Вес ребра (по умолчанию 1.0)</summary>
    public double Weight { get; }
}

/// <summary>
/// Фиксированная вершина с заданными координатами
/// </summary>
public class FixedVertex
{
    /// <summary>Индекс вершины</summary>
    public int Index { get; }
    
    /// <summary>Координата X</summary>
    public double X { get; }
    
    /// <summary>Координата Y</summary>
    public double Y { get; }
}

/// <summary>
/// Результат размещения графа
/// </summary>
public class PlacementResult
{
    /// <summary>Массив X координат всех вершин</summary>
    public double[] XCoordinates { get; }
    
    /// <summary>Массив Y координат всех вершин</summary>
    public double[] YCoordinates { get; }
    
    /// <summary>Метрики качества размещения</summary>
    public Metrics Metrics { get; }
    
    /// <summary>Время вычисления</summary>
    public TimeSpan ComputationTime { get; }
}

/// <summary>
/// Метрики качества размещения
/// </summary>
public class Metrics
{
    /// <summary>Суммарная взвешенная длина всех рёбер</summary>
    public double TotalWeightedLength { get; }
    
    /// <summary>Максимальная длина ребра</summary>
    public double MaxEdgeLength { get; }
    
    /// <summary>Минимальная длина ребра</summary>
    public double MinEdgeLength { get; }
    
    /// <summary>Средняя длина ребра</summary>
    public double AverageEdgeLength { get; }
}
```

### 3.2 Интерфейсы

```csharp
/// <summary>
/// Интерфейс алгоритма размещения
/// </summary>
public interface IPlacementSolver
{
    /// <summary>Название алгоритма</summary>
    string Name { get; }
    
    /// <summary>
    /// Решает задачу размещения для заданного графа
    /// </summary>
    PlacementResult Solve(Graph graph);
}
```

### 3.3 Исключения

```csharp
/// <summary>
/// Базовое исключение для ошибок размещения
/// </summary>
public class PlacementException : Exception
{
    public PlacementException(string message) : base(message) { }
    public PlacementException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Исключение для ошибок парсинга графа
/// </summary>
public class GraphParseException : PlacementException
{
    public int LineNumber { get; }
    
    public GraphParseException(string message, int lineNumber) 
        : base(message) 
    {
        LineNumber = lineNumber;
    }
}

/// <summary>
/// Исключение когда алгоритм не сходится
/// </summary>
public class ConvergenceException : PlacementException
{
    public int IterationsCompleted { get; }
    public double FinalError { get; }
    
    public ConvergenceException(string message, int iterations, double error) 
        : base(message)
    {
        IterationsCompleted = iterations;
        FinalError = error;
    }
}
```

## 4. Алгоритмы (Algorithm Layer)

### 4.1 Базовый алгоритм (BasicSolver)

```csharp
/// <summary>
/// Базовый алгоритм размещения через решение системы линейных уравнений
/// с использованием матрицы Лапласа
/// </summary>
public class BasicSolver : IPlacementSolver
{
    public string Name => "Базовый алгоритм (матрица Лапласа)";
    
    /// <summary>
    /// Решает задачу размещения
    /// </summary>
    public PlacementResult Solve(Graph graph)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // 1. Построить разреженную матрицу Лапласа в формате CSR
        var laplacian = BuildLaplacianMatrix(graph);
        
        // 2. Разделить вершины на фиксированные и свободные
        var (freeIndices, fixedIndices) = PartitionVertices(graph);
        
        // 3. Сформировать систему L_free * x_free = b для X координат
        var (systemMatrix, rhs) = BuildLinearSystem(laplacian, freeIndices, fixedIndices, graph, Coordinate.X);
        var xCoordinates = ConjugateGradientSolver.Solve(systemMatrix, rhs);
        
        // 4. Сформировать и решить систему для Y координат
        var (systemMatrixY, rhsY) = BuildLinearSystem(laplacian, freeIndices, fixedIndices, graph, Coordinate.Y);
        var yCoordinates = ConjugateGradientSolver.Solve(systemMatrixY, rhsY);
        
        // 5. Собрать полный массив координат
        var fullX = AssembleCoordinates(xCoordinates, freeIndices, fixedIndices, graph, Coordinate.X);
        var fullY = AssembleCoordinates(yCoordinates, freeIndices, fixedIndices, graph, Coordinate.Y);
        
        stopwatch.Stop();
        
        // 6. Вычислить метрики
        var metrics = ComputeMetrics(graph, fullX, fullY);
        
        return new PlacementResult(fullX, fullY, metrics, stopwatch.Elapsed);
    }
    
    private SparseMatrixCSR BuildLaplacianMatrix(Graph graph) { }
    private (int[] free, int[] fixed) PartitionVertices(Graph graph) { }
    private (SparseMatrixCSR, double[]) BuildLinearSystem(...) { }
    private double[] AssembleCoordinates(...) { }
    private Metrics ComputeMetrics(Graph graph, double[] x, double[] y) { }
}
```

### 4.2 Эвристический алгоритм (HeuristicSolver)

```csharp
/// <summary>
/// Эвристический силовой алгоритм размещения (force-directed)
/// </summary>
public class HeuristicSolver : IPlacementSolver
{
    public string Name => "Эвристический алгоритм (силовой метод)";
    
    private const int MaxIterations = 1000;
    private const double ConvergenceThreshold = 1e-6;
    private const double CoolingRate = 0.95;
    private const double InitialTemperature = 100.0;
    
    /// <summary>
    /// Решает задачу размещения методом силового направленного размещения
    /// </summary>
    public PlacementResult Solve(Graph graph)
    {
        var stopwatch = Stopwatch.StartNew();
        var random = new Random();
        
        // 1. Инициализация случайных позиций для свободных вершин
        var (x, y) = InitializePositions(graph, random);
        
        // 2. Итеративная оптимизация
        double temperature = InitialTemperature;
        double prevEnergy = double.MaxValue;
        
        for (int iteration = 0; iteration < MaxIterations; iteration++)
        {
            // Вычислить силы
            var (forcesX, forcesY) = ComputeForces(graph, x, y);
            
            // Обновить позиции с учётом температуры
            UpdatePositions(x, y, forcesX, forcesY, graph, temperature, random);
            
            // Проверить сходимость
            double currentEnergy = ComputeSystemEnergy(graph, x, y);
            if (Math.Abs(prevEnergy - currentEnergy) < ConvergenceThreshold)
            {
                break;
            }
            prevEnergy = currentEnergy;
            
            // Охлаждение
            temperature *= CoolingRate;
        }
        
        stopwatch.Stop();
        
        // 3. Вычислить метрики
        var metrics = ComputeMetrics(graph, x, y);
        
        return new PlacementResult(x, y, metrics, stopwatch.Elapsed);
    }
    
    private (double[] x, double[] y) InitializePositions(Graph graph, Random random) { }
    private (double[] fx, double[] fy) ComputeForces(Graph graph, double[] x, double[] y) { }
    private void UpdatePositions(...) { }
    private double ComputeSystemEnergy(Graph graph, double[] x, double[] y) { }
    private Metrics ComputeMetrics(Graph graph, double[] x, double[] y) { }
}
```

### 4.3 Разреженные матрицы

```csharp
/// <summary>
/// Разреженная матрица в формате CSR (Compressed Sparse Row)
/// </summary>
public struct SparseMatrixCSR
{
    /// <summary>Ненулевые значения</summary>
    public double[] Values;
    
    /// <summary>Индексы столбцов</summary>
    public int[] ColumnIndices;
    
    /// <summary>Указатели на начало строк</summary>
    public int[] RowPointers;
    
    /// <summary>Количество строк</summary>
    public int RowCount;
    
    /// <summary>Количество столбцов</summary>
    public int ColumnCount;
}

/// <summary>
/// Сопряжённый градиент solver для разреженных матриц
/// </summary>
public class ConjugateGradientSolver
{
    private const int MaxIterations = 1000;
    private const double Tolerance = 1e-10;
    
    /// <summary>
    /// Решает систему Ax = b методом сопряжённых градиентов
    /// </summary>
    public static double[] Solve(SparseMatrixCSR A, double[] b, double tolerance = Tolerance)
    {
        int n = A.RowCount;
        double[] x = new double[n];  // Начальное приближение = 0
        
        double[] r = new double[n];  // Остаток
        double[] p = new double[n];  // Направление поиска
        double[] Ap = new double[n]; // A * p
        
        // r = b - A * x (при x=0: r = b)
        Array.Copy(b, r, n);
        Array.Copy(r, p, n);
        
        double rsOld = DotProduct(r, r);
        
        for (int iter = 0; iter < MaxIterations; iter++)
        {
            // Ap = A * p
            MultiplyMatrixVector(A, p, Ap);
            
            double alpha = rsOld / DotProduct(p, Ap);
            
            // x = x + alpha * p
            AddVectors(x, p, alpha, x);
            
            // r = r - alpha * Ap
            AddVectors(r, Ap, -alpha, r);
            
            double rsNew = DotProduct(r, r);
            
            if (Math.Sqrt(rsNew) < tolerance)
                break;
            
            double beta = rsNew / rsOld;
            
            // p = r + beta * p
            AddVectors(r, p, beta, p);
            
            rsOld = rsNew;
        }
        
        return x;
    }
    
    private static double DotProduct(double[] a, double[] b) { }
    private static void MultiplyMatrixVector(SparseMatrixCSR A, double[] x, double[] result) { }
    private static void AddVectors(double[] a, double[] b, double scalar, double[] result) { }
}
```

## 5. Данные (Data Layer)

### 5.1 Парсеры

```csharp
/// <summary>
/// Парсер текстового формата графа
/// Формат:
/// <вершин> <рёбер> <фиксированных>
/// u v (для каждого ребра)
/// индекс x y (для каждой фиксированной вершины)
/// </summary>
public class TextGraphParser
{
    /// <summary>
    /// Парсит граф из текстового файла
    /// </summary>
    public Graph Parse(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        
        // Первая строка: <вершин> <рёбер> <фиксированных>
        var header = ParseHeader(lines[0]);
        
        // Следующие edgeCount строк: рёбра
        var edges = ParseEdges(lines.Skip(1).Take(header.EdgeCount).ToArray());
        
        // Последние fixedVertexCount строк: фиксированные вершины
        var fixedVertices = ParseFixedVertices(
            lines.Skip(1 + header.EdgeCount).Take(header.FixedVertexCount).ToArray()
        );
        
        return new Graph(header.VertexCount, edges, fixedVertices);
    }
    
    private (int VertexCount, int EdgeCount, int FixedVertexCount) ParseHeader(string line) { }
    private List<Edge> ParseEdges(string[] lines) { }
    private Dictionary<int, FixedVertex> ParseFixedVertices(string[] lines) { }
}

/// <summary>
/// Парсер JSON формата графа
/// </summary>
public class JsonGraphParser
{
    public Graph Parse(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var data = JsonSerializer.Deserialize<GraphDataContract>(json);
        return data.ToGraph();
    }
}

/// <summary>
/// Контракт для JSON сериализации
/// </summary>
public class GraphDataContract
{
    public int VertexCount { get; set; }
    public EdgeDataContract[] Edges { get; set; }
    public FixedVertexDataContract[] FixedVertices { get; set; }
    
    public Graph ToGraph() { }
    public static GraphDataContract FromGraph(Graph graph) { }
}

/// <summary>
/// Конвертер между форматами
/// </summary>
public class GraphFormatConverter
{
    private readonly TextGraphParser _textParser = new();
    private readonly JsonGraphParser _jsonParser = new();
    
    /// <summary>
    /// Конвертирует текстовый формат в JSON
    /// </summary>
    public void TextToJson(string textPath, string jsonPath)
    {
        var graph = _textParser.Parse(textPath);
        var contract = GraphDataContract.FromGraph(graph);
        var json = JsonSerializer.Serialize(contract, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        File.WriteAllText(jsonPath, json);
    }
    
    /// <summary>
    /// Конвертирует JSON в текстовый формат
    /// </summary>
    public void JsonToText(string jsonPath, string textPath)
    {
        var graph = _jsonParser.Parse(jsonPath);
        var lines = new List<string>();
        
        // Заголовок
        lines.Add($"{graph.VertexCount} {graph.EdgeCount} {graph.FixedVertices.Count}");
        
        // Рёбра
        foreach (var edge in graph.Edges)
        {
            lines.Add($"{edge.From} {edge.To}");
        }
        
        // Фиксированные вершины
        foreach (var fv in graph.FixedVertices.Values)
        {
            lines.Add($"{fv.Index} {fv.X} {fv.Y}");
        }
        
        File.WriteAllLines(textPath, lines);
    }
}
```

### 5.2 Генератор тестовых данных

```csharp
/// <summary>
/// Генератор тестовых графов
/// </summary>
public class GraphGenerator
{
    private readonly Random _random;
    
    public GraphGenerator(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }
    
    /// <summary>
    /// Генерирует случайный граф
    /// </summary>
    public Graph GenerateRandom(
        int vertexCount,
        int edgeCount,
        int fixedVertexCount,
        double coordinateRange = 1000.0)
    {
        // Генерация случайных рёбер
        var edges = new List<Edge>();
        var usedPairs = new HashSet<(int, int)>();
        
        while (edges.Count < edgeCount)
        {
            int u = _random.Next(1, vertexCount + 1);
            int v = _random.Next(1, vertexCount + 1);
            
            if (u != v && !usedPairs.Contains((Math.Min(u, v), Math.Max(u, v))))
            {
                edges.Add(new Edge(u, v, 1.0));
                usedPairs.Add((Math.Min(u, v), Math.Max(u, v)));
            }
        }
        
        // Генерация фиксированных вершин
        var fixedVertices = new Dictionary<int, FixedVertex>();
        var indices = Enumerable.Range(1, vertexCount).OrderBy(x => _random.Next()).Take(fixedVertexCount);
        
        foreach (int idx in indices)
        {
            fixedVertices[idx] = new FixedVertex(
                idx,
                _random.NextDouble() * coordinateRange,
                _random.NextDouble() * coordinateRange
            );
        }
        
        return new Graph(vertexCount, edges, fixedVertices);
    }
    
    /// <summary>
    /// Генерирует регулярную решётку (сетку)
    /// </summary>
    public Graph GenerateGrid(int rows, int columns, bool fixCorners = true)
    {
        int vertexCount = rows * columns;
        var edges = new List<Edge>();
        var fixedVertices = new Dictionary<int, FixedVertex>();
        
        // Горизонтальные рёбра
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns - 1; c++)
            {
                int u = r * columns + c + 1;
                int v = r * columns + c + 2;
                edges.Add(new Edge(u, v, 1.0));
            }
        }
        
        // Вертикальные рёбра
        for (int r = 0; r < rows - 1; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                int u = r * columns + c + 1;
                int v = (r + 1) * columns + c + 1;
                edges.Add(new Edge(u, v, 1.0));
            }
        }
        
        // Зафиксировать углы
        if (fixCorners)
        {
            fixedVertices[1] = new FixedVertex(1, 0, 0);
            fixedVertices[columns] = new FixedVertex(columns, 1000, 0);
            fixedVertices[(rows - 1) * columns + 1] = new FixedVertex((rows - 1) * columns + 1, 0, 1000);
            fixedVertices[rows * columns] = new FixedVertex(rows * columns, 1000, 1000);
        }
        
        return new Graph(vertexCount, edges, fixedVertices);
    }
    
    /// <summary>
    /// Генерирует граф с "горячими" связями
    /// </summary>
    public Graph GenerateWithHotConnections(
        int vertexCount,
        int hotPathCount,
        int edgesPerPath)
    {
        var edges = new List<Edge>();
        var fixedVertices = new Dictionary<int, FixedVertex>();
        
        // Создаём несколько цепочек с большим количеством рёбер
        for (int path = 0; path < hotPathCount; path++)
        {
            int startVertex = path * (vertexCount / hotPathCount) + 1;
            int endVertex = Math.Min(startVertex + edgesPerPath, vertexCount);
            
            for (int v = startVertex; v < endVertex; v++)
            {
                edges.Add(new Edge(v, v + 1, 2.0));  // Увеличенный вес для "горячей" связи
            }
        }
        
        // Добавляем случайные рёбра
        int randomEdges = vertexCount * 2;
        for (int i = 0; i < randomEdges; i++)
        {
            int u = _random.Next(1, vertexCount + 1);
            int v = _random.Next(1, vertexCount + 1);
            if (u != v)
            {
                edges.Add(new Edge(u, v, 1.0));
            }
        }
        
        // Фиксируем несколько вершин случайно
        for (int i = 0; i < hotPathCount; i++)
        {
            int idx = _random.Next(1, vertexCount + 1);
            fixedVertices[idx] = new FixedVertex(
                idx,
                _random.NextDouble() * 1000,
                _random.NextDouble() * 1000
            );
        }
        
        return new Graph(vertexCount, edges, fixedVertices);
    }
}
```

## 6. Визуализация (Visualization Layer)

```csharp
/// <summary>
/// Генератор диаграмм рассеяния
/// </summary>
public class ScatterPlotGenerator
{
    /// <summary>
    /// Создаёт диаграмму размещения вершин
    /// </summary>
    public byte[] GeneratePlot(
        Graph graph,
        PlacementResult result,
        int width = 1200,
        int height = 800)
    {
        var plot = new ScottPlot.Plot(width, height);
        
        // Разделяем вершины на фиксированные и свободные
        var freeX = new List<double>();
        var freeY = new List<double>();
        var fixedX = new List<double>();
        var fixedY = new List<double>();
        
        for (int i = 0; i < graph.VertexCount; i++)
        {
            int vertexIndex = i + 1;  // нумерация с 1
            
            if (graph.FixedVertices.ContainsKey(vertexIndex))
            {
                fixedX.Add(result.XCoordinates[i]);
                fixedY.Add(result.YCoordinates[i]);
            }
            else
            {
                freeX.Add(result.XCoordinates[i]);
                freeY.Add(result.YCoordinates[i]);
            }
        }
        
        // Добавляем свободные вершины
        plot.Add.ScatterPoints(freeX, freeY, color: ScottPlot.Color.Blue, markerSize: 5)
             .Label = "Свободные вершины");
        
        // Добавляем фиксированные вершины
        plot.Add.ScatterPoints(fixedX, fixedY, color: ScottPlot.Color.Red, markerSize: 8)
             .Label = "Фиксированные вершины");
        
        // Добавляем рёбра (для малых графов)
        if (graph.VertexCount < 1000)
        {
            foreach (var edge in graph.Edges)
            {
                int u = edge.From - 1;
                int v = edge.To - 1;
                plot.Add.Line(
                    result.XCoordinates[u], result.YCoordinates[u],
                    result.XCoordinates[v], result.YCoordinates[v],
                    color: ScottPlot.Color.WithAlpha(ScottPlot.Color.Gray, 0.3),
                    width: 1
                );
            }
        }
        
        plot.Legend();
        plot.Title($"Размещение графа ({graph.VertexCount} вершин, {graph.EdgeCount} рёбер)");
        plot.XLabel("Координата X");
        plot.YLabel("Координата Y");
        
        using var ms = new MemoryStream();
        plot.SaveImage(ms, ScottPlot.ImageFormat.Png);
        return ms.ToArray();
    }
    
    /// <summary>
    /// Создаёт сравнительный график двух алгоритмов
    /// </summary>
    public byte[] GenerateComparisonPlot(
        Graph graph,
        PlacementResult result1,
        PlacementResult result2,
        string algorithm1Name,
        string algorithm2Name)
    {
        var plot = new ScottPlot.Plot(1200, 800);
        
        // Левый subplot - алгоритм 1
        plot.Add.Subplot(2, 1, 0);
        AddSinglePlot(plot, graph, result1, algorithm1Name);
        
        // Правый subplot - алгоритм 2
        plot.Add.Subplot(2, 1, 1);
        AddSinglePlot(plot, graph, result2, algorithm2Name);
        
        using var ms = new MemoryStream();
        plot.SaveImage(ms, ScottPlot.ImageFormat.Png);
        return ms.ToArray();
    }
    
    private void AddSinglePlot(ScottPlot.Plot plot, Graph graph, PlacementResult result, string title)
    {
        // ... логика добавления одного графика
    }
}

/// <summary>
/// Генератор тепловых карт
/// </summary>
public class HeatmapGenerator
{
    /// <summary>
    /// Создаёт тепловую карту плотности размещения
    /// </summary>
    public byte[] GenerateHeatmap(
        Graph graph,
        PlacementResult result,
        int bins = 100,
        int width = 1200,
        int height = 800)
    {
        var plot = new ScottPlot.Plot(width, height);
        
        // Вычисляем 2D гистограмму
        var (histogram, xEdges, yEdges) = Compute2DHistogram(
            result.XCoordinates,
            result.YCoordinates,
            bins
        );
        
        // Добавляем тепловую карту
        var heatmap = plot.Add.Heatmap(histogram);
        heatmap.Update(xEdges, yEdges);
        
        plot.Add.Colorbar(heatmap);
        plot.Title($"Тепловая карта плотности ({graph.VertexCount} вершин)");
        plot.XLabel("Координата X");
        plot.YLabel("Координата Y");
        
        using var ms = new MemoryStream();
        plot.SaveImage(ms, ScottPlot.ImageFormat.Png);
        return ms.ToArray();
    }
    
    private (double[,] histogram, double[] xEdges, double[] yEdges) Compute2DHistogram(
        double[] x, double[] y, int bins) { }
}
```

## 7. Отчёты (Reporting Layer)

```csharp
/// <summary>
/// Строитель HTML отчётов
/// </summary>
public class HtmlReportBuilder
{
    private readonly ScatterPlotGenerator _scatterGenerator = new();
    private readonly HeatmapGenerator _heatmapGenerator = new();
    
    /// <summary>
    /// Генерирует полный HTML отчёт
    /// </summary>
    public string GenerateReport(
        Graph graph,
        AlgorithmRun basicResult,
        AlgorithmRun heuristicResult,
        ReportMetadata metadata)
    {
        var sb = new StringBuilder();
        
        // Начало HTML
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang='ru'>");
        sb.AppendLine("<head>");
        sb.AppendLine("  <meta charset='UTF-8'>");
        sb.AppendLine("  <title>Отчёт о квадратичном размещении</title>");
        sb.AppendLine(GetCssStyles());
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        
        // Заголовок
        sb.AppendLine("  <h1>Результаты размещения графа</h1>");
        
        // Метаданные
        sb.AppendLine(GetMetadataSection(metadata));
        
        // Сравнительная таблица метрик
        sb.AppendLine(GetMetricsComparisonSection(basicResult, heuristicResult));
        
        // Визуализация базового алгоритма
        sb.AppendLine(GetAlgorithmSection("Базовый алгоритм", basicResult));
        
        // Визуализация эвристического алгоритма
        sb.AppendLine(GetAlgorithmSection("Эвристический алгоритм", heuristicResult));
        
        // Анализ
        sb.AppendLine(GetAnalysisSection(basicResult, heuristicResult));
        
        // Конец HTML
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }
    
    /// <summary>
    /// Сохраняет отчёт в файл
    /// </summary>
    public void SaveReport(string htmlContent, string outputPath)
    {
        File.WriteAllText(outputPath, htmlContent, Encoding.UTF8);
    }
    
    private string GetCssStyles() { }
    private string GetMetadataSection(ReportMetadata metadata) { }
    private string GetMetricsComparisonSection(AlgorithmRun basic, AlgorithmRun heuristic) { }
    private string GetAlgorithmSection(string title, AlgorithmRun run) { }
    private string GetAnalysisSection(AlgorithmRun basic, AlgorithmRun heuristic) { }
}

/// <summary>
/// Метаданные отчёта
/// </summary>
public class ReportMetadata
{
    public DateTime Timestamp { get; set; }
    public string GraphSource { get; set; }
    public int VertexCount { get; set; }
    public int EdgeCount { get; set; }
    public int FixedVertexCount { get; set; }
}

/// <summary>
/// Результат работы алгоритма для отчёта
/// </summary>
public class AlgorithmRun
{
    public string AlgorithmName { get; set; }
    public PlacementResult Result { get; set; }
    public byte[] VisualizationImage { get; set; }
    public byte[] HeatmapImage { get; set; }
}
```

## 8. CLI Интерфейс

```csharp
/// <summary>
/// Главная точка входа консольного приложения
/// </summary>
public class Program
{
    public static void Main(string[] args)
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
                    
                case "visualize":
                    HandleVisualizeCommand(args);
                    break;
                    
                case "report":
                    HandleReportCommand(args);
                    break;
                    
                case "benchmark":
                    HandleBenchmarkCommand(args);
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
            Environment.Exit(1);
        }
    }
    
    private static void PrintUsage()
    {
        Console.WriteLine("QuadraticPlacement - Система квадратичного размещения");
        Console.WriteLine();
        Console.WriteLine("Использование:");
        Console.WriteLine("  generate --output <file> --vertices <n> --edges <m> --fixed <f> [--format text|json]");
        Console.WriteLine("  convert --input <file> --output <file> --to-format text|json");
        Console.WriteLine("  solve --input <file> --algorithm basic|heuristic --output <file>");
        Console.WriteLine("  visualize --input <graph> --solution <file> --output <image>");
        Console.WriteLine("  report --input <graph> --output <html> [--run-both]");
        Console.WriteLine("  benchmark --input <graph> --output <html> [--iterations <n>]");
    }
    
    private static void HandleGenerateCommand(string[] args)
    {
        // Парсинг аргументов
        var options = ParseGenerateOptions(args);
        
        // Генерация графа
        var generator = new GraphGenerator(seed: 42);
        var graph = generator.GenerateRandom(
            options.Vertices,
            options.Edges,
            options.FixedVertices
        );
        
        // Сохранение
        if (options.Format == "json")
        {
            var contract = GraphDataContract.FromGraph(graph);
            var json = JsonSerializer.Serialize(contract, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(options.Output, json);
        }
        else
        {
            // Сохранение в текстовом формате
            var lines = new List<string>();
            lines.Add($"{graph.VertexCount} {graph.EdgeCount} {graph.FixedVertices.Count}");
            foreach (var edge in graph.Edges)
                lines.Add($"{edge.From} {edge.To}");
            foreach (var fv in graph.FixedVertices.Values)
                lines.Add($"{fv.Index} {fv.X} {fv.Y}");
            File.WriteAllLines(options.Output, lines);
        }
        
        Console.WriteLine($"Граф сгенерирован: {graph.VertexCount} вершин, {graph.EdgeCount} рёбер");
        Console.WriteLine($"Сохранён в: {options.Output}");
    }
    
    // ... остальные обработчики команд
}
```

## 9. Тестирование

### 9.1 Стратегия тестирования

**Юнит-тесты:**
- Тесты парсеров (корректные и некорректные данные)
- Тесты генераторов графов
- Тесты алгоритмов на малых графах с известными решениями
- Тесты вычисления метрик
- Тесты разреженных матриц

**Интеграционные тесты:**
- Полный цикл: генерация → решение → визуализация → отчёт
- Тесты на графах разных размеров
- Сравнение алгоритмов на идентичных данных

### 9.2 Тестовые данные

```csharp
/// <summary>
/// Хелпер для создания тестовых графов
/// </summary>
public static class TestDataHelper
{
    /// <summary>
    /// Создаёт простой граф для тестирования
    /// </summary>
    public static Graph CreateSimpleTriangle()
    {
        var edges = new List<Edge>
        {
            new Edge(1, 2, 1.0),
            new Edge(2, 3, 1.0),
            new Edge(3, 1, 1.0)
        };
        
        var fixedVertices = new Dictionary<int, FixedVertex>
        {
            [1] = new FixedVertex(1, 0, 0),
            [2] = new FixedVertex(2, 1, 0)
        };
        
        return new Graph(3, edges, fixedVertices);
    }
    
    /// <summary>
    /// Создаёт граф-решётку для тестирования
    /// </summary>
    public static Graph CreateTestGrid()
    {
        var generator = new GraphGenerator(seed: 42);
        return generator.GenerateGrid(3, 3, fixCorners: true);
    }
}
```

## 10. Зависимости

### 10.1 NuGet пакеты

```
QuadraticPlacement.Core:
  - (нет внешних зависимостей)

QuadraticPlacement.Algorithms:
  - QuadraticPlacement.Core

QuadraticPlacement.Data:
  - QuadraticPlacement.Core
  - System.Text.Json

QuadraticPlacement.Visualization:
  - QuadraticPlacement.Core
  - ScottPlot (>= 5.0.0)

QuadraticPlacement.Reporting:
  - QuadraticPlacement.Core
  - QuadraticPlacement.Visualization
  - ScottPlot (>= 5.0.0)

QuadraticPlacement.CLI:
  - QuadraticPlacement.Data
  - QuadraticPlacement.Algorithms
  - QuadraticPlacement.Reporting
  - System.CommandLine (опционально для парсинга аргументов)

QuadraticPlacement.Tests:
  - QuadraticPlacement.Core
  - QuadraticPlacement.Algorithms
  - QuadraticPlacement.Data
  - xUnit
  - FluentAssertions
```

## 11. Производительность

### 11.1 Ожидаемая производительность

**Базовый алгоритм:**
- 1000 вершин: < 1 секунда
- 10,000 вершин: ~5-10 секунд
- 100,000 вершин: ~1-2 минуты

**Эвристический алгоритм:**
- 1000 вершин: < 5 секунд
- 10,000 вершин: ~30-60 секунд
- 100,000 вершин: ~5-10 минут

### 11.2 Оптимизации

- Использование разреженных матриц
- Параллельное вычисление сил (для эвристического алгоритма)
- Оптимизированное умножение матрица-вектор (CSR формат)
- Ранний выход при достижении сходимости

## 12. Следующие шаги

После утверждения этого дизайна:

1. Создать структуру solution и проектов
2. Реализовать доменную модель (Core)
3. Реализовать парсеры и генераторы (Data)
4. Реализовать базовый алгоритм с тестами
5. Реализовать эвристический алгоритм с тестами
6. Реализовать визуализацию
7. Реализовать генерацию отчётов
8. Реализовать CLI интерфейс
9. Интеграционное тестирование
10. Документация и примеры использования

---

**Конец дизайна**
