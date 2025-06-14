using RanseiLink.Core.RomFs;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Enums;
using FluentResults;

namespace RanseiLink.Core.Services.Concrete;

public class FallbackDataProvider : IFallbackDataProvider
{
    private readonly RomFsFactory _ndsFactory;
    public FallbackDataProvider(RomFsFactory ndsFactory)
    {
        _ndsFactory = ndsFactory;
    }
    public bool IsDefaultsPopulated(ConquestGameCode gameCode) => Directory.Exists(Path.Combine(Constants.DefaultDataFolder(gameCode), Constants.GraphicsFolderPath));

    public Result Populate(string ndsFile, IProgress<ProgressInfo>? progress = null)
    {
        ConquestGameCode gc;
        using (var br = new BinaryReader(File.OpenRead(ndsFile)))
        {
            var header = new NdsHeader(br);
            if (!Enum.TryParse(header.GameCode, out gc))
            {
                return Result.Fail($"Unexpected game code '{header.GameCode}', this may not be a conquest rom, or it may be a culture we don't know of yet");
            }
        }

        string defaultDataFolder = Constants.DefaultDataFolder(gc);
        // reset the graphics folder
        if (IsDefaultsPopulated(gc))
        {
            progress?.Report(new ProgressInfo(StatusText: "Deleting Existing...", IsIndeterminate: true));
            Directory.Delete(defaultDataFolder, true);
        }
        Directory.CreateDirectory(defaultDataFolder);

        // populate
        progress?.Report(new ProgressInfo(StatusText: "Extracting files from rom...", IsIndeterminate: true));
        using (var nds = _ndsFactory(ndsFile))
        {
            nds.ExtractCopyOfDirectory(Constants.GraphicsFolderPath, defaultDataFolder);
        }
            
        var infos = GraphicsInfoResource.All;
        progress?.Report(new ProgressInfo(StatusText: "Converting Images...", IsIndeterminate: false, MaxProgress: infos.Count));
        int count = 0;
        var context = new PopulateDefaultsContext(defaultDataFolder);
        Parallel.ForEach(infos, gfxInfo =>
        {
            gfxInfo.ProcessExportedFiles(context);
            progress?.Report(new ProgressInfo(Progress: ++count));
        });
        progress?.Report(new ProgressInfo(StatusText: "Done!"));

        return Result.Ok();
    }

    public List<SpriteFile> GetAllSpriteFiles(ConquestGameCode gc, SpriteType type)
    {
        string defaultDataFolder = Constants.DefaultDataFolder(gc);
        var info = GraphicsInfoResource.Get(type);
        return info.GetAllSpriteFiles(isOverride: false, defaultDataFolder);  
    }

    public SpriteFile GetSpriteFile(ConquestGameCode gc, SpriteType type, int id)
    {
        string defaultDataFolder = Constants.DefaultDataFolder(gc);
        var relPath = GraphicsInfoResource.Get(type).GetRelativeSpritePath(id);
        return new SpriteFile(type, id, relPath, Path.Combine(defaultDataFolder, relPath), false);
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