using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Services
{
    public interface IDataService<TId, TModel>
    {
        TModel Retrieve(TId id);
        void Save(TId id, TModel model);
    }
}
