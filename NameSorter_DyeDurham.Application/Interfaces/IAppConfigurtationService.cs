// IAppConfiguration.cs
namespace NameSorter_DyeDurham.Application.Interfaces;

public interface IAppConfigurationService
{
    bool UseConsoleOutput { get; }
    bool UseFileOutput { get; }
    string OutputFilename { get; }
}
