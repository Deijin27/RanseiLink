using GongSolutions.Wpf.DragDrop;
using System;
using System.Windows;

namespace RanseiLink.DragDrop;

public class FolderDropHandler : IDropTarget
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
                    dropInfo.Effects = DragDropEffects.All;
                    return;
                }
            }
        }
        dropInfo.Effects = DragDropEffects.None;
    }

    public void Drop(IDropInfo dropInfo)
    {
        string folder = ((DataObject)dropInfo.Data).GetFileDropList()[0];
        FolderDropped?.Invoke(folder);
    }

    public event Action<string> FolderDropped;
}
