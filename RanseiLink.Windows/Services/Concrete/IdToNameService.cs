using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Windows.ViewModels;
using System.Collections.Generic;

namespace RanseiLink.Windows.Services.Concrete;

public class IdToNameService : IIdToNameService
{
    private readonly IServiceGetter _modServiceGetter;
    public IdToNameService(IServiceGetter serviceGetter)
    {
        _modServiceGetter = serviceGetter;
    }

    public List<SelectorComboBoxItem> GetComboBoxItemsExceptDefault<TService>() where TService : IModelService
    {
        return _modServiceGetter.Get<TService>().GetComboBoxItemsExceptDefault();
    }

    public List<SelectorComboBoxItem> GetComboBoxItemsPlusDefault<TService>() where TService : IModelService
    {
        return _modServiceGetter.Get<TService>().GetComboBoxItemsPlusDefault();
    }

    public string IdToName<TService>(int id) where TService : IModelService
    {
        return _modServiceGetter.Get<TService>().IdToName(id);
    }
}