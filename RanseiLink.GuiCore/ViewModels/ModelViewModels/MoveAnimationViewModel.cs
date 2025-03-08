using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Xml.Linq;

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

        //Dictionary<int, string>? _moveAnimationCache = [];
        ////var _moveAnimationFile = @"C:\Users\Mia\source\repos\RanseiLink\RanseiLink.Windows\External\MoveAnimations.txt";
        //var _moveAnimationFile = @"C:\Users\Mia\source\repos\RanseiLink\RanseiLink.Windows\External\MoveMovementAnimations.txt";
        //int count = 0;
        //foreach (string line in File.ReadAllLines(_moveAnimationFile))
        //{
        //    _moveAnimationCache[count] = line;
        //    count++;
        //}

        //var el = new XElement("ExternalCollection");
        //Dictionary<int, string>? coll = _moveAnimationCache; // [];
        ////for (var i = 0; i < 255; i++)
        ////{
        ////    foreach (var id in moveAnimationService.ValidIds())
        ////    {
        ////        var model = moveAnimationService.Retrieve(id);
        ////        if ((int)model.Animation == i)
        ////        {
        ////            coll[i] = _moveAnimationCache[id];
        ////        }
        ////    }

        ////}

        

        //foreach (var (key, value) in coll.OrderBy(x => x.Key))
        //{
        //    el.Add(new XElement("Item", new XAttribute("Key", key), new XAttribute("Value", value)));
        //}
        //new XDocument(el).Save(@"C:\Users\Mia\Desktop\outtt.xml");
    }


    public ObservableCollection<MoveAnimationViewModel> Items { get; } = [];
}