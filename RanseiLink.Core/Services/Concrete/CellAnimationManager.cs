using FluentResults;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;

namespace RanseiLink.Core.Services.Concrete;

public class CellAnimationManager(IOverrideDataProvider overrideDataProvider) : ICellAnimationManager
{

    private readonly IOverrideDataProvider _overrideDataProvider = overrideDataProvider;

    public (DataFile? AnimationLink, DataFile? BackgroundLink) GetDataFile(AnimationTypeInfo info, int id)
    {
        var anim = _overrideDataProvider.GetDataFile(info.AnimationRelativePath(id));
        if (!File.Exists(anim.File))
        {
            anim = null;
        }
        var bgPath = info.BackgroundRelativePath(id);
        DataFile? background = null;
        if (bgPath != null)
        {
            background = _overrideDataProvider.GetDataFile(bgPath);
        }
        return (anim, background);
    }

    public void ClearOverride(AnimationTypeInfo info, int id)
    {
        _overrideDataProvider.ClearOverride(info.AnimationRelativePath(id));
        var bgPath = info.BackgroundRelativePath(id);
        if (bgPath != null)
        {
            _overrideDataProvider.ClearOverride(bgPath);
        }
    }

    public void SetOverride(AnimationTypeInfo info, int id, string? animationLink = null, string? backgroundLink = null)
    {
        if (animationLink != null)
        {
            FileUtil.EnsureFileIsNotEmpty(animationLink);
            _overrideDataProvider.SetOverride(info.AnimationRelativePath(id), animationLink);
        }
        var bgPath = info.BackgroundRelativePath(id);
        if (bgPath != null && backgroundLink != null)
        {
            FileUtil.EnsureFileIsNotEmpty(backgroundLink);
            _overrideDataProvider.SetOverride(bgPath, backgroundLink);
        }
    }

    /// <summary>
    /// If the default format is oneImagePerBank, then you can choose
    /// </summary>
    public void Export(AnimationTypeInfo info, int id, string outputFolder, RLAnimationFormat format)
    {
        Directory.CreateDirectory(outputFolder);
        var animFile = _overrideDataProvider.GetDataFile(info.AnimationRelativePath(id));
        var animExists = File.Exists(animFile.File);
        var bgRelPath = info.BackgroundRelativePath(id);
        var settings = new CellImageSettings(info.Prt);
        if (bgRelPath != null)
        {
            var bgFile = _overrideDataProvider.GetDataFile(bgRelPath);
            CellAnimationSerialiser.Export(settings, format, outputFolder, bgFile.File, animExists ? animFile.File : null);
        }
        else if (animExists)
        {
            CellAnimationSerialiser.ExportAnimationOnly(settings, outputFolder, animFile.File, info.Width, info.Height, format, null);
        }
    }
}


