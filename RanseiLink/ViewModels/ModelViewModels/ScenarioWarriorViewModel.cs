using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public delegate ScenarioWarriorViewModel ScenarioWarriorViewModelFactory(ScenarioId scenarioId, IEditorContext context, IScenarioWarrior model);

public abstract class ScenarioWarriorViewModelBase : ViewModelBase
{
    protected readonly IScenarioWarrior _model;
    
    public ScenarioWarriorViewModelBase(IScenarioWarrior model)
    {
        _model = model;
    }

    public WarriorId Warrior
    {
        get => _model.Warrior;
        set => RaiseAndSetIfChanged(_model.Warrior, value, v => _model.Warrior = v);
    }

    public WarriorClassId Class
    {
        get => _model.Class;
        set => RaiseAndSetIfChanged(_model.Class, value, v => _model.Class = v);
    }

    public KingdomId Kingdom
    {
        get => _model.Kingdom;
        set => RaiseAndSetIfChanged(_model.Kingdom, value, v => _model.Kingdom = v);
    }

    public uint Army
    {
        get => _model.Army;
        set => RaiseAndSetIfChanged(_model.Army, value, v => _model.Army = v);
    }
}

public class ScenarioWarriorPokemonViewModel : ViewModelBase
{
    private readonly IScenarioWarrior _model;
    private readonly ScenarioWarriorViewModel _parent;
    public ScenarioWarriorPokemonViewModel(int id, IScenarioWarrior model, ScenarioWarriorViewModel parent)
    {
        Id = id;
        _parent = parent;
        _model = model;
        SetToDefaultCommand = new RelayCommand(() =>
        {
            ScenarioPokemonId = 1100;
        });
    }
    public int Id { get; }
    
    public uint ScenarioPokemonId
    {
        get => _model.GetScenarioPokemon(Id);
        set 
        {
            if (ScenarioPokemonId != value)
            {
                value = CoerceValue(value);
                if (_parent.SelectedPokemon == this)
                {
                    _parent.Save();
                    _model.SetScenarioPokemon(Id, (ushort)value);
                    _parent.UpdateEmbedded(Id);
                }
                else
                {
                    _model.SetScenarioPokemon(Id, (ushort)value);
                }
                RaisePropertyChanged();
            } 
        }
    }

    private uint CoerceValue(uint value)
    {
        if (value >= Core.Services.Constants.ScenarioPokemonCount)
        {
            if (value > ScenarioPokemonId)
            {
                value = 1100;
            }
            else
            {
                value = Core.Services.Constants.ScenarioPokemonCount - 1;
            }
        }
        return value;
    }

    public ICommand SetToDefaultCommand { get; }
}

public class ScenarioWarriorViewModel : ScenarioWarriorViewModelBase, ISaveable
{
    private readonly IEditorContext _context;
    private readonly IDataService _dataService;
    private IScenarioPokemon _currentScenarioPokemon;
    private readonly ScenarioId _scenario;
    private readonly ScenarioPokemonViewModelFactory _scenarioPokemonVmFactory;
    private ScenarioPokemonViewModel _scenarioPokemonVm;
    private ScenarioWarriorPokemonViewModel _selectedPokemon;

    public ScenarioWarriorViewModel(IServiceContainer container, IEditorContext context, ScenarioId scenario, IScenarioWarrior model)
        : base(model)
    {
        _scenario = scenario;
        _context = context;
        _dataService = context.DataService;
        _scenarioPokemonVmFactory = container.Resolve<ScenarioPokemonViewModelFactory>();

        for (int i = 0; i < 8; i++)
        {
            var swp = new ScenarioWarriorPokemonViewModel(i, _model, this);
            Pokemon[i] = swp;
        }

        SelectedPokemon = Pokemon[0];
        UpdateEmbedded(0);

        var jumpService = context.JumpService;
        JumpToBaseWarriorCommand = new RelayCommand<WarriorId>(jumpService.JumpToBaseWarrior);
        JumpToMaxLinkCommand = new RelayCommand<WarriorId>(jumpService.JumpToMaxLink);
        JumpToScenarioPokemon = new RelayCommand(() =>
        {
            if (ScenarioPokemonVm != null)
            {
                jumpService.JumpToScenarioPokemon(scenario, SelectedPokemon.ScenarioPokemonId);
            }
        });
    }

    public ICommand JumpToBaseWarriorCommand { get; }
    public ICommand JumpToMaxLinkCommand { get; }
    public ICommand JumpToScenarioPokemon { get; }

    public ScenarioWarriorPokemonViewModel[] Pokemon { get; } = new ScenarioWarriorPokemonViewModel[8];

    public ScenarioWarriorPokemonViewModel SelectedPokemon
    {
        get => _selectedPokemon;
        set
        {
            if (_selectedPokemon != value)
            {
                Save();
                _selectedPokemon = value;
                RaisePropertyChanged();
                UpdateEmbedded(value.Id);
            }
        }
    }

    public void UpdateEmbedded(int warriorPokemonId)
    {
        if (_model.ScenarioPokemonIsDefault(warriorPokemonId))
        {
            ScenarioPokemonVm = null;
        }
        else
        {
            var spid = _model.GetScenarioPokemon(warriorPokemonId);
            _currentScenarioPokemon = _dataService.ScenarioPokemon.Retrieve(_scenario, spid);
            ScenarioPokemonVm = _scenarioPokemonVmFactory(_currentScenarioPokemon, _context, _scenario, spid);
        }
    }

    public ScenarioPokemonViewModel ScenarioPokemonVm
    {
        get => _scenarioPokemonVm;
        set => RaiseAndSetIfChanged(ref _scenarioPokemonVm, value);
    }

    public void Save()
    {
        if (_scenarioPokemonVm != null)
        {
            _dataService.ScenarioPokemon.Save(_scenario, (int)SelectedPokemon.ScenarioPokemonId, _currentScenarioPokemon);
        }
    }
}

public class ScenarioWarriorGridItemViewModel : ScenarioWarriorViewModelBase
{
    public ScenarioWarriorGridItemViewModel(int id, IScenarioWarrior model)
        : base(model)
    {
        Id = id;
    }

    public int Id { get; }

    public new WarriorId Warrior
    {
        get => _model.Warrior;
        set => RaiseAndSetIfChanged(_model.Warrior, value, v => _model.Warrior = v);
    }

    private int _scenarioPokemonId;
    public int ScenarioPokemonId 
    {
        get => _scenarioPokemonId;
        set
        {
            if (value >= -1  && value < 200)
            {
                _scenarioPokemonId = value;
                RaisePropertyChanged();
            }
        }
    }

    public PokemonId Pokemon { get; set; }

    public AbilityId PokemonAbility { get; set; }

}