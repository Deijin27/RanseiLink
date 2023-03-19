using RanseiLink.Core.RomFs;
using System.IO;

namespace RanseiLink.Core.Services;

public interface IBannerService
{
    BannerInfo BannerInfo { get; }
    void Save();
    string ImagePath { get; }
    void SetImage(string file);
}

public class BannerService : IBannerService
{
    private readonly string _infoXmlPath;

    public BannerService(ModInfo mod)
    {
        _infoXmlPath = Path.Combine(mod.FolderPath, Constants.BannerInfoFile);
        ImagePath = Path.Combine(mod.FolderPath, Constants.BannerImageFile);

        BannerInfo = GetBannerInfo();
    }
    private BannerInfo GetBannerInfo()
    {
        var info = new BannerInfo();
        if (!(File.Exists(_infoXmlPath) && info.TryLoadInfoFromXml(_infoXmlPath).IsSuccess))
        {
            // set defaults if file is not present.
            info.SetAllTitles("Pokémon Conquest\nNintendo");
        }
        return info;
    }

    public BannerInfo BannerInfo { get; }

    public string ImagePath { get; }

    public void SetImage(string file)
    {
        File.Copy(file, ImagePath, true);
    }

    public void Save()
    {
        BannerInfo.SaveInfoToXml(_infoXmlPath);
    }
}
