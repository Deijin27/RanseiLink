using RanseiLink.Core.Resources;

namespace RanseiLink.Core.Services.Concrete;

public class OverrideDataProvider(IFallbackDataProvider fallbackSpriteProvider, ModInfo mod) : IOverrideDataProvider
{
    public event EventHandler<SpriteModifiedArgs>? SpriteModified;

    public void ClearOverride(SpriteType type, int id)
    {
        var file = GetSpriteFilePathWithoutFallback(type, id).File;
        if (File.Exists(file))
        {
            File.Delete(file);
        }
        SpriteModified?.Invoke(this, new SpriteModifiedArgs(type, id));
    }

    public List<SpriteFile> GetAllSpriteFiles(SpriteType type)
    {
        if (!fallbackSpriteProvider.IsDefaultsPopulated(mod.GameCode))
        {
            return [];
        }
        var dict = new Dictionary<int, SpriteFile>();
        foreach (var i in fallbackSpriteProvider.GetAllSpriteFiles(mod.GameCode, type))
        {
            dict[i.Id] = i;
        }

        var info = GraphicsInfoResource.Get(type);
        if (info is MiscConstants miscInfo)
        {
            foreach (var item in miscInfo.Items)
            {
                var file = Path.Combine(mod.FolderPath, item.PngFile);
                if (File.Exists(file))
                {
                    var fi = new SpriteFile(type, item.Id, item.PngFile, file, IsOverride: true);
                    dict[fi.Id] = fi;
                }
            }
        }
        else if (info is IGroupedGraphicsInfo groupedInfo)
        {
            string overrideFolder = Path.Combine(mod.FolderPath, groupedInfo.PngFolder);
            if (Directory.Exists(overrideFolder))
            {
                foreach (var i in Directory.GetFiles(overrideFolder))
                {
                    var romPath = i[(mod.FolderPath.Length + 1)..];
                    var fi = new SpriteFile(type, int.Parse(Path.GetFileNameWithoutExtension(i)), romPath, i, IsOverride: true);
                    dict[fi.Id] = fi;
                }
            }
            
        }
        else
        {
            throw new Exception("Unhandled graphics info type");
        }

        return dict.Values.ToList();
    }

    private SpriteFile GetSpriteFilePathWithoutFallback(SpriteType type, int id)
    {
        var romPath = GraphicsInfoResource.Get(type).GetRelativeSpritePath(id);
        return new SpriteFile(type, id, romPath, Path.Combine(mod.FolderPath, romPath), true);
    }

    public SpriteFile GetSpriteFile(SpriteType type, int id)
    {
        var file = GetSpriteFilePathWithoutFallback(type, id);
        if (!File.Exists(file.File))
        {
            file = fallbackSpriteProvider.GetSpriteFile(mod.GameCode, type, id);
        }
        return file;
    }

    public void SetOverride(SpriteType type, int id, string file)
    {
        string targetFile = GetSpriteFilePathWithoutFallback(type, id).File;
        Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);
        File.Copy(file, targetFile, overwrite: true);
        SpriteModified?.Invoke(this, new SpriteModifiedArgs(type, id));
    }

    private DataFile GetDataFilePathWithoutFallback(string pathInRom)
    {
        return new DataFile(pathInRom, Path.Combine(mod.FolderPath, pathInRom), true);
    }

    public void SetOverride(string pathInRom, string file)
    {
        string targetFile = GetDataFilePathWithoutFallback(pathInRom).File;
        Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);
        File.Copy(file, targetFile, overwrite: true);
    }

    public void ClearOverride(string pathInRom)
    {
        var file = GetDataFilePathWithoutFallback(pathInRom).File;
        if (File.Exists(file))
        {
            File.Delete(file);
        }
    }

    public DataFile GetDataFile(string pathInRom)
    {
        var file = GetDataFilePathWithoutFallback(pathInRom);
        if (!File.Exists(file.File))
        {
            file = fallbackSpriteProvider.GetDataFile(mod.GameCode, pathInRom);
        }
        return file;
    }

    public List<DataFile> GetAllDataFilesInFolder(string pathOfFolderInRom)
    {
        var files = fallbackSpriteProvider.GetAllDataFilesInFolder(mod.GameCode, pathOfFolderInRom).ToDictionary(x => x.RomPath);
        var dir = Path.Combine(mod.FolderPath, pathOfFolderInRom);
        if (Directory.Exists(dir))
        {
            foreach (var file in Directory.GetFiles(dir))
            {
                string pathOfFileInRom = Path.Combine(pathOfFolderInRom, Path.GetFileName(file));
                files[pathOfFileInRom] = new DataFile(pathOfFileInRom, file, true);
            }
        }
        
        return files.Values.ToList();

    }

    public bool IsDefaultsPopulated()
    {
        return fallbackSpriteProvider.IsDefaultsPopulated(mod.GameCode);
    }
}