
namespace Core.Services.Concrete
{
    public class DataServiceFactory : IDataServiceFactory
    {
        public IDataService Create(ModInfo modInfo)
        {
            return new DataService(modInfo);
        }
    }
}
