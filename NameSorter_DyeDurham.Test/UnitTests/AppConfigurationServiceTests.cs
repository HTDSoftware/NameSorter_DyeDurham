using Microsoft.Extensions.Options;
using NameSorter_DyeDurham.Application.Services;
using NameSorter_DyeDurham.Domain.Entities;

namespace NameSorter_DyeDurham.Test.UnitTests;

public class AppConfigurationServiceTests
{
    [Fact]
    public void Constructor_ShouldInitializeSettings_WhenValidConfigurationProvided()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            UseConsoleOutput = true,
            UseFileOutput = true,
            OutputFilename = "output.txt"
        };
        var options = Options.Create(appSettings);

        // Act
        var service = new AppConfigurationService(options);

        // Assert
        Assert.True(service.UseConsoleOutput);
        Assert.True(service.UseFileOutput);
        Assert.Equal("output.txt", service.OutputFilename);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenUseFileOutputIsTrueAndOutputFilenameIsNull()
    {
        // Arrange
        #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var appSettings = new AppSettings
        {
            UseConsoleOutput = false,
            UseFileOutput = true,
            OutputFilename = null // Invalid configuration
        };
        #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        var options = Options.Create(appSettings);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => new AppConfigurationService(options));
        Assert.Equal("UseFileOutput is true, but OutputFileName is not set. File output mode requires a file name.", exception.Message);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenUseFileOutputIsTrueAndOutputFilenameIsEmpty()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            UseConsoleOutput = false,
            UseFileOutput = true,
            OutputFilename = "" // Invalid configuration
        };
        var options = Options.Create(appSettings);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => new AppConfigurationService(options));
        Assert.Equal("UseFileOutput is true, but OutputFileName is not set. File output mode requires a file name.", exception.Message);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenUseFileOutputIsTrueAndOutputFilenameIsWhitespace()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            UseConsoleOutput = false,
            UseFileOutput = true,
            OutputFilename = "   " // Invalid configuration
        };
        var options = Options.Create(appSettings);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => new AppConfigurationService(options));
        Assert.Equal("UseFileOutput is true, but OutputFileName is not set. File output mode requires a file name.", exception.Message);
    }

    [Fact]
    public void UseConsoleOutput_ShouldReturnCorrectValue()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            UseConsoleOutput = true,
            UseFileOutput = false,
            OutputFilename = "output.txt"
        };
        var options = Options.Create(appSettings);
        var service = new AppConfigurationService(options);

        // Act
        var result = service.UseConsoleOutput;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void UseFileOutput_ShouldReturnCorrectValue()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            UseConsoleOutput = false,
            UseFileOutput = true,
            OutputFilename = "output.txt"
        };
        var options = Options.Create(appSettings);
        var service = new AppConfigurationService(options);

        // Act
        var result = service.UseFileOutput;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OutputFilename_ShouldReturnCorrectValue()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            UseConsoleOutput = false,
            UseFileOutput = true,
            OutputFilename = "output.txt"
        };
        var options = Options.Create(appSettings);
        var service = new AppConfigurationService(options);

        // Act
        var result = service.OutputFilename;

        // Assert
        Assert.Equal("output.txt", result);
    }
}
