// IOutputWriterService.cs
using NameSorter_DyeDurham.Domain.Entities;

namespace NameSorter_DyeDurham.Application.Interfaces;

public interface IOutputWriterService
{
    Task WriteAsync(IEnumerable<Person> people);
}