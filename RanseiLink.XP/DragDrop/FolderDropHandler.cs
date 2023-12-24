using Avalonia.Input;
using Avalonia.Platform.Storage;
using RanseiLink.GuiCore.DragDrop;
using System;
using System.Linq;
#nullable enable

namespace RanseiLink.XP.DragDrop;

public class FolderDropHandler : IFolderDropHandler, IDropTarget
{
    public event Action<string>? FolderDropped;

    public DragDropEffects DragOver(DragEventArgs dropInfo)
    {
        var local = dropInfo.Data.GetFiles()?.FirstOrDefault()?.TryGetLocalPath();
        if (string.IsNullOrEmpty(local))
        {
            return DragDropEffects.None;
        }
        if (!System.IO.Directory.Exists(local))
        {
            return DragDropEffects.None;
        }
        return DragDropEffects.Copy;
    }

    public void Drop(DragEventArgs dropInfo)
    {
        var local = dropInfo.Data.GetFiles()?.FirstOrDefault()?.TryGetLocalPath();
        if (string.IsNullOrEmpty(local))
        {
            return;
        }
        FolderDropped?.Invoke(local);
    }
}