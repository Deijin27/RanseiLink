using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Windows.Markup;

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

        var sb = new StringBuilder();
        for (var i = 0; i < 255; i++)
        {
            //sb.Append(i.ToString() + ": ");
            int matches = 0;
            foreach (var id in moveAnimationService.ValidIds())
            {
                var model = moveAnimationService.Retrieve(id);
                if ((int)model.Animation == i)
                {
                    matches++;
                    sb.Append($"{(MoveAnimationId)id},");
                }
            }
            if (matches > 1)
            {
                throw new Exception("Dupe!");
            }
            if (matches == 0)
            {
                sb.Append("Black_" + i.ToString() + ",");
            }
            sb.AppendLine();

        }
        File.WriteAllText(@"C:\Users\Mia\Desktop\anims.txt", sb.ToString());
    }


    public ObservableCollection<MoveAnimationViewModel> Items { get; } = [];
}