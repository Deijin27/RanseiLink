namespace Core.Services
{
    public interface IDataServiceFactory
    {
        IDataService Create(ModInfo modInfo);
    }
}