using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public abstract class BaseNewableDataModelService<TModel> : BaseDataModelService<TModel> where TModel : BaseDataWindow, new()
    {
        protected BaseNewableDataModelService(string dataFile, int minId, int maxId, int defaultId = -1)
            : base(dataFile, minId, maxId, () => new TModel(), defaultId)
        {
        }
    }
}