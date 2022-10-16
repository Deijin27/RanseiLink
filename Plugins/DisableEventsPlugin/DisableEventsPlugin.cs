using RanseiLink.Core.RomFs;
using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using System.Text;
using static RanseiLink.Core.RomFs.RomFsNameTable;

namespace DisableEventsPlugin;

[Plugin("Disable Events", "Deijin", "1.0")]
public class DisableEventsPlugin : IPlugin
{
    public void Run(IPluginContext context)
    {
        var dialogService = context.Services.Get<IDialogService>();

        var optionService = context.Services.Get<IPluginService>();
        var options = new DisableEventsOptionForm();
        if (!optionService.RequestOptions(options))
        {
            return;
        }

        byte replace = options.Action == ConstOptions.DisableEvents ? (byte)0x31 : (byte)0x30;

        string find64 = "00000064.eve";
        string find65 = "00000065.eve";
        string find64Alt = "10000064.eve";
        string find65Alt = "10000065.eve";

        var result = dialogService.RequestRomFile();
        if (string.IsNullOrEmpty(result))
        {
            return;
        }

        var stream = File.Open(result, FileMode.Open, FileAccess.ReadWrite);
        var bw = new BinaryWriter(stream);
        var br = new BinaryReader(stream);

        var header = new NdsHeader(br);
        var ntOffset = header.FileNameTableOffset;

        var alloc = GetFolderAllocationFromPath(br, ntOffset, "event");

        stream.Position = alloc.Offset + ntOffset;


        // read the first header to get the length of the first name
        var nameHeader = new NameHeader(br.ReadByte());

        bool found64 = false;
        bool found65 = false;

        // 0 as the length marks the end of the names in the folder
        while (nameHeader.Length.ToInt32() != 0)
        {
            if (nameHeader.IsFolder)
            {
                br.ReadUInt16(); // skip folder contents index
            }
            else
            {
                var startOfName = br.BaseStream.Position;
                var name = Encoding.UTF8.GetString(br.ReadBytes(nameHeader.Length.ToInt32()));
                var endOfName = br.BaseStream.Position;

                if (!found64 && (name == find64 || name == find64Alt))
                {
                    found64 = true;
                    stream.Position = startOfName;
                    bw.Write(replace);
                    stream.Position = endOfName;

                }
                else if (!found65 && (name == find65 || name == find65Alt))
                {
                    found65 = true;
                    stream.Position = startOfName;
                    bw.Write(replace);
                    stream.Position = endOfName;
                }
            }

            if (found64 && found65)
            {
                break;
            }

            nameHeader = new NameHeader(br.ReadByte());
        }

        stream.Dispose();

        if (!found64 && !found65)
        {
            dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                "Failed to locate event files", "", MessageBoxType.Error
                ));
        }
        else
        {
            dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                "Success", "The events have been " + options.Action == ConstOptions.DisableEvents ? "disabled" : "enabled"
                ));
        }
    }
}
