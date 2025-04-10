// INameParserService.cs
using NameSorter_DyeDurham.Domain.Entities;

namespace NameSorter_DyeDurham.Application.Interfaces;

public interface INameParserService
{
    Person Parse(string input);
}