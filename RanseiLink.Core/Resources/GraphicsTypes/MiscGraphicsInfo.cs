using RanseiLink.Core.Services;
using SixLabors.ImageSharp;
using System.Xml.Linq;

namespace RanseiLink.Core.Resources;

public class MiscGraphicsInfo : GraphicsInfo
{
    public MiscItem[] Items { get; }

    public override bool FixedAmount => true;

    public MiscGraphicsInfo(MetaSpriteType metaType, XElement element) : base(metaType, element)
    {
        Items = element.Elements().Select((miscItemElement, id) =>
        {
            var metaId = Enum.Parse<MetaMiscItemId>(miscItemElement.Name.ToString());
            MiscItem result = metaId switch
            {
                MetaMiscItemId.NCER => new NCERMiscItem(metaId, id, miscItemElement),
                MetaMiscItemId.NSCR => new NSCRMiscItem(metaId, id, miscItemElement),
                MetaMiscItemId.IconInstS => new BuildingIconSmallMiscItem(metaId, id, miscItemElement),
                _ => throw new Exception("Invalid misc item element in GraphicsInfo.xml"),
            };
            return result;
        }).ToArray();
    }

    public override string GetRelativeSpritePath(int id)
    {
        return Items[id].PngFile;
    }

    public override int GetPaletteCapacity(int id)
    {
        return Items[id].PaletteCapacity;
    }

    public override void ProcessExportedFiles(string defaultDataFolder)
    {
        foreach (var item in Items)
        {
            item.ProcessExportedFiles(defaultDataFolder, this);
        }
    }

    public override void GetFilesToPatch(GraphicsPatchContext context)
    {
        foreach (var item in Items)
        {
            var spriteFile = context.OverrideDataProvider.GetSpriteFile(Type, item.Id);
            if (!spriteFile.IsOverride)
            {
                continue;
            }

            item.GetFilesToPatch(context, this, spriteFile.File);
        }
    }

    public override List<SpriteFile> GetAllSpriteFiles(bool isOverride, string folder)
    {
        var result = new List<SpriteFile>();
        foreach (var item in Items)
        {
            var file = Path.Combine(folder, item.PngFile);
            if (!File.Exists(file))
            {
                continue;
            }
            result.Add(new SpriteFile(Type, item.Id, item.PngFile, file, IsOverride: isOverride));
        }
        return result;
    }
}
