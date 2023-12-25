using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;
using RanseiLink.GuiCore.DragDrop;
using System.IO.Compression;
using System.Xml.Linq;

namespace RanseiLink.GuiCore.ViewModels;

public class ModImportViewModel : ViewModelBase, IModalDialogViewModel<bool>
{
    private readonly IAsyncDialogService _dialogService;
    public ModImportViewModel(IAsyncDialogService dialogService, IFileDropHandlerFactory fdhFactory)
    {
        _dialogService = dialogService;
        ModDropHandler = fdhFactory.NewModDropHandler();
        ModDropHandler.FileDropped += SafeSetAndPreviewFile;
        _file = string.Empty;

        FilePickerCommand = new RelayCommand(async () =>
        {
            var file = await _dialogService.RequestModFile();
            if (!string.IsNullOrEmpty(file))
            {
                SafeSetAndPreviewFile(file);
            }
        });
    }

    private void SafeSetAndPreviewFile(string file)
    {
        try
        {
            ModInfo? modInfo;
            using (ZipArchive zip = ZipFile.OpenRead(file))
            {
                ZipArchiveEntry? entry = zip.GetEntry(ModManager.ModInfoFileName);
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

            ModInfo = modInfo;
            File = file;
        }
        catch (Exception e)
        {
            _dialogService.ShowMessageBox(MessageBoxSettings.Ok(
                title: "Unable to load mod",
                message: e.Message,
                type: MessageBoxType.Error
            ));
        }
    }

    public bool Result { get; private set; }
    public void OnClosing(bool result)
    {
        Result = result;
    }

    public ICommand FilePickerCommand { get; }

    public IFileDropHandler ModDropHandler { get; }

    private string _file;
    public string File
    {
        get => _file;
        set
        {
            if (RaiseAndSetIfChanged(ref _file, value))
            {
                RaisePropertyChanged(nameof(OkEnabled));
            }
        }
    }

    public bool OkEnabled => _file != null && System.IO.File.Exists(_file);

    private ModInfo? _modInfo;
    public ModInfo? ModInfo
    {
        get => _modInfo;
        set => RaiseAndSetIfChanged(ref _modInfo, value);
    }
}
