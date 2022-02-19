using RanseiLink.Core;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Services;
using RanseiLink.Services;
using System.IO;
using System.Linq;

namespace RanseiLink.ViewModels;

public class MapSelectorViewModel : SelectorViewModelBase<MapId, PSLM, MapViewModel>
{
    private readonly IServiceContainer _container;
    private readonly IEditorContext _editorContext;
    private readonly IDialogService _dialogService;
    public MapSelectorViewModel(IServiceContainer container, IEditorContext context)
        : base(container, context.DataService.Map, context.DataService.MapName.GetMaps().ToArray())
    {
        _container = container;
        _editorContext = context;
        _dialogService = _container.Resolve<IDialogService>();
        Selected = Items.First();
        SupportsImportExportFile = true;
    }

    protected override MapViewModel NewViewModel(PSLM model) => new(_container, _editorContext, model, Selected);

    protected override void ExportFile()
    {
        string exportFolder = FileUtil.DesktopDirectory;
        string exportFile = FileUtil.MakeUniquePath(Path.Combine(exportFolder, Selected.ToExternalFileName()));

        using (var bw = new BinaryWriter(File.OpenWrite(exportFile)))
        {
            _currentModel.WriteTo(bw);
        }

        _dialogService.ShowMessageBox(MessageBoxArgs.Ok("Map Exported", $"Map exported to desktop:\n'{exportFile}'"));
    }

    protected override void ImportFile()
    {
        if (!_dialogService.RequestFile("Import PSL Map", PSLM.ExternalFileExtension, "PSL Map (.pslm) | *.pslm", out string file))
        {
            return;
        }

        using var br = new BinaryReader(File.OpenRead(file));
        _currentModel = new PSLM(br);
        NestedViewModel = NewViewModel(_currentModel);
    }

}
