using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public interface IScenarioWarriorViewModel
{
    void SetModel(ScenarioId scenario, int id, ScenarioWarrior model);
}

public class ScenarioWarriorPokemonViewModel : ViewModelBase
{
    private readonly ScenarioWarrior _model;
    private readonly ScenarioWarriorViewModel _parent;
    public ScenarioWarriorPokemonViewModel(int id, ScenarioWarrior model, ScenarioWarriorViewModel parent)
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

public class ScenarioWarriorViewModel : ViewModelBase, IScenarioWarriorViewModel
{
    private ScenarioWarrior _model;
    private ScenarioPokemon _currentScenarioPokemon;
    private ScenarioId _scenario;
    private readonly IScenarioPokemonViewModel _scenarioPokemonVm;
    private readonly IScenarioPokemonService _scenarioPokemonService;
    private ScenarioWarriorPokemonViewModel _selectedPokemon;

    public ScenarioWarriorViewModel(IJumpService jumpService, IScenarioPokemonViewModel newScenarioPokemonViewModel, IScenarioPokemonService scenarioPokemonService, IIdToNameService idToNameService)
    {
        _scenarioPokemonService = scenarioPokemonService;
        _scenarioPokemonVm = newScenarioPokemonViewModel;

        WarriorItems = idToNameService.GetComboBoxItemsExceptDefault<IBaseWarriorService>();
        KingdomItems = idToNameService.GetComboBoxItemsPlusDefault<IKingdomService>();

        JumpToBaseWarriorCommand = new RelayCommand<int>(id => jumpService.JumpTo(BaseWarriorSelectorEditorModule.Id, id));
        JumpToMaxLinkCommand = new RelayCommand<int>(id => jumpService.JumpTo(MaxLinkSelectorEditorModule.Id, id));
        JumpToScenarioPokemon = new RelayCommand(() =>
        {
            if (ScenarioPokemonVm != null)
            {
                jumpService.JumpToScenarioPokemon(_scenario, SelectedPokemon.ScenarioPokemonId);
            }
        });
    }

    public void SetModel(ScenarioId scenario, int id, ScenarioWarrior model)
    {
        _scenario = scenario;
        _model = model;
        for (int i = 0; i < 8; i++)
        {
            var swp = new ScenarioWarriorPokemonViewModel(i, _model, this);
            Pokemon[i] = swp;
        }

        SelectedPokemon = Pokemon[0];
        UpdateEmbedded(0);

        RaiseAllPropertiesChanged();
    }

    public ICommand JumpToBaseWarriorCommand { get; }
    public ICommand JumpToMaxLinkCommand { get; }
    public ICommand JumpToScenarioPokemon { get; }

    public List<SelectorComboBoxItem> WarriorItems { get; }
    public List<SelectorComboBoxItem> KingdomItems { get; }

    public int Warrior
    {
        get => (int)_model.Warrior;
        set => RaiseAndSetIfChanged(_model.Warrior, (WarriorId)value, v => _model.Warrior = v);
    }

    public WarriorClassId Class
    {
        get => _model.Class;
        set => RaiseAndSetIfChanged(_model.Class, value, v => _model.Class = v);
    }

    public int Kingdom
    {
        get => (int)_model.Kingdom;
        set => RaiseAndSetIfChanged(_model.Kingdom, (KingdomId)value, v => _model.Kingdom = v);
    }

    public uint Army
    {
        get => _model.Army;
        set => RaiseAndSetIfChanged(_model.Army, value, v => _model.Army = v);
    }

    public ScenarioWarriorPokemonViewModel[] Pokemon { get; } = new ScenarioWarriorPokemonViewModel[8];

    public ScenarioWarriorPokemonViewModel SelectedPokemon
    {
        get => _selectedPokemon;
        set
        {
            if (_selectedPokemon != value)
            {
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
            ScenarioPokemonVm.SetModel(_scenario, 0, new ScenarioPokemon());
        }
        else
        {
            var spid = _model.GetScenarioPokemon(warriorPokemonId);
            _currentScenarioPokemon = _scenarioPokemonService.Retrieve((int)_scenario).Retrieve(spid);
            ScenarioPokemonVm.SetModel(_scenario, spid, _currentScenarioPokemon);
        }
    }

    public IScenarioPokemonViewModel ScenarioPokemonVm => _scenarioPokemonVm;
}
