using GongSolutions.Wpf.DragDrop;
using System;
using System.Windows;

namespace RanseiWpf.DragDrop
{
    public class RomDropHandler : IDropTarget
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
                        if (System.IO.Path.GetExtension(file) == ".nds")
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
}
