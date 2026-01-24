using RanseiLink.Core.Enums;
using System.Collections.Concurrent;

namespace RanseiLink.Core.Services.ModPatchBuilders;

[PatchBuilder]
public class DataPatchBuilder(ModInfo mod) : IPatchBuilder
{
    public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, PatchOptions patchOptions)
    {
        List<string> dataRomPaths = new List<string>()
        {
            Constants.MoveRomPath,
            Constants.AbilityRomPath,
            Constants.WarriorSkillRomPath,
            Constants.GimmickRomPath,
            Constants.BuildingRomPath,
            Constants.ItemRomPath,
            Constants.KingdomRomPath,
            Constants.MoveRangeRomPath,
            Constants.EventSpeakerRomPath,
            Constants.MaxLinkRomPath,
            Constants.BaseBushouRomPath,
            Constants.BattleConfigRomPath,
            Constants.GimmickRangeRomPath,
            Constants.MoveAnimationRomPath,
            Constants.GimmickObjectRomPath,
            Constants.EpisodeRomPath
        };

        foreach (var i in EnumUtil.GetValues<ScenarioId>())
        {
            dataRomPaths.Add(Constants.ScenarioPokemonPathFromId((int)i));
            dataRomPaths.Add(Constants.ScenarioWarriorPathFromId((int)i));
            dataRomPaths.Add(Constants.ScenarioAppearPokemonPathFromId((int)i));
            dataRomPaths.Add(Constants.ScenarioKingdomPathFromId((int)i));
            dataRomPaths.Add(Constants.ScenarioBuildingPathFromId((int)i));
            dataRomPaths.Add(Constants.ScenarioArmyPathFromId((int)i));
        }

        filesToPatch.Add(new FileToPatch(Constants.PokemonRomPath, Path.Combine(mod.FolderPath, Constants.PokemonRomPath), FilePatchOptions.VariableLength));
        foreach (string drp in dataRomPaths)
        {
            filesToPatch.Add(new FileToPatch(drp, Path.Combine(mod.FolderPath, drp), FilePatchOptions.None));
        }
    }
}