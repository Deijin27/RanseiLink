#nullable enable
using DryIoc;
using FactoryGenie;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Windows.ViewModels;

[System.Diagnostics.DebuggerDisplay("SwKingdomMiniViewModel: {_kingdom}")]
[GenerateFactory]
public class SwKingdomMiniViewModel : SwSimpleKingdomMiniViewModel
{
    private readonly int _scenario;
    private readonly ScenarioKingdom _scenarioKingdom;
    private readonly ScenarioWarriorWorkspaceViewModel _parent;
    private readonly IScenarioKingdomService _scenarioKingdomService;
    private readonly IStrengthService _strengthService;

    public SwKingdomMiniViewModel(
        IScenarioKingdomService scenarioKingdomService,
        ICachedSpriteProvider spriteProvider,
        IStrengthService strengthService,
        IIdToNameService idToNameService,
        [FactoryParam] ScenarioId scenario,
        [FactoryParam] KingdomId kingdom,
        [FactoryParam] ICommand itemClickedCommand,
        [FactoryParam] ScenarioWarriorWorkspaceViewModel parent
        ) : base(spriteProvider, idToNameService, kingdom)
    {
        _scenarioKingdomService = scenarioKingdomService;
        _strengthService = strengthService;

        SelectCommand = itemClickedCommand;
        _scenario = (int)scenario;

        _scenarioKingdom = _scenarioKingdomService.Retrieve((int)scenario);

        _parent = parent;

        UpdateArmyInfo();
    }

    public ICommand? SelectCommand { get; private set; }

    public int Strength => _strengthService.CalculateScenarioKingdomStrength((ScenarioId)_scenario, Kingdom, Army);

    public void UpdateStrength()
    {
        RaisePropertyChanged(nameof(Strength));
    }

    public override int Army
    {
        get => Kingdom == KingdomId.Default ? 17 : _scenarioKingdom.GetArmy(Kingdom);
        set 
        {
            if (Kingdom != KingdomId.Default)
            {
                var oldArmy = Army;
                if (SetProperty(Army, value, v => _scenarioKingdom.SetArmy(Kingdom, v)))
                {
                    UpdateArmyInfo();
                }
            }
        }
    }

    private void UpdateArmyInfo()
    {
        ArmyInfo = _parent.GetArmy(Army);
    }

    public ScenarioArmyViewModel? ArmyInfo
    {
        get;
        private set => SetProperty(ref field, value);
    }
}
