using System.Windows;

namespace RanseiWpf.Services
{
    public class DialogService : IDialogService
    {
        public MessageBoxResult ShowMessageBox(MessageBoxArgs options)
        {
            return MessageBox.Show(options.Message, options.Title, options.Button, options.Icon, options.DefaultResult);
        }
        public bool RequestRomFile(string dialogWindowTitle, out string result)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = dialogWindowTitle,
                DefaultExt = ".nds",
                Filter = "Pokemon Conquest Rom (.nds)|*.nds",
                CheckFileExists = true,
                CheckPathExists = true
            };

            // Show save file dialog box
            bool? proceed = dialog.ShowDialog();
            result = dialog.FileName;
            // Process save file dialog box results
            if (proceed == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
