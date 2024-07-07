#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public class EpisodeViewModel : ViewModelBase
{
    private Episode _model;
    private readonly ICachedMsgBlockService _msgService;
    private readonly IIdToNameService _idToNameService;

    public EpisodeViewModel(ICachedMsgBlockService msgService, IEpisodeService episodeService, IIdToNameService idToNameService)
    {
        _idToNameService = idToNameService;
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

        StartKingdomItems.Clear();
        UnlockedKingdomItems.Clear();
        foreach (KingdomId kingdom in EnumUtil.GetValuesExceptDefaults<KingdomId>())
        {
            string kingdomName = _idToNameService.IdToName<IKingdomService>((int)kingdom);
            StartKingdomItems.Add(new StartKingdomItem(model, kingdom, kingdomName));
            UnlockedKingdomItems.Add(new UnlockedKingdomItem(model, kingdom, kingdomName));
        }

        RaiseAllPropertiesChanged();
    }

    public int Id { get; private set; }

    public int Order
    {
        get => _model.Order;
        set => SetProperty(_model.Order, value, v => _model.Order = value);
    }

    public List<ScenarioId> ScenarioItems { get; } = EnumUtil.GetValues<ScenarioId>().ToList();

    public ScenarioId Scenario
    {
        get => _model.Scenario;
        set => SetProperty(_model.Scenario, value, v => _model.Scenario = v);
    }

    public List<SelectorComboBoxItem> UnlockConditionItems { get; }

    public int UnlockCondition
    {
        get => (int)_model.UnlockCondition;
        set => SetProperty(_model.UnlockCondition, (EpisodeId)value, v => _model.UnlockCondition = v);
    }

    public List<EpisodeClearConditionId> ClearConditionItems { get; } = EnumUtil.GetValues<EpisodeClearConditionId>().ToList();

    public EpisodeClearConditionId ClearCondition
    {
        get => _model.ClearCondition;
        set => SetProperty(_model.ClearCondition, value, v => _model.ClearCondition = v);
    }

    public int Difficulty
    {
        get => _model.Difficulty;
        set => SetProperty(_model.Difficulty, value, v => _model.Difficulty = value);
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

    public ObservableCollection<StartKingdomItem> StartKingdomItems { get; } = new();
    public ObservableCollection<UnlockedKingdomItem> UnlockedKingdomItems { get; } = new();

    public class StartKingdomItem : ViewModelBase
    {
        private readonly KingdomId _kingdom;
        private readonly Episode _model;
        public string KingdomName { get; }

        public StartKingdomItem(Episode episode, KingdomId kingdom, string kingdomName)
        {
            _kingdom = kingdom;
            _model = episode;
            KingdomName = kingdomName;
        }

        public bool IsStartKingdom
        {
            get => _model.IsStartKingdom(_kingdom);
            set => SetProperty(IsStartKingdom, value, v => _model.SetIsStartKingdom(_kingdom, v));
        }
    }

    public class UnlockedKingdomItem : ViewModelBase
    {
        private readonly KingdomId _kingdom;
        private readonly Episode _model;
        public string KingdomName { get; }

        public UnlockedKingdomItem(Episode episode, KingdomId kingdom, string kingdomName)
        {
            _kingdom = kingdom;
            _model = episode;
            KingdomName = kingdomName;
        }

        public bool IsUnlockedKingdom
        {
            get => _model.IsUnlockedKingdom(_kingdom);
            set => SetProperty(IsUnlockedKingdom, value, v => _model.SetIsUnlockedKingdom(_kingdom, v));
        }
    }

}