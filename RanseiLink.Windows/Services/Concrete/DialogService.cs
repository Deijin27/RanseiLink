using RanseiLink.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace RanseiLink.Windows.Services.Concrete;

internal static class DialogLocatorExtensions
{
    public static void Register<TDialog, TViewModel>(this RegistryDialogLocator locator) where TDialog : Window
    {
        locator.Register(typeof(TDialog), typeof(TViewModel));
    }
}

internal class DialogService : IDialogService
{
    private readonly IDialogLocator _locator;
    public DialogService(IDialogLocator locator)
    {
        _locator = locator;
    }

    private Dispatcher Dispatcher => System.Windows.Application.Current.Dispatcher;
    private Window MainWindow
    {
        get 
        {
            var mw = System.Windows.Application.Current.MainWindow as MainWindow;
            if (mw != null && mw.Shown)
            {
                return mw;
            }
            return null;

        }
    }

    public void ShowDialog(object dialogViewModel)
    {
        var type = _locator.Locate(dialogViewModel);
        if (type == null)
        {
            throw new TypeLoadException($"Dialog type for view model '{dialogViewModel.GetType().FullName}' not found.");
        }
        if (!typeof(Window).IsAssignableFrom(type))
        {
            throw new Exception($"Dialog type '{type.FullName}' is not assignable to Window");
        }

        Dispatcher.Invoke(() =>
        {
            var dialog = (Window)Activator.CreateInstance(type);
            dialog.Owner = MainWindow;
            dialog.DataContext = dialogViewModel;
            dialog.ShowDialog();
        });
    }

    public Core.Services.MessageBoxResult ShowMessageBox(MessageBoxSettings options)
    {
        return Dispatcher.Invoke(() =>
        {
            var dialog = new Dialogs.MessageBoxDialog(options)
            {
                Owner = MainWindow
            };
            dialog.ShowDialog();
            return dialog.Result;
        });
    }

    private static string GenerateWin32FilterString(IEnumerable<FileDialogFilter> filters)
    {
        var parts = new List<string>();
        foreach (var filter in filters)
        {
            parts.Add(filter.Name);
            parts.Add(string.Join(';', filter.Extensions.Select(ext =>
            {
                if (!ext.StartsWith("."))
                {
                    ext = "." + ext;
                }
                return "*" + ext;
            })));
        }
        return string.Join('|', parts);
    }

    public string[] ShowOpenFileDialog(OpenFileDialogSettings settings)
    {
        return Dispatcher.Invoke(() =>
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = settings.Title,
                InitialDirectory = settings.InitialDirectory,
                Filter = GenerateWin32FilterString(settings.Filters),
                Multiselect = settings.AllowMultiple,
                CheckFileExists = true,
                CheckPathExists = true,
            };
            var result = dialog.ShowDialog();
            return result == true ? dialog.FileNames : null;
        });
    }

    public string ShowSaveFileDialog(SaveFileDialogSettings settings)
    {
        return Dispatcher.Invoke(() =>
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = settings.Title,
                InitialDirectory = settings.InitialDirectory,
                DefaultExt = settings.DefaultExtension,
                Filter = GenerateWin32FilterString(settings.Filters),
            };
            var result = dialog.ShowDialog();
            return result == true ? dialog.FileName : null;
        });
    }

    public string ShowOpenFolderDialog(OpenFolderDialogSettings settings)
    {
        return Dispatcher.Invoke(() =>
        {
            var dialog = new FolderBrowserDialog
            {
                Description = settings.Title,
                SelectedPath = settings.InitialDirectory,
            };

            var resultEnum = dialog.ShowDialog();

            bool? result = resultEnum switch
            {
                DialogResult.OK => true,
                DialogResult.Yes => true,
                DialogResult.No => false,
                DialogResult.Abort => false,
                _ => null
            };

            string resultString = result == true ? dialog.SelectedPath : null;
            dialog.Dispose();
            return resultString;
        });
    }

    public void ProgressDialog(Action<IProgress<ProgressInfo>> work, bool delayOnCompletion = true)
    {
        var progressWindow = new Dialogs.LoadingDialog
        {
            Owner = MainWindow
        };

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

        progressWindow.ShowDialog();
    }
}
