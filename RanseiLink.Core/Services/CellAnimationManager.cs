using FluentResults;

namespace RanseiLink.Core.Services;

public interface ICellAnimationManager
{
    void ClearOverride(AnimationTypeId type, int id);
    void Export(AnimationTypeId type, int id, string outputFolder, CellAnimationSerialiser.Format format);
    (DataFile AnimationLink, DataFile? BackgroundLink) GetDataFile(AnimationTypeId type, int id);
    Result Import(AnimationTypeId type, int id, string? animationXml, string? backgroundImg);
    void SetOverride(AnimationTypeId type, int id, string? animationLink = null, string? backgroundLink = null);
}

public class CellAnimationManager(IOverrideDataProvider overrideDataProvider) : ICellAnimationManager
{

    private readonly IOverrideDataProvider _overrideDataProvider = overrideDataProvider;

    public (DataFile AnimationLink, DataFile? BackgroundLink) GetDataFile(AnimationTypeId type, int id)
    {
        var info = AnimationTypeInfoResource.Get(type);
        var anim = _overrideDataProvider.GetDataFile(info.AnimationRelativePath(id));
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
            _overrideDataProvider.SetOverride(info.AnimationRelativePath(id), animationLink);
        }
        var bgPath = info.BackgroundRelativePath(id);
        if (bgPath != null && backgroundLink != null)
        {
            _overrideDataProvider.SetOverride(bgPath, backgroundLink);
        }
    }

    /// <summary>
    /// If the default format is oneImagePerBank, then you can choose
    /// </summary>
    public void Export(AnimationTypeId type, int id, string outputFolder, CellAnimationSerialiser.Format format)
    {
        Directory.CreateDirectory(outputFolder);
        var info = AnimationTypeInfoResource.Get(type);
        var animFile = _overrideDataProvider.GetDataFile(info.AnimationRelativePath(id));
        var animExists = File.Exists(animFile.File);
        var bgRelPath = info.BackgroundRelativePath(id);
        if (bgRelPath != null)
        {
            var bgFile = _overrideDataProvider.GetDataFile(bgRelPath);
            CellAnimationSerialiser.Export(new(info.Prt), format, outputFolder, bgFile.File, animExists ? animFile.File : null);
        }
        else if (animExists)
        {
            CellAnimationSerialiser.ExportAnimation(new(info.Prt), outputFolder, animFile.File, info.Width, info.Height, format);
        }
    }

    public Result Import(AnimationTypeId type, int id, string? animationXml, string? backgroundImg)
    {
        var info = AnimationTypeInfoResource.Get(type);
        var animFile = _overrideDataProvider.GetDataFile(info.AnimationRelativePath(id));
        var animExists = File.Exists(animFile.File);
        var bgRelPath = info.BackgroundRelativePath(id);
        var animOut = Path.GetTempFileName();
        var bgOut = Path.GetTempFileName();

        var importResult = CellAnimationSerialiser.Import(
                new(info.Prt),
                animationXml: animationXml,
                animLinkFile: animExists ? animFile.File : null,
                outputAnimLinkFile: animOut,
                bgImage: backgroundImg,
                bgLinkFile: bgRelPath != null ? _overrideDataProvider.GetDataFile(bgRelPath).File : null,
                outputBgLinkFile: bgOut
                );
        if (importResult.IsFailed)
        {
            return importResult;
        }

        ClearOverride(type, id);
        SetOverride(type, id, animOut, bgOut);

        File.Delete(animOut);
        File.Delete(bgOut);

        return Result.Ok();

    }
}


