namespace RanseiLink.GuiCore.DragDrop;

public interface IFolderDropHandler
{
    event Action<string>? FolderDropped;
}



