using RanseiLink.Core.Graphics;
using RanseiLink.Core.Services;
using System.Xml.Linq;

namespace RanseiLink.Core.Resources;

public class ScbgGraphicsInfo : GroupedGraphicsInfo
{
    public string Info { get; }
    public string Data { get; }
    public override string PngFolder { get; }
    public override int PaletteCapacity => 256;

    public ScbgGraphicsInfo(MetaSpriteType metaType, XElement element) : base(metaType, element)
    {
        Info = FileUtil.NormalizePath(element.Element("Info")!.Value);
        Data = FileUtil.NormalizePath(element.Element("Data")!.Value);
        PngFolder = Path.Combine(Path.GetDirectoryName(Data)!, Path.GetFileNameWithoutExtension(Data) + "-Pngs");
    }

    public override void ProcessExportedFiles(PopulateDefaultsContext context)
    {
        string data = Path.Combine(context.DefaultDataFolder, Data);
        string info = Path.Combine(context.DefaultDataFolder, Info);
        bool tiled = true;
        string pngDir = Path.Combine(context.DefaultDataFolder, PngFolder);
        SCBGCollection.Load(data, info).SaveAsPngs(pngDir, tiled);

    }

    public override void GetFilesToPatch(GraphicsPatchContext context)
    {
        var spriteFiles = context.OverrideDataProvider.GetAllSpriteFiles(Type);
        if (!spriteFiles.Any(i => i.IsOverride))
        {
            return;
        }

        string[] filesToPack = spriteFiles.Select(i => i.File).ToArray();
        string data = Path.GetTempFileName();
        string info = Path.GetTempFileName();

        SCBGCollection
            .LoadPngs(filesToPack, tiled: true)
            .Save(data, info);

        context.FilesToPatch.Add(new FileToPatch(Data, data, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        context.FilesToPatch.Add(new FileToPatch(Info, info, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));

    }
}