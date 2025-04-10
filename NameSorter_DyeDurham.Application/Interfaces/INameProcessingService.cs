// INameProcessingService.cs
namespace NameSorter_DyeDurham.Application.Interfaces;

public interface INameProcessingService
{
    Task ProcessAsync(string inputFilename);
}
