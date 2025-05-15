using RanseiLink.Core.RomFs;

namespace RanseiLink.Core.Services;

public interface IBannerService
{
    BannerInfo BannerInfo { get; }
    void Save();
    string ImagePath { get; }
    void SetImage(string file);
    event Action ImageSet;
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

    public event Action? ImageSet;

    public void SetImage(string file)
    {
        File.Copy(file, ImagePath, true);
        ImageSet?.Invoke();
    }

    public void Save()
    {
        BannerInfo.SaveInfoToXml(_infoXmlPath);
    }
}
