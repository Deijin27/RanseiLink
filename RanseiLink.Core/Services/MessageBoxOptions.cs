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

    public class MessageBoxButton
    {
        public string Text { get; } 
        public MessageBoxResult Result { get; }
        public MessageBoxButton(string text, MessageBoxResult result) 
        { 
            Text = text; 
            Result = result; 
        }
    }

    public class MessageBoxArgs
    {
        public MessageBoxArgs(
            string title,
            string message,
            MessageBoxButton[] buttons,
            MessageBoxType type = MessageBoxType.Information,
            MessageBoxResult defaultResult = MessageBoxResult.Ok)
        {
            Title = title;
            Message = message;
            Buttons = buttons;
            Type = type;
            DefaultResult = defaultResult;
        }

        /// <summary>
        /// Simple way to create a message box that just has one button on it saying "OK"
        /// </summary>
        public static MessageBoxArgs Ok(string title,
            string message,
            MessageBoxType type = MessageBoxType.Information)
        {
            return new MessageBoxArgs(
                title,
                message,
                new MessageBoxButton[] { new MessageBoxButton("OK", MessageBoxResult.Ok) },
                type,
                MessageBoxResult.Ok
                );
        }

        public string Title { get; }
        public string Message { get; }
        public MessageBoxButton[] Buttons { get; }
        public MessageBoxType Type { get; }
        public MessageBoxResult DefaultResult { get; }
    }
}