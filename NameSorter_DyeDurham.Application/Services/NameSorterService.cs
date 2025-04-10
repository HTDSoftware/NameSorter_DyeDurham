// NameSorterService.cs
using NameSorter_DyeDurham.Domain.Entities;
using NameSorter_DyeDurham.Domain.Interfaces;

namespace NameSorter_DyeDurham.Application.Services;

/// <summary>
/// Service responsible for sorting a collection of Person objects.
/// </summary>
public class NameSorterService : INameSorterService
{
    /// <summary>
    /// Sorts a collection of Person objects by surname and then by given names.
    /// </summary>
    /// <param name="people">The collection of Person objects to sort.</param>
    /// <returns>
    /// A sorted collection of Person objects. If the input collection is null or empty, an empty collection is returned.
    /// </returns>
    public IEnumerable<Person> Sort(IEnumerable<Person> people)
    {
        if (people == null || !people.Any())
        {
            return [];
        }

        // Sort by surname first, then by given names
        return people.OrderBy(p => p.Surname).ThenBy(p => string.Join(" ", p.GivenNames));
    }
}
