using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.GuiCore.Services;
using RanseiLink.PluginModule.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChangelistPlugin;

[Plugin("Changelist", "Deijin", "2.6")]
public class ChangelistPlugin : IPlugin
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

        var options = new ChangelistOptionForm()
        {
            Mods = modDict.Select(i => i.Key).OrderBy(i => i).ToList(),
            UnchangedMod = currentKey,
            ChangedMod = currentKey
        };

        var dialogService = context.Services.Get<IAsyncDialogService>();


        var optionService = context.Services.Get<IAsyncPluginService>();
        do
        {
            if (!await optionService.RequestOptions(options))
            {
                return;
            }

            if (options.UnchangedMod == options.ChangedMod)
            {
                await dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                    "Invalid Options",
                    "The source mod must be different to the destination mod"
                    ));
            }

        } while (options.UnchangedMod == options.ChangedMod);

        var unchangedMod = modDict[options.UnchangedMod];
        var changedMod = modDict[options.ChangedMod];

        var kernelFactory = context.Services.Get<IModServiceGetterFactory>();
        IServiceGetter unchangedServices = context.Services;
        IServiceGetter changedServices = context.Services;

        if (unchangedMod.FolderPath != activeMod.FolderPath)
        {
            unchangedServices = kernelFactory.Create(unchangedMod);
        }
        if (changedMod.FolderPath != activeMod.FolderPath)
        {
            changedServices = kernelFactory.Create(changedMod);
        }

        var changelist = BuildChangelist(options, unchangedServices, changedServices);

        string ext = options.OutputType switch
        {
            OutputType.XML => ".xml",
            OutputType.TSV => ".tsv",
            _ => throw new Exception("Unexpected output type")
        };

        var file = FileUtil.MakeUniquePath(Path.Combine(FileUtil.DesktopDirectory, "changelist" + ext));
        
        if (options.OutputType == OutputType.XML)
        {
            OutputXml(file, changelist);
            var proc = Process.Start("notepad.exe", file);
        }
        else if (options.OutputType == OutputType.TSV)
        {
            OutputTsv(file, changelist);
            await dialogService.ShowMessageBox(MessageBoxSettings.Ok("Generation complete", $"Changelist output to file:\n'{file}'"));
        }

        unchangedServices.Dispose();
        changedServices.Dispose();
    }

    private static void OutputXml(string file, IEnumerable<ChangeInfo> changelist)
    {
        var doc = new XDocument();

        var root = new XElement("changelist");

        foreach (var groupByTypeName in changelist.GroupBy(i => i.TypeName))
        {
            var typeNameElement = new XElement("type", new XAttribute("name", groupByTypeName.Key));

            foreach (var groupByObjectId in groupByTypeName.GroupBy(i => i.ObjectId))
            {
                var objectIdElement = new XElement("object", new XAttribute("name", groupByObjectId.Key));

                foreach (var changeInfo in groupByObjectId)
                {
                    if (!string.IsNullOrEmpty(changeInfo.PropertyName))
                    {
                        var propertyElement = new XElement("property", new XAttribute("name", changeInfo.PropertyName),
                        new XElement("before", changeInfo.OldValue),
                        new XElement("after_", changeInfo.NewValue)
                        );
                        objectIdElement.Add(propertyElement);
                    }
                }

                typeNameElement.Add(objectIdElement);
            }

            root.Add(typeNameElement);
        }

        doc.Add(root);

        doc.Save(file);
    }

    private static void OutputTsv(string file, IEnumerable<ChangeInfo> changelist)
    {
        using var sw = new StreamWriter(file);
        sw.WriteLine("Type\tId\tProperty\tBefore\tAfter");

        foreach (var groupByTypeName in changelist.GroupBy(i => i.TypeName))
        {
            foreach (var groupByObjectId in groupByTypeName.GroupBy(i => i.ObjectId))
            {
                foreach (var changeInfo in groupByObjectId)
                {
                    sw.WriteLine($"{changeInfo.TypeName}\t{changeInfo.ObjectId}\t{changeInfo.PropertyName}\t{changeInfo.OldValue}\t{changeInfo.NewValue}");
                }
            }
        }
    }

    private static IEnumerable<ChangeInfo> BuildChangelist(ChangelistOptionForm options, IServiceGetter unchangedServices, IServiceGetter changedServices)
    {
        List<ChangeInfo> changelist = new();

        if (options.Ability)
        {
            changelist.AddRange(GenericGetChangelist(unchangedServices.Get<IAbilityService>(), changedServices.Get<IAbilityService>()));
        }
        if (options.BaseWarrior)
        {
            changelist.AddRange(GenericGetChangelist(unchangedServices.Get<IBaseWarriorService>(), changedServices.Get<IBaseWarriorService>()));
        }
        if (options.BattleConfigs)
        {
            changelist.AddRange(GenericGetChangelist(unchangedServices.Get<IBattleConfigService>(), changedServices.Get<IBattleConfigService>()));
        }
        if (options.Building)
        {
            changelist.AddRange(GenericGetChangelist(unchangedServices.Get<IBuildingService>(), changedServices.Get<IBuildingService>()));
        }
        if (options.Episode)
        {
            changelist.AddRange(GenericGetChangelist(unchangedServices.Get<IEpisodeService>(), changedServices.Get<IEpisodeService>()));
        }
        if (options.EventSpeaker)
        {
            changelist.AddRange(GenericGetChangelist(unchangedServices.Get<IEventSpeakerService>(), changedServices.Get<IEventSpeakerService>()));
        }
        if (options.Gimmicks)
        {
            changelist.AddRange(GenericGetChangelist(unchangedServices.Get<IGimmickService>(), changedServices.Get<IGimmickService>()));
        }
        if (options.GimmickRange)
        {
            changelist.AddRange(GenericGetBinaryDiff(unchangedServices.Get<IGimmickRangeService>(), changedServices.Get<IGimmickRangeService>()));
        }
        if (options.Item)
        {
            changelist.AddRange(GenericGetChangelist(unchangedServices.Get<IItemService>(), changedServices.Get<IItemService>()));
        }
        if (options.Kingdoms)
        {
            changelist.AddRange(GenericGetChangelist(unchangedServices.Get<IKingdomService>(), changedServices.Get<IKingdomService>()));
        }
        if (options.Move)
        {
            changelist.AddRange(GenericGetChangelist(unchangedServices.Get<IMoveService>(), changedServices.Get<IMoveService>()));
        }
        if (options.MoveRange)
        {
            changelist.AddRange(GenericGetBinaryDiff(unchangedServices.Get<IMoveRangeService>(), changedServices.Get<IMoveRangeService>()));
        }
        if (options.Pokemon)
        {
            changelist.AddRange(GenericGetChangelist(unchangedServices.Get<IPokemonService>(), changedServices.Get<IPokemonService>()));
        }
        if (options.ScenarioPokemon)
        {
            changelist.AddRange(GetScenarioPokemonChangelist(unchangedServices.Get<IScenarioPokemonService>(), changedServices.Get<IScenarioPokemonService>()));
        }
        if (options.ScenarioWarrior)
        {
            changelist.AddRange(GetScenarioWarriorChangelist(unchangedServices.Get<IScenarioWarriorService>(), changedServices.Get<IScenarioWarriorService>()));
        }
        if (options.Text)
        {
            changelist.AddRange(GetTextChangelist(unchangedServices.Get<IMsgBlockService>(), changedServices.Get<IMsgBlockService>()));
        }
        if (options.WarriorSkill)
        {
            changelist.AddRange(GenericGetChangelist(unchangedServices.Get<IWarriorSkillService>(), changedServices.Get<IWarriorSkillService>()));
        }
        if (options.ScenarioBuilding)
        {
            changelist.AddRange(GetScenarioBuildingChangelist(unchangedServices.Get<IScenarioBuildingService>(), changedServices.Get<IScenarioBuildingService>()));
        }

        return changelist;
    }

    private static IEnumerable<ChangeInfo> GenericGetChangelist<TModel>(
        IModelService<TModel> beforeService,
        IModelService<TModel> afterService)
    {
        List<ChangeInfo> changelist = new();

        // get only value types, because they can usually implement value equality, and can be converted to string
        PropertyInfo[] props = typeof(TModel).GetProperties().Where(i => i.PropertyType.IsValueType).ToArray();
        int id = 0;
        foreach (var (beforeObj, afterObj) in beforeService.Enumerate().Zip(afterService.Enumerate()))
        {
            foreach (var prop in props)
            {
                var beforeVal = prop.GetValue(beforeObj);
                var afterVal = prop.GetValue(afterObj);
                if (!beforeVal.Equals(afterVal))
                {
                    changelist.Add(new(typeof(TModel).Name, afterService.IdToName(id), prop.Name, beforeVal.ToString(), afterVal.ToString()));
                }
            }
            id++;
        }
        return changelist;
    }

    /// <summary>
    /// Gets whether two datawrappers differ, without specifics
    /// </summary>
    private static IEnumerable<ChangeInfo> GenericGetBinaryDiff<TModel>(
        IModelService<TModel> beforeService,
        IModelService<TModel> afterService) where TModel : IDataWrapper
    {
        List<ChangeInfo> changelist = new();

        int id = 0;
        foreach (var (beforeObj, afterObj) in beforeService.Enumerate().Zip(afterService.Enumerate()))
        {
            for (int i = 0; i < beforeObj.Data.Length; i++)
            {
                var beforeVal = beforeObj.Data[i];
                var afterVal = afterObj.Data[i];
                if (beforeVal != afterVal)
                {
                    changelist.Add(new(typeof(TModel).Name, afterService.IdToName(id), string.Empty, string.Empty, string.Empty));
                    return changelist;
                }
            }
            id++;
        }
        return changelist;
    }

    private static IEnumerable<ChangeInfo> GetScenarioPokemonChangelist(IScenarioPokemonService beforeService, IScenarioPokemonService afterService)
    {
        List<ChangeInfo> changelist = new();

        PropertyInfo[] props = typeof(ScenarioPokemon).GetProperties().Where(i => i.PropertyType.IsValueType).ToArray();

        foreach (var scenario in EnumUtil.GetValues<ScenarioId>())
        {
            foreach (int i in beforeService.Retrieve((int)scenario).ValidIds())
            {
                var beforeObj = beforeService.Retrieve((int)scenario).Retrieve(i);
                var afterObj = afterService.Retrieve((int)scenario).Retrieve(i);

                foreach (var prop in props)
                {
                    var beforeVal = prop.GetValue(beforeObj);
                    var afterVal = prop.GetValue(afterObj);
                    if (!beforeVal.Equals(afterVal))
                    {
                        changelist.Add(new("ScenarioPokemon", $"Scenario_{scenario}_Id_{i}", prop.Name, beforeVal.ToString(), afterVal.ToString()));
                    }
                }
            }
        }

        return changelist;
    }

    private static IEnumerable<ChangeInfo> GetScenarioWarriorChangelist(IScenarioWarriorService beforeService, IScenarioWarriorService afterService)
    {
        List<ChangeInfo> changelist = new();

        PropertyInfo[] props = typeof(ScenarioWarrior).GetProperties().Where(i => i.PropertyType.IsValueType).ToArray();

        foreach (var scenario in EnumUtil.GetValues<ScenarioId>())
        {
            foreach (int i in beforeService.Retrieve((int)scenario).ValidIds())
            {
                var beforeObj = beforeService.Retrieve((int)scenario).Retrieve(i);
                var afterObj = afterService.Retrieve((int)scenario).Retrieve(i);

                foreach (var prop in props)
                {
                    var beforeVal = prop.GetValue(beforeObj);
                    var afterVal = prop.GetValue(afterObj);
                    if (!beforeVal.Equals(afterVal))
                    {
                        changelist.Add(new("ScenarioWarrior", $"Scenario_{scenario}_Id_{i}", prop.Name, beforeVal.ToString(), afterVal.ToString()));
                    }
                }
            }
        }

        return changelist;
    }

    private static IEnumerable<ChangeInfo> GetScenarioBuildingChangelist(IScenarioBuildingService unchanged, IScenarioBuildingService changed)
    {
        foreach (var id in unchanged.ValidIds())
        {
            var unchangedItem = unchanged.Retrieve(id);
            var changedItem = changed.Retrieve(id);
            foreach (var kingdom in EnumUtil.GetValuesExceptDefaults<KingdomId>())
            {
                for (int slot = 0; slot < ScenarioBuilding.SlotCount; slot++)
                {
                    var unchangedInitExp = unchangedItem.GetInitialExp(kingdom, slot);
                    var changedInitExp = changedItem.GetInitialExp(kingdom, slot);
                    if (unchangedInitExp != changedInitExp)
                    {
                        yield return new ChangeInfo("ScenarioBuilding", $"Kingdom_{kingdom}_Slot_{slot}", "InitExp",
                            unchangedInitExp.ToString(), changedInitExp.ToString());
                    }
                    var unchangedUnk = unchangedItem.GetUnknown2(kingdom, slot);
                    var changedUnk = changedItem.GetUnknown2(kingdom, slot);
                    if (unchangedUnk != changedUnk)
                    {
                        yield return new ChangeInfo("ScenarioBuilding", $"Kingdom_{kingdom}_Slot_{slot}", "Unknown2",
                            unchangedUnk.ToString(), changedUnk.ToString());
                    }
                }
            }
        }
    }

    private static IEnumerable<ChangeInfo> GetTextChangelist(IMsgBlockService unchangedService, IMsgBlockService changedService)
    {
        var changelist = new List<ChangeInfo>();

        for (int i = 0; i < Math.Min(unchangedService.BlockCount, changedService.BlockCount); i++)
        {
            var unchangedBlock = unchangedService.Retrieve(i);
            var changedBlock = changedService.Retrieve(i);

            for (int j = 0; j < Math.Min(unchangedBlock.Count, changedBlock.Count); j++)
            {
                var unchangedMsg = unchangedBlock[j];
                var changedMsg = changedBlock[j];

                bool textChanged = unchangedMsg.Text != changedMsg.Text;
                bool contextChanged = unchangedMsg.Context != changedMsg.Context;
                bool boxConfigChanged = unchangedMsg.BoxConfig != changedMsg.BoxConfig;

                if (textChanged || contextChanged || boxConfigChanged)
                {
                    string objectId = $"Block_{i}_Msg_{j}";

                    if (textChanged)
                    {
                        changelist.Add(new ChangeInfo(
                        TypeName: "Messages",
                        ObjectId: objectId,
                        PropertyName: "Text",
                        OldValue: unchangedMsg.Text,
                        NewValue: changedMsg.Text
                        ));
                    }

                    if (contextChanged)
                    {
                        changelist.Add(new ChangeInfo(
                        TypeName: "Messages",
                        ObjectId: objectId,
                        PropertyName: "Context",
                        OldValue: unchangedMsg.Context,
                        NewValue: changedMsg.Context
                        ));
                    }

                    if (boxConfigChanged)
                    {
                        changelist.Add(new ChangeInfo(
                        TypeName: "Messages",
                        ObjectId: objectId,
                        PropertyName: "BoxConfig",
                        OldValue: unchangedMsg.BoxConfig,
                        NewValue: changedMsg.BoxConfig
                        ));
                    }
                }
            }
        }

        return changelist;
    }

}

public record ChangeInfo(string TypeName, string ObjectId, string PropertyName, string OldValue, string NewValue);
