using Avalonia.Input;
using Avalonia.Platform.Storage;
using RanseiLink.GuiCore.DragDrop;
using System;
using System.Linq;
#nullable enable

namespace RanseiLink.XP.DragDrop;

public class FileDropHandler(params string[] allowedExtensions) : IFileDropHandler, IDropTarget
{
    public event Action<string>? FileDropped;

    public DragDropEffects DragOver(DragEventArgs dropInfo)
    {
        var local = dropInfo.Data.GetFiles()?.FirstOrDefault()?.TryGetLocalPath();
        if (string.IsNullOrEmpty(local))
        {
            return DragDropEffects.None;
        }
        var ext = System.IO.Path.GetExtension(local);
        if (!allowedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
        {
            return DragDropEffects.None;
        }
        if (!System.IO.File.Exists(local))
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
        FileDropped?.Invoke(local);
    }
}
