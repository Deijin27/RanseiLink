//#define PATCHER_BUG_FIXING
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.IO.Compression;
using System;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Nds;
using System.Linq;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Graphics;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RanseiLink.Core.Services.Concrete;

public class ModService : IModService
{
    private readonly string _graphicsProviderFolder;
    private readonly string _modFolder;
    public const string ModInfoFileName = "RanseiLinkModInfo.xml";
    public const string ExportModFileExtension = ".rlmod";
    private const uint CurrentModVersion = 2;

    private readonly NdsFactory _ndsFactory;
    private readonly IMsgService _msgService;
    private readonly IFallbackSpriteProvider _fallbackSpriteProvider;

    public ModService(string rootFolder, NdsFactory ndsFactory, IMsgService msgService, IFallbackSpriteProvider fallbackSpriteProvider)
    {
        _fallbackSpriteProvider = fallbackSpriteProvider;
        _msgService = msgService;
        _ndsFactory = ndsFactory;
        _graphicsProviderFolder = Path.Combine(rootFolder, "DataProvider");
        _modFolder = _modFolder = Path.Combine(rootFolder, "Mods");
        Directory.CreateDirectory(_modFolder);
    }

    public string Export(ModInfo modInfo, string destinationFolder)
    {
        Directory.CreateDirectory(destinationFolder);
        string exportFileName = modInfo.Name;
        if (string.IsNullOrEmpty(exportFileName))
        {
            exportFileName = "UnnamedMod";
        }
        string fileName = FileUtil.MakeValidFileName($"{exportFileName} v{modInfo.Version}") + ExportModFileExtension;
        string exportPath = FileUtil.MakeUniquePath(Path.Combine(destinationFolder, fileName));
        ZipFile.CreateFromDirectory(modInfo.FolderPath, exportPath);
        return exportPath;
    }

    public ModInfo Import(string modPath)
    {
        ModInfo modInfo;
        using (ZipArchive zip = ZipFile.OpenRead(modPath))
        {
            ZipArchiveEntry entry = zip.GetEntry(ModInfoFileName);
            if (entry == null)
            {
                throw new Exception("Failed to load mod because ModInfoFile not found.");
            }
            using (Stream modInfoStream = entry.Open())
            {
                if (!ModInfo.TryLoadFrom(XDocument.Load(modInfoStream), out modInfo))
                {
                    throw new Exception("Failed to load mod because failed to load ModInfo from ModInfoFile.");
                }
            }
        }
        modInfo.FolderPath = GetNewModDirectory();
        ZipFile.ExtractToDirectory(modPath, modInfo.FolderPath);
        return modInfo;
    }

    private string GetNewModDirectory()
    {
        return FileUtil.MakeUniquePath(Path.Combine(_modFolder, "Mod"));
    }

    public IList<ModInfo> GetAllModInfo()
    {
        return GetAllModInfoIncludingPreviousVersions().Where(i => i.RLModVersion == CurrentModVersion).ToList();
    }

    private IList<ModInfo> GetAllModInfoIncludingPreviousVersions()
    {
        var modInfos = new List<ModInfo>();
        foreach (string folder in Directory.GetDirectories(_modFolder))
        {
            string modInfoPath = Path.Combine(folder, ModInfoFileName);
            if (File.Exists(modInfoPath) && ModInfo.TryLoadFrom(XDocument.Load(modInfoPath), out ModInfo info))
            {
                info.FolderPath = folder;
                modInfos.Add(info);

            }
        }
        return modInfos;
    }

    public IList<ModInfo> GetModInfoPreviousVersions()
    {
        return GetAllModInfoIncludingPreviousVersions().Where(i => i.RLModVersion < CurrentModVersion).ToList();
    }

    public ModInfo Create(string baseRomPath, string name = "", string version = "", string author = "")
    {
        var modInfo = new ModInfo
        {
            FolderPath = GetNewModDirectory(),
            Name = name,
            Version = version,
            Author = author,
            RLModVersion = CurrentModVersion
        };
        Directory.CreateDirectory(modInfo.FolderPath);

        using (var nds = _ndsFactory(baseRomPath))
        {
            nds.ExtractCopyOfDirectory(Constants.DataFolderPath, modInfo.FolderPath);
        }

        var msgPath = Path.Combine(modInfo.FolderPath, Constants.MsgRomPath);
        _msgService.ExtractFromMsgDat(msgPath, Path.Combine(modInfo.FolderPath, Constants.MsgFolderPath));
        File.Delete(msgPath);

        Update(modInfo);
        return modInfo;
    }

    public ModInfo CreateBasedOn(ModInfo baseMod, string name = "", string version = "", string author = "")
    {
        var modInfo = new ModInfo
        {
            FolderPath = GetNewModDirectory(),
            Name = name,
            Version = version,
            Author = author,
            RLModVersion = CurrentModVersion
        };
        FileUtil.CopyFilesRecursively(baseMod.FolderPath, modInfo.FolderPath);
        Update(modInfo);
        return modInfo;
    }

    public void UpgradeModsToLatestVersion(IEnumerable<ModInfo> mods, string romPath)
    {
        using INds nds = _ndsFactory(romPath);
        foreach (ModInfo mod in mods)
        {
            switch (mod.RLModVersion)
            {
                case 1:
                    AdvanceVersion1To2(mod, nds);
                    goto case 2;
                case 2:
                default:
                    break;
            }
            Update(mod);
        }

    }

    private void AdvanceVersion1To2(ModInfo modInfo, INds nds)
    {
        string msgPath = Path.Combine(modInfo.FolderPath, Constants.MsgRomPath);
        nds.ExtractCopyOfFile(Constants.MsgRomPath, Path.GetDirectoryName(msgPath));
        _msgService.ExtractFromMsgDat(msgPath, Path.Combine(modInfo.FolderPath, Constants.MsgFolderPath));
        File.Delete(msgPath);
        modInfo.RLModVersion = 2;
    }

    public void Update(ModInfo modInfo)
    {
        var xdoc = new XDocument();
        modInfo.SaveTo(xdoc);
        using (var file = File.Create(Path.Combine(modInfo.FolderPath, ModInfoFileName)))
        {
            xdoc.Save(file);
        }
    }

    public void Delete(ModInfo modInfo)
    {
        Directory.Delete(modInfo.FolderPath, true);
    }

    public void Commit(ModInfo modInfo, string path, PatchOptions patchOptions = 0, IProgress<ProgressInfo> progress = null)
    {
        progress?.Report(new ProgressInfo(StatusText: "Preparing to patch...", IsIndeterminate: true));

        ConcurrentBag<FileToPatch> filesToPatch = new();
        Exception exception = null;
        try
        {
            GetFilesToPatch(modInfo, filesToPatch, patchOptions);

#if PATCHER_BUG_FIXING
            string debugOut = FileUtil.MakeUniquePath(Path.Combine(FileUtil.DesktopDirectory, "patch_debug_dump"));
            Directory.CreateDirectory(debugOut);
            foreach (var file in filesToPatch)
            {
                string dest = Path.Combine(debugOut, file.GamePath.Replace(Path.DirectorySeparatorChar, '~'));
                if (file.Options.HasFlag(FilePatchOptions.DeleteSourceWhenDone))
                {
                    File.Move(file.FileSystemPath, dest);
                }
                else
                {
                    File.Copy(file.FileSystemPath, dest);
                }
                
            }
            return;
#endif

            progress?.Report(new ProgressInfo(IsIndeterminate: false, MaxProgress: filesToPatch.Count, StatusText: "Patching..."));
            int count = 0;
            using var nds = _ndsFactory(path);
            foreach (var file in filesToPatch)
            {
                if (file.Options.HasFlag(FilePatchOptions.VariableLength))
                {
                    nds.InsertVariableLengthFile(file.GamePath, file.FileSystemPath);
                }
                else
                {
                    nds.InsertFixedLengthFile(file.GamePath, file.FileSystemPath);
                }
                progress?.Report(new ProgressInfo(Progress: ++count));
            }
        }
        catch (Exception e)
        {
            exception = e;
        }
        finally
        {
            progress?.Report(new ProgressInfo(StatusText: "Cleaning up temporary files...", IsIndeterminate: true));

            foreach (var file in filesToPatch)
            {
                if (file.Options.HasFlag(FilePatchOptions.DeleteSourceWhenDone))
                {
                    File.Delete(file.FileSystemPath);
                }
            }
        }
        
        if (exception != null)
        {
            throw exception;
        }

        progress?.Report(new ProgressInfo(StatusText: "Done!", IsIndeterminate: false));
    }

    private void GetFilesToPatch(ModInfo mod, ConcurrentBag<FileToPatch> filesToPatch, PatchOptions patchOptions)
    {
        List<string> dataRomPaths = new()
        {
            Constants.PokemonRomPath,
            Constants.MoveRomPath,
            Constants.AbilityRomPath,
            Constants.WarriorSkillRomPath,
            Constants.GimmickRomPath,
            Constants.BuildingRomPath,
            Constants.ItemRomPath,
            Constants.KingdomRomPath,
            Constants.MoveRangeRomPath,
            Constants.EventSpeakerRomPath,
            Constants.BaseBushouMaxSyncTableRomPath,
            Constants.BaseBushouRomPath,
            Constants.MapRomPath,
            Constants.GimmickRangeRomPath,
            Constants.MoveEffectRomPath,
            Constants.GimmickObjectRomPath
        };

        foreach (var i in EnumUtil.GetValues<ScenarioId>())
        {
            dataRomPaths.Add(Constants.ScenarioPokemonPathFromId(i));
            dataRomPaths.Add(Constants.ScenarioWarriorPathFromId(i));
            dataRomPaths.Add(Constants.ScenarioAppearPokemonPathFromId(i));
            dataRomPaths.Add(Constants.ScenarioKingdomPathFromId(i));
        }

        foreach (string drp in dataRomPaths)
        {
            filesToPatch.Add(new FileToPatch(drp, Path.Combine(mod.FolderPath, drp), FilePatchOptions.None));
        }

        foreach (var mapFilePath in Directory.GetFiles(Path.Combine(mod.FolderPath, Constants.DataFolderPath, "map")))
        {
            string mapRomPath = Path.Combine(Constants.DataFolderPath, "map", Path.GetFileName(mapFilePath));
            filesToPatch.Add(new FileToPatch(mapRomPath, mapFilePath, FilePatchOptions.VariableLength));
        }

        string msgTmpFile = Path.GetTempFileName();
        _msgService.CreateMsgDat(Path.Combine(mod.FolderPath, Constants.MsgFolderPath), msgTmpFile);
        filesToPatch.Add(new FileToPatch(Constants.MsgRomPath, msgTmpFile, FilePatchOptions.VariableLength | FilePatchOptions.DeleteSourceWhenDone));

        if (patchOptions.HasFlag(PatchOptions.IncludeSprites))
        {
            IOverrideSpriteProvider spriteProvider = new OverrideSpriteProvider(_fallbackSpriteProvider, mod);

            Parallel.ForEach(GraphicsInfoResource.All, gInfo =>
            {
                switch (gInfo)
                {
                    case StlConstants stlInfo:
                        PackStl(stlInfo, filesToPatch, spriteProvider);
                        break;
                    case ScbgConstants scbgInfo:
                        PackScbg(scbgInfo, filesToPatch, spriteProvider);
                        break;
                    case PkmdlConstants pkdmlInfo:
                        PokemonModelManager.PackModels(pkdmlInfo, filesToPatch, spriteProvider, _graphicsProviderFolder);
                        break;
                    default:
                        throw new Exception($"Other types of {nameof(IGraphicsInfo)} not supported");
                }
            });
        }
    }

    private void PackStl(StlConstants stlInfo, ConcurrentBag<FileToPatch> filesToPatch, IOverrideSpriteProvider overrideSpriteProvider)
    {
        var spriteFiles = overrideSpriteProvider.GetAllSpriteFiles(stlInfo.Type);
        if (!spriteFiles.Any(i => i.IsOverride))
        {
            return;
        }

        string[] filesToPack = spriteFiles.Select(i => i.File).ToArray();
        var ncer = NCER.Load(Path.Combine(_graphicsProviderFolder, stlInfo.Ncer));
        if (stlInfo.TexInfo != null)
        {
            string texData = Path.GetTempFileName();
            string texInfo = Path.GetTempFileName();
            STLCollection
                .LoadPngs(filesToPack, ncer, tiled: false)
                .Save(texData, texInfo);
            filesToPatch.Add(new FileToPatch(stlInfo.TexInfo, texInfo, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
            filesToPatch.Add(new FileToPatch(stlInfo.TexData, texData, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        }

        if (stlInfo.Info != null)
        {
            string info = Path.GetTempFileName();
            string data = Path.GetTempFileName();
            STLCollection
                .LoadPngs(filesToPack, ncer, tiled: true)
                .Save(data, info);
            filesToPatch.Add(new FileToPatch(stlInfo.Info, info, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
            filesToPatch.Add(new FileToPatch(stlInfo.Data, data, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        }
    }

    private void PackScbg(ScbgConstants scbgInfo, ConcurrentBag<FileToPatch> filesToPatch, IOverrideSpriteProvider overrideSpriteProvider)
    {
        var spriteFiles = overrideSpriteProvider.GetAllSpriteFiles(scbgInfo.Type);
        if (!spriteFiles.Any(i => i.IsOverride))
        {
            return;
        }

        string[] filesToPack = spriteFiles.Select(i => i.File).ToArray();
        string data = Path.GetTempFileName();
        string info = Path.GetTempFileName();

        SCBGCollection
            .LoadPngs(filesToPack, tiled:true)
            .Save(data, info);

        filesToPatch.Add(new FileToPatch(scbgInfo.Data, data, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        filesToPatch.Add(new FileToPatch(scbgInfo.Info, info, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
    }
}

[Flags]
internal enum FilePatchOptions
{
    None = 0,
    DeleteSourceWhenDone = 1,
    VariableLength = 2,
}

internal record FileToPatch(string GamePath, string FileSystemPath, FilePatchOptions Options);