// NameProcessingService.cs
using NameSorter_DyeDurham.Application.Interfaces;
using NameSorter_DyeDurham.Domain.Entities;
using NameSorter_DyeDurham.Domain.Interfaces;
using Serilog;

namespace NameSorter_DyeDurham.Application.Services;

/// <summary>
/// Service responsible for processing names: retrieving, parsing, sorting, and writing output.
/// </summary>
public class NameProcessingService(
    INameSourceService nameSourceService,
    INameParserService nameParserService,
    INameSorterService nameSorterService,
    IOutputWriterService outputWriterService,
    ILogger logger) : INameProcessingService
{
    private readonly INameSourceService _nameSourceService = nameSourceService;
    private readonly INameParserService _nameParserService = nameParserService;
    private readonly INameSorterService _nameSorterService = nameSorterService;
    private readonly IOutputWriterService _outputWriterService = outputWriterService;
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Processes names by retrieving, parsing, sorting, and writing them to the configured output.
    /// </summary>
    /// <param name="inputFilename">The input file containing raw names.</param>
    public async Task ProcessAsync(string inputFilename)
    {
        _logger.Information("Starting name processing for file: {InputFilename}", inputFilename);

        try
        {
            // Get the names from the source specified in the config file
            var rawNames = _nameSourceService.GetNames();
            var people = new List<Person>();

            _logger.Information("Retrieved {Count} raw names from the source.", rawNames.Count());

            foreach (var raw in rawNames)
            {
                try
                {
                    // Parse each name into a Person object
                    var person = _nameParserService.Parse(raw);
                    people.Add(person);
                }
                catch (Exception ex)
                {
                    _logger.Warning("Could not parse: '{RawName}'. Error: {ErrorMessage}", raw, ex.Message);
                }
            }

            // Sort the names using the specified sorting method
            _logger.Information("Sorting {Count} valid names.", people.Count);
            var sorted = _nameSorterService.Sort(people);

            // Write the sorted names to the output specified in the config file
            _logger.Information("Writing sorted names to the output.");
            await _outputWriterService.WriteAsync(sorted);

            _logger.Information("Name processing completed successfully for file: {InputFilename}", inputFilename);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unexpected error occurred during name processing for file: {InputFilename}", inputFilename);
            throw; // Re-throw the exception to ensure it propagates if needed
        }
    }
}
