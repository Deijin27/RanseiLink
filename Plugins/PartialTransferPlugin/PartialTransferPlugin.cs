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
using System.Threading.Tasks;

namespace PartialTransferPlugin;

[Plugin("Partial Transfer", "Deijin", "3.2")]
public class PartialTransferPlugin : IPlugin
{
    public async Task Run(IPluginContext context)
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

        var dialogService = context.Services.Get<IAsyncDialogService>();


        var optionService = context.Services.Get<IAsyncPluginService>();
        do
        {
            if (!await optionService.RequestOptions(options))
            {
                return;
            }

            if (options.SourceMod == options.DestinationMod)
            {
                await dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                    "Invalid Options",
                    "The source mod must be different to the destination mod"
                    ));
            }

        } while (options.SourceMod == options.DestinationMod);
        

        var sourceMod = modDict[options.SourceMod];
        var destinationMod = modDict[options.DestinationMod];

        await dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo(StatusText: "Preparing...", IsIndeterminate:true));

            var kernelFactory = context.Services.Get<IModServiceGetterFactory>();
            IServiceGetter sourceServices = context.Services;
            IServiceGetter destinationServices = context.Services;

            if (destinationMod.FolderPath != activeMod.FolderPath)
            {
                destinationServices = kernelFactory.Create(destinationMod);
            }
            if (sourceMod.FolderPath != activeMod.FolderPath)
            {
                sourceServices = kernelFactory.Create(sourceMod);
            }
            
            List<string> filesToTransfer = new();
            List<string> dirsToTransfer = new();
            List<Action> actionsToDo = new();

            if (options.BattleConfigs)
                filesToTransfer.Add(Constants.BattleConfigRomPath);
            if (options.Episode)
                filesToTransfer.Add(Constants.EpisodeRomPath);
            if (options.Maps)
                dirsToTransfer.Add(Constants.MapFolderPath);
            if (options.MaxLink)
                filesToTransfer.Add(Constants.MaxLinkRomPath);
            if (options.MoveRange)
                filesToTransfer.Add(Constants.MoveRangeRomPath);
            if (options.GimmickRange)
                filesToTransfer.Add(Constants.GimmickRangeRomPath);
            
            if (options.TransferNames)
            {
                // data same in all game codes
                if (options.Ability)
                    filesToTransfer.Add(Constants.AbilityRomPath);
                if (options.Item)
                    filesToTransfer.Add(Constants.ItemRomPath);
                if (options.Move)
                    filesToTransfer.Add(Constants.MoveRomPath);
                if (options.Pokemon)
                    filesToTransfer.Add(Constants.PokemonRomPath);
                if (options.BaseWarrior)
                    filesToTransfer.Add(Constants.BaseBushouRomPath);

                // data varies per game code
                if (sourceMod.GameCode == destinationMod.GameCode)
                {
                    if (options.Building)
                        filesToTransfer.Add(Constants.BuildingRomPath);
                    if (options.EventSpeaker)
                        filesToTransfer.Add(Constants.EventSpeakerRomPath);
                    if (options.Gimmicks)
                        filesToTransfer.Add(Constants.GimmickRomPath);
                    if (options.Kingdoms)
                        filesToTransfer.Add(Constants.KingdomRomPath);
                    if (options.WarriorSkill)
                        filesToTransfer.Add(Constants.WarriorSkillRomPath);
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
                if (options.ScenarioWarrior)
                {
                    filesToTransfer.Add(Constants.ScenarioPokemonPathFromId((int)i));
                    filesToTransfer.Add(Constants.ScenarioWarriorPathFromId((int)i));
                    filesToTransfer.Add(Constants.ScenarioKingdomPathFromId((int)i));
                }
                if (options.ScenarioAppearPokemon)
                    filesToTransfer.Add(Constants.ScenarioAppearPokemonPathFromId((int)i));

                if (options.ScenarioBuilding)
                    filesToTransfer.Add(Constants.ScenarioBuildingPathFromId((int)i));
                    
            }
            if (options.Sprites)
                dirsToTransfer.Add(Constants.GraphicsFolderPath);

            if (sourceMod.GameCode == destinationMod.GameCode)
            {
                if (options.Text)
                {
                    dirsToTransfer.Add(Constants.MsgFolderPath);
                }
            }

            if (options.Banner)
            {
                actionsToDo.Add(TransferBanner(sourceServices.Get<IBannerService>(), destinationServices.Get<IBannerService>()));
            }

            progress.Report(new ProgressInfo(StatusText:"Transferring...", MaxProgress:filesToTransfer.Count+dirsToTransfer.Count+actionsToDo.Count, IsIndeterminate:false));

            int count = 0;
            foreach (var action in actionsToDo)
            {
                action();
                count++;
                progress.Report(new ProgressInfo(Progress: count));
            }
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

            sourceServices.Dispose();
            destinationServices.Dispose();
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

    private Action TransferBanner(IBannerService source, IBannerService destination)
    {
        return () =>
        {
            File.Copy(source.ImagePath, destination.ImagePath, true);
            destination.BannerInfo.JapaneseTitle = source.BannerInfo.JapaneseTitle;
            destination.BannerInfo.EnglishTitle = source.BannerInfo.EnglishTitle;
            destination.BannerInfo.FrenchTitle = source.BannerInfo.FrenchTitle;
            destination.BannerInfo.GermanTitle = source.BannerInfo.GermanTitle;
            destination.BannerInfo.ItalianTitle = source.BannerInfo.ItalianTitle;
            destination.BannerInfo.SpanishTitle = source.BannerInfo.SpanishTitle;
            destination.Save();
        };
    }
}
