namespace RanseiLink.Core.Text;

public class Message(string text, string context, string boxConfig, int groupId = 0, int elementId = 0)
{
    public string Text { get; set; } = text;
    public string Context { get; set; } = context;
    public string BoxConfig { get; set; } = boxConfig;
    public int GroupId { get; set; } = groupId;
    public int ElementId { get; set; } = elementId;

    public bool IsEmpty
    {
        get
        {
            return string.IsNullOrEmpty(Text)
                && string.IsNullOrEmpty(Context)
                && string.IsNullOrEmpty(BoxConfig);
        }
    }

    public Message Clone()
    {
        return new Message(
            text: Text,
            context: Context,
            boxConfig: BoxConfig,
            groupId: GroupId,
            elementId: ElementId
            );
    }
}