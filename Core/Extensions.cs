using System.IO;

namespace Core
{
    internal static class Extensions
    {
        internal static string ReadMagicNumber(this BinaryReader br)
        {
            return new string(br.ReadChars(4));
        }
    }
}