#nullable enable

namespace RanseiLink.XP.DragDrop;

public class ModDropHandler : FileDropHandler
{
    public ModDropHandler() : base(RanseiLink.Core.Services.Concrete.ModManager.ExportModFileExtension)
    {
    }
}
