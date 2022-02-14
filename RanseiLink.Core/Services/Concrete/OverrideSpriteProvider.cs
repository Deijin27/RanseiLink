using RanseiLink.Core.Resources;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Services.Concrete;

internal class OverrideSpriteProvider : IOverrideSpriteProvider
{
    private readonly ISpriteProvider _fallbackSpriteProvider;
    private readonly ModInfo _mod;
    public OverrideSpriteProvider(ISpriteProvider fallbackSpriteProvider, ModInfo mod)
    {
        _mod = mod;
        _fallbackSpriteProvider = fallbackSpriteProvider;
    }

    public void ClearOverride(SpriteType type, uint id)
    {
        File.Delete(GetSpriteFilePathWithoutFallback(type, id));
    }

    public List<SpriteFile> GetAllSpriteFiles(SpriteType type)
    {
        var dict = new Dictionary<SpriteType, Dictionary<uint, SpriteFile>>();
        foreach (var i in _fallbackSpriteProvider.GetAllSpriteFiles(type))
        {
            if (!dict.TryGetValue(i.Type, out Dictionary<uint, SpriteFile> subDict))
            {
                subDict = new Dictionary<uint, SpriteFile>();
                dict[i.Type] = subDict;
            }
            subDict[i.Id] = i;
        }
        string overrideFolder = Path.Combine(_mod.FolderPath, GraphicsInfoResource.Get(type).PngFolder);
        if (Directory.Exists(overrideFolder))
        {
            foreach (var i in Directory.GetFiles(overrideFolder)
            .Select(i => new SpriteFile(Type: type, Id: uint.Parse(Path.GetFileNameWithoutExtension(i)), File: i, IsOverride: true)))
            {
                dict[i.Type][i.Id] = i;
            }
        }
        return dict.Values.SelectMany(i => i.Values).ToList();
    }

    private string GetSpriteFilePathWithoutFallback(SpriteType type, uint id)
    {
        return Path.Combine(_mod.FolderPath, GraphicsInfoResource.GetRelativeSpritePath(type, id));
    }

    public string GetSpriteFilePath(SpriteType type, uint id)
    {
        string file = GetSpriteFilePathWithoutFallback(type, id);
        if (!File.Exists(file))
        {
            file = _fallbackSpriteProvider.GetSpriteFilePath(type, id);
        }
        return file;
    }

    public void SetOverride(SpriteType type, uint id, string file)
    {
        string targetFile = GetSpriteFilePathWithoutFallback(type, id);
        Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
        File.Copy(file, targetFile, overwrite:true);
    }
}
