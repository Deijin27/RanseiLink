using RanseiLink.Core;
using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System;
using System.IO;

namespace RanseiLink.Services.Concrete;

public class MapManager : IMapManager
{
    private const string _pacExt = ".pac";
    private const string _objExt = ".obj";
    private const string _pslmExt = ".pslm";
    private static readonly FileDialogFilter _pacFilter = new("Pokemon Conquest 3D Archive (.pac)", ".pac");
    private static readonly FileDialogFilter _objFilter = new("Wavefront OBJ (.obj)", ".obj");
    private static readonly FileDialogFilter _pslmFilter = new("Pokemon Conquest Map Data (.pslm)", ".pslm");
    
    private static string ResolveMapModelFileNameWithoutExt(MapId id)
    {
        return $"MAP{id.Map.ToString().PadLeft(2, '0')}_{id.Variant.ToString().PadLeft(2, '0')}";
    }

    private static string ResolveMapModelFilePath(MapId id)
    {
        return Path.Combine("graphics", "ikusa_map", ResolveMapModelFileNameWithoutExt(id) + _pacExt);
    }

    private readonly IDialogService _dialogService;
    private readonly IOverrideDataProvider _overrideDataProvider;
    private readonly IMapService _mapService;
    public MapManager(IDialogService dialogService, IOverrideDataProvider overrideDataProvider, IMapService mapService)
    {
        _dialogService = dialogService;
        _overrideDataProvider = overrideDataProvider;
        _mapService = mapService;
    }

    public bool RevertModelToDefault(MapId id)
    {
        if (!IsOverriden(id))
        {
            return false;
        }
        var result = _dialogService.ShowMessageBox(new MessageBoxSettings(
            $"Revert map 3D Model {id} to default?",
            "Confirm to permanently delete the internally stored 3D model which overrides the default",
            new MessageBoxButton[]
            {
                new("Cancel", MessageBoxResult.Cancel),
                new("Yes, revert to default", MessageBoxResult.Yes)
            }
            ));

        if (result != MessageBoxResult.Yes)
        {
            return false;
        }
        _overrideDataProvider.ClearOverride(ResolveMapModelFilePath(id));
        return true;
    }

    public bool ImportPac(MapId id)
    {
        var result = _dialogService.ShowOpenSingleFileDialog(new("Choose a PAC file", _pacFilter));
        if (string.IsNullOrEmpty(result))
        {
            return false;
        }

        _overrideDataProvider.SetOverride(ResolveMapModelFilePath(id), result);
        return true;
    }

    public bool ExportPac(MapId id)
    {
        if (!_overrideDataProvider.IsDefaultsPopulated() && !IsOverriden(id))
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Cannot export", "You must populate default graphics."));
            return false;
        }
        var dataFile = _overrideDataProvider.GetDataFile(ResolveMapModelFilePath(id));
        var exportFile = _dialogService.ShowSaveFileDialog(new SaveFileDialogSettings
        {
            Title = "Export Map PAC",
            DefaultExtension = ".pac",
            InitialFileName = Path.GetFileName(dataFile.File),
            Filters = new() { new("Pokemon Conquest 3D Archive (.pac)", ".pac") }
        });
        if (string.IsNullOrEmpty(exportFile))
        {
            return false;
        }

        exportFile = FileUtil.MakeUniquePath(exportFile);
        File.Copy(dataFile.File, exportFile);
        return true;
    }

    public bool ImportObj(MapId id)
    {
        var objFile = _dialogService.ShowOpenSingleFileDialog(new("Choose an OBJ file", _objFilter));
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
                ModelName: ResolveMapModelFileNameWithoutExt(id),
                DestinationFolder: tempFolder,
                ModelGenerator: new MapModelGenerator()
            );

            var result = ModelExtractorGenerator.GenerateModel(settings);
            if (result.IsSuccess)
            {
                PAC.Pack(tempFolder, tempPac);
                _overrideDataProvider.SetOverride(ResolveMapModelFilePath(id), tempPac);
                success = true;
            }
            else
            {
                _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Error importing map", result.ToString(), MessageBoxType.Error));
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Unhandled exception importing map", ex.ToString(), MessageBoxType.Error));
        }
        finally
        {
            Directory.Delete(tempFolder, true);
            File.Delete(tempPac);
        }

        return success;
    }

    public bool ExportObj(MapId id)
    {
        if (!_overrideDataProvider.IsDefaultsPopulated() && !IsOverriden(id))
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Cannot export", "You must populate default graphics."));
            return false;
        }
        var destinationFolder = _dialogService.ShowOpenFolderDialog(new("Choose a folder in which to place the exported OBJ"));
        if (string.IsNullOrEmpty(destinationFolder))
        {
            return false;
        }

        var dataFile = _overrideDataProvider.GetDataFile(ResolveMapModelFilePath(id));
        var exportFolder = Path.Combine(destinationFolder, Path.GetFileNameWithoutExtension(dataFile.File));
        exportFolder = FileUtil.MakeUniquePath(exportFolder);

        try
        {
            var result = ModelExtractorGenerator.ExtractModelFromPac(dataFile.File, exportFolder);
            if (result.IsFailed)
            {
                _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Error exporting map as obj", result.ToString(), MessageBoxType.Error));
                return false;
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Unhandled exception importing map", ex.ToString(), MessageBoxType.Error));
            return false;
        }

        return true;
    }

    public bool ExportPslm(MapId id)
    {
        var destinationFolder = _dialogService.ShowOpenFolderDialog(new OpenFolderDialogSettings
        {
            Title = "Choose a folder in which to place the exported PSLM"
        });
        if (string.IsNullOrEmpty(destinationFolder))
        {
            return false;
        }

        var internalFile = _mapService.GetFilePath(id);
        string exportTo = FileUtil.MakeUniquePath(Path.Combine(destinationFolder, id.ToExternalFileName()));

        File.Copy(internalFile, exportTo);

        return true;
    }

    public bool ImportPslm(MapId id)
    {
        var pslmFile = _dialogService.ShowOpenSingleFileDialog(new("Choose a PSLM file", _pslmFilter));
        if (string.IsNullOrEmpty(pslmFile))
        {
            return false;
        }
        var internalFile = _mapService.GetFilePath(id);
        File.Copy(pslmFile, internalFile, true);
        return true;
    }

    public bool IsOverriden(MapId id)
    {
        var info = _overrideDataProvider.GetDataFile(ResolveMapModelFilePath(id));
        return info.IsOverride;
    }

    
}

