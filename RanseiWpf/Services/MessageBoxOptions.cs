using System.Windows;

namespace RanseiWpf.Services
{
    public class MessageBoxArgs
    {
        public string Message { get; set; } = "";
        public string Title { get; set; } = "";
        public MessageBoxButton Button { get; set; } = MessageBoxButton.OK;
        public MessageBoxImage Icon { get; set; } = MessageBoxImage.None;
        public MessageBoxResult DefaultResult { get; set; } = MessageBoxResult.OK;
    }
}
