using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using System.Linq;

namespace RanseiLink.ViewModels;

public delegate AbilityViewModel AbilityViewModelFactory(IAbility model);

public class AbilityViewModel : ViewModelBase
{
    private readonly IAbility _model;

    public AbilityViewModel(IAbility model)
    {
        _model = model;
    }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public AbilityEffectId[] EffectItems { get; } = EnumUtil.GetValues<AbilityEffectId>().ToArray();
    public AbilityEffectId Effect1
    {
        get => _model.Effect1;
        set => RaiseAndSetIfChanged(_model.Effect1, value, v => _model.Effect1 = v);
    }

    public uint Effect1Amount
    {
        get => _model.Effect1Amount;
        set => RaiseAndSetIfChanged(_model.Effect1Amount, value, v => _model.Effect1Amount = value);
    }

    public AbilityEffectId Effect2
    {
        get => _model.Effect2;
        set => RaiseAndSetIfChanged(_model.Effect2, value, v => _model.Effect2 = v);
    }

    public uint Effect2Amount
    {
        get => _model.Effect2Amount;
        set => RaiseAndSetIfChanged(_model.Effect2Amount, value, v => _model.Effect2Amount = value);
    }

}
