#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Util;
using System.Xml.Linq;

namespace RanseiLink.GuiCore.ViewModels;


public class EpisodeMiniViewModel(
    ICachedSpriteProvider spriteProvider,
    IBaseWarriorService baseWarriorService,
    IScenarioWarriorService scenarioWarriorService,
    IScenarioKingdomService scenarioKingdomService,
    ICachedMsgBlockService cachedMsgBlockService,
    Episode model,
    int id,
    ICommand selectCommand) : ViewModelBase, IMiniViewModel
{
    public int Id => id;

    public string Name
    {
        get => cachedMsgBlockService.GetMsgOfType(MsgShortcut.EpisodeName, Id);
    }

    public object? Image
    {
        get
        {
            var scenario = (int)model.Scenario;
            KingdomId startKingdom = KingdomId.Default;
            foreach (KingdomId kingdom in EnumUtil.GetValuesExceptDefaults<KingdomId>())
            {
                if (model.GetIsStartKingdom(kingdom))
                {
                    startKingdom = kingdom;
                    break;
                }
            }
            if (startKingdom == KingdomId.Default)
            {
                return null;
            }
            var army = scenarioKingdomService.Retrieve(scenario).GetArmy(startKingdom);
            foreach (var scenWarrior in scenarioWarriorService.Retrieve(scenario).Enumerate())
            {
                if (scenWarrior.Army == army && scenWarrior.Class == WarriorClassId.ArmyLeader)
                {
                    var baseWarrior = baseWarriorService.Retrieve((int)scenWarrior.Warrior);
                    return spriteProvider.GetSprite(SpriteType.StlBushouS, baseWarrior.Sprite);
                }
            }

            return null;
            
        }
    }

    public int Difficulty => model.Difficulty;

    public ICommand SelectCommand { get; } = selectCommand;

    public bool MatchSearchTerm(string searchTerm)
    {
        if (Name.ContainsIgnoreCaseAndAccents(searchTerm))
        {
            return true;
        }

        return false;
    }

    public void NotifyPropertyChanged(string? name)
    {
        switch (name)
        {
            case nameof(EpisodeViewModel.Name):
                RaisePropertyChanged(nameof(Name));
                break;
            case nameof(EpisodeViewModel.Scenario):
            case nameof(EpisodeViewModel.StartKingdomChanged):
                RaisePropertyChanged(nameof(Image));
                break;
            case nameof(EpisodeViewModel.Difficulty):
                RaisePropertyChanged(nameof(Difficulty));
                break;
        }
    }
}