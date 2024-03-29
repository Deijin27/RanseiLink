using RanseiLink.Core.Models;

namespace RanseiLink.GuiCore.ViewModels;

public class CopyPasteViewModel
{
    private readonly IClipboardService _clipboardService;
    private readonly IAsyncDialogService _dialogService;

    public CopyPasteViewModel(IClipboardService clipboardService, IAsyncDialogService dialogService)
    {
        CopyCommand = new RelayCommand(Copy);
        PasteCommand = new RelayCommand(Paste);
        _clipboardService = clipboardService;
        _dialogService = dialogService;
    }

    public ICommand CopyCommand { get; }
    public ICommand PasteCommand { get; }

    public IDataWrapper? Model { get; set; }


    public event EventHandler? ModelPasted;

    private async void Copy()
    {
        if (Model == null)
        {
            return;
        }
        var text = Model.Serialize();
        var success = await _clipboardService.SetText(text);
        if (success)
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Copied", "Data of this entry is copied to your clipboard"));
        }
        else
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Could not Copy", "Failed to set clipboard text", MessageBoxType.Error));
        }
    }

    private async void Paste()
    {
        if (Model == null)
        {
            return;
        }
        
        var text = await _clipboardService.GetText();
        if (text == null)
        {
            return;
        }
        var result = await _dialogService.ShowMessageBox(new MessageBoxSettings(
            "Paste Data",
            "You want to replace this entry's data with the clipboard contents?",
            [
                new("Yes", MessageBoxResult.Yes),
                new("Cancel", MessageBoxResult.No)
            ]
            ));
        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        if (!Model.TryDeserialize(text))
        {
            await _dialogService.ShowMessageBox(MessageBoxSettings.Ok("Could not Paste", "Failed to load data from clipboard", MessageBoxType.Error));
            return;
        }

        ModelPasted?.Invoke(this, EventArgs.Empty);
    }
}
