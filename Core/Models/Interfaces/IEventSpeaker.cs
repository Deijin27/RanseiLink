namespace Core.Models.Interfaces
{
    public interface IEventSpeaker : IDataWrapper, ICloneable<IEventSpeaker>
    {
        string Name { get; set; }
    }
}