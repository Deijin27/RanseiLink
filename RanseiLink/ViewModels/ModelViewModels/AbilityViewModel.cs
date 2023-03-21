#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;

namespace RanseiLink.ViewModels;

public class AbilityViewModel : ViewModelBase
{
    private Ability _model;
    private readonly ICachedMsgBlockService _msgService;

    public AbilityViewModel(ICachedMsgBlockService msgService)
    {
        _msgService = msgService;
        _model = new Ability();
    }

    public void SetModel(AbilityId id, Ability model)
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

    public AbilityEffectId Effect1
    {
        get => _model.Effect1;
        set => RaiseAndSetIfChanged(_model.Effect1, value, v => _model.Effect1 = v);
    }

    public int Effect1Amount
    {
        get => _model.Effect1Amount;
        set => RaiseAndSetIfChanged(_model.Effect1Amount, value, v => _model.Effect1Amount = value);
    }

    public AbilityEffectId Effect2
    {
        get => _model.Effect2;
        set => RaiseAndSetIfChanged(_model.Effect2, value, v => _model.Effect2 = v);
    }

    public int Effect2Amount
    {
        get => _model.Effect2Amount;
        set => RaiseAndSetIfChanged(_model.Effect2Amount, value, v => _model.Effect2Amount = value);
    }

    public string Description
    {
        get => _msgService.GetMsgOfType(MsgShortcut.AbilityDescription, Id);
        set => _msgService.SetMsgOfType(MsgShortcut.AbilityDescription, Id, value);
    }

    public string HotSpringsDescription
    {
        get => _msgService.GetMsgOfType(MsgShortcut.AbilityHotSpringsDescription, Id);
        set => _msgService.SetMsgOfType(MsgShortcut.AbilityHotSpringsDescription, Id, value);
    }

    public string HotSpringsDescription2
    {
        get => _msgService.GetMsgOfType(MsgShortcut.AbilityHotSpringsDescription2, Id);
        set => _msgService.SetMsgOfType(MsgShortcut.AbilityHotSpringsDescription2, Id, value);
    }
}