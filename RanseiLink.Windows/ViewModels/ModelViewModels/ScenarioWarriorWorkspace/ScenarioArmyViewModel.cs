#nullable enable
using RanseiLink.Core.Models;
using System.Windows.Controls;

namespace RanseiLink.Windows.ViewModels;

public class ScenarioArmyViewModel : ViewModelBase
{
    private readonly ScenarioArmy _model;

    public int Id { get; }

    public ICommand? SelectCommand { get; private set; }

    public ScenarioArmyViewModel(int id, ScenarioArmy model, ICommand selectCommand)
    {
        Id = id;
        Name = $"Army {id}";
        _model = model;
        SelectCommand = selectCommand;
    }

    public string Name { get; }

    public int LeaderId => _model.Leader;

    public SwMiniViewModel? Leader
    {
        get;
        set
        {
            if (value != field)
            {
                field = value;
                if (value == null)
                {
                    _model.Leader = RanseiLink.Core.Services.Constants.ScenarioWarriorCount;
                }
                else
                {
                    _model.Leader = value.Id;
                }
                RaisePropertyChanged();
            }
        }
    }

    public int Money
    {
        get => _model.Money;
        set => SetProperty(_model.Money, value, v => _model.Money = v);
    }

    public int Unknown_4_16_2
    {
        get => _model.Unknown_4_16_2;
        set => SetProperty(_model.Unknown_4_16_2, value, v => _model.Unknown_4_16_2 = v);
    }

    public int Unknown_4_18_5
    {
        get => _model.Unknown_4_18_5;
        set => SetProperty(_model.Unknown_4_18_5, value, v => _model.Unknown_4_18_5 = v);
    }

}
