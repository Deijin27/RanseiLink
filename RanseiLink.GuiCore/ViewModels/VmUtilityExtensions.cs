using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;
public static class VmUtilityExtensions
{
    public static void ResetTo<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        collection.Clear();
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
}
