using NameSorter_DyeDurham.Application.Services;

namespace NameSorter_DyeDurham.Test.UnitTests;

public class NameParserServiceTests
{
    private readonly NameParserService _service;

    public NameParserServiceTests()
    {
        _service = new NameParserService();
    }

    [Fact]
    public void Parse_ShouldReturnPerson_WhenInputIsValidWithOneGivenName()
    {
        // Arrange
        var input = "John Doe";

        // Act
        var result = _service.Parse(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Doe", result.Surname);
        Assert.Single(result.GivenNames);
        Assert.Contains("John", result.GivenNames);
    }

    [Fact]
    public void Parse_ShouldReturnPerson_WhenInputIsValidWithMultipleGivenNames()
    {
        // Arrange
        var input = "John Michael Doe";

        // Act
        var result = _service.Parse(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Doe", result.Surname);
        Assert.Equal(2, result.GivenNames.Count);
        Assert.Contains("John", result.GivenNames);
        Assert.Contains("Michael", result.GivenNames);
    }

    [Fact]
    public void Parse_ShouldReturnPerson_WhenInputIsValidWithMaximumParts()
    {
        // Arrange
        var input = "John Michael David Doe";

        // Act
        var result = _service.Parse(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Doe", result.Surname);
        Assert.Equal(3, result.GivenNames.Count);
        Assert.Contains("John", result.GivenNames);
        Assert.Contains("Michael", result.GivenNames);
        Assert.Contains("David", result.GivenNames);
    }

    [Fact]
    public void Parse_ShouldThrowArgumentException_WhenInputIsNull()
    {
        // Arrange
        #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        string input = null;
        #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        // Act & Assert
        #pragma warning disable CS8604 // Possible null reference argument.
        var exception = Assert.Throws<ArgumentException>(() => _service.Parse(input));
        #pragma warning restore CS8604 // Possible null reference argument.
        Assert.Equal("Person Input cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void Parse_ShouldThrowArgumentException_WhenInputIsEmpty()
    {
        // Arrange
        var input = "";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.Parse(input));
        Assert.Equal("Person Input cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void Parse_ShouldThrowArgumentException_WhenInputIsWhitespace()
    {
        // Arrange
        var input = "   ";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.Parse(input));
        Assert.Equal("Person Input cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void Parse_ShouldThrowArgumentException_WhenInputHasNoSurname()
    {
        // Arrange
        var input = "John";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.Parse(input));
        Assert.Equal("Invalid name format.", exception.Message);
    }

    [Fact]
    public void Parse_ShouldThrowArgumentException_WhenInputHasTooManyParts()
    {
        // Arrange
        var input = "John Michael David Edward Doe";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.Parse(input));
        Assert.Equal("Invalid name format.", exception.Message);
    }
}
