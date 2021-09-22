using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public class EventSpeakerService : BaseModelService, IModelDataService<EventSpeakerId, IEventSpeaker>
    {
        public EventSpeakerService(ModInfo mod) : base(mod, Constants.EventSpeakerRomPath, EventSpeaker.DataLength) { }

        public IEventSpeaker Retrieve(EventSpeakerId id)
        {
            return new EventSpeaker(RetrieveData((int)id));
        }

        public void Save(EventSpeakerId id, IEventSpeaker model)
        {
            SaveData((int)id, model.Data);
        }
    }
}
