using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RanseiLink.ViewModels;

public interface IScenarioKingdomViewModel
{
    void SetModel(ScenarioKingdom model);
}

public class ScenarioKingdomItem : ViewModelBase
{
    private readonly ScenarioKingdom _model;
    private readonly KingdomId _kingdomId;
    public ScenarioKingdomItem(ScenarioKingdom model, KingdomId kingdom, string kingdomName)
    {
        Kingdom = kingdomName;
        _kingdomId = kingdom;
        _model = model;
    }
    public string Kingdom { get; }

    public uint Army
    {
        get => _model.GetArmy(_kingdomId);
        set => RaiseAndSetIfChanged(Army, value, v => _model.SetArmy(_kingdomId, v));
    }
}
public class ScenarioKingdomViewModel : ViewModelBase, IScenarioKingdomViewModel
{
    private readonly IIdToNameService _idToNameService;
    public ScenarioKingdomViewModel(IIdToNameService idToNameService)
    {
        _idToNameService = idToNameService;   
    }

    public void SetModel(ScenarioKingdom model)
    {
        KingdomItems.Clear();
        foreach (var item in EnumUtil.GetValuesExceptDefaults<KingdomId>().Select(i => new ScenarioKingdomItem(model, i, _idToNameService.IdToName<IKingdomService>((int)i))))
        {
            KingdomItems.Add(item);
        }
    }

    public ObservableCollection<ScenarioKingdomItem> KingdomItems { get; } = new();
}
