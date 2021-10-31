
namespace RanseiLink.ValueConverters
{
    public class InvertBoolConverter : ValueConverter<bool, bool>
    {
        protected override bool Convert(bool value)
        {
            return !value;
        }

        protected override bool ConvertBack(bool value)
        {
            return !value;
        }
    }
}
