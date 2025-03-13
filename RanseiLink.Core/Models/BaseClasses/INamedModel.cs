namespace RanseiLink.Core.Models;

public interface INamedModel
{
    string Name { get; }
    event EventHandler NameChanged;
}