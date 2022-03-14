using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace TextChangelistPlugin;

[Plugin("Text Changelist", "Deijin", "1.0")]
public class TextChangelistPlugin : IPlugin
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

        var options = new TextChangelistOptionForm()
        {
            Mods = modDict.Select(i => i.Key).OrderBy(i => i).ToList(),
            UnchangedMod = currentKey,
            ChangedMod = currentKey
        };

        var dialogService = context.ServiceContainer.Resolve<IDialogService>();


        var optionService = context.ServiceContainer.Resolve<IPluginService>();
        do
        {
            if (!optionService.RequestOptions(options))
            {
                return;
            }

            if (options.UnchangedMod == options.ChangedMod)
            {
                dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    "Invalid Options",
                    "The source mod must be different to the destination mod"
                    ));
            }

        } while (options.UnchangedMod == options.ChangedMod);

        var sb = new StringBuilder();

        var unchangedMod = modDict[options.UnchangedMod];
        var changedMod = modDict[options.ChangedMod];

        var serviceFactory = context.ServiceContainer.Resolve<DataServiceFactory>();

        var unchangedService = serviceFactory(unchangedMod).Msg;
        var changedService = serviceFactory(changedMod).Msg;

        for (int i = 0; i < unchangedService.BlockCount; i++)
        {
            var unchangedBlock = unchangedService.Retrieve(i);
            var changedBlock = changedService.Retrieve(i);

            for (int j = 0; j < unchangedBlock.Count; j++)
            {
                var unchangedMsg = unchangedBlock[j];
                var changedMsg = changedBlock[j];

                bool textChanged = options.IncludeText && unchangedMsg.Text != changedMsg.Text;
                bool contextChanged = options.IncludeContext && unchangedMsg.Context != changedMsg.Context;
                bool boxConfigChanged = options.IncludeBoxConfig && unchangedMsg.BoxConfig != changedMsg.BoxConfig;

                if (textChanged || contextChanged || boxConfigChanged)
                {
                    sb.AppendLine($"\nBlock {i} Msg {j}: ------------------------------------------------------------------------------\n");

                    if (textChanged)
                    {
                        sb.AppendLine($"Text Before: \"{unchangedMsg.Text}\"");
                        sb.AppendLine($"Text After : \"{changedMsg.Text}\"");
                    }
                    if (contextChanged)
                    {
                        sb.AppendLine($"Context Before: \"{unchangedMsg.Context}\"");
                        sb.AppendLine($"Context After : \"{changedMsg.Context}\"");
                    }
                    if (boxConfigChanged)
                    {
                        sb.AppendLine($"BoxConfig Before: \"{unchangedMsg.BoxConfig}\"");
                        sb.AppendLine($"BoxConfig After : \"{changedMsg.BoxConfig}\"");
                    }
                }
            }
        }

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, sb.ToString());
        var proc = Process.Start("notepad.exe", tempFile);
        // Wait for idle stopped working after windows 11 notpad update, so have to do this hack
        Thread.Sleep(1000);
        File.Delete(tempFile);

    }
}
