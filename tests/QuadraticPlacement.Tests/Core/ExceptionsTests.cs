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
