using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using System.Linq;

namespace RanseiLink.ViewModels;

public class ScenarioWarriorViewModel : ViewModelBase, IViewModelForModel<IScenarioWarrior>, ISaveable
{
    private readonly IDataService DataService;
    public ScenarioWarriorViewModel(IDataService service, ScenarioId scenario)
    {
        DataService = service;
        Scenario = scenario;
    }

    private readonly ScenarioId Scenario;

    private IScenarioWarrior _model;
    public IScenarioWarrior Model
    {
        get => _model;
        set
        {
            _model = value;
            if (_model != null)
            {
                UpdateEmbedded();
            }
        }
    }

    public WarriorId[] WarriorItems { get; } = EnumUtil.GetValues<WarriorId>().ToArray();

    public WarriorId Warrior
    {
        get => Model.Warrior;
        set
        {
            if (Model.Warrior != value)
            {
                Save();
                Model.Warrior = value;
                RaisePropertyChanged();
                UpdateEmbedded();
            }
        }
    }

    public uint ScenarioPokemon
    {
        get => Model.ScenarioPokemon;
        set
        {
            if (Model.ScenarioPokemon != value)
            {
                Save();
                Model.ScenarioPokemon = value;
                RaisePropertyChanged();
                UpdateEmbedded();
            }
        }
    }

    public bool ScenarioPokemonIsDefault
    {
        get => Model.ScenarioPokemonIsDefault;
        set
        {
            if (Model.ScenarioPokemonIsDefault != value)
            {
                Save();
                Model.ScenarioPokemonIsDefault = value;
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
            DataService.ScenarioPokemon.Save(Scenario, (int)ScenarioPokemon, ScenarioPokemonVm.Model);
        }

        if (MaxLinkTable != null)
        {
            DataService.MaxLink.Save(Warrior, MaxLinkTable);
        }
    }

    private void UpdateEmbedded()
    {
        MaxLinkTable = DataService.MaxLink.Retrieve(Warrior);
        if (ScenarioPokemonIsDefault)
        {
            ScenarioPokemonVm = null;
        }
        else
        {
            var sp = DataService.ScenarioPokemon.Retrieve(Scenario, (int)ScenarioPokemon);
            ScenarioPokemonVm = new ScenarioPokemonViewModel() { Model = sp };
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
