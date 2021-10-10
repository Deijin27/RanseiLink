namespace Core.Models.Interfaces
{
    public interface IBaseWarrior : IDataWrapper, ICloneable<IBaseWarrior>
    {
        uint WarriorName { get; set; }
    }
}