namespace RanseiLink.GuiCore.DragDrop;

public interface IFileDropHandler
{
    event Action<string>? FileDropped;
}

