using Avalonia.Input;
using Avalonia.Platform.Storage;
using System;
using System.Linq;
#nullable enable

namespace RanseiLink.XP.DragDrop;

public class FileDropHandler : IDropTarget
{
    private readonly string[] _allowedExtensions;

    public event Action<string>? FileDropped;

    public FileDropHandler(params string[] allowedExtensions)
    {
        _allowedExtensions = allowedExtensions;
    }

    public DragDropEffects DragOver(DragEventArgs dropInfo)
    {
        var local = dropInfo.Data.GetFiles()?.FirstOrDefault()?.TryGetLocalPath();
        if (string.IsNullOrEmpty(local))
        {
            return DragDropEffects.None;
        }
        var ext = System.IO.Path.GetExtension(local);
        if (!_allowedExtensions.Contains(ext))
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
