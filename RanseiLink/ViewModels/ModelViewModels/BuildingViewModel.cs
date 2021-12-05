using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using System.Linq;

namespace RanseiLink.ViewModels;

public delegate BuildingViewModel BuildingViewModelFactory(IBuilding model);

public class BuildingViewModel : ViewModelBase
{
    private readonly IBuilding _model;

    public BuildingViewModel(IBuilding model)
    {
        _model = model;
    }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public KingdomId[] KingdomItems { get; } = EnumUtil.GetValues<KingdomId>().ToArray();

    public KingdomId Kingdom
    {
        get => _model.Kingdom;
        set => RaiseAndSetIfChanged(_model.Kingdom, value, v => _model.Kingdom = v);
    }
}
