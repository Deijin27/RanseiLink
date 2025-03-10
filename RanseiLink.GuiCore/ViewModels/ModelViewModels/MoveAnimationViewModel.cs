using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;
public partial class MoveAnimationViewModel()
{
    public void SetModel(MoveAnimationId id, MoveAnimation model)
    {
        _id = id;
        _model = model;
        RaiseAllPropertiesChanged();
    }
}


public enum MovementAnimationSortMode
{
    Id,
    AnimationId,
    AnimationName,
    SoundId,
    SoundName
}

public class MoveAnimationCollectionViewModel : ViewModelBase
{
    public MoveAnimationCollectionViewModel()
    {
        SortItems = EnumUtil.GetValues<MovementAnimationSortMode>().ToList();
        Items = [];
        _sorter = new(Items);
    }

    public void Init(IMoveAnimationService moveAnimationService)
    {
        Items.Clear();
        foreach (var id in moveAnimationService.ValidIds())
        {
            var model = moveAnimationService.Retrieve(id);
            var vm = new MoveAnimationViewModel();
            vm.SetModel((MoveAnimationId)id, model);
            Items.Add(vm);
        }
        Sort();
    }

    private readonly CollectionSorter<MoveAnimationViewModel> _sorter;

    public ObservableCollection<MoveAnimationViewModel> Items { get; }

    private void Sort()
    {
        var mode = _selectedSortItem;
        _sorter.Clear();

        if (mode == MovementAnimationSortMode.Id)
        {
            // default
        }
        else if (mode == MovementAnimationSortMode.AnimationId)
        {
            _sorter.OrderBy(x => x.Animation);
        }
        else if (mode == MovementAnimationSortMode.AnimationName)
        {
            _sorter.OrderBy(x => x.Animation.ToString());
        }
        else if (mode == MovementAnimationSortMode.SoundId)
        {
            _sorter.OrderBy(x => x.Sound);
        }
        else if (mode == MovementAnimationSortMode.SoundName)
        {
            _sorter.OrderBy(x => x.Sound.ToString());
        }

        // this is always present, either for ID-only sorting
        // or as a "ThenBy" sort to give a consistent order to Name and MaxLinkValue sort value conflicts
        _sorter.OrderBy(x => x.Id);
        _sorter.ApplySort();
    }

    public List<MovementAnimationSortMode> SortItems { get; }

    private static MovementAnimationSortMode _selectedSortItem = MovementAnimationSortMode.Id;
    public MovementAnimationSortMode SelectedSortItem
    {
        get => _selectedSortItem;
        set
        {
            if (SetProperty(ref _selectedSortItem, value))
            {
                Sort();
            }
        }
    }
}