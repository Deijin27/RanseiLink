using RanseiLink.Core.Graphics;
using System.IO;

namespace RanseiLink.Core.Services;

public class CellAnimationManager
{

    private readonly IOverrideDataProvider _overrideDataProvider;

    public CellAnimationManager(IOverrideDataProvider overrideDataProvider)
    {
        _overrideDataProvider = overrideDataProvider;
    }

    public DataFile GetDataFile(AnimationTypeId type, int id)
    {
        var info = AnimationTypeInfoResource.Get(type);
        return _overrideDataProvider.GetDataFile(info.AnimationRelativePath(id));
    }

    public void ClearOverride(AnimationTypeId type, int id)
    {
        var info = AnimationTypeInfoResource.Get(type);
        _overrideDataProvider.ClearOverride(info.AnimationRelativePath(id));
        var bgPath = info.BackgroundRelativePath(id);
        if (bgPath != null)
        {
            _overrideDataProvider.ClearOverride(bgPath);
        }
    }

    public void SetOverride(AnimationTypeId type, int id, string file, string? backgroundFile = null)
    {
        var info = AnimationTypeInfoResource.Get(type);
        _overrideDataProvider.SetOverride(info.AnimationRelativePath(id), file);
        var bgPath = info.BackgroundRelativePath(id);
        if (bgPath != null && backgroundFile != null)
        {
            _overrideDataProvider.SetOverride(bgPath, backgroundFile);
        }
    }

    public void Export(AnimationTypeId type, int id, string outputFolder)
    {
        var info = AnimationTypeInfoResource.Get(type);
        var animFile = _overrideDataProvider.GetDataFile(info.AnimationRelativePath(id));
        var animExists = File.Exists(animFile.File);
        var bgRelPath = info.BackgroundRelativePath(id);
        if (bgRelPath != null)
        {
            var bgFile = _overrideDataProvider.GetDataFile(bgRelPath);
            CellAnimationSerialiser.Export(info.Prt, info.Format, outputFolder, outputFolder, bgFile.File);
        }
    }
}


