#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public partial class EpisodeViewModel : ViewModelBase
{
    private readonly ICachedMsgBlockService _msgService;
    private readonly IIdToNameService _idToNameService;

    public EpisodeViewModel(ICachedMsgBlockService msgService, IEpisodeService episodeService, IIdToNameService idToNameService)
    {
        _idToNameService = idToNameService;
        _msgService = msgService;

        UnlockConditionItems = episodeService
            .ValidIds()
            .Select(i => new SelectorComboBoxItem(i, msgService.GetMsgOfType(MsgShortcut.EpisodeName, i)))
            .Append(new SelectorComboBoxItem(38, "Default"))
            .Append(new SelectorComboBoxItem(50, "Special"))
            .ToList();
    }

    public void SetModel(EpisodeId id, Episode model)
    {
        _id = id;
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

    public List<ScenarioId> ScenarioItems { get; } = EnumUtil.GetValues<ScenarioId>().ToList();

    public List<SelectorComboBoxItem> UnlockConditionItems { get; }

    public List<EpisodeClearConditionId> ClearConditionItems { get; } = EnumUtil.GetValues<EpisodeClearConditionId>().ToList();

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
            get => _model.GetIsStartKingdom(_kingdom);
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
            get => _model.GetIsUnlockedKingdom(_kingdom);
            set => SetProperty(IsUnlockedKingdom, value, v => _model.SetIsUnlockedKingdom(_kingdom, v));
        }
    }

}