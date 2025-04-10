// Program.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NameSorter_DyeDurham.Application.Interfaces;
using NameSorter_DyeDurham.Application.Services;
using NameSorter_DyeDurham.Domain.Entities;
using NameSorter_DyeDurham.Domain.Interfaces;
using NameSorter_DyeDurham.Infrastructure.Services;
using Serilog;

class Program
{
    /// <summary>
    /// Entry point for the NameSorter application.
    /// </summary>
    static async Task Main()
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("Application starting...");

            // Get the command-line arguments
            var commandArgs = Environment.GetCommandLineArgs();

            // Ensure a file path is provided
            if (commandArgs.Length < 2)
            {
                Log.Error("No input file specified. Usage: ./NameSorter_DyeDurham.exe <input-file-name-with-path>");
                return;
            }

            var inputFilename = commandArgs[1];

            // Validate the file path
            if (!File.Exists(inputFilename))
            {
                Log.Error("File not found: {InputFilename}", inputFilename);
                return;
            }

            // Create a HostBuilder
            var host = Host.CreateDefaultBuilder()

                // Add Serilog to the Host
                .UseSerilog()

                // Configure the app settings
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })

                // Configure the services
                .ConfigureServices((context, services) =>
                {
                    // Bind app settings
                    services.Configure<AppSettings>(context.Configuration.GetSection("AppSettings"));
                    services.AddSingleton<IAppConfigurationService, AppConfigurationService>();

                    // Register services Name Processing Services
                    services.AddSingleton<INameParserService, NameParserService>();
                    services.AddSingleton<INameSorterService, NameSorterService>();
                    services.AddSingleton<INameProcessingService, NameProcessingService>();

                    // Pass the filename to FilenameSourceService
                    services.AddSingleton<INameSourceService>(provider =>
                    {
                        return new FilenameSourceService(inputFilename, Log.Logger);
                    });

                    // Register the Writer Services as concrete
                    services.AddSingleton<FileOutputWriterService>();
                    services.AddSingleton<ConsoleOutputWriterService>();

                    // Register however many output writers have been configured
                    services.AddSingleton<IOutputWriterService>(provider =>
                    {
                        var config = provider.GetRequiredService<IAppConfigurationService>();
                        var writers = new List<IOutputWriterService>();

                        if (config.UseFileOutput)
                        {
                            // Resolve the concrete type for file output
                            writers.Add(provider.GetRequiredService<FileOutputWriterService>());
                        }

                        if (config.UseConsoleOutput)
                        {
                            // Resolve the concrete type for console output
                            writers.Add(provider.GetRequiredService<ConsoleOutputWriterService>());
                        }

                        if (writers.Count == 0)
                        {
                            throw new InvalidOperationException("No output method is configured. Please enable either UseFileOutput or UseConsoleOutput in the configuration.");
                        }

                        // If both are set, use a composite writer to handle both outputs
                        return new CompositeOutputWriterService(writers, Log.Logger);
                    });

                    // Register Serilog as the logging provider
                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.AddSerilog();
                    });
                })
                .Build();

            Log.Information("Host built successfully. Starting application...");

            // Run the application
            var processor = host.Services.GetRequiredService<INameProcessingService>();
            await processor.ProcessAsync(inputFilename);

            Log.Information("Application completed successfully.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unexpected error occurred.");
        }
        finally
        {
            Log.Information("Application shutting down...");
            Log.CloseAndFlush();
        }
    }
}
