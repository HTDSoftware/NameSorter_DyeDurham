using Microsoft.Extensions.DependencyInjection;
using NameSorter_DyeDurham.Application.Interfaces;
using NameSorter_DyeDurham.Application.Services;
using NameSorter_DyeDurham.Domain.Interfaces;
using NameSorter_DyeDurham.Infrastructure.Services;
using NSubstitute;
using Serilog;

namespace NameSorter_DyeDurham.Test.IntegrationTests;

public class IntegrationTests
{
    private static readonly string[] expected = ["Charlie Brown", "John Doe", "Jane Smith"];
    private static readonly string[] expectedArray = ["Charlie Brown", "John Doe", "Jane Smith"];

    private static ServiceProvider BuildServiceProvider(bool useConsoleOutput, bool useFileOutput, string inputFilePath, string outputFilePath = "output.txt")
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .CreateLogger();

        var services = new ServiceCollection();

        // Register Serilog as a singleton ILogger
        services.AddSingleton<ILogger>(Log.Logger);

        // NSubstitute
        var config = Substitute.For<IAppConfigurationService>();
        config.UseConsoleOutput.Returns(useConsoleOutput);
        config.UseFileOutput.Returns(useFileOutput);
        config.OutputFilename.Returns(outputFilePath);
        services.AddSingleton(config);

        // Register services
        services.AddSingleton<INameParserService, NameParserService>();
        services.AddSingleton<INameSorterService, NameSorterService>();
        services.AddSingleton<INameProcessingService, NameProcessingService>();

        // Pass the filename to FilenameSourceService
        services.AddSingleton<INameSourceService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger>();
            return new FilenameSourceService(inputFilePath, logger);
        });

        services.AddSingleton<FileOutputWriterService>();
        services.AddSingleton<ConsoleOutputWriterService>();

        // Register composite output writer
        services.AddSingleton<IOutputWriterService>(provider =>
        {
            var writers = new List<IOutputWriterService>();

            if (config.UseFileOutput)
            {
                writers.Add(provider.GetRequiredService<FileOutputWriterService>());
            }

            if (config.UseConsoleOutput)
            {
                writers.Add(provider.GetRequiredService<ConsoleOutputWriterService>());
            }

            if (writers.Count == 0)
            {
                throw new InvalidOperationException("No output method is configured.");
            }

            var logger = provider.GetRequiredService<ILogger>();
            return new CompositeOutputWriterService(writers, logger);
        });

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task ShouldProcessNamesAndWriteToFile()
    {
        // Arrange
        var inputFilePath = "test-input.txt";
        var outputFilePath = "test-output_1.txt";
        File.WriteAllLines(inputFilePath, ["John Doe", "Jane Smith", "Charlie Brown"]);

        var serviceProvider = BuildServiceProvider(useConsoleOutput: false, useFileOutput: true, inputFilePath, outputFilePath);
        var processor = serviceProvider.GetRequiredService<INameProcessingService>();

        // Act
        await processor.ProcessAsync(inputFilePath);

        // Assert
        Assert.True(File.Exists(outputFilePath));
        var output = File.ReadAllLines(outputFilePath);
        Assert.Equal(expectedArray, output);

        // Cleanup
        File.Delete(inputFilePath);
        File.Delete(outputFilePath);
    }

    [Fact]
    public async Task ShouldProcessNamesAndWriteToConsole()
    {
        // Arrange
        var inputFilePath = "test-input.txt";
        File.WriteAllLines(inputFilePath, ["John Doe", "Jane Smith", "Charlie Brown"]);

        var serviceProvider = BuildServiceProvider(useConsoleOutput: true, useFileOutput: false, inputFilePath);
        var processor = serviceProvider.GetRequiredService<INameProcessingService>();

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        await processor.ProcessAsync(inputFilePath);

        // Assert
        var output = consoleOutput.ToString();
        Assert.Contains("Charlie Brown", output);
        Assert.Contains("Jane Smith", output);
        Assert.Contains("John Doe", output);

        // Cleanup
        File.Delete(inputFilePath);
    }

    [Fact]
    public async Task ShouldProcessNamesAndWriteToBothConsoleAndFile()
    {
        // Arrange
        var inputFilePath = "test-input.txt";
        var outputFilePath = "test-output_2.txt";
        File.WriteAllLines(inputFilePath, ["John Doe", "Jane Smith", "Charlie Brown"]);

        var serviceProvider = BuildServiceProvider(useConsoleOutput: true, useFileOutput: true, inputFilePath, outputFilePath);
        var processor = serviceProvider.GetRequiredService<INameProcessingService>();

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        await processor.ProcessAsync(inputFilePath);

        // Assert
        // Verify file output
        Assert.True(File.Exists(outputFilePath));
        var fileOutput = File.ReadAllLines(outputFilePath);
        Assert.Equal(expected, fileOutput);

        // Verify console output
        var consoleOutputText = consoleOutput.ToString();
        Assert.Contains("Charlie Brown", consoleOutputText);
        Assert.Contains("Jane Smith", consoleOutputText);
        Assert.Contains("John Doe", consoleOutputText);

        // Cleanup
        File.Delete(inputFilePath);
        File.Delete(outputFilePath);
    }

    [Fact]
    public void ShouldThrowExceptionWhenDependencyResolutionFails()
    {
        // Arrange
        var inputFilePath = "test-input.txt";
        var outputFilePath = "test-output_3.txt";

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            var serviceProvider = BuildServiceProvider(useConsoleOutput: false, useFileOutput: false, inputFilePath, outputFilePath);
            var processor = serviceProvider.GetRequiredService<INameProcessingService>();
        });

        // Verify the exception message or inner exception if needed
        Assert.Contains("No output method is configured.", exception.Message);

        // Cleanup
        File.Delete(inputFilePath);
        File.Delete(outputFilePath);
    }
}
