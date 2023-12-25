#nullable enable

using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.Services.Concrete;

public class IdToNameService(IServiceGetter serviceGetter) : IIdToNameService
{
    public List<SelectorComboBoxItem> GetComboBoxItemsExceptDefault<TService>() where TService : IModelService
    {
        return serviceGetter.Get<TService>().GetComboBoxItemsExceptDefault();
    }

    public List<SelectorComboBoxItem> GetComboBoxItemsPlusDefault<TService>() where TService : IModelService
    {
        return serviceGetter.Get<TService>().GetComboBoxItemsPlusDefault();
    }

    public string IdToName<TService>(int id) where TService : IModelService
    {
        return serviceGetter.Get<TService>().IdToName(id);
    }
}