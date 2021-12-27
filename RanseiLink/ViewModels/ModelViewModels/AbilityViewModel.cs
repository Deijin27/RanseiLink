using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate AbilityViewModel AbilityViewModelFactory(AbilityId id, IAbility model, IEditorContext context);

public abstract class AbilityViewModelBase : ViewModelBase
{
    private readonly IAbility _model;

    public AbilityViewModelBase(AbilityId id, IAbility model)
    {
        Id = id;
        _model = model;
    }

    public AbilityId Id { get; }

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
    private readonly ICachedMsgBlockService _msgService;
    public AbilityViewModel(AbilityId id, IAbility model, IEditorContext context) : base(id, model) 
    {
        _msgService = context.CachedMsgBlockService;
    }

    public string Description
    {
        get => _msgService.GetAbilityDescription(Id);
        set => _msgService.SetAbilityDescription(Id, value);
    }

    public string HotSpringsDescription
    {
        get => _msgService.GetAbilityHotSpringsDescription(Id);
        set => _msgService.SetAbilityHotSpringsDescription(Id, value);
    }

    public string HotSpringsDescription2
    {
        get => _msgService.GetAbilityHotSpringsDescription2(Id);
        set => _msgService.SetAbilityHotSpringsDescription2(Id, value);
    }
}

public class AbilityGridItemViewModel : AbilityViewModelBase
{
    public AbilityGridItemViewModel(AbilityId id, IAbility model) : base(id, model)
    {
    }
    
}