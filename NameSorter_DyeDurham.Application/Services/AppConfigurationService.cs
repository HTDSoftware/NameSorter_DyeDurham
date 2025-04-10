// AppConfigurationService.cs
using Microsoft.Extensions.Options;
using NameSorter_DyeDurham.Application.Interfaces;
using NameSorter_DyeDurham.Domain.Entities;

namespace NameSorter_DyeDurham.Application.Services;

/// <summary>
/// Service responsible for managing application configuration settings.
/// </summary>
public class AppConfigurationService : IAppConfigurationService
{
    private readonly AppSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppConfigurationService"/> class.
    /// </summary>
    /// <param name="options">The application settings provided via dependency injection.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <see cref="AppSettings.UseFileOutput"/> is true but <see cref="AppSettings.OutputFilename"/> is not set.
    /// </exception>
    public AppConfigurationService(IOptions<AppSettings> options)
    {
        _settings = options.Value;

        // Validate configuration for file output
        if (_settings.UseFileOutput && string.IsNullOrWhiteSpace(_settings.OutputFilename))
            throw new InvalidOperationException("UseFileOutput is true, but OutputFileName is not set. File output mode requires a file name.");
    }

    public bool UseConsoleOutput => _settings.UseConsoleOutput;

    public bool UseFileOutput => _settings.UseFileOutput;

    public string OutputFilename => _settings.OutputFilename;
}
