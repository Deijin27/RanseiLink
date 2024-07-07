#nullable enable
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

    private readonly IScenarioKingdomService _scenarioKingdomService;
    private readonly IScenarioWarriorService _scenarioWarriorService;
    private readonly IBaseWarriorService _baseWarriorService;
    private readonly IStrengthService _strengthService;


    public SwKingdomMiniViewModel(
        IScenarioKingdomService scenarioKingdomService,
        IBaseWarriorService baseWarriorService,
        IScenarioWarriorService scenarioWarriorService,
        ICachedSpriteProvider spriteProvider,
        IStrengthService strengthService) : base(spriteProvider)
    {
        _scenarioKingdomService = scenarioKingdomService;
        _baseWarriorService = baseWarriorService;
        _scenarioWarriorService = scenarioWarriorService;
        _strengthService = strengthService;
    }

    public ICommand? SelectCommand { get; private set; }

    public SwKingdomMiniViewModel Init(ScenarioId scenario, KingdomId kingdom, ICommand itemClickedCommand)
    {
        base.Init(kingdom);
        SelectCommand = itemClickedCommand;
        _scenario = (int)scenario;

        _scenarioKingdom = _scenarioKingdomService.Retrieve((int)scenario);

        UpdateWarriorImage();

        return this;
    }

    public object? WarriorImage => _spriteProvider.GetSprite(SpriteType.StlBushouS, _warriorImageId);

    private int _warriorImageId;

    private void UpdateWarriorImage()
    {
        int warrior = -1;
        foreach (var w in _scenarioWarriorService.Retrieve(_scenario).Enumerate())
        {
            if (w.Class == WarriorClassId.ArmyLeader && w.Army == Army)
            {
                warrior = (int)w.Warrior;
                break;
            }
        }

        if (!_baseWarriorService.ValidateId(warrior))
        {
            _warriorImageId = -1;
        }
        else
        {
            _warriorImageId = _baseWarriorService.Retrieve(warrior).Sprite;
        }
        RaisePropertyChanged(nameof(WarriorImage));
    }

    public int Strength => _strengthService.CalculateScenarioKingdomStrength((ScenarioId)_scenario, Kingdom, Army);

    public void UpdateStrength()
    {
        RaisePropertyChanged(nameof(Strength));
    }

    public void UpdateLeader()
    {
        UpdateWarriorImage();
    }

    public override int Army
    {
        get => Kingdom == KingdomId.Default ? -1 : _scenarioKingdom.GetArmy(Kingdom);
        set 
        {
            if (Kingdom != KingdomId.Default)
            {
                SetProperty(Army, value, v => _scenarioKingdom.SetArmy(Kingdom, v));
            }
        }
    }
}
