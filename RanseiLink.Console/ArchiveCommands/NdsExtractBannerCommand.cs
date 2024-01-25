using RanseiLink.Core;
using RanseiLink.Core.RomFs;

namespace RanseiLink.Console.ArchiveCommands;

[Command("nds extract banner", Description = "Extract information from the banner of the rom.")]
public class NdsExtractBannerCommand : ICommand
{
    [CommandParameter(0, Description = "Path of nds file.", Name = "NdsPath")]
    public string NdsPath { get; set; }

    [CommandOption("destinationFolder", 'd', Description = "Optional destination folder to extract to; default is a in the same location as the nds file.")]
    public string DestinationFolder { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (string.IsNullOrEmpty(DestinationFolder))
        {
            DestinationFolder = Path.GetDirectoryName(NdsPath);
        }
        else
        {
            Directory.CreateDirectory(DestinationFolder);
        }

        var xmlInfoOut = FileUtil.MakeUniquePath(Path.Combine(DestinationFolder, "BannerInfo.xml"));
        var imgOut = FileUtil.MakeUniquePath(Path.Combine(DestinationFolder, "BannerImage.png"));

        using IRomFs romFs = new RomFs(NdsPath);

        var banner = romFs.GetBanner();

        console.Output.WriteLine(banner);

        banner.SaveInfoToXml(xmlInfoOut);
        banner.SaveImageToPng(imgOut);

        return default;
    }
}
