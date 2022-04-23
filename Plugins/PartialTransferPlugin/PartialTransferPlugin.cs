using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PartialTransferPlugin;

[Plugin("Partial Transfer", "Deijin", "2.1")]
public class PartialTransferPlugin : IPlugin
{
    public void Run(IPluginContext context)
    {
        Dictionary<string, ModInfo> modDict = new();
        var modService = context.Services.Get<IModManager>();
        foreach (ModInfo mod in modService.GetAllModInfo())
        {
            string key = $"{mod.Name} v{mod.Version} by {mod.Author}";
            string finalKey = key;
            int count = 0;
            while (modDict.ContainsKey(finalKey))
            {
                finalKey = key + $" [{count++}]";
            }
            modDict.Add(finalKey, mod);
        }
        var activeMod = context.Services.Get<ModInfo>();
        string currentKey = modDict.First(i => i.Value.FolderPath == activeMod.FolderPath).Key;

        var options = new PartialTransferOptionForm()
        {
            Mods = modDict.Select(i => i.Key).OrderBy(i => i).ToList(),
            SourceMod = currentKey,
            DestinationMod = currentKey
        };

        var dialogService = context.Services.Get<IDialogService>();


        var optionService = context.Services.Get<IPluginService>();
        do
        {
            if (!optionService.RequestOptions(options))
            {
                return;
            }

            if (options.SourceMod == options.DestinationMod)
            {
                dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    "Invalid Options",
                    "The source mod must be different to the destination mod"
                    ));
            }

        } while (options.SourceMod == options.DestinationMod);
        

        var sourceMod = modDict[options.SourceMod];
        var destinationMod = modDict[options.DestinationMod];

        List<string> filesToTransfer = new();
        List<string> dirsToTransfer = new();

        if (options.Ability)
            filesToTransfer.Add(Constants.AbilityRomPath);
        if (options.BaseWarrior)
            filesToTransfer.Add(Constants.BaseBushouRomPath);
        if (options.BattleConfigs)
            filesToTransfer.Add(Constants.BattleConfigRomPath);
        if (options.Building)
            filesToTransfer.Add(Constants.BuildingRomPath);
        if (options.EventSpeaker)
            filesToTransfer.Add(Constants.EventSpeakerRomPath);
        if (options.Episode)
            filesToTransfer.Add(Constants.EpisodeRomPath);
        if (options.Gimmicks)
            filesToTransfer.Add(Constants.GimmickRomPath);
        if (options.Item)
            filesToTransfer.Add(Constants.ItemRomPath);
        if (options.Kingdoms)
            filesToTransfer.Add(Constants.KingdomRomPath);
        if (options.Maps)
            dirsToTransfer.Add(Constants.MapFolderPath);
        if (options.MaxLink)
            filesToTransfer.Add(Constants.MaxLinkRomPath);
        if (options.MoveRange)
            filesToTransfer.Add(Constants.MoveRangeRomPath);
        if (options.Move)
            filesToTransfer.Add(Constants.MoveRomPath);
        if (options.Pokemon)
            filesToTransfer.Add(Constants.PokemonRomPath);
        if (options.WarriorSkill)
            filesToTransfer.Add(Constants.WarriorSkillRomPath);

        foreach (var i in EnumUtil.GetValues<ScenarioId>())
        {
            if (options.ScenarioPokemon)
                filesToTransfer.Add(Constants.ScenarioPokemonPathFromId((int)i));
            if (options.ScenarioWarrior)
                filesToTransfer.Add(Constants.ScenarioWarriorPathFromId((int)i));
            if (options.ScenarioAppearPokemon)
                filesToTransfer.Add(Constants.ScenarioAppearPokemonPathFromId((int)i));
            if (options.ScenarioKingdom)
                filesToTransfer.Add(Constants.ScenarioKingdomPathFromId((int)i));
        }

        if (options.Sprites)
            dirsToTransfer.Add(Constants.GraphicsFolderPath);
        if (options.Text)
            dirsToTransfer.Add(Constants.MsgFolderPath);
        
        
        dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo(statusText:"Transferring...", maxProgress:filesToTransfer.Count+dirsToTransfer.Count));

            int count = 0;
            foreach (var file in filesToTransfer)
            {
                string sourcePath = Path.Combine(sourceMod.FolderPath, file);
                string destinationPath = Path.Combine(destinationMod.FolderPath, file);
                File.Copy(sourcePath, destinationPath, true);
                count++;
                progress.Report(new ProgressInfo(progress:count));
            }
            foreach(var dir in dirsToTransfer)
            {
                string sourcePath = Path.Combine(sourceMod.FolderPath, dir);
                string destinationPath = Path.Combine(destinationMod.FolderPath, dir);
                if (Directory.Exists(destinationPath))
                {
                    Directory.Delete(destinationPath, true);
                }
                Directory.CreateDirectory(destinationPath);
                FileUtil.CopyFilesRecursively(sourcePath, destinationPath);
                count++;
                progress.Report(new ProgressInfo(progress: count));
            }

            progress.Report(new ProgressInfo(statusText:"Transfer Complete!"));
        });
    }
}
