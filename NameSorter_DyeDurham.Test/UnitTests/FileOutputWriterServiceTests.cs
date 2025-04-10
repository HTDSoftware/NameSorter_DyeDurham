using NameSorter_DyeDurham.Application.Interfaces;
using NameSorter_DyeDurham.Domain.Entities;
using NameSorter_DyeDurham.Infrastructure.Services;
using NSubstitute;
using Serilog;

namespace NameSorter_DyeDurham.Test.UnitTests;

public class FileOutputWriterServiceTests
{
    private readonly IAppConfigurationService _configService;
    private readonly FileOutputWriterService _service;

    public FileOutputWriterServiceTests()
    {
        _configService = Substitute.For<IAppConfigurationService>();
        var logger = Substitute.For<ILogger>();
        _service = new FileOutputWriterService(_configService, logger);
    }

    [Fact]
    public async Task WriteAsync_ShouldWriteNamesToFile_WhenListIsNotEmpty()
    {
        // Arrange
        var outputFilePath = "test-output-1.txt";
        _configService.OutputFilename.Returns(outputFilePath);

        var people = new List<Person>
        {
            new("Doe", ["John"]),
            new("Smith", ["Jane"])
        };

        try
        {
            // Act
            await _service.WriteAsync(people);

            // Assert
            Assert.True(File.Exists(outputFilePath));
            var fileContent = await File.ReadAllLinesAsync(outputFilePath);
            Assert.Contains("John Doe", fileContent);
            Assert.Contains("Jane Smith", fileContent);
        }
        finally
        {
            // Cleanup
            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }
        }
    }

    [Fact]
    public async Task WriteAsync_ShouldWriteNoNamesMessage_WhenListIsEmpty()
    {
        // Arrange
        var people = new List<Person>();
        var outputFilePath = "test-output-2.txt";
        _configService.OutputFilename.Returns(outputFilePath);

        // Act
        await _service.WriteAsync(people);

        // Assert
        Assert.False(File.Exists(outputFilePath)); // No file should be created
    }

    [Fact]
    public async Task WriteAsync_ShouldCreateOutputDirectory_WhenDirectoryDoesNotExist()
    {
        // Arrange
        var people = new List<Person>
        {
            new("Doe", ["John"])
        };

        var outputDirectory = "test-output-dir";
        var outputFilePath = Path.Combine(outputDirectory, "output.txt");
        _configService.OutputFilename.Returns(outputFilePath);

        // Act
        await _service.WriteAsync(people);

        // Assert
        Assert.True(Directory.Exists(outputDirectory));
        Assert.True(File.Exists(outputFilePath));

        // Cleanup
        File.Delete(outputFilePath);
        Directory.Delete(outputDirectory);
    }

    [Fact]
    public async Task WriteAsync_ShouldOverwriteFile_WhenFileAlreadyExists()
    {
        // Arrange
        var people = new List<Person>
        {
            new("Doe", ["John"])
        };

        var outputFilePath = "test-output-3.txt";
        _configService.OutputFilename.Returns(outputFilePath);

        // Create an existing file
        await File.WriteAllTextAsync(outputFilePath, "Existing content");

        // Act
        await _service.WriteAsync(people);

        // Assert
        var fileContent = await File.ReadAllLinesAsync(outputFilePath);
        Assert.DoesNotContain("Existing content", fileContent); // Ensure old content is overwritten
        Assert.Contains("John Doe", fileContent);

        // Cleanup
        File.Delete(outputFilePath);
    }
}
