// CompositeOutputWriterService.cs
using NameSorter_DyeDurham.Application.Interfaces;
using NameSorter_DyeDurham.Domain.Entities;
using Serilog;

namespace NameSorter_DyeDurham.Infrastructure.Services;

/// <summary>
/// Service that writes output using multiple configured output writers.
/// </summary>
public class CompositeOutputWriterService(IEnumerable<IOutputWriterService> outputWriters, ILogger logger) : IOutputWriterService
{
    private readonly IEnumerable<IOutputWriterService> _outputWriters = outputWriters;
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Writes the sorted names to all configured output writers.
    /// </summary>
    /// <param name="people">The list of people to write.</param>
    public async Task WriteAsync(IEnumerable<Person> people)
    {
        _logger.Information("Starting to write output using {Count} output writers.", _outputWriters.Count());

        // Check if the people list is null or empty
        if (people == null || !people.Any())
        {
            _logger.Warning("No people to write. Throwing an exception.");
            throw new ArgumentException("No people to write.");
        }

        // Check if the output writers are null or empty
        if (_outputWriters == null || !_outputWriters.Any())
        {
            _logger.Error("No output writers configured. Throwing an exception.");
            throw new InvalidOperationException("No output writers configured.");
        }

        // Iterate through each output writer and write the sorted names
        foreach (var writer in _outputWriters)
        {
            try
            {
                _logger.Information("Writing output using writer: {WriterType}", writer.GetType().Name);
                await writer.WriteAsync(people);
                _logger.Information("Successfully wrote output using writer: {WriterType}", writer.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while writing output using writer: {WriterType}", writer.GetType().Name);
                // Optionally, rethrow the exception if you want to fail the entire process
                throw;
            }
        }

        _logger.Information("Completed writing output using all configured writers.");
    }
}
