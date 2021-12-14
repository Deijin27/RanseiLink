using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.ViewModels;

public delegate AbilityViewModel AbilityViewModelFactory(IAbility model);

public abstract class AbilityViewModelBase : ViewModelBase
{
    private readonly IAbility _model;

    public AbilityViewModelBase(IAbility model)
    {
        _model = model;
    }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

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

public class AbilityViewModel : AbilityViewModelBase
{
    public AbilityViewModel(IAbility model) : base(model) { }
}

public class AbilityGridItemViewModel : AbilityViewModelBase
{
    public AbilityGridItemViewModel(AbilityId id, IAbility model) : base(model)
    {
        Id = id;
    }

    public AbilityId Id { get; }
}