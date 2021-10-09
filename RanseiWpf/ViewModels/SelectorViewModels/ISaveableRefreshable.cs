namespace RanseiWpf.ViewModels
{
    public interface ISaveable
    {
        void Save();
    }

    public interface IRefreshable
    {
        void Refresh();
    }

    public interface ISaveableRefreshable : ISaveable, IRefreshable
    {
    }
}
