using GongSolutions.Wpf.DragDrop;

namespace RanseiLink.ViewModels;

public class DragHandlerPro : DefaultDragHandler
{
    public override bool CanStartDrag(IDragInfo dragInfo)
    {
        if (dragInfo.SourceItem is SwMiniViewModel)
        {
            return base.CanStartDrag(dragInfo);
        }
        else
        {
            return false;
        }
    }
}

public class DropHandlerPro : DefaultDropHandler
{
    public override void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.InsertIndex == 0)
        {
            return;
        }
        base.DragOver(dropInfo);
    }
}
