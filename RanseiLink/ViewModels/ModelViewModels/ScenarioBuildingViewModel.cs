using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace RanseiLink.ViewModels;

public class ScenarioBuildingSlotItem : ViewModelBase
{
    private readonly ScenarioBuilding _model;
    private readonly KingdomId _kingdomId;
    private readonly int _slot;
    public ScenarioBuildingSlotItem(ScenarioBuilding model, KingdomId kingdom, int slot)
    {
        _slot = slot;
        _kingdomId = kingdom;
        _model = model;
    }

    public int Slot => _slot;

    public int InitialExp
    {
        get => _model.GetInitialExp(_kingdomId, _slot);
        set => RaiseAndSetIfChanged(InitialExp, value, v => _model.SetInitialExp(_kingdomId, _slot, v));
    }

    public int Unknown2
    {
        get => _model.GetUnknown2(_kingdomId, _slot);
        set => RaiseAndSetIfChanged(Unknown2, value, v => _model.SetUnknown2(_kingdomId, _slot, v));
    }
}

public class ScenarioBuildingKingdomItem : ViewModelBase
{
    public ScenarioBuildingKingdomItem(ScenarioBuilding model, KingdomId kingdom, string kingdomName)
    {
        Kingdom = kingdomName;
        for (int i = 0; i < ScenarioBuilding.SlotCount; i++)
        {
            Slots.Add(new ScenarioBuildingSlotItem(model, kingdom, i));
        }
    }
    public string Kingdom { get; }

    public ObservableCollection<ScenarioBuildingSlotItem> Slots { get; } = new();

}

public class ScenarioBuildingViewModel
{
    private readonly IIdToNameService _idToNameService;
    public ScenarioBuildingViewModel(IIdToNameService idToNameService)
    {
        _idToNameService = idToNameService;
    }

    public void SetModel(int scenario, ScenarioBuilding model)
    {
        KingdomItems.Clear();
        foreach (var item in EnumUtil.GetValuesExceptDefaults<KingdomId>().Select(i => new ScenarioBuildingKingdomItem(model, i, _idToNameService.IdToName<IKingdomService>((int)i))))
        {
            KingdomItems.Add(item);
        }
    }

    public ObservableCollection<ScenarioBuildingKingdomItem> KingdomItems { get; } = new();
}
