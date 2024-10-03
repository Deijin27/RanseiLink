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

        EpisodeItems = episodeService
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

        IsStartKingdomItems.Clear();
        IsUnlockedKingdomItems.Clear();
        foreach (KingdomId kingdom in EnumUtil.GetValuesExceptDefaults<KingdomId>())
        {
            string kingdomName = _idToNameService.IdToName<IKingdomService>((int)kingdom);
            IsStartKingdomItems.Add(new CheckBoxViewModel(kingdomName, 
                () => _model.GetIsStartKingdom(kingdom), 
                v => _model.SetIsStartKingdom(kingdom, v)
                ));
            IsUnlockedKingdomItems.Add(new CheckBoxViewModel(kingdomName,
                () => _model.GetIsUnlockedKingdom(kingdom),
                v => _model.SetIsUnlockedKingdom(kingdom, v)
                ));
        }

        RaiseAllPropertiesChanged();
    }

    public List<ScenarioId> ScenarioItems { get; } = EnumUtil.GetValues<ScenarioId>().ToList();

    public List<EpisodeClearConditionId> ClearConditionItems { get; } = EnumUtil.GetValues<EpisodeClearConditionId>().ToList();
}