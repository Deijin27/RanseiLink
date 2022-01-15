using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.IO.Compression;
using System;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Nds;
using System.Linq;

namespace RanseiLink.Core.Services.Concrete;

public class ModService : IModService
{
    private readonly string modFolder;
    public const string ModInfoFileName = "RanseiLinkModInfo.xml";
    public const string ExportModFileExtension = ".rlmod";
    private const uint CurrentModVersion = 2;

    private readonly NdsFactory _ndsFactory;
    private readonly IMsgService _msgService;

    public ModService(string rootFolder, NdsFactory ndsFactory, IMsgService msgService)
    {
        _msgService = msgService;
        _ndsFactory = ndsFactory;
        modFolder = modFolder = Path.Combine(rootFolder, "Mods");
        Directory.CreateDirectory(modFolder);
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
        return FileUtil.MakeUniquePath(Path.Combine(modFolder, "Mod"));
    }

    public IList<ModInfo> GetAllModInfo()
    {
        return GetAllModInfoIncludingPreviousVersions().Where(i => i.RLModVersion == CurrentModVersion).ToList();
    }

    private IList<ModInfo> GetAllModInfoIncludingPreviousVersions()
    {
        var modInfos = new List<ModInfo>();
        foreach (string folder in Directory.GetDirectories(modFolder))
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
        LoadRom(baseRomPath, modInfo);
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

    public void LoadRom(string path, ModInfo modInfo)
    {
        using (var nds = _ndsFactory(path))
        {
            nds.ExtractCopyOfDirectory(Constants.DataFolderPath, modInfo.FolderPath);
        }

        var msgPath = Path.Combine(modInfo.FolderPath, Constants.MsgRomPath);
        _msgService.ExtractFromMsgDat(msgPath, Path.Combine(modInfo.FolderPath, Constants.MsgFolderPath));
        File.Delete(msgPath);
    }

    public void Commit(ModInfo modInfo, string path)
    {
        string currentModFolder = modInfo.FolderPath;
        string msgTmpFile = Path.GetTempFileName();
        _msgService.CreateMsgDat(Path.Combine(currentModFolder, Constants.MsgFolderPath), msgTmpFile);

        using (var nds = _ndsFactory(path))
        {
            nds.InsertFixedLengthFile(Constants.PokemonRomPath, Path.Combine(currentModFolder, Constants.PokemonRomPath));
            nds.InsertFixedLengthFile(Constants.MoveRomPath, Path.Combine(currentModFolder, Constants.MoveRomPath));
            nds.InsertFixedLengthFile(Constants.AbilityRomPath, Path.Combine(currentModFolder, Constants.AbilityRomPath));
            nds.InsertFixedLengthFile(Constants.WarriorSkillRomPath, Path.Combine(currentModFolder, Constants.WarriorSkillRomPath));
            nds.InsertFixedLengthFile(Constants.GimmickRomPath, Path.Combine(currentModFolder, Constants.GimmickRomPath));
            nds.InsertFixedLengthFile(Constants.BuildingRomPath, Path.Combine(currentModFolder, Constants.BuildingRomPath));
            nds.InsertFixedLengthFile(Constants.ItemRomPath, Path.Combine(currentModFolder, Constants.ItemRomPath));
            nds.InsertFixedLengthFile(Constants.KingdomRomPath, Path.Combine(currentModFolder, Constants.KingdomRomPath));
            nds.InsertFixedLengthFile(Constants.MoveRangeRomPath, Path.Combine(currentModFolder, Constants.MoveRangeRomPath));
            nds.InsertFixedLengthFile(Constants.EventSpeakerRomPath, Path.Combine(currentModFolder, Constants.EventSpeakerRomPath));
            nds.InsertFixedLengthFile(Constants.BaseBushouMaxSyncTableRomPath, Path.Combine(currentModFolder, Constants.BaseBushouMaxSyncTableRomPath));
            nds.InsertFixedLengthFile(Constants.BaseBushouRomPath, Path.Combine(currentModFolder, Constants.BaseBushouRomPath));
            nds.InsertFixedLengthFile(Constants.MapRomPath, Path.Combine(currentModFolder, Constants.MapRomPath));
            nds.InsertFixedLengthFile(Constants.GimmickRangeRomPath, Path.Combine(currentModFolder, Constants.GimmickRangeRomPath));
            nds.InsertFixedLengthFile(Constants.MoveEffectRomPath, Path.Combine(currentModFolder, Constants.MoveEffectRomPath));
            nds.InsertFixedLengthFile(Constants.GimmickObjectRomPath, Path.Combine(currentModFolder, Constants.GimmickObjectRomPath));

            foreach (var i in EnumUtil.GetValues<ScenarioId>())
            {
                var spPath = Constants.ScenarioPokemonPathFromId(i);
                nds.InsertFixedLengthFile(spPath, Path.Combine(currentModFolder, spPath));
                var swPath = Constants.ScenarioWarriorPathFromId(i);
                nds.InsertFixedLengthFile(swPath, Path.Combine(currentModFolder, swPath));
                var sapPath = Constants.ScenarioAppearPokemonPathFromId(i);
                nds.InsertFixedLengthFile(sapPath, Path.Combine(currentModFolder, sapPath));
                var skPath = Constants.ScenarioKingdomPathFromId(i);
                nds.InsertFixedLengthFile(skPath, Path.Combine(currentModFolder, skPath));
            }

            nds.InsertVariableLengthFile(Constants.MsgRomPath, msgTmpFile);

            foreach (var mapFilePath in Directory.GetFiles(Path.Combine(currentModFolder, "data", "map")))
            {
                string mapRomPath = Path.Combine("data", "map", Path.GetFileName(mapFilePath));
                nds.InsertVariableLengthFile(mapRomPath, mapFilePath);
            }
        }
    }
}
