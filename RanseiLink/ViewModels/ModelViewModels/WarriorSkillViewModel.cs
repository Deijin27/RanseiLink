using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Services;

namespace RanseiLink.ViewModels;

public delegate WarriorSkillViewModel WarriorSkillViewModelFactory(WarriorSkillId id, IWarriorSkill model, IEditorContext context);

public class WarriorSkillViewModelBase : ViewModelBase
{
    public IWarriorSkill _model;

    public WarriorSkillViewModelBase(WarriorSkillId id, IWarriorSkill model)
    {
        Id = id;
        _model = model;
    }

    public WarriorSkillId Id { get; }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public WarriorSkillEffectId Effect1
    {
        get => _model.Effect1;
        set => RaiseAndSetIfChanged(_model.Effect1, value, v => _model.Effect1 = v);
    }

    public uint Effect1Amount
    {
        get => _model.Effect1Amount;
        set => RaiseAndSetIfChanged(_model.Effect1Amount, value, v => _model.Effect1Amount = v);
    }

    public WarriorSkillEffectId Effect2
    {
        get => _model.Effect2;
        set => RaiseAndSetIfChanged(_model.Effect2, value, v => _model.Effect2 = v);
    }

    public uint Effect2Amount
    {
        get => _model.Effect2Amount;
        set => RaiseAndSetIfChanged(_model.Effect2Amount, value, v => _model.Effect2Amount = v);
    }

    public WarriorSkillEffectId Effect3
    {
        get => _model.Effect3;
        set => RaiseAndSetIfChanged(_model.Effect3, value, v => _model.Effect3 = v);
    }

    public uint Effect3Amount
    {
        get => _model.Effect3Amount;
        set => RaiseAndSetIfChanged(_model.Effect3Amount, value, v => _model.Effect3Amount = v);
    }

    public uint Duration
    {
        get => _model.Duration;
        set => RaiseAndSetIfChanged(_model.Duration, value, v => _model.Duration = v);
    }

    public WarriorSkillTargetId Target
    {
        get => _model.Target;
        set => RaiseAndSetIfChanged(_model.Target, value, v => _model.Target = v);
    }

}

public class WarriorSkillViewModel : WarriorSkillViewModelBase
{
    private readonly ICachedMsgBlockService _msgService;
    public WarriorSkillViewModel(WarriorSkillId id, IWarriorSkill model, IEditorContext context) : base(id, model)
    {
        _msgService = context.CachedMsgBlockService;
    }
    public string Description
    {
        get => _msgService.GetWarriorSkillDescription(Id);
        set => _msgService.SetWarriorSkillDescription(Id, value);
    }
}

public class WarriorSkillGridItemViewModel : WarriorSkillViewModelBase 
{
    public WarriorSkillGridItemViewModel(WarriorSkillId id, IWarriorSkill model) : base(id, model)
    {
    }

}
