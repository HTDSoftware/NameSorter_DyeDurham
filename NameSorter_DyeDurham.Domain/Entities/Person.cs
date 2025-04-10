// Person.cs
namespace NameSorter_DyeDurham.Domain.Entities;

public class Person(string surname, IEnumerable<string> givenNames)
{
    public string Surname { get; } = surname;
    public List<string> GivenNames { get; } = [.. givenNames];

    public override string ToString() => $"{string.Join(" ", GivenNames)} {Surname}";
}
