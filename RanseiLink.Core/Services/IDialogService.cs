using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.Core.Services
{
    public interface IDialogService
    {
        void ShowDialog(object dialogViewModel);
        MessageBoxResult ShowMessageBox(MessageBoxSettings options);
        string[]? ShowOpenFileDialog(OpenFileDialogSettings settings);
        string? ShowSaveFileDialog(SaveFileDialogSettings settings);
        string? ShowOpenFolderDialog(OpenFolderDialogSettings settings);
        void ProgressDialog(Action<IProgress<ProgressInfo>> work, bool delayOnCompletion = true);
    }

    public interface IModalDialogViewModel
    {
        void OnClosing(bool result);
    }

    public interface IModalDialogViewModel<TDialogResult> : IModalDialogViewModel
    {
        TDialogResult Result { get; }
    }

    public interface IDialogLocator
    {
        Type? Locate(object viewModel);
    }

    public class NamingConventionLocator : IDialogLocator
    {
        public Type? Locate(object viewModel)
        {
            var viewModelName = viewModel.GetType().FullName ?? throw new Exception("Cannot get type name for viewModel type");
            var name = viewModelName.Replace("ViewModel", "Dialog");
            return Type.GetType(name);
        }
    }

    public class RegistryDialogLocator : IDialogLocator
    {
        private readonly ConcurrentDictionary<Type, Type> _registry = new ConcurrentDictionary<Type, Type>();

        public Type? Locate(object viewModel)
        {
            if (viewModel == null)
            {
                return null;
            }
            var vmType = viewModel.GetType();
            if (!_registry.TryGetValue(vmType, out var dialogType))
            {
                return null;
            }
            return dialogType;
        }

        public void Register(Type dialogType, Type viewModelType)
        {
            _registry[viewModelType] = dialogType;
        }
    }

    public class ProgressInfo
    {
        public string? StatusText { get; }
        public int? Progress { get; }
        public int? MaxProgress { get; }
        public bool? IsIndeterminate { get; }
        public ProgressInfo(string? statusText = null, int? progress = null, int? maxProgress = null, bool? isIndeterminate = null)
        {
            StatusText = statusText;
            Progress = progress;
            MaxProgress = maxProgress;
            IsIndeterminate = isIndeterminate;
        }
    }

    public abstract class FileSystemDialogSettings
    {
        public string Title { get; set; } = string.Empty;
        public string InitialDirectory { get; set; } = string.Empty;
    }

    public abstract class FileDialogSettings : FileSystemDialogSettings
    {
        public string InitialFileName { get; set; } = string.Empty;
        public List<FileDialogFilter> Filters { get; set; } = new List<FileDialogFilter>();
    }

    public class SaveFileDialogSettings : FileDialogSettings
    {
        public SaveFileDialogSettings() { }
        public string DefaultExtension { get; set; } = string.Empty;
    }

    public class OpenFileDialogSettings : FileDialogSettings
    {
        public OpenFileDialogSettings() { }
        public OpenFileDialogSettings(string title, params FileDialogFilter[] filters)
        {
            Title = title;
            Filters.AddRange(filters);
        }
        public bool AllowMultiple { get; set; }
    }

    public class OpenFolderDialogSettings : FileSystemDialogSettings
    {
        public OpenFolderDialogSettings() { }
        public OpenFolderDialogSettings(string title)
        {
            Title = title;
        }
    }

    public class FileDialogFilter
    {
        public FileDialogFilter() { }

        public FileDialogFilter(string name, params string[] extensions)
        {
            Name = name;
            Extensions.AddRange(extensions);
        }

        /// <summary>
        /// Gets or sets the name of the filter, e.g. ("Text files (.txt)").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a list of file extensions matched by the filter (e.g. "txt" or "*" for all
        /// files).
        /// </summary>
        public List<string> Extensions { get; set; } = new List<string>();
    }

    public static class DialogServiceExtensions
    {
        public static TDialogResult ShowDialogWithResult<TDialogResult>(this IDialogService dialogService, IModalDialogViewModel<TDialogResult> dialogViewModel)
        {
            dialogService.ShowDialog(dialogViewModel);
            return dialogViewModel.Result;
        }

        public static string? RequestRomFile(this IDialogService dialogService)
        {
            return dialogService.ShowOpenSingleFileDialog(new OpenFileDialogSettings
            {
                Title = "Select a Rom",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter("Pokemon Conquest Rom (.nds)", ".nds")
                }
            });
        }

        public static string? RequestModFile(this IDialogService dialogService)
        {
            return dialogService.ShowOpenSingleFileDialog(new OpenFileDialogSettings
            {
                Title = "Select a Mod",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter("RanseiLink Mod (.rlmod)", ".rlmod")
                }
            });
        }

        public static string? ShowOpenSingleFileDialog(this IDialogService dialogService, OpenFileDialogSettings settings)
        {
            settings.AllowMultiple = false;
            var result = dialogService.ShowOpenFileDialog(settings);
            var file = result?.FirstOrDefault();
            if (string.IsNullOrEmpty(file))
            {
                file = null;
            }
            return file;
        }

        public static string[]? ShowOpenMultipleFilesDialog(this IDialogService dialogService, OpenFileDialogSettings settings)
        {
            settings.AllowMultiple = true;
            return dialogService.ShowOpenFileDialog(settings);
        }
    }
}
