**NameSorter Application Documentation**

Overview: The NameSorter application is designed to sort names from an input file and output the results to a file, the console, or both. It is built using a modular architecture with dependency injection and logging for flexibility and maintainability.

**Key Features:**
1.	Input File Processing:
  •	Reads names from a specified input file.
  •	Validates the input file's existence.
2.	Name Sorting:
  •	Parses names into structured objects.
  •	Sorts names alphabetically by surname and given names.
3.	Output Options:
  •	Writes sorted names to a file.
  •	Displays sorted names in the console.
  •	Supports both file and console outputs simultaneously.
4.	Configuration:
  •	Configurable via appsettings.json or dependency injection.
  •	Supports enabling or disabling file and console outputs.
5.	Logging:
  •	Uses Serilog for logging application events and errors.

**Architecture:**
**Services: The application is built around several services, each responsible for a specific task.**
   
  **AppConfigurationService:**
    •	Purpose: Manages application settings.
    •	Key Responsibilities:
    •	Reads settings like UseConsoleOutput, UseFileOutput, and OutputFilename from configuration.
    •	Validates that OutputFilename is set if file output is enabled.
    •	Key Properties:
    •	UseConsoleOutput: Indicates if console output is enabled.
    •	UseFileOutput: Indicates if file output is enabled.
    •	OutputFilename: Specifies the file name for output.

  **FilenameSourceService:**
    •	Purpose: Reads names from the input file.
    •	Key Responsibilities:
    •	Validates the input file's existence.
    •	Reads and returns the list of names from the file.

  **NameParserService:**
    •	Purpose: Parses raw name strings into structured Person objects.
    •	Example: Converts "John Doe" into Person(Surname: "Doe", GivenNames: ["John"]).

  **NameSorterService:**
    •	Purpose: Sorts a list of Person objects alphabetically by surname and given names.
  
  **Output Writer Services:**
    •	Purpose: Handles writing sorted names to the configured outputs.
    •	Implementations:
    •	FileOutputWriterService: Writes sorted names to a file.
    •	ConsoleOutputWriterService: Displays sorted names in the console.
    •	CompositeOutputWriterService: Combines multiple output writers to handle both file and console outputs.

**Configuration: **
  ** appsettings.json: The application settings are defined in the appsettings.json file. Below is an example configuration:**
  { "AppSettings": { "UseConsoleOutput": true, "UseFileOutput": true, "OutputFilename": "output.txt" } }
  UseConsoleOutput: Enables or disables console output. 
  UseFileOutput: Enables or disables file output. 
  OutputFilename: Specifies the file name for output.

**Usage: Command-Line: Run the application from the command line with the following syntax:**
**  NameSorter_DyeDurham.exe <input-file-name>**
  Example: Input File (input.txt): John Doe Jane Smith Charlie Brown
  Command: NameSorter_DyeDurham.exe input.txt
  Output: Console: Charlie Brown John Doe Jane Smith
  File (output.txt): Charlie Brown John Doe Jane Smith
  Error Handling: Missing Input File: Error: File not found: <filename> Resolution: Ensure the input file exists at the specified path.
  No Output Method Configured: Error: No output method is configured. Resolution: Enable either UseConsoleOutput or UseFileOutput in the configuration.
  Invalid Configuration: Error: UseFileOutput is true, but OutputFileName is not set. Resolution: Set a valid OutputFilename in the configuration.
  Logging: Library: Serilog Purpose: Logs application events, errors, and debugging information. Configuration: Logs are written to the console and a rolling file (logs/log-.txt). Usage: Injected into services for logging important events.
  
  **Testing: The application includes both unit and integration tests to ensure reliability.**
  **Unit Tests: Validate individual services like FileOutputWriterService and NameProcessingService.**
  **Integration Tests: Test the end-to-end functionality of the application, including file and console outputs.**
