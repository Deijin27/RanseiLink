using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;
using RanseiLink.DragDrop;
using System;
using System.IO;
using System.IO.Compression;
using System.Windows.Input;
using System.Xml.Linq;

namespace RanseiLink.ViewModels;

public class ModImportViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    public ModImportViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
        ModInfo = new ModInfo();
        ModDropHandler = new ModDropHandler();
        ModDropHandler.FileDropped += SafeSetAndPreviewFile;

        FilePickerCommand = new RelayCommand(() =>
        {
            if (_dialogService.RequestModFile(out string file))
            {
                SafeSetAndPreviewFile(file);
            }
        });
    }

    private void SafeSetAndPreviewFile(string file)
    {
        try
        {
            ModInfo modInfo;
            using (ZipArchive zip = ZipFile.OpenRead(file))
            {
                ZipArchiveEntry entry = zip.GetEntry(ModService.ModInfoFileName);
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
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                title: "Unable to load mod",
                message: e.Message,
                type: MessageBoxType.Error
            ));
        }
    }

    public ICommand FilePickerCommand { get; }

    public ModDropHandler ModDropHandler { get; }

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

    private ModInfo _modInfo;
    public ModInfo ModInfo
    {
        get => _modInfo;
        set => RaiseAndSetIfChanged(ref _modInfo, value);
    }
}
