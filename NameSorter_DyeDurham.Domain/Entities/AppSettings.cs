// AppSettings.cs
namespace NameSorter_DyeDurham.Domain.Entities;

public class AppSettings
{
    public bool UseConsoleOutput { get; set; }
    public bool UseFileOutput { get; set; }
    public string OutputFilename { get; set; } = string.Empty;
}
