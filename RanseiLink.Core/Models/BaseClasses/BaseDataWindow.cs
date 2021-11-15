using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace RanseiLink.Core.Models;

public class BaseDataWindow : IDataWindow, IDataWrapper
{
    public byte[] Data { get; private set; }

    public BaseDataWindow(byte[] data, int length, bool doCompressionWhenSerializing = false)
    {
        if (data.Length != length)
        {
            throw new ArgumentException($"Byte array passed into {GetType().Name} constructor must have a length of {length}.");
        }
        Data = data;
        _doCompressionWhenSerializing = doCompressionWhenSerializing;
    }

    public static uint GetMask(int bitCount)
    {
        uint mask = 0;
        for (int i = 0; i < bitCount; i++)
        {
            mask = (mask << 1) | 1u;
        }
        return mask;
    }

    public byte GetByte(int offset) => Data[offset];
    public void SetByte(int offset, byte value) => Data[offset] = value;

    public ushort GetUInt16(int offset) => BitConverter.ToUInt16(Data, offset);
    public void SetUInt16(int offset, ushort newValue) => BitConverter.GetBytes(newValue).CopyTo(Data, offset);

    public uint GetUInt32(int index, int bitCount, int offset)
    {
        return (BitConverter.ToUInt32(Data, index * 4) >> offset) & GetMask(bitCount);
    }
    public void SetUInt32(int index, int bitCount, int offset, uint value)
    {
        // Maybe throw exception / warning when value is too big
        uint mask = GetMask(bitCount);
        uint current = BitConverter.ToUInt32(Data, index * 4);
        uint newValue = (current & ~(mask << offset)) | ((value & mask) << offset);
        BitConverter.GetBytes(newValue).CopyTo(Data, index * 4);
    }


    public string GetUtf8String(int index, int length)
    {
        return Encoding.UTF8.GetString(Data, index, length);
    }

    /// <summary>
    /// Read text from data, removing 0x00 padding.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public string GetPaddedUtf8String(int index, int maxLength)
    {
        var bytes = Data.Skip(index).TakeWhile((i, c) => i != 0 && c <= maxLength).ToArray();

        var reader = new Text.PNAReader(bytes, false, true);

        return reader.Text;
    }

    /// <summary>
    /// Write text to data, padding to max length with 0x00
    /// </summary>
    /// <param name="index">byte index in data to start name</param>
    /// <param name="maxLength">Max length of the text <strong>excluding</strong> the required 0x00 terminator</param>
    /// <param name="value">name to write</param>
    public void SetPaddedUtf8String(int index, int maxLength, string value)
    {
        if (value.Length > maxLength)
        {
            value = value.Substring(0, maxLength);
        }
        byte[] output;
        while (true)
        {
            var writer = new Text.PNAWriter(value, false, true);
            output = writer.Data;
            if (output.Length <= maxLength)
            {
                break;
            }
            value = value.Substring(0, value.Length - 1);
        }

        byte[] final = new byte[maxLength + 1]; // ensure padding section is overwritten and required 0x00 terminator exists.
        output.CopyTo(final, 0);

        final.CopyTo(Data, index);
    }

    private string SerializationPrefix => GetType().Name;


    bool _doCompressionWhenSerializing;

    public string Serialize()
    {
        byte[] data = Data;
        if (_doCompressionWhenSerializing)
        {
            data = Compress(data);
        }
        return $"{SerializationPrefix}({Convert.ToBase64String(data)})";
    }

    public bool TryDeserialize(string serialized)
    {
        if (!serialized.StartsWith(SerializationPrefix))
        {
            return false;
        }

        serialized = serialized.Substring(SerializationPrefix.Length);

        if (!(serialized.StartsWith("(") && serialized.EndsWith(")")))
        {
            return false;
        }

        serialized = serialized.Substring(1, serialized.Length - 2);

        byte[] data = Convert.FromBase64String(serialized);

        if (_doCompressionWhenSerializing)
        {
            data = Decompress(data);
        }


        if (Data.Length != data.Length)
        {
            return false;
        }

        Data = data;
        return true;
    }

    private static byte[] Compress(byte[] data)
    {
        MemoryStream output = new MemoryStream();
        using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
        {
            dstream.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }

    private static byte[] Decompress(byte[] data)
    {
        MemoryStream input = new MemoryStream(data);
        MemoryStream output = new MemoryStream();
        using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
        {
            dstream.CopyTo(output);
        }
        return output.ToArray();
    }
}
