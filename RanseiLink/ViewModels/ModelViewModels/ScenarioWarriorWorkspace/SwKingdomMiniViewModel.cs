using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Windows.Input;
using System.Windows.Media;

namespace RanseiLink.ViewModels;

public class SwKingdomMiniViewModel : SwSimpleKingdomMiniViewModel
{
    public new delegate SwKingdomMiniViewModel Factory();

    private int _scenario;
    private KingdomId _kingdom;
    private ScenarioKingdom _scenarioKingdom;

    private readonly IScenarioKingdomService _scenarioKingdomService;
    private readonly IScenarioWarriorService _scenarioWarriorService;
    private readonly IScenarioPokemonService _scenarioPokemonService;
    private readonly IBaseWarriorService _baseWarriorService;
    private readonly IPokemonService _pokemonService;
    
    public SwKingdomMiniViewModel(
        IScenarioKingdomService scenarioKingdomService,
        IBaseWarriorService baseWarriorService,
        IScenarioWarriorService scenarioWarriorService,
        IScenarioPokemonService scenarioPokemonService,
        ICachedSpriteProvider spriteProvider, 
        IPokemonService pokemonService) : base(spriteProvider)
    {
        _scenarioKingdomService = scenarioKingdomService;
        _baseWarriorService = baseWarriorService;
        _scenarioWarriorService = scenarioWarriorService;
        _scenarioPokemonService = scenarioPokemonService;
        _pokemonService = pokemonService;
    }

    public ICommand SelectCommand { get; private set; }

    public SwKingdomMiniViewModel Init(ScenarioId scenario, KingdomId kingdom, ICommand itemClickedCommand)
    {
        base.Init(kingdom);
        SelectCommand = itemClickedCommand;
        _kingdom = kingdom;
        _scenario = (int)scenario;

        _scenarioKingdom = _scenarioKingdomService.Retrieve((int)scenario);

        UpdateWarriorImage();

        return this;
    }

    public ImageSource WarriorImage => _spriteProvider.GetSprite(SpriteType.StlBushouS, _warriorImageId);

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

    public int Strength
    {
        get
        {
            int strength = 0;
            foreach (var warrior in _scenarioWarriorService.Retrieve(_scenario).Enumerate())
            {
                if (warrior.Kingdom == _kingdom 
                    && warrior.Army == Army 
                    && (warrior.Class == WarriorClassId.ArmyLeader || warrior.Class == WarriorClassId.ArmyMember)
                    )
                {
                    if (warrior.ScenarioPokemonIsDefault(0))
                    {
                        continue;
                    }
                    strength += CalculateStrength(warrior.GetScenarioPokemon(0));
                }
            }
            return strength;
        }
    }

    public void UpdateStrength()
    {
        RaisePropertyChanged(nameof(Strength));
    }

    public void UpdateLeader()
    {
        UpdateWarriorImage();
    }

    private int CalculateStrength(int ScenarioPokemon)
    {
        var sp = _scenarioPokemonService.Retrieve(_scenario).Retrieve(ScenarioPokemon);
        int pokemonId = (int)sp.Pokemon;
        if (!_pokemonService.ValidateId(pokemonId))
        {
            return 0;
        }
        var pokemon = _pokemonService.Retrieve(pokemonId);
        return StrengthCalculator.CalculateStrength(pokemon, (double)LinkCalculator.CalculateLink(sp.Exp));
    }

    public override int Army
    {
        get => _kingdom == KingdomId.Default ? -1 : _scenarioKingdom.GetArmy(_kingdom);
        set 
        {
            if (_kingdom != KingdomId.Default)
            {
                RaiseAndSetIfChanged(Army, value, v => _scenarioKingdom.SetArmy(_kingdom, v));
            }
        }
    }
}
