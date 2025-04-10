using NameSorter_DyeDurham.Domain.Entities;
using NameSorter_DyeDurham.Infrastructure.Services;
using NSubstitute;
using Serilog;

namespace NameSorter_DyeDurham.Test.UnitTests;

public class ConsoleOutputWriterServiceTests : IDisposable
{
    private readonly TextWriter _originalConsoleOut;
    private bool _disposed;

    public ConsoleOutputWriterServiceTests()
    {
        // Capture the original Console.Out
        _originalConsoleOut = Console.Out;
    }

    [Fact]
    public async Task WriteAsync_ShouldWriteNamesToConsole_WhenListIsNotEmpty()
    {
        // Arrange
        var people = new List<Person>
        {
            new("Doe", ["John"]),
            new("Smith", ["Jane"])
        };

        var logger = Substitute.For<ILogger>();
        var service = new ConsoleOutputWriterService(logger);

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        await service.WriteAsync(people);

        // Assert
        var output = consoleOutput.ToString();
        Assert.Contains("John Doe", output);
        Assert.Contains("Jane Smith", output);
    }

    [Fact]
    public async Task WriteAsync_ShouldWriteNoNamesMessage_WhenListIsEmpty()
    {
        // Arrange
        var people = new List<Person>();
        var logger = Substitute.For<ILogger>();
        var service = new ConsoleOutputWriterService(logger);

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        await service.WriteAsync(people);

        // Assert
        var output = consoleOutput.ToString();
        Assert.Contains("No names to display.", output);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Reset Console.Out to its original state
                Console.SetOut(_originalConsoleOut);
            }

            _disposed = true;
        }
    }
}

