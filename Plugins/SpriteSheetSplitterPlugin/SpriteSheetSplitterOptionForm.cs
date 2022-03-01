using RanseiLink.PluginModule.Api;

namespace SpriteSheetSplitterPlugin;

internal class SpriteSheetSplitterOptionForm : IPluginForm
{
    public string Title => "Sprite Sheet Splitter";
    public string ProceedButtonText => "Go!";
    public string CancelButtonText => "Cancel";

    [UIntOption("Sprite Width")]
    public uint SpriteWidth { get; set; } = 32;

    [UIntOption("Sprite Width")]
    public uint SpriteHeight { get; set; } = 32;

    [CollectionOption("Split Direction", new[] { SplitDirection.ColumnsThenRows, SplitDirection.RowsThenColumns })]
    public SplitDirection SplitDirection { get; set; } = SplitDirection.ColumnsThenRows;
}
