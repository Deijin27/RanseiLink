using System;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IModelService
    {
        /// <summary>
        /// Returns model downcast to object for given id. Error if not found
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        object RetrieveObject(int id);

        /// <summary>
        /// Returns true if the id is in the allowed range
        /// </summary>
        bool ValidateId(int id);

        /// <summary>
        /// Discards the current cache, and re-populates it
        /// </summary>
        void Reload();

        /// <summary>
        /// Saves the current contents of the cache
        /// </summary>
        void Save();

        /// <summary>
        /// Gets name corresponding to the id
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        string IdToName(int id);

        IEnumerable<int> ValidIds();

        bool TryGetDefaultId(out int defaultId);
    }

    public interface IModelService<TModel> : IModelService
    {
        /// <summary>
        /// Returns model for given id. Error if not found
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        TModel Retrieve(int id);

        /// <summary>
        /// Enumerates the models in ID order
        /// </summary>
        IEnumerable<TModel> Enumerate();
    }

    public abstract class BaseModelService<TModel> : IModelService<TModel>
    {
        protected readonly string _dataFile;
        protected readonly int _minId;
        protected readonly int _maxId;
        protected readonly int _defaultId;
        protected readonly bool _hasDefaultId;
        protected readonly List<TModel> _cache = new List<TModel>();

        protected BaseModelService(string dataFile, int minId, int maxId, int defaultId, bool delayReload = false)
        {
            _dataFile = dataFile;
            _minId = minId;
            _maxId = maxId;
            _defaultId = defaultId;
            _hasDefaultId = true;
            if (!delayReload)
            {
                Reload();
            }
        }

        protected BaseModelService(string dataFile, int minId, int maxId, bool delayReload = false)
        {
            _dataFile = dataFile;
            _minId = minId;
            _maxId = maxId;
            _hasDefaultId = false;
            if (!delayReload)
            {
                Reload();
            }
        }

        public bool ValidateId(int id)
        {
            return id >= _minId && id <= _maxId;
        }

        public TModel Retrieve(int id)
        {
            if (!ValidateId(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            return _cache[id];
        }

        public object RetrieveObject(int id) => Retrieve(id)!;

        public abstract string IdToName(int id);

        public IEnumerable<int> ValidIds()
        {
            return Enumerable.Range(_minId, _maxId - _minId + 1);
        }

        public IEnumerable<TModel> Enumerate() => _cache;

        public abstract void Reload();
        public abstract void Save();

        public bool TryGetDefaultId(out int defaultId)
        {
            defaultId = _defaultId;
            return _hasDefaultId;
        }
    } 
}