using RanseiLink.GuiCore.DragDrop;

namespace RanseiLink.DragDrop;

public class FileDropHandlerFactory : IFileDropHandlerFactory
{
    public IFileDropHandler New(params string[] allowedExtensions)
    {
        return new FileDropHandler(allowedExtensions);
    }
}