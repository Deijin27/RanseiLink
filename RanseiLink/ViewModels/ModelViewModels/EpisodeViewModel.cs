using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.ViewModels;

public interface IEpisodeViewModel
{
    void SetModel(EpisodeId id, Episode model);
}

public class EpisodeViewModel : ViewModelBase, IEpisodeViewModel
{
    private Episode _model;
    private readonly ICachedMsgBlockService _msgService;

    public EpisodeViewModel(ICachedMsgBlockService msgService, IEpisodeService episodeService)
    {
        _msgService = msgService;
        _model = new Episode();

        UnlockConditionItems = episodeService
            .ValidIds()
            .Select(i => new SelectorComboBoxItem(i, msgService.GetMsgOfType(MsgShortcut.EpisodeName, i)))
            .Append(new SelectorComboBoxItem(38, "Default"))
            .Append(new SelectorComboBoxItem(50, "Special"))
            .ToList();
    }

    public void SetModel(EpisodeId id, Episode model)
    {
        Id = (int)id;
        _model = model;
        RaiseAllPropertiesChanged();
    }

    public int Id { get; private set; }

    public int Order
    {
        get => _model.Order;
        set => RaiseAndSetIfChanged(_model.Order, value, v => _model.Order = value);
    }

    public List<ScenarioId> ScenarioItems { get; } = EnumUtil.GetValues<ScenarioId>().ToList();

    public ScenarioId Scenario
    {
        get => _model.Scenario;
        set => RaiseAndSetIfChanged(_model.Scenario, value, v => _model.Scenario = v);
    }

    public List<SelectorComboBoxItem> UnlockConditionItems { get; }

    public int UnlockCondition
    {
        get => (int)_model.UnlockCondition;
        set => RaiseAndSetIfChanged(_model.UnlockCondition, (EpisodeId)value, v => _model.UnlockCondition = v);
    }

    public int Difficulty
    {
        get => _model.Difficulty;
        set => RaiseAndSetIfChanged(_model.Difficulty, value, v => _model.Difficulty = value);
    }

    public string Name
    {
        get => _msgService.GetMsgOfType(MsgShortcut.EpisodeName, Id);
        set => _msgService.SetMsgOfType(MsgShortcut.EpisodeName, Id, value);
    }

    public string Description
    {
        get => _msgService.GetMsgOfType(MsgShortcut.EpisodeDescription, Id);
        set => _msgService.SetMsgOfType(MsgShortcut.EpisodeDescription, Id, value);
    }

}