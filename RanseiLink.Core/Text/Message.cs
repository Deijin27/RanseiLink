#nullable enable
namespace RanseiLink.Core.Text
{
    public class Message
    {
        public Message(string text, string context, string boxConfig, int groupId = 0, int elementId = 0)
        {
            GroupId = groupId;
            ElementId = elementId;
            Text = text;
            Context = context;
            BoxConfig = boxConfig;
        }

        public string Text { get; set; }
        public string Context { get; set; }
        public string BoxConfig { get; set; }
        public int GroupId { get; set; }
        public int ElementId { get; set; }

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
}