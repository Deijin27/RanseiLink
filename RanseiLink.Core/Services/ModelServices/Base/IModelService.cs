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
}