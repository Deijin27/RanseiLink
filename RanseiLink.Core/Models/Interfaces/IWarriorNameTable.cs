namespace RanseiLink.Core.Models.Interfaces
{
    public interface IWarriorNameTable : IDataWrapper, ICloneable<IWarriorNameTable>
    {
        string GetEntry(uint id);
        void SetEntry(uint id, string value);
    }
}