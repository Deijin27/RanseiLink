#nullable enable
using DryIoc;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Windows.ViewModels;

[System.Diagnostics.DebuggerDisplay("SwKingdomMiniViewModel: {_kingdom}")]
public class SwKingdomMiniViewModel : SwSimpleKingdomMiniViewModel
{
    public new delegate SwKingdomMiniViewModel Factory();

    private int _scenario;
    private ScenarioKingdom _scenarioKingdom = new();
    private ScenarioWarriorWorkspaceViewModel _parent = null!;

    private readonly IScenarioKingdomService _scenarioKingdomService;
    private readonly IScenarioWarriorService _scenarioWarriorService;
    private readonly IBaseWarriorService _baseWarriorService;
    private readonly IStrengthService _strengthService;


    public SwKingdomMiniViewModel(
        IScenarioKingdomService scenarioKingdomService,
        IBaseWarriorService baseWarriorService,
        IScenarioWarriorService scenarioWarriorService,
        ICachedSpriteProvider spriteProvider,
        IStrengthService strengthService,
        IIdToNameService idToNameService) : base(spriteProvider, idToNameService)
    {
        _scenarioKingdomService = scenarioKingdomService;
        _baseWarriorService = baseWarriorService;
        _scenarioWarriorService = scenarioWarriorService;
        _strengthService = strengthService;
    }

    public ICommand? SelectCommand { get; private set; }

    public SwKingdomMiniViewModel Init(ScenarioId scenario, KingdomId kingdom, ICommand itemClickedCommand, ScenarioWarriorWorkspaceViewModel parent)
    {
        base.Init(kingdom);
        SelectCommand = itemClickedCommand;
        _scenario = (int)scenario;

        _scenarioKingdom = _scenarioKingdomService.Retrieve((int)scenario);

        _parent = parent;

        UpdateArmyInfo();

        return this;
    }

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
