#nullable enable
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using RanseiLink.Core.Services;

namespace RanseiLink.XP.Services;

internal static class DialogLocatorExtensions
{
    public static void Register<TDialog, TViewModel>(this RegistryDialogLocator locator) where TDialog : Window
    {
        locator.Register(typeof(TDialog), typeof(TViewModel));
    }
}

internal class DialogService(IDialogLocator locator) : IAsyncDialogService
{
    public Task ShowDialog(object dialogViewModel)
    {
        var type = locator.Locate(dialogViewModel);
        if (type == null)
        {
            throw new TypeLoadException($"Dialog type for view model '{dialogViewModel.GetType().FullName}' not found.");
        }
        if (!typeof(Window).IsAssignableFrom(type))
        {
            throw new Exception($"Dialog type '{type.FullName}' is not assignable to Window");
        }

        var dialog = Activator.CreateInstance(type) as Window;
        if (dialog == null)
        {
            throw new Exception($"Failed to activate dialog type '{type.FullName}'");
        }
        dialog.DataContext = dialogViewModel;
        return dialog.ShowDialog(App.MainWindow);
    }

    public Task<MessageBoxResult> ShowMessageBox(MessageBoxSettings options)
    {
        return Dialogs.MessageBoxDialog.ShowDialog(options, App.MainWindow);
    }

    private TopLevel GetTopLevel() => TopLevel.GetTopLevel(App.MainWindow) ?? throw new Exception("Failed to get main window top level");

    public async Task<string[]?> ShowOpenFileDialog(OpenFileDialogSettings settings)
    {
        var topLevel = GetTopLevel();

        var avaSettings = new FilePickerOpenOptions
        {
            Title = settings.Title,
            AllowMultiple = settings.AllowMultiple,
            FileTypeFilter = settings.Filters.Select(x => 
                new FilePickerFileType(x.Name) { Patterns = x.Extensions.Select(ExtToFilter).ToList() }
                ).ToList(),
        };

        string[] files = (await topLevel.StorageProvider.OpenFilePickerAsync(avaSettings))
            .Select(x => x.TryGetLocalPath() ?? throw new Exception("Try get local path failed. This should only happen on web and mobile platforms."))
            .ToArray();

        return files;
    }

    public async Task<string?> ShowOpenFolderDialog(OpenFolderDialogSettings settings)
    {
        var topLevel = GetTopLevel();

        var avaSettings = new FolderPickerOpenOptions
        {
            Title = settings.Title,
            AllowMultiple = false
        };

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(avaSettings);

        if (folders.Count == 0)
        {
            return null;
        }
        return folders[0].TryGetLocalPath();
    }

    private static string ExtToFilter(string ext)
    {
        if (!ext.StartsWith("."))
        {
            ext = "." + ext;
        }
        return "*" + ext;
    }

    public async Task<string?> ShowSaveFileDialog(SaveFileDialogSettings settings)
    {
        var topLevel = GetTopLevel();

        var avaSettings = new FilePickerSaveOptions
        {
            Title = settings.Title,
            DefaultExtension = settings.DefaultExtension,
            SuggestedFileName = settings.SuggestedFileName,
            FileTypeChoices = settings.Filters.Select(x =>
                new FilePickerFileType(x.Name) { Patterns = x.Extensions.Select(ExtToFilter).ToList() }
                ).ToList(),
        };

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(avaSettings);
        return file?.Path.AbsolutePath;
    }

    public Task ProgressDialog(Action<IProgress<ProgressInfo>> work, bool delayOnCompletion = true)
    {
        var progressWindow = new Dialogs.LoadingDialog();

        var progressReporter = new Progress<ProgressInfo>(info =>
        {
            if (info.StatusText != null)
            {
                progressWindow.HeaderTextBlock.Text = info.StatusText;
            }
            if (info.MaxProgress != null)
            {
                progressWindow.ProgressBar.Maximum = (int)info.MaxProgress;
            }
            if (info.Progress != null)
            {
                progressWindow.ProgressBar.Value = (int)info.Progress;
            }
            if (info.IsIndeterminate != null)
            {
                progressWindow.ProgressBar.IsIndeterminate = (bool)info.IsIndeterminate;
            }
        });

        progressWindow.Loaded += async (s, e) =>
        {
            await Task.Run(() => work(progressReporter));
            if (delayOnCompletion)
            {
                await Task.Delay(500);
            }
            progressWindow.Close();
        };

        return progressWindow.ShowDialog(App.MainWindow);
    }
}
