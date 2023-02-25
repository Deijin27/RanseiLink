using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.ValueConverters;
using System.Windows.Media;

namespace RanseiLink.ViewModels;


public delegate SwKingdomMiniViewModel SwKingdomMiniViewModelFactory(ScenarioId scenario, KingdomId kingdom);

public class SwKingdomMiniViewModel : SwSimpleKingdomMiniViewModel
{
    private readonly KingdomId _kingdom;
    private readonly ScenarioKingdom _scenarioKingdom;
    
    private readonly IBaseWarriorService _baseWarriorService;
    private readonly IChildScenarioWarriorService _scenarioWarriorService;
    private readonly IPokemonService _pokemonService;
    private readonly IChildScenarioPokemonService _scenarioPokemonService;
    public SwKingdomMiniViewModel(ScenarioId scenario, KingdomId kingdom,
        IScenarioKingdomService scenarioKingdomService,
        IBaseWarriorService baseWarriorService,
        IScenarioWarriorService scenarioWarriorService,
        IScenarioPokemonService scenarioPokemonService,
        IOverrideDataProvider dataProvider, 
        IPokemonService pokemonService) : base(kingdom, dataProvider)
    {
        _kingdom = kingdom;
        _scenarioKingdom = scenarioKingdomService.Retrieve((int)scenario);
        _baseWarriorService = baseWarriorService;
        _scenarioWarriorService = scenarioWarriorService.Retrieve((int)scenario);
        _pokemonService = pokemonService;
        _scenarioPokemonService = scenarioPokemonService.Retrieve((int)scenario);
        UpdateWarriorImage();
    }

    public void Init()
    {

    }

    private ImageSource _warriorImage;
    public ImageSource WarriorImage
    {
        get => _warriorImage;
        set => RaiseAndSetIfChanged(ref _warriorImage, value);
    }

    private void UpdateWarriorImage()
    {
        int warrior = -1;
        foreach (var w in _scenarioWarriorService.Enumerate())
        {
            if (w.Class == WarriorClassId.ArmyLeader && w.Army == Army)
            {
                warrior = (int)w.Warrior;
                break;
            }
        }

        if (!_baseWarriorService.ValidateId(warrior))
        {
            WarriorImage = null;
            return;
        }
        int image = _baseWarriorService.Retrieve(warrior).Sprite;
        string spriteFile = _spriteProvider.GetSpriteFile(SpriteType.StlBushouS, image).File;
        if (!PathToImageSourceConverter.TryConvert(spriteFile, out var img))
        {
            WarriorImage = null;
            return;
        }
        WarriorImage = img;
    }

    public int Strength
    {
        get
        {
            int strength = 0;
            foreach (var warrior in _scenarioWarriorService.Enumerate())
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
        var sp = _scenarioPokemonService.Retrieve(ScenarioPokemon);
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
