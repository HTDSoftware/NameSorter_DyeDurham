using NameSorter_DyeDurham.Application.Interfaces;
using Serilog;

namespace NameSorter_DyeDurham.Infrastructure.Services
{
    /// <summary>
    /// Service that retrieves names from a specified file.
    /// </summary>
    public class FilenameSourceService(string filename, ILogger logger) : INameSourceService
    {
        private readonly string _filename = filename;
        private readonly ILogger _logger = logger;

        /// <summary>
        /// Retrieves names from the specified file.
        /// </summary>
        /// <returns>An enumerable of names read from the file.</returns>
        public IEnumerable<string> GetNames()
        {
            _logger.Information("Attempting to read names from file: {Filename}", _filename);

            try
            {
                if (!File.Exists(_filename))
                {
                    _logger.Error("File not found: {Filename}", _filename);
                    throw new FileNotFoundException($"Input file not found at: {_filename}");
                }

                _logger.Information("File found: {Filename}. Reading names...");
                return File.ReadLines(_filename); // Lazy reading
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while reading the file: {Filename}", _filename);
                throw; // Re-throw the exception to propagate it if needed
            }
        }
    }
}
