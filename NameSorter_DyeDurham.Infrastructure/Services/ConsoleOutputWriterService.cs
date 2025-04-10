// ConsoleOutputWriterService.cs
using NameSorter_DyeDurham.Application.Interfaces;
using NameSorter_DyeDurham.Domain.Entities;
using Serilog;

namespace NameSorter_DyeDurham.Infrastructure.Services;

/// <summary>
/// Service that writes output to the console.
/// </summary>
public class ConsoleOutputWriterService(ILogger logger) : IOutputWriterService
{
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Writes the sorted names to the console.
    /// </summary>
    /// <param name="people">The list of people to write.</param>
    public Task WriteAsync(IEnumerable<Person> people)
    {
        _logger.Information("Starting to write output to the console.");

        try
        {
            // Check if the list is empty
            if (people == null || !people.Any())
            {
                _logger.Warning("No names to display.");
                Console.WriteLine("No names to display.");
                return Task.CompletedTask;
            }

            // Write each person's name to the console
            foreach (var person in people)
            {
                Console.WriteLine(person.ToString());
            }

            _logger.Information("Successfully wrote output to the console.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while writing output to the console.");
            throw; // Re-throw the exception to ensure it propagates if needed
        }

        return Task.CompletedTask;
    }
}
