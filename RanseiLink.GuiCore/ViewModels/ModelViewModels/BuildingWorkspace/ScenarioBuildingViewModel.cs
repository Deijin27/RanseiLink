using RanseiLink.Core.Enums;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public class ScenarioBuildingViewModel(IScenarioBuildingService scenarioBuildingService) : ViewModelBase
{
    public void SetSelected(KingdomId kingdom, int slot)
    {
        Slots.Clear();
        foreach (var scenarioId in scenarioBuildingService.ValidIds())
        {
            var model = scenarioBuildingService.Retrieve(scenarioId);
            Slots.Add(new ScenarioBuildingSlotItem(model, (ScenarioId)scenarioId, kingdom, slot));
        }
    }

    public ObservableCollection<ScenarioBuildingSlotItem> Slots { get; } = [];

}
