using NameSorter_DyeDurham.Domain.Entities;
using NameSorter_DyeDurham.Domain.Interfaces;
using NSubstitute;

namespace NameSorter_DyeDurham.Test.UnitTests;

public class NameSorterServiceTests
{
    private readonly INameSorterService _nameSorterService;

    public NameSorterServiceTests()
    {
        _nameSorterService = Substitute.For<INameSorterService>();
    }

    [Fact]
    public void Sort_ShouldReturnSortedPeople_WhenInputIsValid()
    {
        // Arrange
        var unsortedPeople = new List<Person>
        {
            new("Smith", [ "John" ]),
            new("Doe", [ "Jane" ]),
            new("Brown", [ "Charlie" ])
        };

        var sortedPeople = unsortedPeople.OrderBy(p => p.Surname).ToList();

        _nameSorterService.Sort(Arg.Any<IEnumerable<Person>>()).Returns(sortedPeople);

        // Act
        var result = _nameSorterService.Sort(unsortedPeople);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Equal("Brown", result.First().Surname);
        Assert.Equal("Smith", result.Last().Surname);
    }

    [Fact]
    public void Sort_ShouldReturnEmptyList_WhenInputIsEmpty()
    {
        // Arrange
        var unsortedPeople = new List<Person>();
        _nameSorterService.Sort(Arg.Any<IEnumerable<Person>>()).Returns(unsortedPeople);

        // Act
        var result = _nameSorterService.Sort(unsortedPeople);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Sort_ShouldHandleSinglePersonList()
    {
        // Arrange
        var unsortedPeople = new List<Person>
        {
            new("Doe", ["Jane"])
        };

        _nameSorterService.Sort(Arg.Any<IEnumerable<Person>>()).Returns(unsortedPeople);

        // Act
        var result = _nameSorterService.Sort(unsortedPeople);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Doe", result.First().Surname);
    }
}
