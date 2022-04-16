namespace RanseiLink.Core.Text
{
    public class Message
    {
        public int GroupId { get; set; }

        public int ElementId { get; set; }

        public string Text { get; set; }

        public string Context { get; set; }

        public string BoxConfig { get; set; }

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(Text)
                    && string.IsNullOrEmpty(Context)
                    && string.IsNullOrEmpty(BoxConfig);
            }
        }
    }
}