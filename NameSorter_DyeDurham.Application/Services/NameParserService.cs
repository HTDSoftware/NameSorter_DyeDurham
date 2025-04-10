// NameParserService.cs
using NameSorter_DyeDurham.Application.Interfaces;
using NameSorter_DyeDurham.Domain.Entities;

namespace NameSorter_DyeDurham.Application.Services;

/// <summary>
/// Service responsible for parsing raw name strings into Person objects.
/// </summary>
public class NameParserService : INameParserService
{
    /// <summary>
    /// Parses a raw name string into a Person object.
    /// </summary>
    /// <param name="input">The raw name string to parse.</param>
    /// <returns>A Person object containing the parsed surname and given names.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the input is null, empty, or does not conform to the expected name format.
    /// </exception>
    public Person Parse(string input)
    {
        // Make sure the input is not null or empty
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Person Input cannot be null or empty.");

        // Must be at least 1 part (surname) and at most 4 parts (surname + 3 given names)
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || parts.Length > 4)
            throw new ArgumentException("Invalid name format.");

        // The last part is the surname, the rest are given names
        var surname = parts[^1];
        var givenNames = parts[..^1];

        return new Person(surname, givenNames);
    }
}
