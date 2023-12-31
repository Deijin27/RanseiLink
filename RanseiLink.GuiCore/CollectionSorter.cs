using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore;

public sealed class CollectionSorter<TItem>(ObservableCollection<TItem> items)
{
    private readonly ObservableCollection<TItem> _items = items;
    private IOrderedEnumerable<TItem>? _itemsSorted;

    public void Clear()
    {
        _itemsSorted = null;
    }

    public void ApplySort()
    {
        if (_itemsSorted != null)
        {
            var sorted = _itemsSorted.ToList();
            for (int i = 0; i < sorted.Count; i++)
            {
                var oldIndex = _items.IndexOf(sorted[i]);
                if (oldIndex != i)
                {
                    _items.Move(oldIndex, i);
                }
            }
        }
    }

    public void OrderBy<TSortable>(Func<TItem, TSortable> sorter)
    {
        if (_itemsSorted == null)
        {
            _itemsSorted = _items.OrderBy(sorter);
        }
        else
        {
            _itemsSorted = _itemsSorted.ThenBy(sorter);
        }
    }

    public void OrderByDescending<TSortable>(Func<TItem, TSortable> sorter)
    {
        if (_itemsSorted == null)
        {
            _itemsSorted = _items.OrderByDescending(sorter);
        }
        else
        {
            _itemsSorted = _itemsSorted.ThenByDescending(sorter);
        }
    }
}
