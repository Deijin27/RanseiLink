using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PartialTransferPlugin;

[Plugin("Partial Transfer", "Deijin", "2.0")]
public class PartialTransferPlugin : IPlugin
{
    public void Run(IPluginContext context)
    {
        Dictionary<string, ModInfo> modDict = new();
        var modService = context.ServiceContainer.Resolve<IModManager>();
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
        string currentKey = modDict.First(i => i.Value.FolderPath == context.ActiveMod.FolderPath).Key;

        var options = new PartialTransferOptionForm()
        {
            Mods = modDict.Select(i => i.Key).OrderBy(i => i).ToList(),
            SourceMod = currentKey,
            DestinationMod = currentKey
        };

        var dialogService = context.ServiceContainer.Resolve<IDialogService>();


        var optionService = context.ServiceContainer.Resolve<IPluginService>();
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
        if (options.Gimmicks)
            filesToTransfer.Add(Constants.GimmickRomPath);
        if (options.Item)
            filesToTransfer.Add(Constants.ItemRomPath);
        if (options.Kingdoms)
            filesToTransfer.Add(Constants.KingdomRomPath);
        if (options.Maps)
            dirsToTransfer.Add(Constants.MapFolderPath);
        if (options.MaxLink)
            filesToTransfer.Add(Constants.BaseBushouMaxSyncTableRomPath);
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
                filesToTransfer.Add(Constants.ScenarioPokemonPathFromId(i));
            if (options.ScenarioWarrior)
                filesToTransfer.Add(Constants.ScenarioWarriorPathFromId(i));
            if (options.ScenarioAppearPokemon)
                filesToTransfer.Add(Constants.ScenarioAppearPokemonPathFromId(i));
            if (options.ScenarioKingdom)
                filesToTransfer.Add(Constants.ScenarioKingdomPathFromId(i));
        }

        if (options.Sprites)
            dirsToTransfer.Add(Constants.GraphicsFolderPath);
        if (options.Text)
            dirsToTransfer.Add(Constants.MsgFolderPath);
        
        
        dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo(StatusText:"Transferring...", MaxProgress:filesToTransfer.Count+dirsToTransfer.Count));

            int count = 0;
            foreach (var file in filesToTransfer)
            {
                string sourcePath = Path.Combine(sourceMod.FolderPath, file);
                string destinationPath = Path.Combine(destinationMod.FolderPath, file);
                File.Copy(sourcePath, destinationPath, true);
                count++;
                progress.Report(new ProgressInfo(Progress:count));
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
                progress.Report(new ProgressInfo(Progress: count));
            }

            progress.Report(new ProgressInfo(StatusText:"Transfer Complete!"));
        });
    }
}
