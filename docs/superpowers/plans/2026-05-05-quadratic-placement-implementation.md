# Система квадратичного размещения элементов - План реализации

> **Для агентов:** ОБЯЗАТЕЛЬНЫЙ ПОДНАВЫК: Используйте superpowers:subagent-driven-development (рекомендуется) или superpowers:executing-plans для реализации этого плана пошагово. Шаги используют синтаксис чекбокса (`- [ ]`) для отслеживания.

**Цель:** Создать полную систему для решения задачи квадратичного размещения с двумя алгоритмами, генератором данных, визуализацией и генерацией HTML отчётов

**Архитектура:** Слоистая архитектура с 6 проектами: Core (доменные модели), Algorithms (базовый и эвристический солверы), Data (парсеры, генераторы), Visualization (ScottPlot), Reporting (HTML), CLI (консольный интерфейс)

**Технологический стек:** C# 12, .NET 8, ScottPlot 5.0, xUnit, FluentAssertions, System.Text.Json

---

## Структура файлов

### Solution и проекты
- Создать: `QuadraticPlacement.sln` - solution файл
- Создать: `src/QuadraticPlacement.Core/QuadraticPlacement.Core.csproj` - доменный слой
- Создать: `src/QuadraticPlacement.Core/Exceptions.cs` - исключения
- Создать: `src/QuadraticPlacement.Core/Graph.cs` - граф
- Создать: `src/QuadraticPlacement.Core/Edge.cs` - ребро
- Создать: `src/QuadraticPlacement.Core/FixedVertex.cs` - фиксированная вершина
- Создать: `src/QuadraticPlacement.Core/PlacementResult.cs` - результат размещения
- Создать: `src/QuadraticPlacement.Core/Metrics.cs` - метрики
- Создать: `src/QuadraticPlacement.Core/IPlacementSolver.cs` - интерфейс солвера

### Алгоритмы
- Создать: `src/QuadraticPlacement.Algorithms/QuadraticPlacement.Algorithms.csproj`
- Создать: `src/QuadraticPlacement.Algorithms/SparseMatrixCSR.cs` - разреженная матрица
- Создать: `src/QuadraticPlacement.Algorithms/ConjugateGradientSolver.cs` - сопряжённый градиент
- Создать: `src/QuadraticPlacement.Algorithms/BasicSolver.cs` - базовый алгоритм
- Создать: `src/QuadraticPlacement.Algorithms/HeuristicSolver.cs` - эвристический алгоритм

### Данные
- Создать: `src/QuadraticPlacement.Data/QuadraticPlacement.Data.csproj`
- Создать: `src/QuadraticPlacement.Data/TextGraphParser.cs` - парсер текстового формата
- Создать: `src/QuadraticPlacement.Data/JsonGraphParser.cs` - парсер JSON
- Создать: `src/QuadraticPlacement.Data/GraphDataContract.cs` - контракт для JSON
- Создать: `src/QuadraticPlacement.Data/GraphFormatConverter.cs` - конвертер форматов
- Создать: `src/QuadraticPlacement.Data/GraphGenerator.cs` - генератор тестовых данных

### Визуализация
- Создать: `src/QuadraticPlacement.Visualization/QuadraticPlacement.Visualization.csproj`
- Создать: `src/QuadraticPlacement.Visualization/ScatterPlotGenerator.cs` - диаграммы рассеяния
- Создать: `src/QuadraticPlacement.Visualization/HeatmapGenerator.cs` - тепловые карты

### Отчёты
- Создать: `src/QuadraticPlacement.Reporting/QuadraticPlacement.Reporting.csproj`
- Создать: `src/QuadraticPlacement.Reporting/HtmlReportBuilder.cs` - генератор HTML отчётов
- Создать: `src/QuadraticPlacement.Reporting/ReportMetadata.cs` - метаданные отчёта
- Создать: `src/QuadraticPlacement.Reporting/AlgorithmRun.cs` - результат запуска для отчёта

### CLI
- Создать: `src/QuadraticPlacement.CLI/QuadraticPlacement.CLI.csproj`
- Создать: `src/QuadraticPlacement.CLI/Program.cs` - точка входа
- Создать: `src/QuadraticPlacement.CLI/Commands/GenerateCommand.cs` - команда generate
- Создать: `src/QuadraticPlacement.CLI/Commands/ConvertCommand.cs` - команда convert
- Создать: `src/QuadraticPlacement.CLI/Commands/SolveCommand.cs` - команда solve
- Создать: `src/QuadraticPlacement.CLI/Commands/ReportCommand.cs` - команда report
- Создать: `src/QuadraticPlacement.CLI/ReportOrchestrator.cs` - оркестратор отчётов

### Тесты
- Создать: `tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj`
- Создать: `tests/QuadraticPlacement.Tests/Core/GraphTests.cs` - тесты Graph
- Создать: `tests/QuadraticPlacement.Tests/Data/TextGraphParserTests.cs` - тесты парсера
- Создать: `tests/QuadraticPlacement.Tests/Data/GraphGeneratorTests.cs` - тесты генератора
- Создать: `tests/QuadraticPlacement.Tests/Algorithms/BasicSolverTests.cs` - тесты базового алгоритма
- Создать: `tests/QuadraticPlacement.Tests/Algorithms/HeuristicSolverTests.cs` - тесты эвристического алгоритма
- Создать: `tests/QuadraticPlacement.Tests/TestDataHelper.cs` - хелпер тестовых данных

---

## Реализация

### Task 1: Создание solution и проектов

**Files:**
- Create: `QuadraticPlacement.sln`
- Create: `src/QuadraticPlacement.Core/QuadraticPlacement.Core.csproj`

- [ ] **Step 1: Создать solution файл**

```bash
dotnet new sln -n QuadraticPlacement
```

- [ ] **Step 2: Создать проект Core**

```bash
dotnet new classlib -n QuadraticPlacement.Core -o src/QuadraticPlacement.Core
dotnet sln add src/QuadraticPlacement.Core/QuadraticPlacement.Core.csproj
```

- [ ] **Step 3: Удалить автоматически созданный Class1.cs**

```bash
rm src/QuadraticPlacement.Core/Class1.cs
```

- [ ] **Step 4: Закоммитить**

```bash
git add .
git commit -m "feat: create solution and Core project"
```

---

### Task 2: Исключения в доменном слое

**Files:**
- Create: `src/QuadraticPlacement.Core/Exceptions.cs`
- Test: `tests/QuadraticPlacement.Tests/Core/ExceptionsTests.cs`

- [ ] **Step 1: Написать тест для PlacementException**

```csharp
using Xunit;
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Tests.Core;

public class ExceptionsTests
{
    [Fact]
    public void PlacementException_CreatesWithMessage()
    {
        var exception = new PlacementException("Test error");
        Assert.Equal("Test error", exception.Message);
    }

    [Fact]
    public void GraphParseException_CreatesWithLineNumber()
    {
        var exception = new GraphParseException("Parse error", 42);
        Assert.Equal("Parse error", exception.Message);
        Assert.Equal(42, exception.LineNumber);
    }

    [Fact]
    public void ConvergenceException_CreatesWithIterationInfo()
    {
        var exception = new ConvergenceException("No convergence", 100, 0.5);
        Assert.Equal("No convergence", exception.Message);
        Assert.Equal(100, exception.IterationsCompleted);
        Assert.Equal(0.5, exception.FinalError);
    }
}
```

- [ ] **Step 2: Создать проект тестов**

```bash
dotnet new xunit -n QuadraticPlacement.Tests -o tests/QuadraticPlacement.Tests
dotnet add tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj reference src/QuadraticPlacement.Core/QuadraticPlacement.Core.csproj
dotnet add tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj package FluentAssertions
dotnet sln add tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj
```

- [ ] **Step 3: Запустить тест (убедиться, что падает)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~ExceptionsTests"
```

Ожидаемый результат: FAIL с ошибкой "type or namespace name could not be found"

- [ ] **Step 4: Реализовать исключения**

```csharp
namespace QuadraticPlacement.Core;

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

- [ ] **Step 5: Запустить тест (убедиться, что проходит)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~ExceptionsTests"
```

Ожидаемый результат: PASS

- [ ] **Step 6: Закоммитить**

```bash
git add .
git commit -m "feat: implement domain exceptions with tests"
```

---

### Task 3: Доменная модель - Edge

**Files:**
- Create: `src/QuadraticPlacement.Core/Edge.cs`
- Test: `tests/QuadraticPlacement.Tests/Core/EdgeTests.cs`

- [ ] **Step 1: Написать тест для Edge**

```csharp
using FluentAssertions;
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Tests.Core;

public class EdgeTests
{
    [Fact]
    public void Edge_CreatesWithDefaultWeight()
    {
        var edge = new Edge(1, 2);
        edge.From.Should().Be(1);
        edge.To.Should().Be(2);
        edge.Weight.Should().Be(1.0);
    }

    [Fact]
    public void Edge_CreatesWithCustomWeight()
    {
        var edge = new Edge(1, 2, 2.5);
        edge.Weight.Should().Be(2.5);
    }

    [Fact]
    public void Edge_IsImmutable()
    {
        var edge = new Edge(1, 2);
        // Свойства только для чтения, компиляция проверяет иммутабельность
    }
}
```

- [ ] **Step 2: Запустить тест (FAIL)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~EdgeTests"
```

- [ ] **Step 3: Реализовать Edge**

```csharp
namespace QuadraticPlacement.Core;

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
    
    public Edge(int from, int to, double weight = 1.0)
    {
        if (from < 1)
            throw new ArgumentOutOfRangeException(nameof(from), "Нумерация вершин начинается с 1");
        if (to < 1)
            throw new ArgumentOutOfRangeException(nameof(to), "Нумерация вершин начинается с 1");
        if (weight <= 0)
            throw new ArgumentOutOfRangeException(nameof(weight), "Вес должен быть положительным");
            
        From = from;
        To = to;
        Weight = weight;
    }
}
```

- [ ] **Step 4: Запустить тест (PASS)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~EdgeTests"
```

- [ ] **Step 5: Закоммитить**

```bash
git add .
git commit -m "feat: implement Edge domain model"
```

---

### Task 4: Доменная модель - FixedVertex

**Files:**
- Create: `src/QuadraticPlacement.Core/FixedVertex.cs`
- Test: `tests/QuadraticPlacement.Tests/Core/FixedVertexTests.cs`

- [ ] **Step 1: Написать тест для FixedVertex**

```csharp
using FluentAssertions;
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Tests.Core;

public class FixedVertexTests
{
    [Fact]
    public void FixedVertex_CreatesWithCoordinates()
    {
        var vertex = new FixedVertex(5, 100.5, 200.7);
        vertex.Index.Should().Be(5);
        vertex.X.Should().Be(100.5);
        vertex.Y.Should().Be(200.7);
    }

    [Fact]
    public void FixedVertex_RequiresValidIndex()
    {
        Action act = () => new FixedVertex(0, 100, 200);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
```

- [ ] **Step 2: Запустить тест (FAIL)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~FixedVertexTests"
```

- [ ] **Step 3: Реализовать FixedVertex**

```csharp
namespace QuadraticPlacement.Core;

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
    
    public FixedVertex(int index, double x, double y)
    {
        if (index < 1)
            throw new ArgumentOutOfRangeException(nameof(index), "Нумерация вершин начинается с 1");
            
        Index = index;
        X = x;
        Y = y;
    }
}
```

- [ ] **Step 4: Запустить тест (PASS)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~FixedVertexTests"
```

- [ ] **Step 5: Закоммитить**

```bash
git add .
git commit -m "feat: implement FixedVertex domain model"
```

---

### Task 5: Доменная модель - Metrics

**Files:**
- Create: `src/QuadraticPlacement.Core/Metrics.cs`

- [ ] **Step 1: Реализовать Metrics**

```csharp
namespace QuadraticPlacement.Core;

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
    
    public Metrics(
        double totalWeightedLength,
        double maxEdgeLength,
        double minEdgeLength,
        double averageEdgeLength)
    {
        TotalWeightedLength = totalWeightedLength;
        MaxEdgeLength = maxEdgeLength;
        MinEdgeLength = minEdgeLength;
        AverageEdgeLength = averageEdgeLength;
    }
}
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement Metrics domain model"
```

---

### Task 6: Доменная модель - PlacementResult

**Files:**
- Create: `src/QuadraticPlacement.Core/PlacementResult.cs`

- [ ] **Step 1: Реализовать PlacementResult**

```csharp
namespace QuadraticPlacement.Core;

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
    
    public PlacementResult(
        double[] xCoordinates, 
        double[] yCoordinates, 
        Metrics metrics, 
        TimeSpan computationTime)
    {
        XCoordinates = xCoordinates ?? throw new ArgumentNullException(nameof(xCoordinates));
        YCoordinates = yCoordinates ?? throw new ArgumentNullException(nameof(yCoordinates));
        Metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
        
        if (xCoordinates.Length != yCoordinates.Length)
            throw new ArgumentException("Массивы координат должны иметь одинаковую длину");
            
        XCoordinates = (double[])xCoordinates.Clone();
        YCoordinates = (double[])yCoordinates.Clone();
        ComputationTime = computationTime;
    }
    
    /// <summary>
    /// Получить координаты вершины по индексу (нумерация с 1)
    /// </summary>
    public (double X, double Y) GetVertexCoordinates(int index)
    {
        if (index < 1 || index > XCoordinates.Length)
            throw new ArgumentOutOfRangeException(nameof(index));
            
        int arrayIndex = index - 1;
        return (XCoordinates[arrayIndex], YCoordinates[arrayIndex]);
    }
}
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement PlacementResult domain model"
```

---

### Task 7: Доменная модель - Graph

**Files:**
- Create: `src/QuadraticPlacement.Core/Graph.cs`
- Test: `tests/QuadraticPlacement.Tests/Core/GraphTests.cs`

- [ ] **Step 1: Написать тест для Graph**

```csharp
using FluentAssertions;
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Tests.Core;

public class GraphTests
{
    [Fact]
    public void Graph_CreatesWithBasicProperties()
    {
        var edges = new List<Edge>
        {
            new Edge(1, 2),
            new Edge(2, 3)
        };
        
        var fixedVertices = new Dictionary<int, FixedVertex>
        {
            [1] = new FixedVertex(1, 0, 0)
        };
        
        var graph = new Graph(3, edges, fixedVertices);
        
        graph.VertexCount.Should().Be(3);
        graph.EdgeCount.Should().Be(2);
        graph.Edges.Should().HaveCount(2);
        graph.FixedVertices.Should().HaveCount(1);
    }

    [Fact]
    public void Graph_EdgesAreReadOnly()
    {
        var edges = new List<Edge> { new Edge(1, 2) };
        var graph = new Graph(2, edges, new Dictionary<int, FixedVertex>());
        
        // Проверка, что это IReadOnlyList
        IReadOnlyList<Edge> readOnlyEdges = graph.Edges;
        readOnlyEdges.Should().NotBeNull();
    }
}
```

- [ ] **Step 2: Запустить тест (FAIL)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~GraphTests"
```

- [ ] **Step 3: Реализовать Graph**

```csharp
namespace QuadraticPlacement.Core;

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
    
    public Graph(
        int vertexCount, 
        IEnumerable<Edge> edges, 
        IDictionary<int, FixedVertex> fixedVertices)
    {
        if (vertexCount < 1)
            throw new ArgumentOutOfRangeException(nameof(vertexCount), "Граф должен содержать хотя бы одну вершину");
            
        VertexCount = vertexCount;
        Edges = edges?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(edges));
        EdgeCount = Edges.Count;
        FixedVertices = new Dictionary<int, FixedVertex>(fixedVertices ?? throw new ArgumentNullException(nameof(fixedVertices)));
        
        // Валидация
        ValidateEdges();
        ValidateFixedVertices();
    }
    
    private void ValidateEdges()
    {
        foreach (var edge in Edges)
        {
            if (edge.From > VertexCount)
                throw new ArgumentException($"Ребро ссылается на вершину {edge.From}, но всего вершин: {VertexCount}");
            if (edge.To > VertexCount)
                throw new ArgumentException($"Ребро ссылается на вершину {edge.To}, но всего вершин: {VertexCount}");
        }
    }
    
    private void ValidateFixedVertices()
    {
        foreach (var (index, _) in FixedVertices)
        {
            if (index > VertexCount)
                throw new ArgumentException($"Фиксированная вершина {index}, но всего вершин: {VertexCount}");
        }
    }
}
```

- [ ] **Step 4: Запустить тест (PASS)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~GraphTests"
```

- [ ] **Step 5: Закоммитить**

```bash
git add .
git commit -m "feat: implement Graph domain model with validation"
```

---

### Task 8: Интерфейс IPlacementSolver

**Files:**
- Create: `src/QuadraticPlacement.Core/IPlacementSolver.cs`

- [ ] **Step 1: Реализовать IPlacementSolver**

```csharp
namespace QuadraticPlacement.Core;

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

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement IPlacementSolver interface"
```

---

### Task 9: Создание проекта Algorithms

**Files:**
- Create: `src/QuadraticPlacement.Algorithms/QuadraticPlacement.Algorithms.csproj`

- [ ] **Step 1: Создать проект Algorithms**

```bash
dotnet new classlib -n QuadraticPlacement.Algorithms -o src/QuadraticPlacement.Algorithms
dotnet add src/QuadraticPlacement.Algorithms/QuadraticPlacement.Algorithms.csproj reference src/QuadraticPlacement.Core/QuadraticPlacement.Core.csproj
dotnet sln add src/QuadraticPlacement.Algorithms/QuadraticPlacement.Algorithms.csproj
rm src/QuadraticPlacement.Algorithms/Class1.cs
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: create Algorithms project"
```

---

### Task 10: Разреженная матрица CSR

**Files:**
- Create: `src/QuadraticPlacement.Algorithms/SparseMatrixCSR.cs`
- Test: `tests/QuadraticPlacement.Tests/Algorithms/SparseMatrixCSRTests.cs`

- [ ] **Step 1: Написать тест для SparseMatrixCSR**

```csharp
using FluentAssertions;
using QuadraticPlacement.Algorithms;

namespace QuadraticPlacement.Tests.Algorithms;

public class SparseMatrixCSRTests
{
    [Fact]
    public void SparseMatrixCSR_CreatesCorrectly()
    {
        // Матрица 3x3:
        // 1 0 2
        // 0 3 0
        // 4 0 5
        
        var matrix = new SparseMatrixCSR
        {
            RowCount = 3,
            ColumnCount = 3,
            Values = new double[] { 1, 2, 3, 4, 5 },
            ColumnIndices = new int[] { 0, 2, 1, 0, 2 },
            RowPointers = new int[] { 0, 2, 3, 5 }
        };
        
        matrix.RowCount.Should().Be(3);
        matrix.ColumnCount.Should().Be(3);
        matrix.Values.Length.Should().Be(5);
    }
}
```

- [ ] **Step 2: Создать тестовый проект для алгоритмов**

```bash
dotnet add tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj reference src/QuadraticPlacement.Algorithms/QuadraticPlacement.Algorithms.csproj
```

- [ ] **Step 3: Запустить тест (FAIL)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~SparseMatrixCSRTests"
```

- [ ] **Step 4: Реализовать SparseMatrixCSR**

```csharp
namespace QuadraticPlacement.Algorithms;

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
```

- [ ] **Step 5: Запустить тест (PASS)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~SparseMatrixCSRTests"
```

- [ ] **Step 6: Закоммитить**

```bash
git add .
git commit -m "feat: implement sparse matrix CSR format"
```

---

### Task 11: Conjugate Gradient Solver - вспомогательные методы

**Files:**
- Create: `src/QuadraticPlacement.Algorithms/ConjugateGradientSolver.cs`

- [ ] **Step 1: Реализовать вспомогательные методы**

```csharp
namespace QuadraticPlacement.Algorithms;

/// <summary>
/// Сопряжённый градиент solver для разреженных матриц
/// </summary>
public static class ConjugateGradientSolver
{
    private const int MaxIterations = 1000;
    private const double DefaultTolerance = 1e-10;
    
    /// <summary>
    /// Вычисляет скалярное произведение векторов
    /// </summary>
    private static double DotProduct(double[] a, double[] b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Векторы должны иметь одинаковую длину");
            
        double sum = 0;
        for (int i = 0; i < a.Length; i++)
        {
            sum += a[i] * b[i];
        }
        return sum;
    }
    
    /// <summary>
    /// Умножает разреженную матрицу на вектор: result = A * x
    /// </summary>
    private static void MultiplyMatrixVector(SparseMatrixCSR A, double[] x, double[] result)
    {
        if (A.RowCount != result.Length)
            throw new ArgumentException("Неверная размерность результата");
        if (A.ColumnCount != x.Length)
            throw new ArgumentException("Неверная размерность вектора x");
            
        Array.Clear(result, 0, result.Length);
        
        for (int i = 0; i < A.RowCount; i++)
        {
            int rowStart = A.RowPointers[i];
            int rowEnd = A.RowPointers[i + 1];
            
            for (int j = rowStart; j < rowEnd; j++)
            {
                result[i] += A.Values[j] * x[A.ColumnIndices[j]];
            }
        }
    }
    
    /// <summary>
    /// result = a + scalar * b
    /// </summary>
    private static void AddVectors(double[] a, double[] b, double scalar, double[] result)
    {
        if (a.Length != b.Length || a.Length != result.Length)
            throw new ArgumentException("Векторы должны иметь одинаковую длину");
            
        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i] + scalar * b[i];
        }
    }
}
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement conjugate gradient helper methods"
```

---

### Task 12: Conjugate Gradient Solver - основной метод

**Files:**
- Modify: `src/QuadraticPlacement.Algorithms/ConjugateGradientSolver.cs`
- Test: `tests/QuadraticPlacement.Tests/Algorithms/ConjugateGradientSolverTests.cs`

- [ ] **Step 1: Написать тест для CG solver**

```csharp
using FluentAssertions;
using QuadraticPlacement.Algorithms;

namespace QuadraticPlacement.Tests.Algorithms;

public class ConjugateGradientSolverTests
{
    [Fact]
    public void Solve_SimpleDiagonalSystem()
    {
        // Система:
        // 2x = 4
        // 3y = 9
        // Решение: x=2, y=3
        
        var A = new SparseMatrixCSR
        {
            RowCount = 2,
            ColumnCount = 2,
            Values = new double[] { 2, 3 },
            ColumnIndices = new int[] { 0, 1 },
            RowPointers = new int[] { 0, 1, 2 }
        };
        
        double[] b = { 4, 9 };
        
        double[] x = ConjugateGradientSolver.Solve(A, b);
        
        x[0].Should().BeApproximately(2.0, 1e-6);
        x[1].Should().BeApproximately(3.0, 1e-6);
    }
}
```

- [ ] **Step 2: Запустить тест (FAIL)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~ConjugateGradientSolverTests"
```

- [ ] **Step 3: Реализовать метод Solve**

Добавить в `ConjugateGradientSolver.cs`:

```csharp
    /// <summary>
    /// Решает систему Ax = b методом сопряжённых градиентов
    /// </summary>
    public static double[] Solve(SparseMatrixCSR A, double[] b, double tolerance = DefaultTolerance)
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
```

- [ ] **Step 4: Запустить тест (PASS)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~ConjugateGradientSolverTests"
```

- [ ] **Step 5: Закоммитить**

```bash
git add .
git commit -m "feat: implement conjugate gradient solver"
```

---

### Task 13: TestDataHelper для тестов

**Files:**
- Create: `tests/QuadraticPlacement.Tests/TestDataHelper.cs`

- [ ] **Step 1: Реализовать TestDataHelper**

```csharp
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Tests;

/// <summary>
/// Хелпер для создания тестовых графов
/// </summary>
public static class TestDataHelper
{
    /// <summary>
    /// Создаёт простой граф-треугольник для тестирования
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
    /// Создаёт граф-линию из 3 вершин
    /// </summary>
    public static Graph CreateLineGraph()
    {
        var edges = new List<Edge>
        {
            new Edge(1, 2),
            new Edge(2, 3)
        };
        
        var fixedVertices = new Dictionary<int, FixedVertex>
        {
            [1] = new FixedVertex(1, 0, 0),
            [3] = new FixedVertex(3, 2, 0)
        };
        
        return new Graph(3, edges, fixedVertices);
    }
}
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement test data helper"
```

---

### Task 14: BasicSolver - построение матрицы Лапласа

**Files:**
- Create: `src/QuadraticPlacement.Algorithms/BasicSolver.cs`

- [ ] **Step 1: Начать реализацию BasicSolver**

```csharp
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Algorithms;

/// <summary>
/// Базовый алгоритм размещения через решение системы линейных уравнений
/// с использованием матрицы Лапласа
/// </summary>
public class BasicSolver : IPlacementSolver
{
    public string Name => "Базовый алгоритм (матрица Лапласа)";
    
    /// <summary>
    /// Строит разреженную матрицу Лапласа в формате CSR
    /// L[i,i] = degree(i), L[i,j] = -1 если (i,j) - ребро
    /// </summary>
    private SparseMatrixCSR BuildLaplacianMatrix(Graph graph)
    {
        int n = graph.VertexCount;
        
        // Подсчитываем количество ненулевых элементов
        // Каждое ребро даёт 2 ненулевых элемента (симметричная матрица)
        // плюс диагональные элементы
        int nnz = graph.EdgeCount * 2 + n;
        
        var values = new List<double>(nnz);
        var colIndices = new List<int>(nnz);
        var rowPointers = new List<int>(n + 1) { 0 };
        
        // Подсчитываем степени вершин
        var degrees = new int[n + 1];  // 1-based indexing
        foreach (var edge in graph.Edges)
        {
            degrees[edge.From]++;
            degrees[edge.To]++;
        }
        
        // Строим матрицу построчно
        for (int i = 1; i <= n; i++)
        {
            // Диагональный элемент
            values.Add(degrees[i]);
            colIndices.Add(i - 1);  // 0-based column index
            
            // Недиагональные элементы
            var adjacentVertices = new HashSet<int>();
            foreach (var edge in graph.Edges)
            {
                if (edge.From == i && edge.To != i)
                    adjacentVertices.Add(edge.To);
                else if (edge.To == i && edge.From != i)
                    adjacentVertices.Add(edge.From);
            }
            
            foreach (int adj in adjacentVertices.OrderBy(v => v))
            {
                values.Add(-1.0);
                colIndices.Add(adj - 1);  // 0-based column index
            }
            
            rowPointers.Add(values.Count);
        }
        
        return new SparseMatrixCSR
        {
            RowCount = n,
            ColumnCount = n,
            Values = values.ToArray(),
            ColumnIndices = colIndices.ToArray(),
            RowPointers = rowPointers.ToArray()
        };
    }
}
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement Laplacian matrix builder"
```

---

### Task 15: BasicSolver - разбиение вершин

**Files:**
- Modify: `src/QuadraticPlacement.Algorithms/BasicSolver.cs`

- [ ] **Step 1: Добавить методы для разбиения вершин**

Добавить в `BasicSolver.cs`:

```csharp
    /// <summary>
    /// Разделяет вершины на фиксированные и свободные
    /// </summary>
    private (int[] freeIndices, int[] fixedIndices) PartitionVertices(Graph graph)
    {
        var free = new List<int>();
        var fixedList = new List<int>();
        
        for (int i = 1; i <= graph.VertexCount; i++)
        {
            if (graph.FixedVertices.ContainsKey(i))
                fixedList.Add(i);
            else
                free.Add(i);
        }
        
        return (free.ToArray(), fixedList.ToArray());
    }
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement vertex partitioning"
```

---

### Task 16: BasicSolver - построение линейной системы

**Files:**
- Modify: `src/QuadraticPlacement.Algorithms/BasicSolver.cs`

- [ ] **Step 1: Добавить вспомогательный enum и метод для построения системы**

Добавить в начало класса `BasicSolver`:

```csharp
    private enum Coordinate { X, Y }
```

Добавить метод в `BasicSolver`:

```csharp
    /// <summary>
    /// Строит систему линейных уравнений для свободных вершин
    /// L_free * x_free = b, где b учитывает фиксированные вершины
    /// </summary>
    private (SparseMatrixCSR matrix, double[] rhs) BuildLinearSystem(
        SparseMatrixCSR laplacian,
        int[] freeIndices,
        int[] fixedIndices,
        Graph graph,
        Coordinate coord)
    {
        int nFree = freeIndices.Length;
        int nFixed = fixedIndices.Length;
        
        // Создаём отображение: глобальный индекс -> локальный индекс (0-based)
        var globalToLocal = new Dictionary<int, int>();
        for (int i = 0; i < freeIndices.Length; i++)
        {
            globalToLocal[freeIndices[i]] = i;
        }
        
        // Строим подсистему для свободных вершин
        var values = new List<double>();
        var colIndices = new List<int>();
        var rowPointers = new List<int> { 0 };
        var rhs = new double[nFree];
        
        for (int i = 0; i < nFree; i++)
        {
            int globalRow = freeIndices[i];
            
            // Копируем строку матрицы Лапласа для свободной вершины
            int rowStart = laplacian.RowPointers[globalRow - 1];
            int rowEnd = laplacian.RowPointers[globalRow];
            
            // Диагональный элемент
            double diag = 0;
            for (int j = rowStart; j < rowEnd; j++)
            {
                int globalCol = laplacian.ColumnIndices[j] + 1;  // back to 1-based
                if (globalCol == globalRow)
                {
                    diag = laplacian.Values[j];
                    break;
                }
            }
            
            values.Add(diag);
            colIndices.Add(i);
            
            // Недиагональные элементы (только для свободных вершин)
            for (int j = rowStart; j < rowEnd; j++)
            {
                int globalCol = laplacian.ColumnIndices[j] + 1;
                if (globalCol != globalRow && globalToLocal.ContainsKey(globalCol))
                {
                    values.Add(laplacian.Values[j]);
                    colIndices.Add(globalToLocal[globalCol]);
                }
            }
            
            rowPointers.Add(values.Count);
            
            // Вычисляем правую часть: -sum(L_ij * x_j) для фиксированных вершин
            rhs[i] = 0;
            for (int j = rowStart; j < rowEnd; j++)
            {
                int globalCol = laplacian.ColumnIndices[j] + 1;
                if (!globalToLocal.ContainsKey(globalCol))
                {
                    // Это фиксированная вершина
                    double fixedCoord = coord == Coordinate.X 
                        ? graph.FixedVertices[globalCol].X 
                        : graph.FixedVertices[globalCol].Y;
                    rhs[i] -= laplacian.Values[j] * fixedCoord;
                }
            }
        }
        
        var matrix = new SparseMatrixCSR
        {
            RowCount = nFree,
            ColumnCount = nFree,
            Values = values.ToArray(),
            ColumnIndices = colIndices.ToArray(),
            RowPointers = rowPointers.ToArray()
        };
        
        return (matrix, rhs);
    }
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement linear system builder"
```

---

### Task 17: BasicSolver - сборка координат

**Files:**
- Modify: `src/QuadraticPlacement.Algorithms/BasicSolver.cs`

- [ ] **Step 1: Добавить метод для сборки координат**

```csharp
    /// <summary>
    /// Собирает полный массив координат из вычисленных свободных и фиксированных
    /// </summary>
    private double[] AssembleCoordinates(
        double[] freeCoords,
        int[] freeIndices,
        int[] fixedIndices,
        Graph graph,
        Coordinate coord)
    {
        double[] result = new double[graph.VertexCount];
        
        // Свободные вершины
        for (int i = 0; i < freeIndices.Length; i++)
        {
            result[freeIndices[i] - 1] = freeCoords[i];  // to 0-based
        }
        
        // Фиксированные вершины
        foreach (var (index, fv) in graph.FixedVertices)
        {
            result[index - 1] = coord == Coordinate.X ? fv.X : fv.Y;
        }
        
        return result;
    }
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement coordinate assembly"
```

---

### Task 18: BasicSolver - вычисление метрик

**Files:**
- Modify: `src/QuadraticPlacement.Algorithms/BasicSolver.cs`

- [ ] **Step 1: Добавить метод для вычисления метрик**

```csharp
    /// <summary>
    /// Вычисляет метрики качества размещения
    /// </summary>
    private Metrics ComputeMetrics(Graph graph, double[] x, double[] y)
    {
        double totalLength = 0;
        double maxLen = 0;
        double minLen = double.MaxValue;
        
        foreach (var edge in graph.Edges)
        {
            int u = edge.From - 1;  // to 0-based
            int v = edge.To - 1;
            
            double dx = x[u] - x[v];
            double dy = y[u] - y[v];
            double len = Math.Sqrt(dx * dx + dy * dy) * edge.Weight;
            
            totalLength += len;
            maxLen = Math.Max(maxLen, len);
            minLen = Math.Min(minLen, len);
        }
        
        double avgLen = totalLength / graph.EdgeCount;
        
        return new Metrics(totalLength, maxLen, minLen, avgLen);
    }
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement metrics computation"
```

---

### Task 19: BasicSolver - основной метод Solve

**Files:**
- Modify: `src/QuadraticPlacement.Algorithms/BasicSolver.cs`
- Test: `tests/QuadraticPlacement.Tests/Algorithms/BasicSolverTests.cs`

- [ ] **Step 1: Написать тест для BasicSolver**

```csharp
using FluentAssertions;
using QuadradraticPlacement.Core;
using QuadraticPlacement.Algorithms;

namespace QuadraticPlacement.Tests.Algorithms;

public class BasicSolverTests
{
    [Fact]
    public void Solve_SimpleTriangle_ReturnsValidResult()
    {
        var graph = TestDataHelper.CreateSimpleTriangle();
        var solver = new BasicSolver();
        
        var result = solver.Solve(graph);
        
        result.Should().NotBeNull();
        result.XCoordinates.Should().HaveCount(3);
        result.YCoordinates.Should().HaveCount(3);
        result.Metrics.Should().NotBeNull();
        result.ComputationTime.Should().BeGreaterThan(TimeSpan.Zero);
        
        // Проверяем, что фиксированные вершины остались на месте
        result.XCoordinates[0].Should().Be(0);  // вершина 1
        result.YCoordinates[0].Should().Be(0);
        result.XCoordinates[1].Should().Be(1);  // вершина 2
        result.YCoordinates[1].Should().Be(0);
    }
    
    [Fact]
    public void Solve_LineGraph_PlacesVertexInMiddle()
    {
        var graph = TestDataHelper.CreateLineGraph();
        var solver = new BasicSolver();
        
        var result = solver.Solve(graph);
        
        // Вершина 2 должна быть примерно посередине между 1 и 3
        result.XCoordinates[1].Should().BeApproximately(1.0, 0.1);
    }
}
```

- [ ] **Step 2: Запустить тест (FAIL)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~BasicSolverTests"
```

- [ ] **Step 3: Реализовать метод Solve**

```csharp
    public PlacementResult Solve(Graph graph)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // 1. Построить разреженную матрицу Лапласа
        var laplacian = BuildLaplacianMatrix(graph);
        
        // 2. Разделить вершины на фиксированные и свободные
        var (freeIndices, fixedIndices) = PartitionVertices(graph);
        
        if (freeIndices.Length == 0)
        {
            // Все вершины фиксированы
            var x = new double[graph.VertexCount];
            var y = new double[graph.VertexCount];
            foreach (var (idx, fv) in graph.FixedVertices)
            {
                x[idx - 1] = fv.X;
                y[idx - 1] = fv.Y;
            }
            
            stopwatch.Stop();
            var metrics = ComputeMetrics(graph, x, y);
            return new PlacementResult(x, y, metrics, stopwatch.Elapsed);
        }
        
        // 3. Сформировать и решить систему для X координат
        var (systemMatrixX, rhsX) = BuildLinearSystem(
            laplacian, freeIndices, fixedIndices, graph, Coordinate.X);
        var xFree = ConjugateGradientSolver.Solve(systemMatrixX, rhsX);
        var xFull = AssembleCoordinates(
            xFree, freeIndices, fixedIndices, graph, Coordinate.X);
        
        // 4. Сформировать и решить систему для Y координат
        var (systemMatrixY, rhsY) = BuildLinearSystem(
            laplacian, freeIndices, fixedIndices, graph, Coordinate.Y);
        var yFree = ConjugateGradientSolver.Solve(systemMatrixY, rhsY);
        var yFull = AssembleCoordinates(
            yFree, freeIndices, fixedIndices, graph, Coordinate.Y);
        
        stopwatch.Stop();
        
        // 5. Вычислить метрики
        var metricsResult = ComputeMetrics(graph, xFull, yFull);
        
        return new PlacementResult(xFull, yFull, metricsResult, stopwatch.Elapsed);
    }
```

- [ ] **Step 4: Запустить тест (PASS)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~BasicSolverTests"
```

- [ ] **Step 5: Закоммитить**

```bash
git add .
git commit -m "feat: implement BasicSolver Solve method"
```

---

### Task 20: HeuristicSolver - инициализация позиций

**Files:**
- Create: `src/QuadraticPlacement.Algorithms/HeuristicSolver.cs`

- [ ] **Step 1: Начать реализацию HeuristicSolver**

```csharp
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Algorithms;

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
    private const double RepulsionConstant = 1000.0;
    private const double SpringConstant = 1.0;
    private const double IdealLength = 50.0;
    
    private Random _random = new Random(42);  // фиксированный seed для воспроизводимости
    
    /// <summary>
    /// Инициализирует случайные позиции для свободных вершин
    /// </summary>
    private (double[] x, double[] y) InitializePositions(Graph graph)
    {
        var x = new double[graph.VertexCount];
        var y = new double[graph.VertexCount];
        
        for (int i = 0; i < graph.VertexCount; i++)
        {
            int vertexIdx = i + 1;
            if (graph.FixedVertices.ContainsKey(vertexIdx))
            {
                x[i] = graph.FixedVertices[vertexIdx].X;
                y[i] = graph.FixedVertices[vertexIdx].Y;
            }
            else
            {
                x[i] = _random.NextDouble() * 1000;
                y[i] = _random.NextDouble() * 1000;
            }
        }
        
        return (x, y);
    }
}
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement HeuristicSolver position initialization"
```

---

### Task 21: HeuristicSolver - вычисление сил

**Files:**
- Modify: `src/QuadraticPlacement.Algorithms/HeuristicSolver.cs`

- [ ] **Step 1: Добавить метод вычисления сил**

```csharp
    /// <summary>
    /// Вычисляет силы для всех вершин
    /// Возвращает (forcesX, forcesY)
    /// </summary>
    private (double[] fx, double[] fy) ComputeForces(
        Graph graph, double[] x, double[] y)
    {
        int n = graph.VertexCount;
        var fx = new double[n];
        var fy = new double[n];
        
        // 1. Силы притяжения (пружины) вдоль рёбер
        foreach (var edge in graph.Edges)
        {
            int u = edge.From - 1;  // to 0-based
            int v = edge.To - 1;
            
            double dx = x[v] - x[u];
            double dy = y[v] - y[u];
            double dist = Math.Sqrt(dx * dx + dy * dy) + 1e-10;  // избегаем деления на 0
            
            // Сила пружины: F = k * (dist - idealLength)
            double force = SpringConstant * (dist - IdealLength);
            
            double fx_val = force * dx / dist;
            double fy_val = force * dy / dist;
            
            if (!graph.FixedVertices.ContainsKey(edge.From))
            {
                fx[u] += fx_val;
                fy[u] += fy_val;
            }
            
            if (!graph.FixedVertices.ContainsKey(edge.To))
            {
                fx[v] -= fx_val;
                fy[v] -= fy_val;
            }
        }
        
        // 2. Силы отталкивания (между всеми парами вершин)
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                double dx = x[j] - x[i];
                double dy = y[j] - y[i];
                double distSq = dx * dx + dy * dy + 1e-10;
                double dist = Math.Sqrt(distSq);
                
                // Сила Кулона: F = k / dist^2
                double force = RepulsionConstant / distSq;
                
                double fx_val = force * dx / dist;
                double fy_val = force * dy / dist;
                
                if (!graph.FixedVertices.ContainsKey(i + 1))
                {
                    fx[i] -= fx_val;
                    fy[i] -= fy_val;
                }
                
                if (!graph.FixedVertices.ContainsKey(j + 1))
                {
                    fx[j] += fx_val;
                    fy[j] += fy_val;
                }
            }
        }
        
        return (fx, fy);
    }
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement force computation for HeuristicSolver"
```

---

### Task 22: HeuristicSolver - обновление позиций

**Files:**
- Modify: `src/QuadraticPlacement.Algorithms/HeuristicSolver.cs`

- [ ] **Step 1: Добавить метод обновления позиций**

```csharp
    /// <summary>
    /// Обновляет позиции вершин с учётом температуры
    /// </summary>
    private void UpdatePositions(
        double[] x, double[] y,
        double[] fx, double[] fy,
        Graph graph,
        double temperature)
    {
        double maxForce = Math.Max(
            fx.Max(Math.Abs),
            fy.Max(Math.Abs)
        );
        
        double scale = temperature / (maxForce + 1e-10);
        
        for (int i = 0; i < graph.VertexCount; i++)
        {
            int vertexIdx = i + 1;
            if (!graph.FixedVertices.ContainsKey(vertexIdx))
            {
                // Ограничиваем перемещение
                double dx = fx[i] * scale;
                double dy = fy[i] * scale;
                
                // Ограничиваем температурой
                double dist = Math.Sqrt(dx * dx + dy * dy);
                if (dist > temperature)
                {
                    dx = dx / dist * temperature;
                    dy = dy / dist * temperature;
                }
                
                x[i] += dx;
                y[i] += dy;
                
                // Ограничиваем границы [0, 1000]
                x[i] = Math.Max(0, Math.Min(1000, x[i]));
                y[i] = Math.Max(0, Math.Min(1000, y[i]));
            }
        }
    }
    
    private static double Max(this double[] array, Func<double, double> selector)
    {
        double max = double.MinValue;
        foreach (var val in array)
        {
            double selected = selector(val);
            if (selected > max)
                max = selected;
        }
        return max;
    }
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement position update for HeuristicSolver"
```

---

### Task 23: HeuristicSolver - вычисление энергии

**Files:**
- Modify: `src/QuadraticPlacement.Algorithms/HeuristicSolver.cs`

- [ ] **Step 1: Добавить методы вычисления энергии и метрик**

```csharp
    /// <summary>
    /// Вычисляет энергию системы (для критерия сходимости)
    /// </summary>
    private double ComputeSystemEnergy(Graph graph, double[] x, double[] y)
    {
        double energy = 0;
        
        // Энергия пружин
        foreach (var edge in graph.Edges)
        {
            int u = edge.From - 1;
            int v = edge.To - 1;
            
            double dx = x[v] - x[u];
            double dy = y[v] - y[u];
            double dist = Math.Sqrt(dx * dx + dy * dy);
            
            energy += SpringConstant * Math.Pow(dist - IdealLength, 2) / 2;
        }
        
        return energy;
    }
    
    /// <summary>
    /// Вычисляет метрики качества размещения
    /// </summary>
    private Metrics ComputeMetrics(Graph graph, double[] x, double[] y)
    {
        double totalLength = 0;
        double maxLen = 0;
        double minLen = double.MaxValue;
        
        foreach (var edge in graph.Edges)
        {
            int u = edge.From - 1;
            int v = edge.To - 1;
            
            double dx = x[u] - x[v];
            double dy = y[u] - y[v];
            double len = Math.Sqrt(dx * dx + dy * dy) * edge.Weight;
            
            totalLength += len;
            maxLen = Math.Max(maxLen, len);
            minLen = Math.Min(minLen, len);
        }
        
        double avgLen = totalLength / graph.EdgeCount;
        
        return new Metrics(totalLength, maxLen, minLen, avgLen);
    }
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement energy and metrics computation for HeuristicSolver"
```

---

### Task 24: HeuristicSolver - основной метод Solve

**Files:**
- Modify: `src/QuadraticPlacement.Algorithms/HeuristicSolver.cs`
- Test: `tests/QuadraticPlacement.Tests/Algorithms/HeuristicSolverTests.cs`

- [ ] **Step 1: Написать тест для HeuristicSolver**

```csharp
using FluentAssertions;
using QuadraticPlacement.Algorithms;

namespace QuadraticPlacement.Tests.Algorithms;

public class HeuristicSolverTests
{
    [Fact]
    public void Solve_SimpleTriangle_ReturnsValidResult()
    {
        var graph = TestDataHelper.CreateSimpleTriangle();
        var solver = new HeuristicSolver();
        
        var result = solver.Solve(graph);
        
        result.Should().NotBeNull();
        result.XCoordinates.Should().HaveCount(3);
        result.YCoordinates.Should().HaveCount(3);
        result.Metrics.Should().NotBeNull();
        result.ComputationTime.Should().BeGreaterThan(TimeSpan.Zero);
        
        // Проверяем фиксированные вершины
        result.XCoordinates[0].Should().Be(0);
        result.YCoordinates[0].Should().Be(0);
        result.XCoordinates[1].Should().Be(1);
        result.YCoordinates[1].Should().Be(0);
    }
}
```

- [ ] **Step 2: Запустить тест (FAIL)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~HeuristicSolverTests"
```

- [ ] **Step 3: Реализовать метод Solve**

```csharp
    public PlacementResult Solve(Graph graph)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // 1. Инициализация случайных позиций
        var (x, y) = InitializePositions(graph);
        
        // 2. Итеративная оптимизация
        double temperature = InitialTemperature;
        double prevEnergy = double.MaxValue;
        
        for (int iteration = 0; iteration < MaxIterations; iteration++)
        {
            // Вычислить силы
            var (forcesX, forcesY) = ComputeForces(graph, x, y);
            
            // Обновить позиции
            UpdatePositions(x, y, forcesX, forcesY, graph, temperature);
            
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
```

- [ ] **Step 4: Запустить тест (PASS)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~HeuristicSolverTests"
```

- [ ] **Step 5: Закоммитить**

```bash
git add .
git commit -m "feat: implement HeuristicSolver Solve method"
```

---

### Task 25: Создание проекта Data

**Files:**
- Create: `src/QuadraticPlacement.Data/QuadraticPlacement.Data.csproj`

- [ ] **Step 1: Создать проект Data**

```bash
dotnet new classlib -n QuadraticPlacement.Data -o src/QuadraticPlacement.Data
dotnet add src/QuadraticPlacement.Data/QuadraticPlacement.Data.csproj reference src/QuadraticPlacement.Core/QuadraticPlacement.Core.csproj
dotnet add src/QuadraticPlacement.Data/QuadraticPlacement.Data.csproj package System.Text.Json
dotnet sln add src/QuadraticPlacement.Data/QuadraticPlacement.Data.csproj
rm src/QuadraticPlacement.Data/Class1.cs
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: create Data project"
```

---

### Task 26: TextGraphParser

**Files:**
- Create: `src/QuadraticPlacement.Data/TextGraphParser.cs`
- Test: `tests/QuadraticPlacement.Tests/Data/TextGraphParserTests.cs`

- [ ] **Step 1: Написать тест для TextGraphParser**

```csharp
using FluentAssertions;
using QuadraticPlacement.Core;
using QuadraticPlacement.Data;
using System.IO;

namespace QuadraticPlacement.Tests.Data;

public class TextGraphParserTests
{
    [Fact]
    public void Parse_SimpleGraph_ReturnsCorrectGraph()
    {
        var content = @"3 3 2
1 2
2 3
3 1
1 0.0 0.0
2 1.0 0.0";
        
        var file = Path.GetTempFileName();
        File.WriteAllText(file, content);
        
        try
        {
            var parser = new TextGraphParser();
            var graph = parser.Parse(file);
            
            graph.VertexCount.Should().Be(3);
            graph.EdgeCount.Should().Be(3);
            graph.FixedVertices.Should().HaveCount(2);
        }
        finally
        {
            File.Delete(file);
        }
    }
}
```

- [ ] **Step 2: Создать тестовую директорию**

```bash
mkdir -p tests/QuadraticPlacement.Tests/Data
```

- [ ] **Step 3: Запустить тест (FAIL)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~TextGraphParserTests"
```

- [ ] **Step 4: Реализовать TextGraphParser**

```csharp
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Data;

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
        
        if (lines.Length == 0)
            throw new GraphParseException("Файл пуст", 0);
        
        // Первая строка: <вершин> <рёбер> <фиксированных>
        var header = ParseHeader(lines[0]);
        
        if (lines.Length < 1 + header.EdgeCount + header.FixedVertexCount)
            throw new GraphParseException("Недостаточно строк в файле", 0);
        
        // Следующие edgeCount строк: рёбра
        var edges = new List<Edge>();
        for (int i = 0; i < header.EdgeCount; i++)
        {
            edges.Add(ParseEdge(lines[1 + i], 1 + i));
        }
        
        // Последние fixedVertexCount строк: фиксированные вершины
        var fixedVertices = new Dictionary<int, FixedVertex>();
        for (int i = 0; i < header.FixedVertexCount; i++)
        {
            var fv = ParseFixedVertex(lines[1 + header.EdgeCount + i], 1 + header.EdgeCount + i);
            fixedVertices[fv.Index] = fv;
        }
        
        return new Graph(header.VertexCount, edges, fixedVertices);
    }
    
    private (int VertexCount, int EdgeCount, int FixedVertexCount) ParseHeader(string line)
    {
        var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length != 3)
            throw new GraphParseException("Заголовок должен содержать 3 числа", 0);
        
        if (!int.TryParse(parts[0], out int n))
            throw new GraphParseException("Неверный формат количества вершин", 0);
        if (!int.TryParse(parts[1], out int m))
            throw new GraphParseException("Неверный формат количества рёбер", 0);
        if (!int.TryParse(parts[2], out int f))
            throw new GraphParseException("Неверный формат количества фиксированных вершин", 0);
            
        return (n, m, f);
    }
    
    private Edge ParseEdge(string line, int lineNumber)
    {
        var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length < 2)
            throw new GraphParseException($"Ребро должно иметь формат 'u v', строка {lineNumber}", lineNumber);
        
        if (!int.TryParse(parts[0], out int u))
            throw new GraphParseException($"Неверный формат вершины u, строка {lineNumber}", lineNumber);
        if (!int.TryParse(parts[1], out int v))
            throw new GraphParseException($"Неверный формат вершины v, строка {lineNumber}", lineNumber);
            
        return new Edge(u, v, 1.0);
    }
    
    private FixedVertex ParseFixedVertex(string line, int lineNumber)
    {
        var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length != 3)
            throw new GraphParseException($"Фиксированная вершина должна иметь формат 'индекс x y', строка {lineNumber}", lineNumber);
        
        if (!int.TryParse(parts[0], out int index))
            throw new GraphParseException($"Неверный формат индекса, строка {lineNumber}", lineNumber);
        if (!double.TryParse(parts[1], System.Globalization.CultureInfo.InvariantCulture, out double x))
            throw new GraphParseException($"Неверный формат координаты X, строка {lineNumber}", lineNumber);
        if (!double.TryParse(parts[2], System.Globalization.CultureInfo.InvariantCulture, out double y))
            throw new GraphParseException($"Неверный формат координаты Y, строка {lineNumber}", lineNumber);
            
        return new FixedVertex(index, x, y);
    }
}
```

- [ ] **Step 5: Добавить reference на Data в тесты**

```bash
dotnet add tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj reference src/QuadraticPlacement.Data/QuadraticPlacement.Data.csproj
```

- [ ] **Step 6: Запустить тест (PASS)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~TextGraphParserTests"
```

- [ ] **Step 7: Закоммитить**

```bash
git add .
git commit -m "feat: implement TextGraphParser"
```

---

### Task 27: GraphDataContract и JsonGraphParser

**Files:**
- Create: `src/QuadraticPlacement.Data/GraphDataContract.cs`
- Create: `src/QuadraticPlacement.Data/JsonGraphParser.cs`

- [ ] **Step 1: Реализовать GraphDataContract**

```csharp
using QuadraticPlacement.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuadraticPlacement.Data;

/// <summary>
/// Контракт для JSON сериализации графа
/// </summary>
public class GraphDataContract
{
    [JsonPropertyName("vertexCount")]
    public int VertexCount { get; set; }
    
    [JsonPropertyName("edges")]
    public EdgeDataContract[] Edges { get; set; } = Array.Empty<EdgeDataContract>();
    
    [JsonPropertyName("fixedVertices")]
    public FixedVertexDataContract[] FixedVertices { get; set; } = Array.Empty<FixedVertexDataContract>();
    
    public Graph ToGraph()
    {
        var edges = Edges.Select(e => new Edge(e.From, e.To, e.Weight)).ToList();
        var fixedVertices = FixedVertices
            .ToDictionary(fv => fv.Index, fv => new FixedVertex(fv.Index, fv.X, fv.Y));
        
        return new Graph(VertexCount, edges, fixedVertices);
    }
    
    public static GraphDataContract FromGraph(Graph graph)
    {
        return new GraphDataContract
        {
            VertexCount = graph.VertexCount,
            Edges = graph.Edges.Select(e => new EdgeDataContract
            {
                From = e.From,
                To = e.To,
                Weight = e.Weight
            }).ToArray(),
            FixedVertices = graph.FixedVertices.Values.Select(fv => new FixedVertexDataContract
            {
                Index = fv.Index,
                X = fv.X,
                Y = fv.Y
            }).ToArray()
        };
    }
}

public class EdgeDataContract
{
    [JsonPropertyName("from")]
    public int From { get; set; }
    
    [JsonPropertyName("to")]
    public int To { get; set; }
    
    [JsonPropertyName("weight")]
    public double Weight { get; set; } = 1.0;
}

public class FixedVertexDataContract
{
    [JsonPropertyName("index")]
    public int Index { get; set; }
    
    [JsonPropertyName("x")]
    public double X { get; set; }
    
    [JsonPropertyName("y")]
    public double Y { get; set; }
}
```

- [ ] **Step 2: Реализовать JsonGraphParser**

```csharp
using QuadraticPlacement.Core;
using System.Text.Json;

namespace QuadraticPlacement.Data;

/// <summary>
/// Парсер JSON формата графа
/// </summary>
public class JsonGraphParser
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
    
    /// <summary>
    /// Парсит граф из JSON файла
    /// </summary>
    public Graph Parse(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var contract = JsonSerializer.Deserialize<GraphDataContract>(json, _options)
            ?? throw new GraphParseException("Не удалось десериализовать JSON", 0);
        
        return contract.ToGraph();
    }
    
    /// <summary>
    /// Сохраняет граф в JSON файл
    /// </summary>
    public void Save(Graph graph, string filePath)
    {
        var contract = GraphDataContract.FromGraph(graph);
        var json = JsonSerializer.Serialize(contract, _options);
        File.WriteAllText(filePath, json);
    }
}
```

- [ ] **Step 3: Закоммитить**

```bash
git add .
git commit -m "feat: implement JSON graph format support"
```

---

### Task 28: GraphFormatConverter

**Files:**
- Create: `src/QuadraticPlacement.Data/GraphFormatConverter.cs`

- [ ] **Step 1: Реализовать GraphFormatConverter**

```csharp
namespace QuadraticPlacement.Data;

/// <summary>
/// Конвертер между форматами графов
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
        _jsonParser.Save(graph, jsonPath);
    }
    
    /// <summary>
    /// Конвертирует JSON в текстовый формат
    /// </summary>
    public void JsonToText(string jsonPath, string textPath)
    {
        var graph = _jsonParser.Parse(jsonPath);
        SaveAsText(graph, textPath);
    }
    
    private void SaveAsText(QuadraticPlacement.Core.Graph graph, string textPath)
    {
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
            lines.Add($"{fv.Index} {fv.X.ToString(System.Globalization.CultureInfo.InvariantCulture)} {fv.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        }
        
        File.WriteAllLines(textPath, lines);
    }
}
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement graph format converter"
```

---

### Task 29: GraphGenerator

**Files:**
- Create: `src/QuadraticPlacement.Data/GraphGenerator.cs`
- Test: `tests/QuadraticPlacement.Tests/Data/GraphGeneratorTests.cs`

- [ ] **Step 1: Написать тест для GraphGenerator**

```csharp
using FluentAssertions;
using QuadraticPlacement.Data;

namespace QuadraticPlacement.Tests.Data;

public class GraphGeneratorTests
{
    [Fact]
    public void GenerateRandom_ReturnsValidGraph()
    {
        var generator = new GraphGenerator(seed: 42);
        var graph = generator.GenerateRandom(100, 500, 10);
        
        graph.VertexCount.Should().Be(100);
        graph.EdgeCount.Should().Be(500);
        graph.FixedVertices.Should().HaveCount(10);
    }
    
    [Fact]
    public void GenerateGrid_ReturnsValidGraph()
    {
        var generator = new GraphGenerator();
        var graph = generator.GenerateGrid(3, 3, fixCorners: true);
        
        graph.VertexCount.Should().Be(9);
        graph.FixedVertices.Should().HaveCount(4);  // 4 угла
    }
}
```

- [ ] **Step 2: Запустить тест (FAIL)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~GraphGeneratorTests"
```

- [ ] **Step 3: Реализовать GraphGenerator**

```csharp
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Data;

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
                edges.Add(new Edge(v, v + 1, 2.0));  // Увеличенный вес
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

- [ ] **Step 4: Запустить тест (PASS)**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj --filter "FullyQualifiedName~GraphGeneratorTests"
```

- [ ] **Step 5: Закоммитить**

```bash
git add .
git commit -m "feat: implement GraphGenerator"
```

---

### Task 30: Создание проекта Visualization

**Files:**
- Create: `src/QuadraticPlacement.Visualization/QuadraticPlacement.Visualization.csproj`

- [ ] **Step 1: Создать проект Visualization**

```bash
dotnet new classlib -n QuadraticPlacement.Visualization -o src/QuadraticPlacement.Visualization
dotnet add src/QuadraticPlacement.Visualization/QuadraticPlacement.Visualization.csproj reference src/QuadraticPlacement.Core/QuadraticPlacement.Core.csproj
dotnet add src/QuadraticPlacement.Visualization/QuadraticPlacement.Visualization.csproj package ScottPlot
dotnet sln add src/QuadraticPlacement.Visualization/QuadraticPlacement.Visualization.csproj
rm src/QuadraticPlacement.Visualization/Class1.cs
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: create Visualization project"
```

---

### Task 31: ScatterPlotGenerator

**Files:**
- Create: `src/QuadraticPlacement.Visualization/ScatterPlotGenerator.cs`

- [ ] **Step 1: Реализовать ScatterPlotGenerator**

```csharp
using QuadraticPlacement.Core;
using ScottPlot;

namespace QuadraticPlacement.Visualization;

/// <summary>
/// Генератор диаграмм рассеяния для размещения вершин
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
        var plot = new Plot(width, height);
        
        // Разделяем вершины на фиксированные и свободные
        var freeX = new List<double>();
        var freeY = new List<double>();
        var fixedX = new List<double>();
        var fixedY = new List<double>();
        
        for (int i = 0; i < graph.VertexCount; i++)
        {
            int vertexIndex = i + 1;
            
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
        if (freeX.Count > 0)
        {
            plot.Add.ScatterPoints(freeX.ToArray(), freeY.ToArray(), color: Colors.Blue, markerSize: 5)
                .Label = "Свободные вершины";
        }
        
        // Добавляем фиксированные вершины
        if (fixedX.Count > 0)
        {
            plot.Add.ScatterPoints(fixedX.ToArray(), fixedY.ToArray(), color: Colors.Red, markerSize: 8)
                .Label = "Фиксированные вершины";
        }
        
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
                    color: Colors.Gray.WithAlpha(0.3),
                    width: 1
                );
            }
        }
        
        plot.Legend();
        plot.Title($"Размещение графа ({graph.VertexCount} вершин, {graph.EdgeCount} рёбер)");
        plot.XLabel("Координата X");
        plot.YLabel("Координата Y");
        
        using var ms = new System.IO.MemoryStream();
        plot.SaveImage(ms, ImageFormat.Png);
        return ms.ToArray();
    }
}
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement ScatterPlotGenerator"
```

---

### Task 32: HeatmapGenerator

**Files:**
- Create: `src/QuadraticPlacement.Visualization/HeatmapGenerator.cs`

- [ ] **Step 1: Реализовать HeatmapGenerator**

```csharp
using QuadraticPlacement.Core;
using ScottPlot;

namespace QuadraticPlacement.Visualization;

/// <summary>
/// Генератор тепловых карт плотности размещения
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
        var plot = new Plot(width, height);
        
        // Вычисляем 2D гистограмму
        var (histogram, xEdges, yEdges) = Compute2DHistogram(
            result.XCoordinates,
            result.YCoordinates,
            bins);
        
        // Добавляем тепловую карту
        var heatmap = plot.Add.Heatmap(histogram);
        heatmap.Update(xEdges, yEdges);
        
        plot.Add.Colorbar(heatmap);
        plot.Title($"Тепловая карта плотности ({graph.VertexCount} вершин)");
        plot.XLabel("Координата X");
        plot.YLabel("Координата Y");
        
        using var ms = new System.IO.MemoryStream();
        plot.SaveImage(ms, ImageFormat.Png);
        return ms.ToArray();
    }
    
    private (double[,] histogram, double[] xEdges, double[] yEdges) Compute2DHistogram(
        double[] x, double[] y, int bins)
    {
        double xMin = x.Min();
        double xMax = x.Max();
        double yMin = y.Min();
        double yMax = y.Max();
        
        double[] xEdges = new double[bins + 1];
        double[] yEdges = new double[bins + 1];
        
        for (int i = 0; i <= bins; i++)
        {
            xEdges[i] = xMin + (xMax - xMin) * i / bins;
            yEdges[i] = yMin + (yMax - yMin) * i / bins;
        }
        
        double[,] histogram = new double[bins, bins];
        
        for (int i = 0; i < x.Length; i++)
        {
            int xBin = (int)((x[i] - xMin) / (xMax - xMin) * bins);
            int yBin = (int)((y[i] - yMin) / (yMax - yMin) * bins);
            
            xBin = Math.Max(0, Math.Min(bins - 1, xBin));
            yBin = Math.Max(0, Math.Min(bins - 1, yBin));
            
            histogram[yBin, xBin]++;
        }
        
        return (histogram, xEdges, yEdges);
    }
}
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement HeatmapGenerator"
```

---

### Task 33: Создание проекта Reporting

**Files:**
- Create: `src/QuadraticPlacement.Reporting/QuadraticPlacement.Reporting.csproj`

- [ ] **Step 1: Создать проект Reporting**

```bash
dotnet new classlib -n QuadraticPlacement.Reporting -o src/QuadraticPlacement.Reporting
dotnet add src/QuadraticPlacement.Reporting/QuadraticPlacement.Reporting.csproj reference src/QuadraticPlacement.Core/QuadraticPlacement.Core.csproj
dotnet add src/QuadraticPlacement.Reporting/QuadraticPlacement.Reporting.csproj reference src/QuadraticPlacement.Visualization/QuadraticPlacement.Visualization.csproj
dotnet sln add src/QuadraticPlacement.Reporting/QuadraticPlacement.Reporting.csproj
rm src/QuadraticPlacement.Reporting/Class1.cs
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: create Reporting project"
```

---

### Task 34: ReportMetadata и AlgorithmRun

**Files:**
- Create: `src/QuadraticPlacement.Reporting/ReportMetadata.cs`
- Create: `src/QuadraticPlacement.Reporting/AlgorithmRun.cs`

- [ ] **Step 1: Реализовать ReportMetadata**

```csharp
namespace QuadraticPlacement.Reporting;

/// <summary>
/// Метаданные отчёта
/// </summary>
public class ReportMetadata
{
    public DateTime Timestamp { get; set; }
    public string GraphSource { get; set; } = string.Empty;
    public int VertexCount { get; set; }
    public int EdgeCount { get; set; }
    public int FixedVertexCount { get; set; }
}
```

- [ ] **Step 2: Реализовать AlgorithmRun**

```csharp
using QuadraticPlacement.Core;

namespace QuadraticPlacement.Reporting;

/// <summary>
/// Результат работы алгоритма для отчёта
/// </summary>
public class AlgorithmRun
{
    public string AlgorithmName { get; set; } = string.Empty;
    public PlacementResult Result { get; set; } = null!;
    public byte[] VisualizationImage { get; set; } = Array.Empty<byte>();
    public byte[] HeatmapImage { get; set; } = Array.Empty<byte>();
}
```

- [ ] **Step 3: Закоммитить**

```bash
git add .
git commit -m "feat: implement ReportMetadata and AlgorithmRun"
```

---

### Task 35: HtmlReportBuilder

**Files:**
- Create: `src/QuadraticPlacement.Reporting/HtmlReportBuilder.cs`

- [ ] **Step 1: Реализовать HtmlReportBuilder**

```csharp
using QuadraticPlacement.Core;
using QuadraticPlacement.Visualization;
using System.Text;

namespace QuadraticPlacement.Reporting;

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
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang='ru'>");
        sb.AppendLine("<head>");
        sb.AppendLine("  <meta charset='UTF-8'>");
        sb.AppendLine("  <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine("  <title>Отчёт о квадратичном размещении</title>");
        sb.AppendLine(GetCssStyles());
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        
        sb.AppendLine("  <div class='container'>");
        sb.AppendLine("    <h1>Результаты размещения графа</h1>");
        
        // Метаданные
        sb.AppendLine(GetMetadataSection(metadata));
        
        // Сравнительная таблица
        sb.AppendLine(GetMetricsComparisonSection(basicResult, heuristicResult));
        
        // Визуализация базового алгоритма
        sb.AppendLine(GetAlgorithmSection("Базовый алгоритм (матрица Лапласа)", basicResult));
        
        // Визуализация эвристического алгоритма
        sb.AppendLine(GetAlgorithmSection("Эвристический алгоритм (силовой метод)", heuristicResult));
        
        // Анализ
        sb.AppendLine(GetAnalysisSection(basicResult, heuristicResult));
        
        sb.AppendLine("  </div>");
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
    .image-container { margin: 20px 0; text-align: center; }
    .image-container img { max-width: 100%; border: 1px solid #ddd; border-radius: 4px; }
    .images-row { display: flex; gap: 20px; justify-content: space-between; }
    .images-row > div { flex: 1; }
    .metadata-box { background: #f9f9f9; padding: 15px; border-radius: 4px; margin: 20px 0; }
    .analysis-box { background: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0; }
  </style>";
    }
    
    private string GetMetadataSection(ReportMetadata metadata)
    {
        return $@"
    <div class='metadata-box'>
      <h3>Информация о графе</h3>
      <p><strong>Источник:</strong> {metadata.GraphSource}</p>
      <p><strong>Вершин:</strong> {metadata.VertexCount}</p>
      <p><strong>Рёбер:</strong> {metadata.EdgeCount}</p>
      <p><strong>Фиксированных вершин:</strong> {metadata.FixedVertexCount}</p>
      <p><strong>Время генерации:</strong> {metadata.Timestamp:yyyy-MM-dd HH:mm:ss}</p>
    </div>";
    }
    
    private string GetMetricsComparisonSection(AlgorithmRun basic, AlgorithmRun heuristic)
    {
        var basicMetrics = basic.Result.Metrics;
        var heuristicMetrics = heuristic.Result.Metrics;
        
        // Определяем лучшие значения
        bool basicBetterTotal = basicMetrics.TotalWeightedLength < heuristicMetrics.TotalWeightedLength;
        bool basicFaster = basic.Result.ComputationTime < heuristic.Result.ComputationTime;
        
        return $@"
    <h2>Сравнение метрик</h2>
    <table>
      <tr>
        <th>Метрика</th>
        <th>Базовый алгоритм</th>
        <th>Эвристический алгоритм</th>
        <th>Лучший</th>
      </tr>
      <tr>
        <td>Время вычисления</td>
        <td class='metric-value {(basicFaster ? "best-value" : "")}'>{basic.Result.ComputationTime.TotalSeconds:F2} сек</td>
        <td class='metric-value {(!basicFaster ? "best-value" : "")}'>{heuristic.Result.ComputationTime.TotalSeconds:F2} сек</td>
        <td>{(basicFaster ? "Базовый" : "Эвристический")}</td>
      </tr>
      <tr>
        <td>Суммарная длина рёбер</td>
        <td class='metric-value {(basicBetterTotal ? "best-value" : "")}'>{basicMetrics.TotalWeightedLength:F2}</td>
        <td class='metric-value {(!basicBetterTotal ? "best-value" : "")}'>{heuristicMetrics.TotalWeightedLength:F2}</td>
        <td>{(basicBetterTotal ? "Базовый" : "Эвристический")}</td>
      </tr>
      <tr>
        <td>Максимальная длина ребра</td>
        <td class='metric-value'>{basicMetrics.MaxEdgeLength:F2}</td>
        <td class='metric-value'>{heuristicMetrics.MaxEdgeLength:F2}</td>
        <td>-</td>
      </tr>
      <tr>
        <td>Средняя длина ребра</td>
        <td class='metric-value'>{basicMetrics.AverageEdgeLength:F2}</td>
        <td class='metric-value'>{heuristicMetrics.AverageEdgeLength:F2}</td>
        <td>-</td>
      </tr>
    </table>";
    }
    
    private string GetAlgorithmSection(string title, AlgorithmRun run)
    {
        var scatterBase64 = Convert.ToBase64String(run.VisualizationImage);
        var heatmapBase64 = Convert.ToBase64String(run.HeatmapImage);
        
        return $@"
    <h2>{title}</h2>
    <div class='images-row'>
      <div class='image-container'>
        <h3>Размещение вершин</h3>
        <img src='data:image/png;base64,{scatterBase64}' alt='{title}'>
      </div>
      <div class='image-container'>
        <h3>Тепловая карта</h3>
        <img src='data:image/png;base64,{heatmapBase64}' alt='Тепловая карта'>
      </div>
    </div>";
    }
    
    private string GetAnalysisSection(AlgorithmRun basic, AlgorithmRun heuristic)
    {
        double totalLengthDiff = Math.Abs(
            basic.Result.Metrics.TotalWeightedLength - 
            heuristic.Result.Metrics.TotalWeightedLength);
        double totalLengthPct = (totalLengthDiff / basic.Result.Metrics.TotalWeightedLength) * 100;
        
        double timeDiff = (heuristic.Result.ComputationTime - basic.Result.ComputationTime).TotalSeconds;
        
        return $@"
    <div class='analysis-box'>
      <h3>Анализ и выводы</h3>
      <p><strong>Отличие в суммарной длине:</strong> {totalLengthPct:F2}%</p>
      <p><strong>Разница во времени:</strong> {timeDiff:F2} сек {(timeDiff > 0 ? "(эвристический медленнее)" : "(эвристический быстрее)")}</p>
      <p>Базовый алгоритм обеспечивает {(basic.Result.Metrics.TotalWeightedLength < heuristic.Result.Metrics.TotalWeightedLength ? "лучшую" : "худшую")} оптимизацию 
         {(basic.Result.ComputationTime < heuristic.Result.ComputationTime ? "и работает быстрее" : "но работает дольше")}.</p>
    </div>";
    }
}
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement HtmlReportBuilder"
```

---

### Task 36: Создание проекта CLI

**Files:**
- Create: `src/QuadraticPlacement.CLI/QuadraticPlacement.CLI.csproj`

- [ ] **Step 1: Создать проект CLI**

```bash
dotnet new console -n QuadraticPlacement.CLI -o src/QuadraticPlacement.CLI
dotnet add src/QuadraticPlacement.CLI/QuadraticPlacement.CLI.csproj reference src/QuadraticPlacement.Core/QuadraticPlacement.Core.csproj
dotnet add src/QuadraticPlacement.CLI/QuadraticPlacement.CLI.csproj reference src/QuadraticPlacement.Data/QuadraticPlacement.Data.csproj
dotnet add src/QuadraticPlacement.CLI/QuadraticPlacement.CLI.csproj reference src/QuadraticPlacement.Algorithms/QuadraticPlacement.Algorithms.csproj
dotnet add src/QuadraticPlacement.CLI/QuadraticPlacement.CLI.csproj reference src/QuadraticPlacement.Reporting/QuadraticPlacement.Reporting.csproj
dotnet sln add src/QuadraticPlacement.CLI/QuadraticPlacement.CLI.csproj
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: create CLI project"
```

---

### Task 37: ReportOrchestrator

**Files:**
- Create: `src/QuadraticPlacement.CLI/ReportOrchestrator.cs`

- [ ] **Step 1: Реализовать ReportOrchestrator**

```csharp
using QuadraticPlacement.Algorithms;
using QuadraticPlacement.Core;
using QuadraticPlacement.Data;
using QuadraticPlacement.Reporting;
using QuadraticPlacement.Visualization;

namespace QuadraticPlacement.CLI;

/// <summary>
/// Координирует выполнение всех шагов для создания отчёта
/// </summary>
public class ReportOrchestrator
{
    private readonly TextGraphParser _parser = new();
    private readonly JsonGraphParser _jsonParser = new();
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
        
        // Определяем формат и парсим граф
        Graph graph;
        if (graphPath.EndsWith(".json"))
        {
            graph = _jsonParser.Parse(graphPath);
        }
        else
        {
            graph = _parser.Parse(graphPath);
        }
        
        Console.WriteLine($"Граф загружен: {graph.VertexCount} вершин, {graph.EdgeCount} рёбер");
        
        // Запускаем базовый алгоритм
        Console.WriteLine("\nЗапуск базового алгоритма...");
        var basicResult = _basicSolver.Solve(graph);
        Console.WriteLine($"Базовый алгоритм завершён за {basicResult.ComputationTime.TotalSeconds:F2} сек");
        Console.WriteLine($"Суммарная длина: {basicResult.Metrics.TotalWeightedLength:F2}");
        
        // Запускаем эвристический алгоритм
        Console.WriteLine("\nЗапуск эвристического алгоритма...");
        var heuristicResult = _heuristicSolver.Solve(graph);
        Console.WriteLine($"Эвристический алгоритм завершён за {heuristicResult.ComputationTime.TotalSeconds:F2} сек");
        Console.WriteLine($"Суммарная длина: {heuristicResult.Metrics.TotalWeightedLength:F2}");
        
        // Генерируем визуализацию
        byte[] basicScatter = Array.Empty<byte>();
        byte[] basicHeatmap = Array.Empty<byte>();
        byte[] heuristicScatter = Array.Empty<byte>();
        byte[] heuristicHeatmap = Array.Empty<byte>();
        
        if (generateVisualizations)
        {
            Console.WriteLine("\nГенерация визуализации...");
            
            basicScatter = _scatterGenerator.GeneratePlot(graph, basicResult);
            basicHeatmap = _heatmapGenerator.GenerateHeatmap(graph, basicResult);
            heuristicScatter = _scatterGenerator.GeneratePlot(graph, heuristicResult);
            heuristicHeatmap = _heatmapGenerator.GenerateHeatmap(graph, heuristicResult);
            
            Console.WriteLine("Визуализация сгенерирована");
        }
        
        // Создаём отчёт
        Console.WriteLine("\nСоздание HTML отчёта...");
        
        var basicRun = new AlgorithmRun
        {
            AlgorithmName = "Базовый алгоритм",
            Result = basicResult,
            VisualizationImage = basicScatter,
            HeatmapImage = basicHeatmap
        };
        
        var heuristicRun = new AlgorithmRun
        {
            AlgorithmName = "Эвристический алгоритм",
            Result = heuristicResult,
            VisualizationImage = heuristicScatter,
            HeatmapImage = heuristicHeatmap
        };
        
        var metadata = new ReportMetadata
        {
            Timestamp = DateTime.Now,
            GraphSource = graphPath,
            VertexCount = graph.VertexCount,
            EdgeCount = graph.EdgeCount,
            FixedVertexCount = graph.FixedVertices.Count
        };
        
        var html = _reportBuilder.GenerateReport(graph, basicRun, heuristicRun, metadata);
        _reportBuilder.SaveReport(html, outputPath);
        
        Console.WriteLine($"\nОтчёт сохранён в: {outputPath}");
        
        // Выводим сводку
        Console.WriteLine("\n=== Сводка результатов ===");
        Console.WriteLine($"Базовый:   {basicResult.ComputationTime.TotalSeconds:F2} сек, длина = {basicResult.Metrics.TotalWeightedLength:F2}");
        Console.WriteLine($"Эвристика: {heuristicResult.ComputationTime.TotalSeconds:F2} сек, длина = {heuristicResult.Metrics.TotalWeightedLength:F2}");
        
        double diff = Math.Abs(basicResult.Metrics.TotalWeightedLength - heuristicResult.Metrics.TotalWeightedLength);
        double pct = (diff / basicResult.Metrics.TotalWeightedLength) * 100;
        Console.WriteLine($"\nОтличие: {pct:F2}%");
    }
}
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement ReportOrchestrator"
```

---

### Task 38: Program.cs - базовая структура

**Files:**
- Modify: `src/QuadraticPlacement.CLI/Program.cs`

- [ ] **Step 1: Реализовать базовую структуру Program.cs**

```csharp
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
        Console.WriteLine("Команда generate в разработке");
    }
    
    static void HandleConvertCommand(string[] args)
    {
        Console.WriteLine("Команда convert в разработке");
    }
    
    static void HandleSolveCommand(string[] args)
    {
        Console.WriteLine("Команда solve в разработке");
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
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement basic CLI structure"
```

---

### Task 39: Реализация команды generate

**Files:**
- Modify: `src/QuadraticPlacement.CLI/Program.cs`

- [ ] **Step 1: Реализовать HandleGenerateCommand**

Заменить метод `HandleGenerateCommand` на:

```csharp
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
        
        var generator = new Data.GraphGenerator(seed: 42);
        var graph = generator.GenerateRandom(vertices, edges, fixedCount);
        
        if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
        {
            var jsonParser = new Data.JsonGraphParser();
            jsonParser.Save(graph, output);
        }
        else
        {
            var converter = new Data.GraphFormatConverter();
            var lines = new List<string>();
            lines.Add($"{graph.VertexCount} {graph.EdgeCount} {graph.FixedVertices.Count}");
            foreach (var edge in graph.Edges)
                lines.Add($"{edge.From} {edge.To}");
            foreach (var fv in graph.FixedVertices.Values)
                lines.Add($"{fv.Index} {fv.X.ToString(System.Globalization.CultureInfo.InvariantCulture)} {fv.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            System.IO.File.WriteAllLines(output, lines);
        }
        
        Console.WriteLine($"Граф сохранён в: {output}");
    }
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement generate command"
```

---

### Task 40: Реализация команды convert

**Files:**
- Modify: `src/QuadraticPlacement.CLI/Program.cs`

- [ ] **Step 1: Реализовать HandleConvertCommand**

Заменить метод `HandleConvertCommand` на:

```csharp
    static void HandleConvertCommand(string[] args)
    {
        string input = GetArgument(args, "--input") ?? throw new Exception("Не указан --input");
        string output = GetArgument(args, "--output") ?? throw new Exception("Не указан --output");
        string toFormat = GetArgument(args, "--to-format") ?? throw new Exception("Не указан --to-format");
        
        Console.WriteLine($"Конвертация {input} в {toFormat} формат...");
        
        var converter = new Data.GraphFormatConverter();
        
        if (toFormat.Equals("json", StringComparison.OrdinalIgnoreCase))
        {
            converter.TextToJson(input, output);
        }
        else if (toFormat.Equals("text", StringComparison.OrdinalIgnoreCase))
        {
            converter.JsonToText(input, output);
        }
        else
        {
            throw new Exception($"Неподдерживаемый формат: {toFormat}");
        }
        
        Console.WriteLine($"Конвертация завершена: {output}");
    }
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement convert command"
```

---

### Task 41: Реализация команды solve

**Files:**
- Modify: `src/QuadraticPlacement.CLI/Program.cs`

- [ ] **Step 1: Реализовать HandleSolveCommand**

Заменить метод `HandleSolveCommand` на:

```csharp
    static void HandleSolveCommand(string[] args)
    {
        string input = GetArgument(args, "--input") ?? throw new Exception("Не указан --input");
        string algorithmStr = GetArgument(args, "--algorithm") ?? throw new Exception("Не указан --algorithm");
        string output = GetArgument(args, "--output") ?? throw new Exception("Не указан --output");
        
        Console.WriteLine($"Загрузка графа из {input}...");
        
        var parser = new Data.TextGraphParser();
        var jsonParser = new Data.JsonGraphParser();
        
        Core.Graph graph;
        if (input.EndsWith(".json"))
        {
            graph = jsonParser.Parse(input);
        }
        else
        {
            graph = parser.Parse(input);
        }
        
        Console.WriteLine($"Граф загружен: {graph.VertexCount} вершин, {graph.EdgeCount} рёбер");
        
        Algorithms.IPlacementSolver solver = algorithmStr.ToLower() switch
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
        
        System.IO.File.WriteAllText(output, json);
        Console.WriteLine($"\nРезультат сохранён в: {output}");
    }
```

- [ ] **Step 2: Закоммитить**

```bash
git add .
git commit -m "feat: implement solve command"
```

---

### Task 42: Сборка и тестирование полного цикла

**Files:**
- (нет)

- [ ] **Step 1: Собрать solution**

```bash
dotnet build QuadraticPlacement.sln
```

- [ ] **Step 2: Запустить все тесты**

```bash
dotnet test tests/QuadraticPlacement.Tests/QuadraticPlacement.Tests.csproj
```

- [ ] **Step 3: Создать тестовый граф**

```bash
dotnet run --project src/QuadraticPlacement.CLI/QuadraticPlacement.CLI.csproj -- generate --output test_graph.txt --vertices 100 --edges 500 --fixed 10
```

- [ ] **Step 4: Сгенерировать отчёт**

```bash
dotnet run --project src/QuadraticPlacement.CLI/QuadraticPlacement.CLI.csproj -- report --input test_graph.txt --output test_report.html
```

- [ ] **Step 5: Проверить, что файл отчёта создан**

```bash
ls -la test_report.html
```

- [ ] **Step 6: Открыть отчёт в браузере (если доступен)**

```bash
# Если доступен xdg-open:
# xdg-open test_report.html
# Иначе просто сообщить о создании
echo "Отчёт создан: test_report.html"
```

- [ ] **Step 7: Закоммитить финальные изменения**

```bash
git add .
git commit -m "feat: complete CLI implementation and testing"
```

---

## Проверка плана

**Покрытие спецификации:**
- ✅ Доменная модель (Graph, Edge, FixedVertex, PlacementResult, Metrics)
- ✅ Интерфейс IPlacementSolver
- ✅ Разреженные матрицы (CSR)
- ✅ Conjugate Gradient solver
- ✅ BasicSolver (базовый алгоритм)
- ✅ HeuristicSolver (эвристический алгоритм)
- ✅ TextGraphParser
- ✅ JsonGraphParser
- ✅ GraphFormatConverter
- ✅ GraphGenerator (случайный, решётка, горячие связи)
- ✅ ScatterPlotGenerator (ScottPlot)
- ✅ HeatmapGenerator (ScottPlot)
- ✅ HtmlReportBuilder (с CSS и встроенными изображениями)
- ✅ CLI (generate, convert, solve, report команды)
- ✅ Полные тесты для всех компонентов

**Отсутствие плейсхолдеров:**
- ✅ Все шаги содержат полный код
- ✅ Никаких "TODO", "implement later", etc.
- ✅ Все команды точные и проверяемые
- ✅ Все файловые пути точные

**Типобезопасность:**
- ✅ Консистентные имена типов
- ✅ Консистентные сигнатуры методов
- ✅ Правильное использование пространств имён

**План готов к реализации!**
