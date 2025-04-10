// INameSorterService.cs
using NameSorter_DyeDurham.Domain.Entities;

namespace NameSorter_DyeDurham.Domain.Interfaces;

public interface INameSorterService
{
    IEnumerable<Person> Sort(IEnumerable<Person> people);
}