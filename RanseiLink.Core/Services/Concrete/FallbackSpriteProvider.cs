using RanseiLink.Core.RomFs;
using RanseiLink.Core.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RanseiLink.Core.Services.DefaultPopulaters;
using System.Threading.Tasks;
using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Services.Concrete
{
    public class FallbackSpriteProvider : IFallbackSpriteProvider
    {
        private readonly RomFsFactory _ndsFactory;
        private readonly IGraphicTypeDefaultPopulater[] _populaters;

        public FallbackSpriteProvider(RomFsFactory ndsFactory, IGraphicTypeDefaultPopulater[] populaters)
        {
            _populaters = populaters;
            _ndsFactory = ndsFactory;
        }
        public bool IsDefaultsPopulated(ConquestGameCode gameCode) => Directory.Exists(Path.Combine(Constants.DefaultDataFolder(gameCode), Constants.GraphicsFolderPath));

        public void Populate(string ndsFile, IProgress<ProgressInfo> progress = null)
        {
            ConquestGameCode gc;
            using (var br = new BinaryReader(File.OpenRead(ndsFile)))
            {
                var header = new NdsHeader(br);
                if (!Enum.TryParse(header.GameCode, out gc))
                {
                    progress?.Report(new ProgressInfo(statusText: "Not a valid conquest rom! :("));
                    return; // need to make this have better error reporting
                }
            }

            string defaultDataFolder = Constants.DefaultDataFolder(gc);
            // reset the graphics folder
            if (IsDefaultsPopulated(gc))
            {
                progress?.Report(new ProgressInfo(statusText: "Deleting Existing...", isIndeterminate: true));
                Directory.Delete(defaultDataFolder, true);
            }
            Directory.CreateDirectory(defaultDataFolder);

            // populate
            progress?.Report(new ProgressInfo(statusText: "Extracting files from rom...", isIndeterminate: true));
            using (var nds = _ndsFactory(ndsFile))
            {
                nds.ExtractCopyOfDirectory(Constants.GraphicsFolderPath, defaultDataFolder);
            }
                
            var infos = GraphicsInfoResource.All;
            progress?.Report(new ProgressInfo(statusText: "Converting Images...", isIndeterminate: false, maxProgress: infos.Count));
            int count = 0;
            Parallel.ForEach(infos, gfxInfo =>
            {
                foreach (var populater in _populaters)
                {
                    populater.ProcessExportedFiles(defaultDataFolder, gfxInfo);
                }
                progress?.Report(new ProgressInfo(progress: ++count));
            });
            progress?.Report(new ProgressInfo(statusText: "Done!"));
        }

        public List<SpriteFile> GetAllSpriteFiles(ConquestGameCode gc, SpriteType type)
        {
            string defaultDataFolder = Constants.DefaultDataFolder(gc);
            var info = GraphicsInfoResource.Get(type);
            if (info is MiscConstants miscInfo)
            {
                List<SpriteFile> result = new List<SpriteFile>();
                foreach (var item in miscInfo.Items)
                {
                    var file = Path.Combine(defaultDataFolder, item.PngFile);
                    result.Add(new SpriteFile(type, item.Id, file, isOverride: false));
                }
                return result;
            }
            else
            {
                string dir = Path.Combine(defaultDataFolder, info.PngFolder);
                if (!Directory.Exists(dir))
                {
                    return new List<SpriteFile>();
                }
                return Directory.GetFiles(dir)
                    .Select(i => new SpriteFile(type, int.Parse(Path.GetFileNameWithoutExtension(i)), i, isOverride: false))
                    .ToList();
            }
            
        }

        public SpriteFile GetSpriteFile(ConquestGameCode gc, SpriteType type, int id)
        {
            string defaultDataFolder = Constants.DefaultDataFolder(gc);
            return new SpriteFile(type, id, Path.Combine(defaultDataFolder, GraphicsInfoResource.GetRelativeSpritePath(type, id)), false);
        }

        public DataFile GetDataFile(ConquestGameCode gc, string pathInRom)
        {
            string defaultDataFolder = Constants.DefaultDataFolder(gc);
            return new DataFile(pathInRom, Path.Combine(defaultDataFolder, pathInRom), false);
        }

        public List<DataFile> GetAllDataFilesInFolder(ConquestGameCode gc, string pathOfFolderInRom)
        {
            string defaultDataFolder = Constants.DefaultDataFolder(gc);
            List<DataFile> files = new List<DataFile>();
            foreach (var file in Directory.GetFiles(Path.Combine(defaultDataFolder, pathOfFolderInRom)))
            {
                string pathOfFileInRom = Path.Combine(pathOfFolderInRom, Path.GetFileName(file));
                files.Add(new DataFile(pathOfFileInRom, file, false));
            }
            return files;
        }
    }
}