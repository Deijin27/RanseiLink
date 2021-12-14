namespace RanseiLink.ViewModels;

public interface ISaveable
{
    bool CanSave() => true;
    void Save();
}

public interface IRefreshable
{
    void Refresh();
}

public interface ISaveableRefreshable : ISaveable, IRefreshable
{
}
