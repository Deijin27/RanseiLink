using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class ScenarioKingdomItem : ViewModelBase
{
    private readonly ScenarioKingdom _model;
    private readonly KingdomId _kingdomId;
    public ScenarioKingdomItem(ScenarioKingdom model, KingdomId kingdom, string kingdomName)
    {
        Kingdom = kingdomName;
        _kingdomId = kingdom;
        _model = model;
        ShowSummaryCommand = new RelayCommand(() => ShowArmySummary?.Invoke(Army));
    }
    public string Kingdom { get; }

    public int Army
    {
        get => _model.GetArmy(_kingdomId);
        set => RaiseAndSetIfChanged(Army, value, v => _model.SetArmy(_kingdomId, v));
    }

    public ICommand ShowSummaryCommand { get; }

    public event Action<int> ShowArmySummary;
    
}
public class ScenarioKingdomViewModel : ViewModelBase
{
    private readonly IIdToNameService _idToNameService;
    private readonly IScenarioWarriorService _scenarioWarriorService;
    private readonly IScenarioPokemonService _scenarioPokemonService;
    private readonly IDialogService _dialogService;
    private readonly IKingdomService _kingdomService;

    private int _scenario;
    private ScenarioKingdom _model;

    public ScenarioKingdomViewModel(
        IIdToNameService idToNameService, 
        IScenarioWarriorService scenarioWarriorService,
        IScenarioPokemonService scenarioPokemonService,
        IDialogService dialogService,
        IKingdomService kingdomService)
    {
        _scenarioWarriorService = scenarioWarriorService;
        _idToNameService = idToNameService;
        _scenarioPokemonService = scenarioPokemonService;
        _dialogService = dialogService;
        _kingdomService = kingdomService;
    }

    public void SetModel(int scenario, ScenarioKingdom model)
    {
        _scenario = scenario;
        _model = model;
        KingdomItems.Clear();
        foreach (var item in EnumUtil.GetValuesExceptDefaults<KingdomId>().Select(i => new ScenarioKingdomItem(model, i, _idToNameService.IdToName<IKingdomService>((int)i))))
        {
            item.ShowArmySummary += ShowArmySummary;
            KingdomItems.Add(item);
        }
    }

    public ObservableCollection<ScenarioKingdomItem> KingdomItems { get; } = new();

    private void ShowArmySummary(int army)
    {
        var scenarioWarriors = _scenarioWarriorService.Retrieve(_scenario);
        var scenarioPokemon = _scenarioPokemonService.Retrieve(_scenario);

        var sb = new StringBuilder();
        foreach (var kingdom in _kingdomService.ValidIds())
        {
            if (_model.GetArmy((KingdomId)kingdom) == army)
            {
                sb.AppendLine(_kingdomService.IdToName(kingdom));
                foreach (var warriorId in scenarioWarriors.ValidIds())
                {
                    var warrior = scenarioWarriors.Retrieve(warriorId);
                    if (warrior.Army == army && warrior.Kingdom == (KingdomId)kingdom && (warrior.Class == WarriorClassId.ArmyLeader || warrior.Class == WarriorClassId.ArmyMember))
                    {
                        var scenarioPokemonId = warrior.GetScenarioPokemon(0);
                        var pokemonName = scenarioPokemon.ValidateId(scenarioPokemonId) ? scenarioPokemon.IdToName(scenarioPokemonId) : "Default";
                        var line = $"- {scenarioWarriors.IdToName(warriorId)}: {pokemonName}";
                        if (warrior.Class == WarriorClassId.ArmyLeader)
                        {
                            line += " (LEADER)";
                        }
                        sb.AppendLine(line);
                    }
                }
                sb.AppendLine();
            }
        }
        string text = sb.ToString();
        System.Windows.Clipboard.SetText(text);

        _dialogService.ShowMessageBox(MessageBoxSettings.Ok($"Army {army} Summary", $"(Copied to clipboard)\n\n{text}"));
    }

}
