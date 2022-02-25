using RanseiLink.PluginModule.Api;

namespace SpriteSheetSplitterPlugin;

internal class SpriteSheetCreaterOptionForm : IPluginForm
{
    public string Title => "Sprite Sheet Creater";
    public string ProceedButtonText => "Go!";
    public string CancelButtonText => "Cancel";

    [UIntOption("Sprite Sheet Width")]
    public uint SheetWidth { get; set; } = 128;

    [UIntOption("Sprite Sheet Width")]
    public uint SheetHeight { get; set; } = 1024;


    [CollectionOption("Split Direction", new[] { SplitDirection.ColumnsThenRows, SplitDirection.RowsThenColumns })]
    public SplitDirection SplitDirection { get; set; } = SplitDirection.ColumnsThenRows;
}
