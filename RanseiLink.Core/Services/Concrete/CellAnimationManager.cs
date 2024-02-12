using FluentResults;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;

namespace RanseiLink.Core.Services.Concrete;

public class CellAnimationManager(IOverrideDataProvider overrideDataProvider) : ICellAnimationManager
{

    private readonly IOverrideDataProvider _overrideDataProvider = overrideDataProvider;

    public (DataFile? AnimationLink, DataFile? BackgroundLink) GetDataFile(AnimationTypeId type, int id)
    {
        var info = AnimationTypeInfoResource.Get(type);
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

    public void SetOverride(AnimationTypeId type, int id, string? animationLink = null, string? backgroundLink = null)
    {
        var info = AnimationTypeInfoResource.Get(type);
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
    public void Export(AnimationTypeId type, int id, string outputFolder, RLAnimationFormat format)
    {
        Directory.CreateDirectory(outputFolder);
        var info = AnimationTypeInfoResource.Get(type);
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

    public Result ImportBackgroundOnly(AnimationTypeId type, int id, string backgroundImg)
    {
        var (_, bgLinkFile) = GetDataFile(type, id);
        if (bgLinkFile == null)
        {
            return Result.Fail($"Cannot import background '{backgroundImg}' because animation type {type} does not support backgrounds");
        }
        var bgOut = Path.GetTempFileName();
        var importResult = CellAnimationSerialiser.ImportBackground(
            bgImage: backgroundImg,
            bgLinkFile: bgLinkFile.File,
            outputBgLinkFile: bgOut
            );
        if (importResult.IsFailed)
        {
            return importResult.ToResult();
        }

        ClearOverride(type, id);
        SetOverride(type, id, null, bgOut);

        File.Delete(bgOut);

        return Result.Ok();
    }

    /// <summary>
    /// Imports animation, and if applicable background
    /// </summary>
    public Result ImportAnimAndBackground(AnimationTypeId type, int id, string animationXml)
    {
        var info = AnimationTypeInfoResource.Get(type);
        var animFile = _overrideDataProvider.GetDataFile(info.AnimationRelativePath(id));
        var animExists = File.Exists(animFile.File);
        if (!animExists)
        {
            // this particular slot has background but not animation
            return Result.Fail($"Importing animation not permitted for type '{type}' id '{id}'");
        }
        var bgRelPath = info.BackgroundRelativePath(id);

        Result importResult;
        var animOut = Path.GetTempFileName();
        string? bgOut;
        if (bgRelPath == null)
        {
            bgOut = null;
            // there is no background associated with this animation
            importResult = CellAnimationSerialiser.ImportAnimation(
                new CellImageSettings(info.Prt),
                animLinkFile: animFile.File,
                animationXml: animationXml,
                width: -1,
                height: -1,
                outputAnimLinkFile: animOut
                );
        }
        else
        {
            bgOut = Path.GetTempFileName();
            // this has both background and animation
            importResult = CellAnimationSerialiser.ImportAnimAndBackground(
                new CellImageSettings(info.Prt),
                animationXml: animationXml,
                animLinkFile: animFile.File,
                outputAnimLinkFile: animOut,
                bgLinkFile: _overrideDataProvider.GetDataFile(bgRelPath).File,
                outputBgLinkFile: bgOut
                );
        }

        if (importResult.IsFailed)
        {
            return importResult;
        }

        ClearOverride(type, id);
        SetOverride(type, id, animOut, bgOut);

        if (File.Exists(animOut))
        {
            File.Delete(animOut);
        }

        if (File.Exists(bgOut))
        {
            File.Delete(bgOut);
        }

        return Result.Ok();
    }
}


