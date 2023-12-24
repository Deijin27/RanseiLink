using RanseiLink.Core.Services.Concrete;

namespace RanseiLink.GuiCore.DragDrop;

public interface IFileDropHandlerFactory
{
    IFileDropHandler New(params string[] allowedExtensions);
}

public static class FileDropHandlerFactoryExtensions
{
    public static IFileDropHandler NewModDropHandler(this IFileDropHandlerFactory factory)
    {
        return factory.New(ModManager.ExportModFileExtension);
    }

    public static IFileDropHandler NewRomDropHandler(this IFileDropHandlerFactory factory)
    {
        return factory.New(".nds");
    }
}
