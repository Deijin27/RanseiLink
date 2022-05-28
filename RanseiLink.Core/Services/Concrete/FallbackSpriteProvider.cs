using RanseiLink.Core.RomFs;
using RanseiLink.Core.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RanseiLink.Core.Services.DefaultPopulaters;
using System.Threading.Tasks;

namespace RanseiLink.Core.Services.Concrete
{
    public class FallbackSpriteProvider : IFallbackSpriteProvider
    {
        private readonly RomFsFactory _ndsFactory;
        private readonly IGraphicTypeDefaultPopulater[] _populaters;
        private readonly string _defaultDataFolder;

        public FallbackSpriteProvider(string defaultDataFolder, RomFsFactory ndsFactory, IGraphicTypeDefaultPopulater[] populaters)
        {
            _defaultDataFolder = defaultDataFolder;
            _populaters = populaters;
            _ndsFactory = ndsFactory;
            Directory.CreateDirectory(_defaultDataFolder);
        }

        public bool IsDefaultsPopulated => Directory.Exists(Path.Combine(_defaultDataFolder, Constants.GraphicsFolderPath));

        public void Populate(string ndsFile, IProgress<ProgressInfo> progress = null)
        {
            // reset the graphics folder
            if (IsDefaultsPopulated)
            {
                progress?.Report(new ProgressInfo(statusText: "Deleting Existing...", isIndeterminate: true));
                Directory.Delete(_defaultDataFolder, true);
            }
            Directory.CreateDirectory(_defaultDataFolder);

            // populate
            progress?.Report(new ProgressInfo(statusText: "Extracting files from rom...", isIndeterminate: true));
            using (var nds = _ndsFactory(ndsFile))
            {
                nds.ExtractCopyOfDirectory(Constants.GraphicsFolderPath, _defaultDataFolder);
            }
                
            var infos = GraphicsInfoResource.All;
            progress?.Report(new ProgressInfo(statusText: "Converting Images...", isIndeterminate: false, maxProgress: infos.Count));
            int count = 0;
            Parallel.ForEach(infos, gfxInfo =>
            {
                foreach (var populater in _populaters)
                {
                    populater.ProcessExportedFiles(_defaultDataFolder, gfxInfo);
                }
                progress?.Report(new ProgressInfo(progress: ++count));
            });
            progress?.Report(new ProgressInfo(statusText: "Done!"));
        }

        public List<SpriteFile> GetAllSpriteFiles(SpriteType type)
        {
            var info = GraphicsInfoResource.Get(type);
            if (info is MiscConstants miscInfo)
            {
                List<SpriteFile> result = new List<SpriteFile>();
                foreach (var item in miscInfo.Items)
                {
                    var file = Path.Combine(_defaultDataFolder, item.PngFile);
                    result.Add(new SpriteFile(type, item.Id, file, isOverride: false));
                }
                return result;
            }
            else
            {
                string dir = Path.Combine(_defaultDataFolder, info.PngFolder);
                if (!Directory.Exists(dir))
                {
                    return new List<SpriteFile>();
                }
                return Directory.GetFiles(dir)
                    .Select(i => new SpriteFile(type, int.Parse(Path.GetFileNameWithoutExtension(i)), i, isOverride: false))
                    .ToList();
            }
            
        }

        public SpriteFile GetSpriteFile(SpriteType type, int id)
        {
            return new SpriteFile(type, id, Path.Combine(_defaultDataFolder, GraphicsInfoResource.GetRelativeSpritePath(type, id)), false);
        }
    }
}