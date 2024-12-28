#nullable enable
using RanseiLink.Core;
using RanseiLink.Core.Archive;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
namespace RanseiLink.GuiCore.Services.Concrete;

public class MapManager(IAsyncDialogService dialogService, IOverrideDataProvider overrideDataProvider, IMapService mapService) : IMapManager
{
    
    private static readonly FileDialogFilter _pacFilter = new($"Pokemon Conquest 3D Archive ({Core.Services.Constants.PacExt})", Core.Services.Constants.PacExt);
    private static readonly FileDialogFilter _objFilter = new($"Wavefront OBJ ({Core.Services.Constants.ObjExt})", Core.Services.Constants.ObjExt);
    private static readonly FileDialogFilter _pslmFilter = new($"Pokemon Conquest Map Data ({Core.Services.Constants.PslmExt})", Core.Services.Constants.PslmExt);
    
    

    public async Task<bool> RevertModelToDefault(MapId id)
    {
        if (!IsOverriden(id))
        {
            return false;
        }
        var result = await dialogService.ShowMessageBox(new(
            $"Revert map 3D Model {id} to default?",
            "Confirm to permanently delete the internally stored 3D model which overrides the default",
            [
                new("Cancel", MessageBoxResult.Cancel),
                new("Yes, revert to default", MessageBoxResult.Yes)
            ]
            ));

        if (result != MessageBoxResult.Yes)
        {
            return false;
        }
        overrideDataProvider.ClearOverride(Core.Services.Constants.ResolveMapModelFilePath(id));
        return true;
    }

    public async Task<bool> ImportPac(MapId id)
    {
        var result = await dialogService.ShowOpenSingleFileDialog(new("Choose a PAC file", _pacFilter));
        if (string.IsNullOrEmpty(result))
        {
            return false;
        }

        overrideDataProvider.SetOverride(Core.Services.Constants.ResolveMapModelFilePath(id), result);
        return true;
    }

    public async Task<bool> ExportPac(MapId id)
    {
        if (!overrideDataProvider.IsDefaultsPopulated() && !IsOverriden(id))
        {
            await dialogService.ShowMessageBox(MessageBoxSettings.Ok("Cannot export", "You must populate default graphics."));
            return false;
        }
        var dataFile = overrideDataProvider.GetDataFile(Core.Services.Constants.ResolveMapModelFilePath(id));
        var exportFile = await dialogService.ShowSaveFileDialog(new SaveFileDialogSettings
        {
            Title = "Export Map PAC",
            DefaultExtension = ".pac",
            InitialFileName = Path.GetFileName(dataFile.File),
            Filters = [new("Pokemon Conquest 3D Archive (.pac)", ".pac")]
        });
        if (string.IsNullOrEmpty(exportFile))
        {
            return false;
        }

        exportFile = FileUtil.MakeUniquePath(exportFile);
        File.Copy(dataFile.File, exportFile);
        return true;
    }

    public async Task<bool> ImportObj(MapId id)
    {
        var objFile = await dialogService.ShowOpenSingleFileDialog(new("Choose an OBJ file", _objFilter));
        if (string.IsNullOrEmpty(objFile))
        {
            return false;
        }

        var tempFolder = FileUtil.GetTemporaryDirectory();
        string tempPac = Path.GetTempFileName();

        bool success = false;

        try
        {
            var settings = new ModelExtractorGenerator.GenerationSettings
            (
                ObjFile: objFile,
                ModelName: Core.Services.Constants.ResolveMapModelFileNameWithoutExt(id),
                DestinationFolder: tempFolder,
                ModelGenerator: new MapModelGenerator()
            );

            var result = ModelExtractorGenerator.GenerateModel(settings);
            if (result.IsSuccess)
            {
                PAC.Pack(tempFolder, tempPac, sharedFileCount: 0);
                overrideDataProvider.SetOverride(Core.Services.Constants.ResolveMapModelFilePath(id), tempPac);
                success = true;
            }
            else
            {
                await dialogService.ShowMessageBox(MessageBoxSettings.Ok("Error importing map", result.ToString(), MessageBoxType.Error));
            }
        }
        catch (Exception ex)
        {
            await dialogService.ShowMessageBox(MessageBoxSettings.Ok("Unhandled exception importing map", ex.ToString(), MessageBoxType.Error));
        }
        finally
        {
            Directory.Delete(tempFolder, true);
            File.Delete(tempPac);
        }

        return success;
    }

    public async Task<bool> ExportObj(MapId id)
    {
        if (!overrideDataProvider.IsDefaultsPopulated() && !IsOverriden(id))
        {
            await dialogService.ShowMessageBox(MessageBoxSettings.Ok("Cannot export", "You must populate default graphics."));
            return false;
        }
        var destinationFolder = await dialogService.ShowOpenFolderDialog(new("Choose a folder in which to place the exported OBJ"));
        if (string.IsNullOrEmpty(destinationFolder))
        {
            return false;
        }

        var dataFile = overrideDataProvider.GetDataFile(Core.Services.Constants.ResolveMapModelFilePath(id));
        var exportFolder = Path.Combine(destinationFolder, Path.GetFileNameWithoutExtension(dataFile.File));
        exportFolder = FileUtil.MakeUniquePath(exportFolder);

        try
        {
            var result = ModelExtractorGenerator.ExtractModelFromPac(dataFile.File, exportFolder);
            if (result.IsFailed)
            {
                await dialogService.ShowMessageBox(MessageBoxSettings.Ok("Error exporting map as obj", result.ToString(), MessageBoxType.Error));
                return false;
            }
        }
        catch (Exception ex)
        {
            await dialogService.ShowMessageBox(MessageBoxSettings.Ok("Unhandled exception exporting map", ex.ToString(), MessageBoxType.Error));
            return false;
        }

        return true;
    }

    public async Task<bool> ExportObj(GimmickObjectId id, int variant)
    {
        if (!overrideDataProvider.IsDefaultsPopulated() && !IsOverriden(id, variant))
        {
            await dialogService.ShowMessageBox(MessageBoxSettings.Ok("Cannot export", "You must populate default graphics."));
            return false;
        }
        var destinationFolder = await dialogService.ShowOpenFolderDialog(new("Choose a folder in which to place the exported OBJ"));
        if (string.IsNullOrEmpty(destinationFolder))
        {
            return false;
        }

        var dataFile = overrideDataProvider.GetDataFile(Core.Services.Constants.ResolveGimmickModelFilePath(id, variant));
        var exportFolder = Path.Combine(destinationFolder, Path.GetFileNameWithoutExtension(dataFile.File));
        exportFolder = FileUtil.MakeUniquePath(exportFolder);

        try
        {
            var result = ModelExtractorGenerator.ExtractModelFromPac(dataFile.File, exportFolder);
            if (result.IsFailed)
            {
                await dialogService.ShowMessageBox(MessageBoxSettings.Ok("Error exporting gimmick as obj", result.ToString(), MessageBoxType.Error));
                return false;
            }
        }
        catch (Exception ex)
        {
            await dialogService.ShowMessageBox(MessageBoxSettings.Ok("Unhandled exception exporting gimmick", ex.ToString(), MessageBoxType.Error));
            return false;
        }

        return true;
    }

    public async Task<bool> ExportPslm(MapId id)
    {
        var destinationFolder = await dialogService.ShowOpenFolderDialog(new OpenFolderDialogSettings
        {
            Title = "Choose a folder in which to place the exported PSLM"
        });
        if (string.IsNullOrEmpty(destinationFolder))
        {
            return false;
        }

        var internalFile = mapService.GetFilePath(id);
        string exportTo = FileUtil.MakeUniquePath(Path.Combine(destinationFolder, id.ToExternalPslmName()));

        File.Copy(internalFile, exportTo);

        return true;
    }

    public async Task<bool> ImportPslm(MapId id)
    {
        var pslmFile = await dialogService.ShowOpenSingleFileDialog(new("Choose a PSLM file", _pslmFilter));
        if (string.IsNullOrEmpty(pslmFile))
        {
            return false;
        }
        var internalFile = mapService.GetFilePath(id);
        File.Copy(pslmFile, internalFile, true);
        return true;
    }

    public bool IsOverriden(MapId id)
    {
        var info = overrideDataProvider.GetDataFile(Core.Services.Constants.ResolveMapModelFilePath(id));
        return info.IsOverride;
    }

    public bool IsOverriden(GimmickObjectId id, int variant)
    {
        var info = overrideDataProvider.GetDataFile(Core.Services.Constants.ResolveGimmickModelFilePath(id, variant));
        return info.IsOverride;
    }


}

