using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.ViewModels;

public delegate EventSpeakerSelectorViewModel EventSpeakerSelectorViewModelFactory(IEventSpeakerService service);

public class EventSpeakerSelectorViewModel : SelectorViewModelBase<EventSpeakerId, IEventSpeaker, EventSpeakerViewModel>
{
    private readonly EventSpeakerViewModelFactory _factory;

    public EventSpeakerSelectorViewModel(IServiceContainer container, IEventSpeakerService dataService)
        : base(container, dataService) 
    { 
        _factory = container.Resolve<EventSpeakerViewModelFactory>();
        Selected = EventSpeakerId.Shopkeeper_0;
    }

    protected override EventSpeakerViewModel NewViewModel(IEventSpeaker model) => _factory(model);
}
