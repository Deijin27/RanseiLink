namespace Core.Models.Interfaces
{
    public interface IKingdom : IDataWrapper, ICloneable<IKingdom>
    {
        string Name { get; set; }
    }
}