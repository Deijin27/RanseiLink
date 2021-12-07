using System.IO;

namespace RanseiLink.Core;

public static class Extensions
{
    internal static string ReadMagicNumber(this BinaryReader br)
    {
        return new string(br.ReadChars(4));
    }
}
