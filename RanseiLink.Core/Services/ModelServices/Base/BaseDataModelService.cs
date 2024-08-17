using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices
{
    public abstract class BaseDataModelService<TModel> : BaseModelService<TModel> where TModel : BaseDataWindow
    {
        private readonly Func<TModel> _newModel;

        protected BaseDataModelService(string dataFile, int minId, int maxId, Func<TModel> newModel, int defaultId = -1)
            : base(dataFile, minId, maxId, defaultId, true)
        {
            _newModel = newModel;
            Reload();
        }

        public virtual void PostLoad(Stream stream)
        {

        }

        public virtual void PostSave(Stream stream)
        {

        }

        public override void Reload()
        {
            _cache.Clear();
            using (var stream = File.OpenRead(_dataFile))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    var model = _newModel();
                    model.Read(stream);
                    _cache.Add(model);
                }
                PostLoad(stream);
            }
        }

        public override void Save()
        {
            using (var stream = File.OpenWrite(_dataFile))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    _cache[id].Write(stream);
                }
                PostSave(stream);
            }
        }
    }
}