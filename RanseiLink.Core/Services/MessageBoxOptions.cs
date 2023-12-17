namespace RanseiLink.Core.Services
{
    public enum MessageBoxType
    {
        Information,
        Warning,
        Error,
    }

    public enum MessageBoxResult
    {
        None,
        Ok,
        Cancel,
        Yes,
        No,
    }

    public record MessageBoxButton(string Text, MessageBoxResult Result);

    public class MessageBoxSettings(
        string title,
        string message,
        MessageBoxButton[] buttons,
        MessageBoxType type = MessageBoxType.Information,
        MessageBoxResult defaultResult = MessageBoxResult.Ok)
    {

        /// <summary>
        /// Simple way to create a message box that just has one button on it saying "OK"
        /// </summary>
        public static MessageBoxSettings Ok(string title,
            string message,
            MessageBoxType type = MessageBoxType.Information)
        {
            return new MessageBoxSettings(
                title,
                message,
                [new("OK", MessageBoxResult.Ok)],
                type,
                MessageBoxResult.Ok
                );
        }

        public string Title { get; } = title;
        public string Message { get; } = message;
        public MessageBoxButton[] Buttons { get; } = buttons;
        public MessageBoxType Type { get; } = type;
        public MessageBoxResult DefaultResult { get; } = defaultResult;
    }
}