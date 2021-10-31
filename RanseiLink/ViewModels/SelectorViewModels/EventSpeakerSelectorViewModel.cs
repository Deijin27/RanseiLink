using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;

namespace RanseiLink.ViewModels
{
    public class EventSpeakerSelectorViewModel : SelectorViewModelBase<EventSpeakerId, IEventSpeaker, EventSpeakerViewModel>
    {
        public EventSpeakerSelectorViewModel(IDialogService dialogService, EventSpeakerId initialSelected, IModelDataService<EventSpeakerId, IEventSpeaker> dataService) 
            : base(dialogService, initialSelected, dataService) { }
    }
}
