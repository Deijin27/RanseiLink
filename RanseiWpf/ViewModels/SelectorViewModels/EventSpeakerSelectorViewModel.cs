using Core.Enums;
using Core.Models.Interfaces;
using Core.Services;
using RanseiWpf.Services;

namespace RanseiWpf.ViewModels
{
    public class EventSpeakerSelectorViewModel : SelectorViewModelBase<EventSpeakerId, IEventSpeaker, EventSpeakerViewModel>
    {
        public EventSpeakerSelectorViewModel(IDialogService dialogService, EventSpeakerId initialSelected, IModelDataService<EventSpeakerId, IEventSpeaker> dataService) 
            : base(dialogService, initialSelected, dataService) { }
    }
}
