
using Core.Nds;

namespace Core.Services
{
    public interface ICoreAppServices
    {
        IDataService DataService(ModInfo mod);
        ISettingsService Settings { get; }
        IModService ModService { get; }
        string RootFolder { get; }
        INds Nds(string path);
    }
}
