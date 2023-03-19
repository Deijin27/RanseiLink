using RanseiLink.Core.Enums;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace RanseiLink.Core.Services.ModPatchBuilders;

public class DataPatchBuilder : IPatchBuilder
{
    private readonly ModInfo _mod;
    public DataPatchBuilder(ModInfo mod)
    {
        _mod = mod;
    }
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
        }

        filesToPatch.Add(new FileToPatch(Constants.PokemonRomPath, Path.Combine(_mod.FolderPath, Constants.PokemonRomPath), FilePatchOptions.VariableLength));
        foreach (string drp in dataRomPaths)
        {
            filesToPatch.Add(new FileToPatch(drp, Path.Combine(_mod.FolderPath, drp), FilePatchOptions.None));
        }
    }
}