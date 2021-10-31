
namespace RanseiLink.Console
{
    internal class PathConverter : CliFx.Extensibility.BindingConverter<string>
    {
        public override string Convert(string rawValue)
        {
            if (rawValue.StartsWith('"'))
            {
                rawValue = rawValue[1..];
            }
            if (rawValue.EndsWith('"'))
            {
                rawValue = rawValue[..^1];
            }
            return rawValue;
        }
    }
}
