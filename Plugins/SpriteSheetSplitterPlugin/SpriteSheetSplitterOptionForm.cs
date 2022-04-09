using RanseiLink.PluginModule.Api;

namespace SpriteSheetSplitterPlugin;

internal class SpriteSheetSplitterOptionForm : IPluginForm
{
    public string Title => "Sprite Sheet Splitter";
    public string ProceedButtonText => "Go!";
    public string CancelButtonText => "Cancel";

    [IntOption("Sprite Width")]
    public int SpriteWidth { get; set; } = 32;

    [IntOption("Sprite Width")]
    public int SpriteHeight { get; set; } = 32;

    [CollectionOption("Split Direction", new[] { SplitDirection.ColumnsThenRows, SplitDirection.RowsThenColumns })]
    public SplitDirection SplitDirection { get; set; } = SplitDirection.ColumnsThenRows;
}
