namespace RanseiLink.Services;

public enum MessageBoxIcon
{
    None,
    Error,
    Warning,
    Question,
    Information
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

public class MessageBoxArgs
{
    public MessageBoxArgs(
        string title,
        string message,
        MessageBoxButton[] buttons,
        MessageBoxIcon icon = MessageBoxIcon.Information,
        MessageBoxResult defaultResult = MessageBoxResult.Ok)
    {
        Title = title;
        Message = message;
        Buttons = buttons;
        Icon = icon;
        DefaultResult = defaultResult;
    }

    /// <summary>
    /// Simple way to create a message box that just has one button on it saying "OK"
    /// </summary>
    public static MessageBoxArgs Ok(string title,
        string message,
        MessageBoxIcon icon = MessageBoxIcon.Information)
    {
        return new MessageBoxArgs(
            title,
            message,
            new MessageBoxButton[] { new MessageBoxButton("OK", MessageBoxResult.Ok) },
            icon,
            MessageBoxResult.Ok
            );
    }

    public string Title { get; }
    public string Message { get; }
    public MessageBoxButton[] Buttons { get; }
    public MessageBoxIcon Icon { get; }
    public MessageBoxResult DefaultResult { get; }
}