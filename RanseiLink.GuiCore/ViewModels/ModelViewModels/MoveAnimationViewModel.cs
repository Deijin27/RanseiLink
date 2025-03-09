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

public class MoveAnimationCollectionViewModel : ViewModelBase
{

    public MoveAnimationCollectionViewModel()
    {

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
        
    }


    public ObservableCollection<MoveAnimationViewModel> Items { get; } = [];
}