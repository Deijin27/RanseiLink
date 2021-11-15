using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using System.Linq;

namespace RanseiLink.ViewModels;

public class WarriorSkillViewModel : ViewModelBase, IViewModelForModel<IWarriorSkill>
{
    public IWarriorSkill Model { get; set; }

    public WarriorSkillTargetId[] TargetItems { get; } = EnumUtil.GetValues<WarriorSkillTargetId>().ToArray();
    public WarriorSkillEffectId[] EffectItems { get; } = EnumUtil.GetValues<WarriorSkillEffectId>().ToArray();

    public string Name
    {
        get => Model.Name;
        set => RaiseAndSetIfChanged(Model.Name, value, v => Model.Name = v);
    }

    public WarriorSkillEffectId Effect1
    {
        get => Model.Effect1;
        set => RaiseAndSetIfChanged(Model.Effect1, value, v => Model.Effect1 = v);
    }

    public uint Effect1Amount
    {
        get => Model.Effect1Amount;
        set => RaiseAndSetIfChanged(Model.Effect1Amount, value, v => Model.Effect1Amount = v);
    }

    public WarriorSkillEffectId Effect2
    {
        get => Model.Effect2;
        set => RaiseAndSetIfChanged(Model.Effect2, value, v => Model.Effect2 = v);
    }

    public uint Effect2Amount
    {
        get => Model.Effect2Amount;
        set => RaiseAndSetIfChanged(Model.Effect2Amount, value, v => Model.Effect2Amount = v);
    }

    public WarriorSkillEffectId Effect3
    {
        get => Model.Effect3;
        set => RaiseAndSetIfChanged(Model.Effect3, value, v => Model.Effect3 = v);
    }

    public uint Effect3Amount
    {
        get => Model.Effect3Amount;
        set => RaiseAndSetIfChanged(Model.Effect3Amount, value, v => Model.Effect3Amount = v);
    }

    public uint Duration
    {
        get => Model.Duration;
        set => RaiseAndSetIfChanged(Model.Duration, value, v => Model.Duration = v);
    }

    public WarriorSkillTargetId Target
    {
        get => Model.Target;
        set => RaiseAndSetIfChanged(Model.Target, value, v => Model.Target = v);
    }


}
