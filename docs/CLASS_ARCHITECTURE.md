# Архитектура классов — QuadraticPlacement

> Система квадратичного размещения элементов на плоскости.
> Проект состоит из 6 сборок (проектов), каждая из которых отвечает за отдельный слой функциональности.

---

## Общая структура решения

```
QuadraticPlacement.sln
├── QuadraticPlacement.Core           — Доменные модели и контракты
├── QuadraticPlacement.Algorithms     — Алгоритмы размещения
├── QuadraticPlacement.Data           — Ввод/вывод данных и генерация графов
├── QuadraticPlacement.Reporting      — Генерация HTML-отчётов
├── QuadraticPlacement.Visualization  — Визуализация результатов (графики)
└── QuadraticPlacement.CLI            — Командная строка (точка входа)
```

### Диаграмма зависимостей проектов

```
CLI ──► Reporting ──► Core
 │         │
 │         ▼
 │    Visualization ──► Core
 │
 ├─► Algorithms ──► Core
 └─► Data ──► Core
```

---

## 1. QuadraticPlacement.Core — Доменные модели

Содержит базовые сущности задачи, интерфейсы алгоритмов и типы исключений.
Не имеет внешних зависимостей.

### Классы

#### `Edge`

> Ребро графа с весом. Неизменяемый (immutable).

| Свойство | Тип | Описание |
|----------|-----|----------|
| `From` | `int` | Индекс начальной вершины (нумерация с 1) |
| `To` | `int` | Индекс конечной вершины (нумерация с 1) |
| `Weight` | `double` | Вес ребра (по умолчанию 1.0) |

**Конструктор:** `Edge(int from, int to, double weight = 1.0)`
- Валидирует: `from ≥ 1`, `to ≥ 1`, `weight > 0`

---

#### `FixedVertex`

> Фиксированная вершина с заданными координатами. Неизменяемый.

| Свойство | Тип | Описание |
|----------|-----|----------|
| `Index` | `int` | Индекс вершины (нумерация с 1) |
| `X` | `double` | Координата X |
| `Y` | `double` | Координата Y |

**Конструктор:** `FixedVertex(int index, double x, double y)`
- Валидирует: `index ≥ 1`

---

#### `Graph`

> Граф связей между элементами. Неизменяемый после создания.

| Свойство | Тип | Описание |
|----------|-----|----------|
| `VertexCount` | `int` | Общее количество вершин |
| `EdgeCount` | `int` | Количество рёбер |
| `Edges` | `IReadOnlyList<Edge>` | Список рёбер графа |
| `FixedVertices` | `IReadOnlyDictionary<int, FixedVertex>` | Словарь фиксированных вершин (индекс → вершина) |

**Конструктор:** `Graph(int vertexCount, IEnumerable<Edge> edges, IDictionary<int, FixedVertex> fixedVertices)`

**Приватные методы:**
- `ValidateEdges()` — проверяет, что все рёбра ссылаются на существующие вершины
- `ValidateFixedVertices()` — проверяет, что фиксированные вершины существуют в графе

---

#### `PlacementResult`

> Результат размещения графа. Координатные массивы клонируются при создании.

| Свойство | Тип | Описание |
|----------|-----|----------|
| `XCoordinates` | `double[]` | Массив X-координат всех вершин |
| `YCoordinates` | `double[]` | Массив Y-координат всех вершин |
| `Metrics` | `Metrics` | Метрики качества размещения |
| `ComputationTime` | `TimeSpan` | Время вычисления |

**Конструктор:** `PlacementResult(double[] xCoordinates, double[] yCoordinates, Metrics metrics, TimeSpan computationTime)`

**Методы:**

| Метод | Возвращает | Описание |
|-------|-----------|----------|
| `GetVertexCoordinates(int index)` | `(double X, double Y)` | Координаты вершины по индексу (нумерация с 1) |

---

#### `Metrics`

> Метрики качества размещения. Неизменяемый.

| Свойство | Тип | Описание |
|----------|-----|----------|
| `TotalWeightedLength` | `double` | Суммарная взвешенная длина всех рёбер |
| `MaxEdgeLength` | `double` | Максимальная длина ребра |
| `MinEdgeLength` | `double` | Минимальная длина ребра |
| `AverageEdgeLength` | `double` | Средняя длина ребра |

**Конструктор:** `Metrics(double totalWeightedLength, double maxEdgeLength, double minEdgeLength, double averageEdgeLength)`

---

### Интерфейсы

#### `IPlacementSolver`

> Контракт алгоритма размещения.

| Член | Тип | Описание |
|------|-----|----------|
| `Name` | `string` | Название алгоритма (только чтение) |
| `Solve(Graph graph)` | `PlacementResult` | Решает задачу размещения для заданного графа |

---

### Исключения

```
Exception
 └── PlacementException          — Базовое исключение для ошибок размещения
      ├── GraphParseException     — Ошибка парсинга графа (содержит LineNumber)
      └── ConvergenceException    — Алгоритм не сходится (содержит IterationsCompleted, FinalError)
```

| Класс | Свойства | Описание |
|-------|----------|----------|
| `PlacementException` | — | Базовый класс исключений домена |
| `GraphParseException` | `int LineNumber` | Номер строки с ошибкой в исходном файле |
| `ConvergenceException` | `int IterationsCompleted`, `double FinalError` | Информация о неудачной сходимости |

---

## 2. QuadraticPlacement.Algorithms — Алгоритмы размещения

Реализации `IPlacementSolver`. Зависит от `Core`.

### Классы

#### `BasicSolver : IPlacementSolver`

> Базовый алгоритм размещения через решение системы линейных уравнений с матрицей Лапласа.
> Метод сопряжённых градиентов для разреженных СЛАУ. Даёт математически оптимальный результат.

| Свойство | Тип | Описание |
|----------|-----|----------|
| `Name` | `string` | `"Базовый алгоритм (матрица Лапласа)"` (только чтение) |

**Публичные методы:**

| Метод | Описание |
|-------|----------|
| `Solve(Graph graph)` | Основной метод. Строит матрицу Лапласа, разделяет вершины, решает СЛАУ для X и Y |

**Приватные методы:**

| Метод | Описание |
|-------|----------|
| `BuildLaplacianMatrix(Graph)` | Строит разреженную матрицу Лапласа в формате CSR |
| `PartitionVertices(Graph)` | Разделяет вершины на свободные и фиксированные |
| `BuildLinearSystem(...)` | Формирует подсистему для свободных вершин (матрица + правая часть) |
| `AssembleCoordinates(...)` | Собирает полный массив координат из вычисленных и фиксированных |
| `CalculateMetrics(Graph, double[], double[])` | Вычисляет метрики качества размещения |

**Алгоритм:**
1. Построение матрицы Лапласа `L` (CSR-формат)
2. Разделение вершин на свободные `V_S` и фиксированные `V_F`
3. Редукция системы: `L_SS · x_S = −L_SF · x_F` (аналогично для Y)
4. Решение СЛАУ методом сопряжённых градиентов
5. Сборка полных координат и вычисление метрик

---

#### `HeuristicSolver : IPlacementSolver`

> Эвристический силовой алгоритм размещения (force-directed).
> Моделирует систему пружин (притяжение) и кулоновских сил (отталкивание) с имитацией отжига.

| Свойство | Тип | Описание |
|----------|-----|----------|
| `Name` | `string` | `"Эвристический алгоритм (силовой метод)"` (только чтение) |
| `EnergyHistory` | `List<double>` | История значений энергии на каждой итерации |

**Публичные методы:**

| Метод | Описание |
|-------|----------|
| `Solve(Graph graph)` | Основной метод. Итерационное силовое размещение с охлаждением |

**Приватные методы:**

| Метод | Описание |
|-------|----------|
| `InitializePositions(Graph)` | Инициализирует случайные позиции свободных вершин |
| `ComputeForces(Graph, double[], double[])` | Вычисляет силы притяжения и отталкивания |
| `UpdatePositions(...)` | Обновляет позиции с учётом температуры |
| `ComputeSystemEnergy(Graph, double[], double[])` | Вычисляет энергию системы |
| `CalculateMetrics(Graph, double[], double[])` | Вычисляет метрики качества |

**Константы:**

| Константа | Значение | Описание |
|-----------|----------|----------|
| `MaxIterations` | 1000 | Максимальное число итераций (переопределяется адаптивно) |
| `ConvergenceThreshold` | 1e-6 | Порог стагнации |
| `CoolingRate` | 0.95 | Коэффициент охлаждения |
| `InitialTemperature` | 100.0 | Начальная температура |
| `RepulsionConstant` | 1000.0 | Константа отталкивания |
| `SpringConstant` | 1.0 | Коэффициент жёсткости пружины |
| `IdealLength` | 50.0 | Идеальная длина пружины |

---

#### `ConjugateGradientSolver` *(статический класс)*

> Решатель СЛАУ методом сопряжённых градиентов для разреженных матриц в CSR-формате.

**Публичные методы:**

| Метод | Описание |
|-------|----------|
| `Solve(SparseMatrixCSR A, double[] b, double tolerance = 1e-10)` | Решает систему `Ax = b` |

**Приватные методы:**

| Метод | Описание |
|-------|----------|
| `DotProduct(double[] a, double[] b)` | Скалярное произведение |
| `MultiplyMatrixVector(SparseMatrixCSR A, double[] x, double[] result)` | Умножение матрицы на вектор |
| `AddVectors(double[] a, double[] b, double scalar, double[] result)` | Векторная операция `a + scalar·b` |

**Константы:** `MaxIterations = 1000`, `DefaultTolerance = 1e-10`

---

#### `SparseMatrixCSR` *(struct)*

> Разреженная матрица в формате CSR (Compressed Sparse Row).

| Поле | Тип | Описание |
|------|-----|----------|
| `Values` | `double[]` | Ненулевые значения |
| `ColumnIndices` | `int[]` | Индексы столбцов (0-based) |
| `RowPointers` | `int[]` | Указатели на начало строк |
| `RowCount` | `int` | Количество строк |
| `ColumnCount` | `int` | Количество столбцов |

---

#### `ArrayExtensions` *(internal static)*

> Методы расширения для массивов.

| Метод | Описание |
|-------|----------|
| `Max(double[] array, Func<double, double> selector)` | Максимальное значение с селектором |

---

### Диаграмма классов Algorithms

```
┌──────────────────────────┐
│   «interface»            │
│   IPlacementSolver       │
│──────────────────────────│
│ + Name: string           │
│ + Solve(Graph): Result   │
└──────────┬───────────────┘
           │ implements
     ┌─────┴──────────────┐
     │                    │
┌────▼──────────┐  ┌──────▼──────────┐
│  BasicSolver  │  │ HeuristicSolver │
│───────────────│  │─────────────────│
│ + Name        │  │ + Name          │
│ + Solve()     │  │ + Solve()       │
│               │  │ + EnergyHistory │
│ -BuildLaplac..│  │                 │
│ -PartitionV.. │  │ -InitPosition.. │
│ -BuildLinear..│  │ -ComputeForces  │
│ -AssembleCoo..│  │ -UpdatePositi.. │
│ -CalculateM.. │  │ -ComputeEnerg.. │
└───────┬───────┘  │ -CalculateMet.. │
        │          └─────────────────┘
        │ uses
┌───────▼───────────────┐
│ ConjugateGradientSolv. │ «static»
│───────────────────────│
│ + Solve(A, b, tol)    │
│ - DotProduct()        │
│ - MultiplyMatrixVec.. │
│ - AddVectors()        │
└───────┬───────────────┘
        │ uses
┌───────▼───────┐
│SparseMatrixCSR│ «struct»
│───────────────│
│ Values        │
│ ColumnIndices │
│ RowPointers   │
│ RowCount      │
│ ColumnCount   │
└───────────────┘
```

---

## 3. QuadraticPlacement.Data — Ввод/вывод данных

Парсеры, конвертеры и генераторы графов. Зависит от `Core`.

### Классы

#### `GraphDataContract`

> DTO для сериализации/десериализации графа в JSON. Содержит вложенные классы `EdgeData` и `FixedVertexData`.

| Свойство | Тип | Описание |
|----------|-----|----------|
| `VertexCount` | `int` | Количество вершин |
| `Edges` | `List<EdgeData>` | Рёбра графа |
| `FixedVertices` | `List<FixedVertexData>` | Фиксированные вершины |

**Вложенные классы:**

##### `GraphDataContract.EdgeData`
| Свойство | Тип |
|----------|-----|
| `From` | `int` |
| `To` | `int` |
| `Weight` | `double` |

Конструкторы: без параметров, из `Edge`.

##### `GraphDataContract.FixedVertexData`
| Свойство | Тип |
|----------|-----|
| `Index` | `int` |
| `X` | `double` |
| `Y` | `double` |

Конструкторы: без параметров, из `FixedVertex`.

**Статические методы:**

| Метод | Описание |
|-------|----------|
| `FromGraph(Graph)` | Создаёт контракт из доменного `Graph` |
| `ToGraph()` | Преобразует контракт в доменный `Graph` |

---

#### `JsonGraphParser`

> Парсер JSON формата графа.

**Статические методы:**

| Метод | Возвращает | Описание |
|-------|-----------|----------|
| `Parse(string json)` | `Graph` | Парсит граф из JSON-строки |
| `ParseFromFile(string filePath)` | `Graph` | Парсит граф из JSON-файла |
| `ToJson(Graph graph)` | `string` | Сериализует граф в JSON-строку |
| `SaveToFile(Graph, string)` | `void` | Сохраняет граф в JSON-файл |

---

#### `TextGraphParser`

> Парсер текстового формата графа (из спецификации задачи).

**Формат:** первая строка — `n m f`, затем `m` строк рёбер `u v [w]`, затем `f` строк фиксированных вершин `idx x y`.

**Статические методы:**

| Метод | Возвращает | Описание |
|-------|-----------|----------|
| `Parse(string text)` | `Graph` | Парсит граф из текстовой строки |
| `ParseFromFile(string filePath)` | `Graph` | Парсит граф из текстового файла |
| `ToText(Graph graph)` | `string` | Преобразует граф в текстовый формат |
| `SaveToFile(Graph, string)` | `void` | Сохраняет граф в текстовый файл |

---

#### `GraphFormatConverter` *(статический)*

> Конвертер между текстовым и JSON форматами графов.

**Статические методы:**

| Метод | Описание |
|-------|----------|
| `TextToJson(string text)` | Текст → JSON строка |
| `TextFileToJsonFile(string, string)` | Текстовый файл → JSON файл |
| `JsonToText(string json)` | JSON строка → текст |
| `JsonFileToTextFile(string, string)` | JSON файл → текстовый файл |

---

#### `GraphGenerator` *(статический)*

> Генератор тестовых графов с различными топологиями.

**Статические методы:**

| Метод | Описание |
|-------|----------|
| `GenerateRandom(...)` | Случайный граф с заданными параметрами |
| `GenerateGrid(int rows, int cols, ...)` | Граф-решётка (2D сетка) |
| `GenerateWithHotConnections(...)` | Граф с «горячими» цепями связей |
| `SetSeed(int seed)` | Установить seed генератора |

**Параметры `GenerateRandom`:** `vertexCount`, `edgeCount`, `fixedVertexCount`, `minWeight`, `maxWeight`, `coordinateRange`

**Параметры `GenerateGrid`:** `rows`, `cols`, `fixCorners`, `cellSize`

**Параметры `GenerateWithHotConnections`:** `vertexCount`, `hotChainsCount`, `chainLength`, `randomEdgesCount`, `hotWeight`, `randomWeight`

---

### Диаграмма классов Data

```
┌──────────────────────┐
│   GraphDataContract  │
│──────────────────────│
│ VertexCount          │
│ Edges: List<EdgeData>│
│ FixedVertices: List..│
│──────────────────────│
│ + FromGraph(Graph)   │
│ + ToGraph(): Graph   │
└──────┬───────────────┘
       │ содержит
 ┌─────┴─────────┐
 │               │
▼               ▼
EdgeData    FixedVertexData

┌──────────────────┐    ┌──────────────────┐
│  JsonGraphParser │    │  TextGraphParser  │
│  «static»        │    │  «static»         │
│──────────────────│    │──────────────────│
│ + Parse()        │    │ + Parse()         │
│ + ParseFromFile()│    │ + ParseFromFile() │
│ + ToJson()       │    │ + ToText()        │
│ + SaveToFile()   │    │ + SaveToFile()    │
└────────┬─────────┘    └────────┬─────────┘
         │                       │
         └───────┬───────────────┘
                 ▼
         ┌───────────────┐
         │GraphFormatCon.│ «static»
         │───────────────│
         │ + TextToJson()│
         │ + JsonToText()│
         │ + FileToFile()│
         └───────────────┘

┌──────────────────┐
│  GraphGenerator  │ «static»
│──────────────────│
│ + GenerateRandom │
│ + GenerateGrid   │
│ + GenerateWithH. │
│ + SetSeed()      │
└──────────────────┘
```

---

## 4. QuadraticPlacement.Reporting — Генерация отчётов

Построение HTML-отчётов с встроенными визуализациями. Зависит от `Core`.

### Классы

#### `AlgorithmRun`

> Контейнер данных о запуске алгоритма. Включает результат, метрики, визуализации и лог-сообщения.

| Свойство | Тип | Описание |
|----------|-----|----------|
| `Metadata` | `ReportMetadata` | Метаданные запуска |
| `Result` | `PlacementResult` | Результат размещения |
| `ExecutionTimeMs` | `long` | Время выполнения (мс) |
| `Iterations` | `int` | Количество итераций |
| `ObjectiveHistory` | `List<double>` | История значений целевой функции |
| `PlacementHistory` | `List<PlacementResult>` | История размещений (для анимации) |
| `Messages` | `List<string>` | Лог-сообщения |
| `Success` | `bool` | Признак успешного завершения |
| `Visualizations` | `Dictionary<string, byte[]>` | Визуализации (ключ → PNG-байты) |

**Методы:**

| Метод | Возвращает | Описание |
|-------|-----------|----------|
| `AddVisualization(string, byte[])` | `void` | Добавляет изображение |
| `AddMessage(string)` | `void` | Добавляет сообщение в лог |
| `AddWarning(string)` | `void` | Добавляет предупреждение |
| `AddError(string)` | `void` | Добавляет ошибку (ставит `Success = false`) |
| `GetObjectiveStatistics()` | `ObjectiveStatistics` | Вычисляет статистику целевой функции |
| `GetSummary()` | `string` | Краткое резюме запуска |

---

#### `ObjectiveStatistics`

> Статистика по значениям целевой функции.

| Свойство | Тип | Описание |
|----------|-----|----------|
| `Initial` | `double` | Начальное значение |
| `Final` | `double` | Финальное значение |
| `Minimum` | `double` | Минимум |
| `Maximum` | `double` | Максимум |
| `Average` | `double` | Среднее |
| `Median` | `double` | Медиана |
| `StandardDeviation` | `double` | Стандартное отклонение |
| `ImprovementPercentage` | `double` | Улучшение в % (вычисляемое) |

---

#### `ReportMetadata`

> Метаданные отчёта: идентификатор, параметры алгоритма, автор и т.д.

| Свойство | Тип | Описание |
|----------|-----|----------|
| `ReportId` | `string` | Уникальный ID (GUID) |
| `GeneratedAt` | `DateTime` | Время создания |
| `GraphName` | `string` | Название графа |
| `Description` | `string` | Описание |
| `AlgorithmName` | `string` | Название алгоритма |
| `AlgorithmVersion` | `string` | Версия алгоритма |
| `Author` | `string` | Автор (по умолчанию — `Environment.UserName`) |
| `Parameters` | `Dictionary<string, string>` | Параметры алгоритма |
| `Tags` | `List<string>` | Теги для группировки |
| `CustomData` | `Dictionary<string, object>` | Произвольные метаданные |

**Методы:**

| Метод | Возвращает | Описание |
|-------|-----------|----------|
| `Clone()` | `ReportMetadata` | Создаёт копию |
| `ToString()` | `string` | Строковое представление |

---

#### `HtmlReportBuilder`

> Генератор структурированных HTML-отчётов с CSS-стилями и встроенными визуализациями.

**Публичные методы:**

| Метод | Возвращает | Описание |
|-------|-----------|----------|
| `BuildReport(AlgorithmRun)` | `string` | Создаёт полный HTML-отчёт |
| `SaveToFile(AlgorithmRun, string)` | `void` | Сохраняет отчёт в файл |
| `GenerateReport(AlgorithmRun)` | `string` | Альтернативный метод генерации |

**Структура генерируемого HTML-отчёта:**
1. Заголовок с названием графа
2. Сводка (карточки метрик: вершин, итераций, время, целевая функция)
3. Метаданные (таблица: ID, дата, алгоритм, автор, параметры)
4. Метрики качества (таблица с длинами рёбер, статистикой)
5. Визуализации (встроенные base64 PNG-изображения)
6. История целевой функции (первые/последние 10 значений)
7. Сообщения лога (ошибки, предупреждения)

---

### Диаграмма классов Reporting

```
┌──────────────────────┐
│    AlgorithmRun       │
│──────────────────────│
│ + Metadata           │───────► ReportMetadata
│ + Result             │───────► PlacementResult  (Core)
│ + ExecutionTimeMs    │
│ + Iterations         │
│ + ObjectiveHistory   │
│ + PlacementHistory   │
│ + Messages           │
│ + Success            │
│ + Visualizations     │
│──────────────────────│
│ + AddVisualization() │
│ + AddMessage/Warning │
│ + AddError()         │
│ + GetObjectiveStat.. │───► ObjectiveStatistics
│ + GetSummary()       │
└──────────┬───────────┘
           │ используется
┌──────────▼───────────┐
│  HtmlReportBuilder   │
│──────────────────────│
│ + BuildReport(run)   │
│ + SaveToFile()       │
│ + GenerateReport()   │
└──────────────────────┘
```

---

## 5. QuadraticPlacement.Visualization — Визуализация

Генерация графических изображений (PNG) с помощью библиотеки **ScottPlot**. Зависит от `Core`.

### Классы

#### `ScatterPlotGenerator`

> Генератор точечных графиков размещения вершин.
> Фиксированные вершины — синие, свободные — красные. Для графов ≤ 100 вершин рисует рёбра.

**Методы:**

| Метод | Возвращает | Описание |
|-------|-----------|----------|
| `GenerateScatterPlot(Graph, PlacementResult, int w, int h)` | `byte[]` | Полный график с рёбрами |
| `GenerateSimpleScatterPlot(PlacementResult, HashSet<int>, int w, int h)` | `byte[]` | Простой график без рёбер |

---

#### `HeatmapGenerator`

> Генератор тепловых карт плотности вершин.
> Вычисляет 2D-гистограмму, отображает цветовой картой Turbo.

**Методы:**

| Метод | Возвращает | Описание |
|-------|-----------|----------|
| `GenerateHeatmap(PlacementResult, int gridSize, int w, int h)` | `byte[]` | Тепловая карта всех вершин |
| `GenerateLayeredHeatmap(Graph, PlacementResult, int gridSize, int w, int h)` | `byte[]` | Раздельные слои: свободные (heatmap) + фиксированные (точки) |

---

## 6. QuadraticPlacement.CLI — Командная строка

Точка входа приложения. Координирует все компоненты. Зависит от всех остальных проектов.

### Классы

#### `Program`

> Точка входа CLI-приложения.

**Поддерживаемые команды:**

| Команда | Описание |
|---------|----------|
| `generate` | Генерация случайного графа |
| `convert` | Конвертация между форматами (text ↔ json) |
| `solve` | Решение задачи размещения |
| `report` | Генерация полного отчёта с визуализациями |

---

#### `ReportOrchestrator`

> Координирует пайплайн: загрузка → алгоритмы → визуализация → HTML-отчёты.

| Поле | Тип | Описание |
|------|-----|----------|
| `_basicSolver` | `BasicSolver` | Базовый алгоритм |
| `_heuristicSolver` | `HeuristicSolver` | Эвристический алгоритм |
| `_scatterGenerator` | `ScatterPlotGenerator` | Точечные графики |
| `_heatmapGenerator` | `HeatmapGenerator` | Тепловые карты |
| `_reportBuilder` | `HtmlReportBuilder` | HTML-отчёты |

**Публичные методы:**

| Метод | Описание |
|-------|----------|
| `GenerateFullReport(graphPath, outputDir, baseFileName, generateVisualizations)` | Полный пайплайн генерации отчётов |

---

## Полная диаграмма взаимодействия

```
                        ┌─────────┐
                        │  CLI /  │
                        │ Program │
                        └────┬────┘
                             │
                 ┌───────────┼───────────┐
                 ▼           ▼           ▼
          ┌──────────┐ ┌──────────┐ ┌──────────────┐
          │  Data /  │ │Algorithms│ │    Report    │
          │ Parsers  │ │ Solvers  │ │ Orchestrator │
          └────┬─────┘ └────┬─────┘ └──────┬───────┘
               │            │              │
               ▼            ▼              │
         ┌────────────────────────┐        │
         │     Core / Graph       │        │
         │  Edge, FixedVertex     │        │
         │  PlacementResult       │        │
         │  Metrics               │        │
         │  IPlacementSolver      │        │
         └────────────────────────┘        │
                                           │
               ┌───────────────────────────┤
               ▼                           ▼
        ┌────────────┐            ┌──────────────────┐
        │ Reporting  │            │  Visualization   │
        │ HtmlReport │            │  Scatter / Heat  │
        │ AlgorithmR.│            │  (ScottPlot)     │
        └────────────┘            └──────────────────┘
```

---

## Поток данных

```
┌─────────────┐    ┌─────────────┐    ┌──────────────┐    ┌──────────────┐
│ Входной файл │───►│   Parser    │───►│    Graph     │───►│   Solver     │
│ (.txt/.json) │    │(Text/JSON)  │    │   (Core)     │    │(Basic/Heur.) │
└─────────────┘    └─────────────┘    └──────────────┘    └──────┬───────┘
                                                                  │
                                                                  ▼
┌─────────────┐    ┌─────────────┐    ┌──────────────┐    ┌──────────────┐
│ HTML-отчёт  │◄───│ ReportBldr  │◄───│ AlgorithmRun │◄───│PlacementRes. │
│ (.html)     │    │ + Orchestr. │    │ + Visualiz.  │    │ + Metrics    │
└─────────────┘    └─────────────┘    └──────────────┘    └──────────────┘
```
