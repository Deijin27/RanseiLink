using System.Text;

namespace RanseiLink.Core;

public static class Extensions
{
    public static string ReadMagicNumber(this BinaryReader br)
    {
        return Encoding.UTF8.GetString(br.ReadBytes(4));
    }

    internal static void WriteMagicNumber(this BinaryWriter bw, string magicNumber)
    {
        bw.Write(Encoding.UTF8.GetBytes(magicNumber));
    }

    /// <summary>
    /// Read a utf8 string that ends with 0x00 from the stream.
    /// </summary>
    /// <remarks>
    /// The length of the provided buffer is interpreted as the maximum length that the string can be.
    /// It being an argument you pass has the additional benefit that it can be shared between 
    /// synchronous reads with the same maximum length.
    /// </remarks>
    public static string ReadNullTerminatedString(this BinaryReader br, byte[] buffer)
    {
        int i;
        for (i = 0; i < buffer.Length; i++)
        {
            buffer[i] = br.ReadByte();
            if (buffer[i] == 0)
            {
                break;
            }
        }
        return Encoding.UTF8.GetString(buffer, 0, i);
    }

    public static void WriteNullTerminatedString(this BinaryWriter bw, string value)
    {
        bw.Write(Encoding.UTF8.GetBytes(value));
        bw.Write((byte)0);
    }

    internal static void Pad(this Stream stream, int count)
    {
        stream.Write(new byte[count]);
    }

    internal static int ReadInt32(this Stream stream)
    {
        var buffer = new byte[4];
        stream.Read(buffer, 0, buffer.Length);
        return BitConverter.ToInt32(buffer, 0);
    }

    internal static void WriteInt32(this Stream stream, int value)
    {
        var buffer = BitConverter.GetBytes(value);
        stream.Write(buffer, 0, buffer.Length);
    }

    /// <summary>
    /// Write zero padding
    /// </summary>
    internal static void Pad(this BinaryWriter bw, int count)
    {
        bw.Write(new byte[count]);
    }

    internal static void Pad(this BinaryWriter bw, int count, byte padByte)
    {
        for (int i = 0; i < count; i++)
        {
            bw.Write(padByte);
        }
    }

    internal static void Skip(this BinaryReader br, int count)
    {
        br.BaseStream.Seek(count, SeekOrigin.Current);
    }

    public static ushort[] ToUInt16Array(this byte[] arr)
    {
        ushort[] shorts = new ushort[arr.Length / 2];
        Buffer.BlockCopy(arr, 0, shorts, 0, arr.Length);
        return shorts;
    }

    public static byte[] ToByteArray(this ushort[] arr)
    {
        byte[] bytes = new byte[arr.Length * 2];
        Buffer.BlockCopy(arr, 0, bytes, 0, arr.Length * 2);
        return bytes;
    }

    public static string Reverse(this string str)
    {
        var chars = str.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }
}