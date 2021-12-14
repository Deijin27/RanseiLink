using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.ViewModels;

public delegate ScenarioKingdomViewModel ScenarioKingdomViewModelFactory(IScenarioKingdom model);

public class ScenarioKingdomItem : ViewModelBase
{
    private readonly IScenarioKingdom _model;
    public ScenarioKingdomItem(IScenarioKingdom model, KingdomId kingdom)
    {
        Kingdom = kingdom;
        _model = model;
    }
    public KingdomId Kingdom { get; }

    public uint Army
    {
        get => _model.GetArmy(Kingdom);
        set => RaiseAndSetIfChanged(Army, value, v => _model.SetArmy(Kingdom, v));
    }
}
public class ScenarioKingdomViewModel : ViewModelBase
{
    public ScenarioKingdomViewModel(IScenarioKingdom model)
    {
        KingdomItems = EnumUtil.GetValuesExceptDefaults<KingdomId>().Select(i => new ScenarioKingdomItem(model, i)).ToList();
    }

    public List<ScenarioKingdomItem> KingdomItems { get; }
}
