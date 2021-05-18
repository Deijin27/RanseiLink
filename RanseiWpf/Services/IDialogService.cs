using System.Windows;

namespace RanseiWpf.Services
{
    public interface IDialogService
    {
        bool RequestRomFile(string dialogWindowTitle, out string result);
        MessageBoxResult ShowMessageBox(MessageBoxArgs options);
    }
}