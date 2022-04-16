using RanseiLink.Core.Resources;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Services.Concrete
{
    internal class OverrideSpriteProvider : IOverrideSpriteProvider
    {
        private readonly IFallbackSpriteProvider _fallbackSpriteProvider;
        private readonly ModInfo _mod;
        public OverrideSpriteProvider(IFallbackSpriteProvider fallbackSpriteProvider, ModInfo mod)
        {
            _mod = mod;
            _fallbackSpriteProvider = fallbackSpriteProvider;
        }

        public void ClearOverride(SpriteType type, int id)
        {
            File.Delete(GetSpriteFilePathWithoutFallback(type, id).File);
        }

        public List<SpriteFile> GetAllSpriteFiles(SpriteType type)
        {
            if (!_fallbackSpriteProvider.IsDefaultsPopulated)
            {
                return new List<SpriteFile>();
            }
            var dict = new Dictionary<SpriteType, Dictionary<int, SpriteFile>>();
            foreach (var i in _fallbackSpriteProvider.GetAllSpriteFiles(type))
            {
                if (!dict.TryGetValue(i.Type, out Dictionary<int, SpriteFile> subDict))
                {
                    subDict = new Dictionary<int, SpriteFile>();
                    dict[i.Type] = subDict;
                }
                subDict[i.Id] = i;
            }
            string overrideFolder = Path.Combine(_mod.FolderPath, GraphicsInfoResource.Get(type).PngFolder);
            if (Directory.Exists(overrideFolder))
            {
                foreach (var i in Directory.GetFiles(overrideFolder)
                .Select(i => new SpriteFile(type: type, id: int.Parse(Path.GetFileNameWithoutExtension(i)), file: i, isOverride: true)))
                {
                    dict[i.Type][i.Id] = i;
                }
            }
            return dict.Values.SelectMany(i => i.Values).ToList();
        }

        private SpriteFile GetSpriteFilePathWithoutFallback(SpriteType type, int id)
        {
            return new SpriteFile(type, id, Path.Combine(_mod.FolderPath, GraphicsInfoResource.GetRelativeSpritePath(type, id)), true);
        }

        public SpriteFile GetSpriteFile(SpriteType type, int id)
        {
            var file = GetSpriteFilePathWithoutFallback(type, id);
            if (!File.Exists(file.File))
            {
                file = _fallbackSpriteProvider.GetSpriteFile(type, id);
            }
            return file;
        }

        public void SetOverride(SpriteType type, int id, string file)
        {
            string targetFile = GetSpriteFilePathWithoutFallback(type, id).File;
            Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
            File.Copy(file, targetFile, overwrite: true);
        }
    }
}