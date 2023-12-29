using RanseiLink.Core.Services;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public interface IModSelectionViewModel
{
    event Action<ModInfo> ModSelected;
    ObservableCollection<IModListItemViewModel> ModItems { get; }
}

