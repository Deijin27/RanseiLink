using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.PluginModule.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PartialTransferPlugin;

[Plugin("Partial Transfer", "Deijin", "3.2")]
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
                dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                    "Invalid Options",
                    "The source mod must be different to the destination mod"
                    ));
            }

        } while (options.SourceMod == options.DestinationMod);
        

        var sourceMod = modDict[options.SourceMod];
        var destinationMod = modDict[options.DestinationMod];

        dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo(statusText: "Preparing...", isIndeterminate:true));

            var kernelFactory = context.Services.Get<IModServiceGetterFactory>();
            bool shouldDisposeSource = false;
            bool shouldDisposeDestination = false;
            IServiceGetter sourceServices = context.Services;
            IServiceGetter destinationServices = context.Services;
            IServiceGetter requiresReload = null;

            if (destinationMod.FolderPath != activeMod.FolderPath)
            {
                shouldDisposeDestination = true;
                destinationServices = kernelFactory.Create(destinationMod);
            }
            else
            {
                requiresReload = destinationServices;
            }
            if (sourceMod.FolderPath != activeMod.FolderPath)
            {
                shouldDisposeSource = true;
                sourceServices = kernelFactory.Create(sourceMod);
            }
            
            List<(string file, IModelService serviceToReload)> filesToTransfer = new();
            List<(string folder, IModelService serviceToReload)> dirsToTransfer = new();
            List<Action> actionsToDo = new();

            if (options.BattleConfigs)
                filesToTransfer.Add((Constants.BattleConfigRomPath, requiresReload?.Get<IBattleConfigService>()));
            if (options.Episode)
                filesToTransfer.Add((Constants.EpisodeRomPath, requiresReload?.Get<IEpisodeService>()));
            if (options.Maps)
                dirsToTransfer.Add((Constants.MapFolderPath, null));
            if (options.MaxLink)
                filesToTransfer.Add((Constants.MaxLinkRomPath, requiresReload?.Get<IMaxLinkService>()));
            if (options.MoveRange)
                filesToTransfer.Add((Constants.MoveRangeRomPath, requiresReload?.Get<IMoveRangeService>()));

            if (options.TransferNames)
            {
                // data same in all game codes
                if (options.Ability)
                    filesToTransfer.Add((Constants.AbilityRomPath, requiresReload?.Get<IAbilityService>()));
                if (options.Item)
                    filesToTransfer.Add((Constants.ItemRomPath, requiresReload?.Get<IItemService>()));
                if (options.Move)
                    filesToTransfer.Add((Constants.MoveRomPath, requiresReload?.Get<IMoveService>()));
                if (options.Pokemon)
                    filesToTransfer.Add((Constants.PokemonRomPath, requiresReload?.Get<IPokemonService>()));
                if (options.BaseWarrior)
                    filesToTransfer.Add((Constants.BaseBushouRomPath, requiresReload?.Get<IBaseWarriorService>()));

                // data varies per game code
                if (sourceMod.GameCode == destinationMod.GameCode)
                {
                    if (options.Building)
                        filesToTransfer.Add((Constants.BuildingRomPath, requiresReload?.Get<IBuildingService>()));
                    if (options.EventSpeaker)
                        filesToTransfer.Add((Constants.EventSpeakerRomPath, requiresReload?.Get<IEventSpeakerService>()));
                    if (options.Gimmicks)
                        filesToTransfer.Add((Constants.GimmickRomPath, requiresReload?.Get<IGimmickService>()));
                    if (options.Kingdoms)
                        filesToTransfer.Add((Constants.KingdomRomPath, requiresReload?.Get<IKingdomService>()));
                    if (options.WarriorSkill)
                        filesToTransfer.Add((Constants.WarriorSkillRomPath, requiresReload?.Get<IWarriorSkillService>()));
                }
            }
            else
            {
                // data same in all game codes
                if (options.Ability)
                    actionsToDo.Add(CopyDataExceptName(sourceServices.Get<IAbilityService>(), destinationServices.Get<IAbilityService>()));
                if (options.Item)
                    actionsToDo.Add(CopyDataExceptName(sourceServices.Get<IItemService>(), destinationServices.Get<IItemService>()));
                if (options.Move)
                    actionsToDo.Add(CopyDataExceptName(sourceServices.Get<IMoveService>(), destinationServices.Get<IMoveService>()));
                if (options.Pokemon)
                    actionsToDo.Add(CopyDataExceptName(sourceServices.Get<IPokemonService>(), destinationServices.Get<IPokemonService>()));
                if (options.BaseWarrior)
                    actionsToDo.Add(CopyDataExceptName(sourceServices.Get<IBaseWarriorService>(), destinationServices.Get<IBaseWarriorService>()));

                // data varies per game code
                if (sourceMod.GameCode == destinationMod.GameCode)
                {
                    if (options.Building)
                        actionsToDo.Add(CopyDataExceptName(sourceServices.Get<IBuildingService>(), destinationServices.Get<IBuildingService>()));
                    if (options.EventSpeaker)
                        actionsToDo.Add(CopyDataExceptName(sourceServices.Get<IEventSpeakerService>(), destinationServices.Get<IEventSpeakerService>()));
                    if (options.Gimmicks)
                        actionsToDo.Add(CopyDataExceptName(sourceServices.Get<IGimmickService>(), destinationServices.Get<IGimmickService>()));
                    if (options.Kingdoms)
                        actionsToDo.Add(CopyDataExceptName(sourceServices.Get<IKingdomService>(), destinationServices.Get<IKingdomService>()));
                    if (options.WarriorSkill)
                        actionsToDo.Add(CopyDataExceptName(sourceServices.Get<IWarriorSkillService>(), destinationServices.Get<IWarriorSkillService>()));
                }
            }

            foreach (var i in EnumUtil.GetValues<ScenarioId>())
            {
                if (options.ScenarioPokemon)
                    filesToTransfer.Add((Constants.ScenarioPokemonPathFromId((int)i), null));
                if (options.ScenarioWarrior)
                    filesToTransfer.Add((Constants.ScenarioWarriorPathFromId((int)i), null));
                if (options.ScenarioAppearPokemon)
                    filesToTransfer.Add((Constants.ScenarioAppearPokemonPathFromId((int)i), null));
                if (options.ScenarioKingdom)
                    filesToTransfer.Add((Constants.ScenarioKingdomPathFromId((int)i), null));
            }
            if (options.ScenarioPokemon)
                requiresReload?.Get<IScenarioPokemonService>().Reload();
            if (options.ScenarioWarrior)
                requiresReload?.Get<IScenarioWarriorService>().Reload();
            if (options.ScenarioAppearPokemon)
                requiresReload?.Get<IScenarioAppearPokemonService>().Reload();
            if (options.ScenarioKingdom)
                requiresReload?.Get<IScenarioKingdomService>().Reload();

            if (options.Sprites)
                dirsToTransfer.Add((Constants.GraphicsFolderPath, null));

            if (sourceMod.GameCode == destinationMod.GameCode)
            {
                if (options.Text)
                {
                    dirsToTransfer.Add((Constants.MsgFolderPath, null));
                    //requiresReload.Get<ICachedMsgBlockService>().RebuildCache();
                    // ^^^ requires ranseilink update to make the msggridviewmodel update correctly.
                }
            }

            progress.Report(new ProgressInfo(statusText:"Transferring...", maxProgress:filesToTransfer.Count+dirsToTransfer.Count+actionsToDo.Count, isIndeterminate:false));

            int count = 0;
            foreach (var action in actionsToDo)
            {
                action();
                count++;
                progress.Report(new ProgressInfo(progress: count));
            }
            foreach (var (file, serviceToReload) in filesToTransfer)
            {
                string sourcePath = Path.Combine(sourceMod.FolderPath, file);
                string destinationPath = Path.Combine(destinationMod.FolderPath, file);
                File.Copy(sourcePath, destinationPath, true);
                serviceToReload?.Reload();
                count++;
                progress.Report(new ProgressInfo(progress:count));
            }
            foreach(var (dir, serviceToReload) in dirsToTransfer)
            {
                string sourcePath = Path.Combine(sourceMod.FolderPath, dir);
                string destinationPath = Path.Combine(destinationMod.FolderPath, dir);
                if (Directory.Exists(destinationPath))
                {
                    Directory.Delete(destinationPath, true);
                }
                Directory.CreateDirectory(destinationPath);
                FileUtil.CopyFilesRecursively(sourcePath, destinationPath);
                serviceToReload?.Reload();
                count++;
                progress.Report(new ProgressInfo(progress: count));
            }
            progress.Report(new ProgressInfo(statusText:"Transfer Complete!"));

            if (shouldDisposeSource)
            {
                sourceServices.Dispose();
            }
            if (shouldDisposeDestination)
            {
                destinationServices.Dispose();
            }
        });
    }

    private static Action CopyDataExceptName<TModel>(IModelService<TModel> sourceService, IModelService<TModel> destinationService) where TModel : IDataWrapper
    {
        return () =>
        {
            var modelType = typeof(TModel);
            var nameProperty = modelType.GetProperty("Name");
            foreach (var i in sourceService.ValidIds())
            {
                var sourceModel = sourceService.Retrieve(i);
                var destinationModel = destinationService.Retrieve(i);
                var destName = nameProperty.GetValue(destinationModel);
                var sourceData = sourceModel.Data;
                var destData = destinationModel.Data;
                if (sourceData.Length != destData.Length)
                {
                    throw new Exception("source and destination data not of same length");
                }
                sourceData.CopyTo(destData, 0);
                nameProperty.SetValue(destinationModel, destName);
            }
            destinationService.Save();
        };
    }
}
