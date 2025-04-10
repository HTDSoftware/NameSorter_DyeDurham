// INameSourceService.cs
namespace NameSorter_DyeDurham.Application.Interfaces;

public interface INameSourceService
{
    IEnumerable<string> GetNames();
}