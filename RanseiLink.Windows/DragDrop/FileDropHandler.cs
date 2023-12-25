using GongSolutions.Wpf.DragDrop;
using RanseiLink.GuiCore.DragDrop;
using System.Windows;

namespace RanseiLink.DragDrop;

public class FileDropHandler(params string[] allowedExtensions) : IFileDropHandler, IDropTarget
{
    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is DataObject data)
        {
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                var drop = data.GetFileDropList();
                if (drop.Count == 1)
                {
                    var file = drop[0];
                    if (allowedExtensions.Contains(System.IO.Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                    {
                        dropInfo.Effects = DragDropEffects.All;
                        return;
                    }
                }
            }
        }
        dropInfo.Effects = DragDropEffects.None;
    }

    public void Drop(IDropInfo dropInfo)
    {
        string file = ((DataObject)dropInfo.Data).GetFileDropList()[0];
        FileDropped?.Invoke(file);
    }

    public event Action<string> FileDropped;
}
