namespace Core.Models.Interfaces
{
    public interface IItem : IDataWrapper, ICloneable<IItem>
    {
        string Name { get; set; }
    }
}