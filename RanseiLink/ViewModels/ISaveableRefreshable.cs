namespace RanseiLink.ViewModels;

public interface ISaveable
{
    bool CanSave() => true;
    void Deactivate();
}

public interface IRefreshable
{
    void Refresh();
}

public interface ISaveableRefreshable
{
    bool CanModuleClose() => true;
    void OnModuleClosing();
    void OnModuleOpening();
    bool CanPluginStart() => true;
    void OnPluginStarting();
    void OnPluginCompletedSuccessfully();
    bool CanApplicationClose() => true;
    void OnApplicationClosing();
}
