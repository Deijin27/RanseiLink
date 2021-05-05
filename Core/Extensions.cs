using System.IO;
using System.Text;

namespace Core
{
    internal static class Extensions
    {
        internal static string ReadMagicNumber(this BinaryReader br)
        {
            return new string(br.ReadChars(4));
        }

        internal static void WriteProperty(this StringBuilder sb, string propertyName, string propertyValue)
        {
            sb.Append($"    {propertyName}: ");
            sb.AppendLine(propertyValue);
        }
    }
}