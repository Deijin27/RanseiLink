using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public delegate ScenarioWarriorViewModel ScenarioWarriorViewModelFactory(ScenarioId scenarioId, IEditorContext context, IScenarioWarrior model);

public class ScenarioWarriorViewModel : ViewModelBase, ISaveable
{
    private readonly IEditorContext _context;
    private readonly IDataService _dataService;
    private readonly ScenarioId _scenario;
    private readonly IScenarioWarrior _model;
    private readonly ScenarioPokemonViewModelFactory _scenarioPokemonVmFactory;
    private IScenarioPokemon _currentScenarioPokemon;

    public ScenarioWarriorViewModel(IServiceContainer container, IEditorContext context, ScenarioId scenario, IScenarioWarrior model)
    {
        _context = context;
        _dataService = context.DataService;
        _scenario = scenario;
        _model = model;
        _scenarioPokemonVmFactory = container.Resolve<ScenarioPokemonViewModelFactory>();
        UpdateEmbedded();

        var jumpService = context.JumpService;
        JumpToBaseWarriorCommand = new RelayCommand<WarriorId>(jumpService.JumpToBaseWarrior);
        JumpToMaxLinkCommand = new RelayCommand<WarriorId>(jumpService.JumpToMaxLink);
    }

    public ICommand JumpToBaseWarriorCommand { get; }
    public ICommand JumpToMaxLinkCommand { get; }

    public WarriorId[] WarriorItems { get; } = EnumUtil.GetValues<WarriorId>().ToArray();

    public WarriorId Warrior
    {
        get => _model.Warrior;
        set
        {
            if (_model.Warrior != value)
            {
                Save();
                _model.Warrior = value;
                RaisePropertyChanged();
                UpdateEmbedded();
            }
        }
    }

    public WarriorClassId[] ClassItems { get; } = EnumUtil.GetValues<WarriorClassId>().ToArray();

    public WarriorClassId Class
    {
        get => _model.Class;
        set => RaiseAndSetIfChanged(_model.Class, value, v => _model.Class = v);
    }

    public KingdomId[] KingdomItems { get; } = EnumUtil.GetValues<KingdomId>().ToArray();

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

    public void Save()
    {
        if (ScenarioPokemonVm != null)
        {
            _dataService.ScenarioPokemon.Save(_scenario, (int)ScenarioPokemon, _currentScenarioPokemon);
        }

        if (MaxLinkTable != null)
        {
            _dataService.MaxLink.Save(Warrior, MaxLinkTable);
        }
    }

    private void UpdateEmbedded()
    {
        MaxLinkTable = _dataService.MaxLink.Retrieve(Warrior);
        if (ScenarioPokemonIsDefault)
        {
            ScenarioPokemonVm = null;
        }
        else
        {
            _currentScenarioPokemon = _dataService.ScenarioPokemon.Retrieve(_scenario, (int)ScenarioPokemon);
            ScenarioPokemonVm = _scenarioPokemonVmFactory(_currentScenarioPokemon, _context);
            RaisePropertyChanged(nameof(MaxLink));
        }
    }

    private ScenarioPokemonViewModel _scenarioPokemonVm;
    public ScenarioPokemonViewModel ScenarioPokemonVm
    {
        get => _scenarioPokemonVm;
        set
        {
            if (RaiseAndSetIfChanged(ref _scenarioPokemonVm, value) && value != null)
            {
                value.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(ScenarioPokemonViewModel.Pokemon))
                    {
                        RaisePropertyChanged(nameof(MaxLink));
                    }
                };
            }
        }
    }

    private IMaxLink MaxLinkTable;

    public uint MaxLink
    {
        get
        {
            if (MaxLinkTable != null && ScenarioPokemonVm != null)
            {
                return MaxLinkTable.GetMaxLink(ScenarioPokemonVm.Pokemon);
            }
            return 0;
        }
        set
        {
            if (MaxLinkTable != null)
            {
                RaiseAndSetIfChanged(MaxLink, value, v => MaxLinkTable.SetMaxLink(ScenarioPokemonVm.Pokemon, v));
            }
        }
    }

}
