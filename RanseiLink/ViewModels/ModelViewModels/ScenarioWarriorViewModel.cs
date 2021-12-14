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

public class ScenarioWarriorViewModel : ScenarioWarriorViewModelBase, ISaveable
{
    private readonly IEditorContext _context;
    private readonly IDataService _dataService;
    private IScenarioPokemon _currentScenarioPokemon;
    private readonly ScenarioId _scenario;
    private readonly ScenarioPokemonViewModelFactory _scenarioPokemonVmFactory;
    private ScenarioPokemonViewModel _scenarioPokemonVm;

    public ScenarioWarriorViewModel(IServiceContainer container, IEditorContext context, ScenarioId scenario, IScenarioWarrior model)
        : base(model)
    {
        _scenario = scenario;
        _context = context;
        _dataService = context.DataService;
        _scenarioPokemonVmFactory = container.Resolve<ScenarioPokemonViewModelFactory>();

        UpdateEmbedded();

        var jumpService = context.JumpService;
        JumpToBaseWarriorCommand = new RelayCommand<WarriorId>(jumpService.JumpToBaseWarrior);
        JumpToMaxLinkCommand = new RelayCommand<WarriorId>(jumpService.JumpToMaxLink);
    }

    public ICommand JumpToBaseWarriorCommand { get; }
    public ICommand JumpToMaxLinkCommand { get; }

    public uint ScenarioPokemon
    {
        get => _model.ScenarioPokemon;
        set
        {
            if (_model.ScenarioPokemon != value)
            {
                Save();
                _model.ScenarioPokemon = value;
                RaisePropertyChanged();
                UpdateEmbedded();
            }
        }
    }

    public bool ScenarioPokemonIsDefault
    {
        get => _model.ScenarioPokemonIsDefault;
        set
        {
            if (_model.ScenarioPokemonIsDefault != value)
            {
                Save();
                _model.ScenarioPokemonIsDefault = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ScenarioPokemon));
                UpdateEmbedded();
            }
        }
    }

    private void UpdateEmbedded()
    {
        if (_model.ScenarioPokemonIsDefault)
        {
            ScenarioPokemonVm = null;
        }
        else
        {
            _currentScenarioPokemon = _dataService.ScenarioPokemon.Retrieve(_scenario, (int)ScenarioPokemon);
            ScenarioPokemonVm = _scenarioPokemonVmFactory(_currentScenarioPokemon, _context);
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
            _dataService.ScenarioPokemon.Save(_scenario, (int)_model.ScenarioPokemon, _currentScenarioPokemon);
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