using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.GuiCore.ViewModels;

public class ScenarioBuildingSlotItem : ViewModelBase
{
    private readonly ScenarioBuilding _model;
    private readonly ScenarioId _scenario;
    private readonly KingdomId _kingdomId;
    private readonly int _slot;
    public ScenarioBuildingSlotItem(ScenarioBuilding model, ScenarioId scenario, KingdomId kingdom, int slot)
    {
        _slot = slot;
        _kingdomId = kingdom;
        _model = model;
        _scenario = scenario;
    }

    public ScenarioId Scenario => _scenario;
    public KingdomId Kingdom => _kingdomId;

    public int Slot => _slot;

    public int InitialExp
    {
        get => _model.GetInitialExp(_kingdomId, _slot);
        set => SetProperty(InitialExp, value, v => _model.SetInitialExp(_kingdomId, _slot, v));
    }

    public int Unknown2
    {
        get => _model.GetUnknown2(_kingdomId, _slot);
        set => SetProperty(Unknown2, value, v => _model.SetUnknown2(_kingdomId, _slot, v));
    }
}
