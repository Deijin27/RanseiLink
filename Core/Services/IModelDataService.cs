
using System;

namespace Core.Services
{
    public interface IModelDataService<TId, TModel>
    {
        TModel Retrieve(TId id);
        void Save(TId id, TModel model);
    }

    public interface IModelDataService<TModel>
    {
        TModel Retrieve();
        void Save(TModel model);
    }

    public interface IModelDataService<TId1, TId2, TModel>
    {
        TModel Retrieve(TId1 id1, TId2 id2);
        void Save(TId1 id1, TId2 id2, TModel model);
    }

    public interface IDisposableModelDataService<TId, TModel> : IDisposable
    {
        TModel Retrieve(TId id);
        void Save(TId id, TModel model);
    }

    public interface IDisposableModelDataService<TId1, TId2, TModel> : IDisposable
    {
        TModel Retrieve(TId1 id1, TId2 id2);
        void Save(TId1 id1, TId2 id2, TModel model);
    }
}
