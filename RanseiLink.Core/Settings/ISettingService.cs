namespace RanseiLink.Core.Settings
{
    public interface ISettingService
    {
        TSetting Get<TSetting>() where TSetting : ISetting, new();
        void Save();
    }
}