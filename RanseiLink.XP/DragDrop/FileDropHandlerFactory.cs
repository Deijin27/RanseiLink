using RanseiLink.GuiCore.DragDrop;
#nullable enable

namespace RanseiLink.XP.DragDrop;

public class FileDropHandlerFactory : IFileDropHandlerFactory
{
    public IFileDropHandler New(params string[] allowedExtensions)
    {
        return new FileDropHandler(allowedExtensions);
    }
}
