using Core.Enums;
using Core.Models.Interfaces;
using Core.Models;

namespace Core.Services.ModelServices
{
    public interface IEventSpeakerService : IModelDataService<EventSpeakerId, IEventSpeaker>
    {
        IDisposableEventSpeakerService Disposable();
    }

    public interface IDisposableEventSpeakerService : IDisposableModelDataService<EventSpeakerId, IEventSpeaker>
    {
    }


    public class EventSpeakerService : BaseModelService, IEventSpeakerService
    {
        public EventSpeakerService(ModInfo mod) : base(mod, Constants.EventSpeakerRomPath, EventSpeaker.DataLength) { }

        public IDisposableEventSpeakerService Disposable()
        {
            return new DisposableEventSpeakerService(Mod);
        }

        public IEventSpeaker Retrieve(EventSpeakerId id)
        {
            return new EventSpeaker(RetrieveData((int)id));
        }

        public void Save(EventSpeakerId id, IEventSpeaker model)
        {
            SaveData((int)id, model.Data);
        }
    }

    public class DisposableEventSpeakerService : BaseDisposableModelService, IDisposableEventSpeakerService
    {
        public DisposableEventSpeakerService(ModInfo mod) : base(mod, Constants.EventSpeakerRomPath, EventSpeaker.DataLength) { }

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
