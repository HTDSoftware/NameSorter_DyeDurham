// FileOutputFileWriterService.cs
using NameSorter_DyeDurham.Application.Interfaces;
using NameSorter_DyeDurham.Domain.Entities;
using Serilog;

namespace NameSorter_DyeDurham.Infrastructure.Services;

/// <summary>
/// Service that writes output to a file.
/// </summary>
public class FileOutputWriterService(IAppConfigurationService config, ILogger logger) : IOutputWriterService
{
    private readonly IAppConfigurationService _config = config;
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Writes the sorted names to the configured output file.
    /// </summary>
    /// <param name="people">The list of people to write.</param>
    public async Task WriteAsync(IEnumerable<Person> people)
    {
        _logger.Information("Starting to write output to file: {OutputFilename}", _config.OutputFilename);

        try
        {
            // Check if the list is empty
            if (people == null || !people.Any())
            {
                _logger.Warning("No names to display.");
                return;
            }

            // Ensure the output directory exists
            var outputDirectory = Path.GetDirectoryName(_config.OutputFilename);
            if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
            {
                _logger.Information("Creating output directory: {OutputDirectory}", outputDirectory);
                Directory.CreateDirectory(outputDirectory);
            }

            // Check if the file already exists
            if (File.Exists(_config.OutputFilename))
            {
                _logger.Warning("File '{OutputFilename}' already exists. Overwriting...", _config.OutputFilename);
                Console.WriteLine($"File '{_config.OutputFilename}' already exists. Overwriting...");
            }

            // Write each person's name to the specified file
            var lines = people.Select(p => p.ToString());

            // Write the lines to the file asynchronously
            await File.WriteAllLinesAsync(_config.OutputFilename, lines);

            _logger.Information("Successfully wrote output to file: {OutputFilename}", _config.OutputFilename);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while writing to file: {OutputFilename}", _config.OutputFilename);
            throw; // Re-throw the exception to ensure it propagates if needed
        }
    }
}
