using NameSorter_DyeDurham.Application.Interfaces;
using NameSorter_DyeDurham.Application.Services;
using NameSorter_DyeDurham.Domain.Entities;
using NameSorter_DyeDurham.Domain.Interfaces;
using NSubstitute;
using Serilog;

namespace NameSorter_DyeDurham.Test.UnitTests;

public class NameProcessingServiceTests
{
    private readonly INameSourceService _nameSourceService = Substitute.For<INameSourceService>();
    private readonly INameParserService _nameParserService = Substitute.For<INameParserService>();
    private readonly INameSorterService _nameSorterService = Substitute.For<INameSorterService>();
    private readonly IOutputWriterService _outputWriterService = Substitute.For<IOutputWriterService>();

    [Fact]
    public async Task ProcessAsync_ShouldProcessNamesCorrectly()
    {
        // Arrange
        var rawNames = new List<string> { "John Doe", "Jane Smith" };
        var parsedPeople = new List<Person>
        {
            new("Doe", ["John"]),
            new("Smith", ["Jane"])
        };
        var sortedPeople = parsedPeople.OrderBy(p => p.Surname).ToList();

        _nameSourceService.GetNames().Returns(rawNames);
        _nameParserService.Parse("John Doe").Returns(parsedPeople[0]);
        _nameParserService.Parse("Jane Smith").Returns(parsedPeople[1]);
        _nameSorterService.Sort(Arg.Any<IEnumerable<Person>>()).Returns(sortedPeople);

        var logger = Substitute.For<ILogger>();

        var service = new NameProcessingService(
            _nameSourceService,
            _nameParserService,
            _nameSorterService,
            _outputWriterService,
            logger
        );

        // Act
        await service.ProcessAsync("input.txt");

        // Assert
        _nameSourceService.Received(1).GetNames();
        _nameParserService.Received(1).Parse("John Doe");
        _nameParserService.Received(1).Parse("Jane Smith");
        _nameSorterService.Received(1).Sort(Arg.Is<IEnumerable<Person>>(p => p.SequenceEqual(parsedPeople)));
        await _outputWriterService.Received(1).WriteAsync(Arg.Is<IEnumerable<Person>>(p => p.SequenceEqual(sortedPeople)));
    }

    [Fact]
    public async Task ProcessAsync_ShouldHandleParsingErrorsGracefully()
    {
        // Arrange
        var rawNames = new List<string> { "John Doe", "Invalid Name" };
        var parsedPeople = new List<Person>
        {
            new("Doe", ["John"])
        };
        var sortedPeople = parsedPeople;

        _nameSourceService.GetNames().Returns(rawNames);
        _nameParserService.Parse("John Doe").Returns(parsedPeople[0]);
        _nameParserService.When(x => x.Parse("Invalid Name")).Do(x => throw new System.Exception("Invalid format"));
        _nameSorterService.Sort(Arg.Any<IEnumerable<Person>>()).Returns(sortedPeople);

        var logger = Substitute.For<ILogger>();
        var service = new NameProcessingService(
            _nameSourceService,
            _nameParserService,
            _nameSorterService,
            _outputWriterService,
            logger
        );

        // Act
        await service.ProcessAsync("input.txt");

        // Assert
        _nameSourceService.Received(1).GetNames();
        _nameParserService.Received(1).Parse("John Doe");
        _nameParserService.Received(1).Parse("Invalid Name");
        _nameSorterService.Received(1).Sort(Arg.Is<IEnumerable<Person>>(p => p.SequenceEqual(parsedPeople)));
        await _outputWriterService.Received(1).WriteAsync(Arg.Is<IEnumerable<Person>>(p => p.SequenceEqual(sortedPeople)));
    }
}
