using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Services.ModelServices;

public interface IEventSpeakerService : IModelService<EventSpeaker>
{
}

public class EventSpeakerService : BaseDataModelService<EventSpeaker>, IEventSpeakerService
{
    private EventSpeakerService(string EventSpeakerDatFile, ConquestGameCode culture) 
        : base(EventSpeakerDatFile, 0, 59, () => new EventSpeaker(culture)) 
    {
    }

    public EventSpeakerService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.EventSpeakerRomPath), mod.GameCode) { }

    public EventSpeaker Retrieve(EventSpeakerId id) => Retrieve((int)id);

    public override string IdToName(int id)
    {
        return Retrieve(id).Name;
    }
} 