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
            if (!_fallbackSpriteProvider.IsDefaultsPopulated(_mod.GameCode))
            {
                return new List<SpriteFile>();
            }
            var dict = new Dictionary<int, SpriteFile>();
            foreach (var i in _fallbackSpriteProvider.GetAllSpriteFiles(_mod.GameCode, type))
            {
                dict[i.Id] = i;
            }

            var info = GraphicsInfoResource.Get(type);
            if (info is MiscConstants miscInfo)
            {
                foreach (var item in miscInfo.Items)
                {
                    var file = Path.Combine(_mod.FolderPath, item.PngFile);
                    var fi = new SpriteFile(type, item.Id, file, isOverride: true);
                    if (File.Exists(fi.File))
                    {
                        dict[fi.Id] = fi;
                    }
                }
            }
            else
            {
                string overrideFolder = Path.Combine(_mod.FolderPath, info.PngFolder);
                if (Directory.Exists(overrideFolder))
                {
                    foreach (var i in Directory.GetFiles(overrideFolder).Select(i => new SpriteFile(type: type, id: int.Parse(Path.GetFileNameWithoutExtension(i)), file: i, isOverride: true)))
                    {
                        dict[i.Id] = i;
                    }
                }
                
            }

            return dict.Values.ToList();
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
                file = _fallbackSpriteProvider.GetSpriteFile(_mod.GameCode, type, id);
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