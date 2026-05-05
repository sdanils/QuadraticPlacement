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
