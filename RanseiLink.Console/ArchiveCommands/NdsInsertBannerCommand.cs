using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.RomFs;
using System.Threading.Tasks;

namespace RanseiLink.Console.ArchiveCommands;

[Command("nds insert banner", Description = "Insert banner information into the rom.")]
public class NdsInsertBannerCommand : ICommand
{
    [CommandParameter(0, Description = "Path of nds file.", Name = "NdsPath")]
    public string NdsPath { get; set; }

    [CommandOption("bannerInfoXml", 'x', Description = "Path of xml file containing banner info")]
    public string BannerInfoPath { get; set; }

    [CommandOption("bannerImage", 'i', Description = "Path of png file containing banner image")]
    public string BannerImagePath { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (BannerImagePath == null && BannerInfoPath == null)
        {
            console.Output.WriteLine("Please also provide a bannerInfoXml and/or bannerImage");
            return default;
        }

        using IRomFs romFs = new RomFs(NdsPath);

        var banner = romFs.GetBanner();

        if (BannerInfoPath != null)
        {
            if (!banner.TryLoadInfoFromXml(BannerInfoPath, out string failureReason))
            {
                console.Output.WriteLine(failureReason);
                return default;
            }
        }
       
        if (BannerImagePath != null)
        {
            if (!banner.TryLoadImageFromPng(BannerImagePath, out string failureReason))
            {
                console.Output.WriteLine(failureReason);
                return default;
            }
        }

        romFs.SetBanner(banner);

        return default;
    }
}