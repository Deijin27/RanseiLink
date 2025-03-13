namespace RanseiLink.Core.Text;

public class Message
{
    private string _text;
    private string _context;
    private string _boxConfig;

    public Message(string text, string context, string boxConfig, int groupId = 0, int elementId = 0)
    {
        _text = text;
        _context = context;
        _boxConfig = boxConfig;
        GroupId = groupId;
        ElementId = elementId;
    }

    public void SetTextNoSanitization(string text)
    {
        _text = text;
    }

    public string Text 
    { 
        get => _text; 
        set => _text = Sanitize(value); 
    }
    public string Context 
    { 
        get => _context; 
        set => _context = Sanitize(value); 
    }
    public string BoxConfig 
    { 
        get => _boxConfig; 
        set => _boxConfig = Sanitize(value); 
    }
    public int GroupId { get; set; }
    public int ElementId { get; set; }

    private string Sanitize(string text)
    {
        // replace windows line endings with just \n ones.
        return text.Replace("\r", "");
    }

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