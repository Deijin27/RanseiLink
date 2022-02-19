using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate EventSpeakerSelectorViewModel EventSpeakerSelectorViewModelFactory(IEditorContext context);

public class EventSpeakerSelectorViewModel : SelectorViewModelBase<EventSpeakerId, IEventSpeaker, EventSpeakerViewModel>
{
    private readonly EventSpeakerViewModelFactory _factory;
    private readonly IEditorContext _editorContext;

    public EventSpeakerSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.EventSpeaker) 
    { 
        _factory = container.Resolve<EventSpeakerViewModelFactory>();
        _editorContext = context;
        Selected = EventSpeakerId.Shopkeeper_0;
    }

    protected override EventSpeakerViewModel NewViewModel(IEventSpeaker model) => _factory(model, _editorContext);
}
