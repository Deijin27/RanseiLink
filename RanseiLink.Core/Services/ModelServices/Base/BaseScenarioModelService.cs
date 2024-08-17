using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public abstract class BaseScenarioModelService<TModel> : BaseModelService<TModel> where TModel : BaseDataWindow, new()
    {
        protected BaseScenarioModelService(string dataFile, int minId, int maxId, int defaultId = -1, bool delayReload = false)
            : base(dataFile, minId, maxId, defaultId, delayReload)
        {
        }
        protected virtual TModel NewModel()
        {
            return new TModel();
        }

        protected abstract string IdToRelativePath(int id);

        public override void Reload()
        {
            _cache.Clear();
            for (int id = _minId; id <= _maxId; id++)
            {
                using (var stream = File.OpenRead(Path.Combine(_dataFile, IdToRelativePath(id))))
                {
                    var model = NewModel();
                    model.Read(stream);
                    _cache.Add(model);
                }
            }
        }

        public override void Save()
        {
            for (int id = _minId; id <= _maxId; id++)
            {
                using (var stream = File.OpenWrite(Path.Combine(_dataFile, IdToRelativePath(id))))
                {
                    _cache[id].Write(stream);
                }
            }
        }
    }
}