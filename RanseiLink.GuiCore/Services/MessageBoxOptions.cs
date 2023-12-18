namespace RanseiLink.GuiCore.Services
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

    public record MessageBoxSettings(
        string Title,
        string Message,
        MessageBoxButton[] Buttons,
        MessageBoxType Type = MessageBoxType.Information,
        MessageBoxResult DefaultResult = MessageBoxResult.Ok)
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
    }
}