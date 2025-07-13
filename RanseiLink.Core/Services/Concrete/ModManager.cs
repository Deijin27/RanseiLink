using System.Xml.Linq;
using System.IO.Compression;
using RanseiLink.Core.RomFs;
using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Services.Concrete;

public class ModManager : IModManager
{
    private readonly string _modFolder;
    public const string ModInfoFileName = "RanseiLinkModInfo.xml";
    public const string ExportModFileExtension = ".rlmod";
    private const uint CurrentModVersion = 2;

    private readonly RomFsFactory _ndsFactory;
    private readonly IMsgService _msgService;

    public string ModFolder => _modFolder;

    public ModManager(string modFolder, RomFsFactory ndsFactory, IMsgService msgService)
    {
        _msgService = msgService;
        _ndsFactory = ndsFactory;
        _modFolder = modFolder;
        Directory.CreateDirectory(_modFolder);
    }

    public string Export(ModInfo modInfo, string destinationFile)
    {
        string exportPath = FileUtil.MakeUniquePath(destinationFile);
        ZipFile.CreateFromDirectory(modInfo.FolderPath, exportPath);
        return exportPath;
    }

    public ModInfo Import(string modPath)
    {
        ModInfo? modInfo;
        using (ZipArchive zip = ZipFile.OpenRead(modPath))
        {
            ZipArchiveEntry? entry = zip.GetEntry(ModInfoFileName);
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
            if (File.Exists(modInfoPath) && ModInfo.TryLoadFrom(XDocument.Load(modInfoPath), out ModInfo? info))
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

    public ModInfo Create(string baseRomPath, ModMetadata modMetadata)
    {
        ConquestGameCode gameCode;
        using (var br = new BinaryReader(File.OpenRead(baseRomPath)))
        {
            var header = new NdsHeader(br);
            gameCode = header.GameCode switch
            {
                "VPYJ" => ConquestGameCode.VPYJ,
                "VPYT" => ConquestGameCode.VPYT,
                "VPYP" => ConquestGameCode.VPYP,
                _ => throw new Exception($"Unexpected game code '{header.GameCode}', this may not be a conquest rom, or it may be a culture we don't know of yet"),
            };
        }

        var modInfo = NewMod(modMetadata, gameCode, CurrentModVersion);
        Directory.CreateDirectory(modInfo.FolderPath);

        Banner banner;

        using (var nds = _ndsFactory(baseRomPath))
        {
            nds.ExtractCopyOfDirectory(Constants.DataFolderPath, modInfo.FolderPath);

            banner = nds.GetBanner();
        }

        banner.SaveInfoToXml(Path.Combine(modInfo.FolderPath, Constants.BannerInfoFile));
        banner.SaveImageToPng(Path.Combine(modInfo.FolderPath, Constants.BannerImageFile));

        var msgPath = Path.Combine(modInfo.FolderPath, Constants.MsgRomPath);
        _msgService.ExtractFromMsgDat(msgPath, Path.Combine(modInfo.FolderPath, Constants.MsgFolderPath));
        File.Delete(msgPath);

        Update(modInfo);
        return modInfo;
    }

    private ModInfo NewMod(ModMetadata metadata, ConquestGameCode gameCode, uint rlModVersion)
    {
        return new ModInfo
        {
            FolderPath = GetNewModDirectory(),
            Name = metadata.Name,
            Author = metadata.Author,
            Version = metadata.Version,
            Tags = [.. metadata.Tags],
            GameCode = gameCode,
            RLModVersion = rlModVersion
        };
    }

    public ModInfo CreateBasedOn(ModInfo baseMod, ModMetadata metadata)
    {
        var modInfo = NewMod(metadata, baseMod.GameCode, baseMod.RLModVersion);
        FileUtil.CopyFilesRecursively(baseMod.FolderPath, modInfo.FolderPath);
        Update(modInfo);
        return modInfo;
    }

    public void UpgradeModsToLatestVersion(IEnumerable<ModInfo> mods, string romPath)
    {
        using (IRomFs nds = _ndsFactory(romPath))
        {
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
    }

    private void AdvanceVersion1To2(ModInfo modInfo, IRomFs nds)
    {
        string msgPath = Path.Combine(modInfo.FolderPath, Constants.MsgRomPath);
        nds.ExtractCopyOfFile(Constants.MsgRomPath, Path.GetDirectoryName(msgPath)!);
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
}