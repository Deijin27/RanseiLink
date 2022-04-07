using RanseiLink.Core.RomFs;
using RanseiLink.Core.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RanseiLink.Core.Services.DefaultPopulaters;
using System.Threading.Tasks;

namespace RanseiLink.Core.Services.Concrete;

public class FallbackSpriteProvider : IFallbackSpriteProvider
{
    private readonly RomFsFactory _ndsFactory;
    private readonly IGraphicTypeDefaultPopulater[] _populaters;
    private readonly string _graphicsProviderFolder = Constants.DefaultDataProviderFolder;

    public FallbackSpriteProvider(RomFsFactory ndsFactory, IGraphicTypeDefaultPopulater[] populaters)
    {
        _populaters = populaters;
        _ndsFactory = ndsFactory;
        Directory.CreateDirectory(_graphicsProviderFolder);
    }

    public bool IsDefaultsPopulated => Directory.Exists(Path.Combine(_graphicsProviderFolder, Constants.GraphicsFolderPath));

    public void Populate(string ndsFile, IProgress<ProgressInfo> progress = null)
    {
        // reset the graphics folder
        if (IsDefaultsPopulated)
        {
            progress?.Report(new ProgressInfo(StatusText:"Deleting Existing...", IsIndeterminate:true));
            Directory.Delete(_graphicsProviderFolder, true);
        }
        Directory.CreateDirectory(_graphicsProviderFolder);

        // populate
        progress?.Report(new ProgressInfo(StatusText: "Extracting files from rom...", IsIndeterminate: true));
        using var nds = _ndsFactory(ndsFile);
        nds.ExtractCopyOfDirectory(Constants.GraphicsFolderPath, _graphicsProviderFolder);
        var infos = GraphicsInfoResource.All;
        progress?.Report(new ProgressInfo(StatusText: "Converting Images...", IsIndeterminate: false, MaxProgress: infos.Count));
        int count = 0;
        Parallel.ForEach(infos, gfxInfo =>
        {
            foreach (var populater in _populaters)
            {
                populater.ProcessExportedFiles(gfxInfo);
            }
            progress?.Report(new ProgressInfo(Progress: ++count));
        });
        progress?.Report(new ProgressInfo(StatusText: "Done!"));
    }

    public List<SpriteFile> GetAllSpriteFiles(SpriteType type)
    {
        string dir = Path.Combine(_graphicsProviderFolder, GraphicsInfoResource.Get(type).PngFolder);
        if (!Directory.Exists(dir))
        {
            return new List<SpriteFile>();
        }
        return Directory.GetFiles(dir)
            .Select(i => new SpriteFile(type, uint.Parse(Path.GetFileNameWithoutExtension(i)), i, IsOverride:false))
            .ToList();
    }

    public SpriteFile GetSpriteFile(SpriteType type, uint id)
    {
        return new SpriteFile(type, id, Path.Combine(_graphicsProviderFolder, GraphicsInfoResource.GetRelativeSpritePath(type, id)), false);
    }
}
