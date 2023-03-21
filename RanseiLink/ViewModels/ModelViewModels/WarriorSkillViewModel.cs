#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;

namespace RanseiLink.ViewModels;

public class WarriorSkillViewModel : ViewModelBase
{
    public WarriorSkill _model;

    private readonly ICachedMsgBlockService _msgService;

    public WarriorSkillViewModel(ICachedMsgBlockService msgService)
    {
        _msgService = msgService;
        _model = new WarriorSkill();
    }

    public void SetModel(WarriorSkillId id, WarriorSkill model)
    {
        Id = (int)id;
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public int Id { get; private set; }

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

    public int Effect1Amount
    {
        get => _model.Effect1Amount;
        set => RaiseAndSetIfChanged(_model.Effect1Amount, value, v => _model.Effect1Amount = v);
    }

    public WarriorSkillEffectId Effect2
    {
        get => _model.Effect2;
        set => RaiseAndSetIfChanged(_model.Effect2, value, v => _model.Effect2 = v);
    }

    public int Effect2Amount
    {
        get => _model.Effect2Amount;
        set => RaiseAndSetIfChanged(_model.Effect2Amount, value, v => _model.Effect2Amount = v);
    }

    public WarriorSkillEffectId Effect3
    {
        get => _model.Effect3;
        set => RaiseAndSetIfChanged(_model.Effect3, value, v => _model.Effect3 = v);
    }

    public int Effect3Amount
    {
        get => _model.Effect3Amount;
        set => RaiseAndSetIfChanged(_model.Effect3Amount, value, v => _model.Effect3Amount = v);
    }

    public int Duration
    {
        get => _model.Duration;
        set => RaiseAndSetIfChanged(_model.Duration, value, v => _model.Duration = v);
    }

    public WarriorSkillTargetId Target
    {
        get => _model.Target;
        set => RaiseAndSetIfChanged(_model.Target, value, v => _model.Target = v);
    }

    public MoveAnimationId Animation
    {
        get => _model.Animation;
        set => RaiseAndSetIfChanged(_model.Animation, value, v => _model.Animation = v);
    }

    public string Description
    {
        get => _msgService.GetMsgOfType(MsgShortcut.WarriorSkillDescription, Id);
        set => _msgService.SetMsgOfType(MsgShortcut.WarriorSkillDescription, Id, value);
    }

}