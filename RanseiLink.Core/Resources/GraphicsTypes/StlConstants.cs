using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Services;
using System.Xml.Linq;

namespace RanseiLink.Core.Resources;

public class StlConstants : GroupedGraphicsInfo
{
    public string? TexInfo { get; }
    public string? TexData { get; }
    public string? Info { get; }
    public string? Data { get; }
    public string Link { get; }
    public string LinkFolder { get; }
    public string Ncer { get; }
    public override string PngFolder { get; }
    public override int PaletteCapacity => 256;

    public StlConstants(MetaSpriteType metaType, XElement element) : base(metaType, element)
    {
        TexInfo = FileUtil.NormalizePath(element.Element("TexInfo")?.Value);
        TexData = FileUtil.NormalizePath(element.Element("TexData")?.Value);
        Info = FileUtil.NormalizePath(element.Element("Info")?.Value);
        Data = FileUtil.NormalizePath(element.Element("Data")?.Value);
        Link = FileUtil.NormalizePath(element.Element("Link")!.Value);
        LinkFolder = Path.Combine(Path.GetDirectoryName(Link)!, "UnpackedLink");
        Ncer = Path.Combine(LinkFolder, "0002.ncer");
        string png;
        if (TexData != null)
        {
            png = TexData;
            if (TexInfo == null) throw new Exception();
        }
        else if (Data != null)
        {
            png = Data;
            if (Info == null) throw new Exception();
        }
        else
        {
            throw new Exception();
        }
        PngFolder = Path.Combine(Path.GetDirectoryName(png)!, Path.GetFileNameWithoutExtension(png) + "-Pngs");
    }

    public override void ProcessExportedFiles(string defaultDataFolder)
    {
        LINK.Unpack(Path.Combine(defaultDataFolder, Link), Path.Combine(defaultDataFolder, LinkFolder), true, 4);
        var ncer = NCER.Load(Path.Combine(defaultDataFolder, Ncer));
        string data = Path.Combine(defaultDataFolder, TexData ?? Data!);
        string info = Path.Combine(defaultDataFolder, TexInfo ?? Info!);
        bool tiled = TexData == null;
        string pngDir = Path.Combine(defaultDataFolder, PngFolder);
        STLCollection.Load(data, info).SaveAsPngs(pngDir, ncer, tiled);
    }

    public override void GetFilesToPatch(GraphicsPatchContext context)
    {
        var spriteFiles = context.OverrideDataProvider.GetAllSpriteFiles(Type);
        if (!spriteFiles.Any(i => i.IsOverride))
        {
            return;
        }

        string[] filesToPack = spriteFiles.Select(i => i.File).ToArray();
        var ncer = NCER.Load(Path.Combine(context.DefaultDataFolder, Ncer));
        if (TexInfo != null)
        {
            string texData = Path.GetTempFileName();
            string texInfo = Path.GetTempFileName();
            STLCollection
                .LoadPngs(filesToPack, ncer, tiled: false)
                .Save(texData, texInfo);
            context.FilesToPatch.Add(new FileToPatch(TexInfo, texInfo, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
            context.FilesToPatch.Add(new FileToPatch(TexData!, texData, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        }

        if (Info != null)
        {
            string info = Path.GetTempFileName();
            string data = Path.GetTempFileName();
            STLCollection
                .LoadPngs(filesToPack, ncer, tiled: true)
                .Save(data, info);
            context.FilesToPatch.Add(new FileToPatch(Info, info, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
            context.FilesToPatch.Add(new FileToPatch(Data!, data, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        }
    }
}