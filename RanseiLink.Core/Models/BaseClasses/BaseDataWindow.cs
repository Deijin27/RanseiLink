using RanseiLink.Core.Text;
using System.IO.Compression;
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

    public static int GetMask(int bitCount)
    {
        return ~(-1 << bitCount);
    }

    public void Read(Stream stream)
    {
        stream.ReadExactly(Data);
    }

    public void Write(Stream stream)
    {
        stream.Write(Data, 0, Data.Length);
    }

    public byte GetByte(int offset) => Data[offset];
    public void SetByte(int offset, byte value) => Data[offset] = value;

    public ushort GetUInt16(int offset) => BitConverter.ToUInt16(Data, offset);
    public void SetUInt16(int offset, ushort newValue) => BitConverter.GetBytes(newValue).CopyTo(Data, offset);

    public int GetInt(int intIndex, int shift, int bitCount)
    {
        return (BitConverter.ToInt32(Data, intIndex * 4) >> shift) & GetMask(bitCount);
    }
    public void SetInt(int intIndex, int shift, int bitCount, int value)
    {
        // Maybe throw exception / warning when value is too big
        int mask = GetMask(bitCount);
        int current = BitConverter.ToInt32(Data, intIndex * 4);
        int newValue = (current & ~(mask << shift)) | ((value & mask) << shift);
        BitConverter.GetBytes(newValue).CopyTo(Data, intIndex * 4);
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

        //var reader = new Text.PNAReader(bytes, false, true);

        //return reader.Text;

        return NameLoader.LoadName(bytes);
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
            //var writer = new Text.PNAWriter(value, false, true);
            //output = writer.Data;
            output = NameLoader.SaveName(value);
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


    protected virtual byte[] GetBytesToSerialise()
    {
        return Data;
    }

    protected virtual void SetBytesFromDeserialise(byte[] data)
    {
        // If we every modified the pasted data inside SetBytesFromDeserialise, this Datalength check in TryDeserialise would need to move
        Data = data;
    }

    public string Serialize()
    {
        byte[] data = GetBytesToSerialise();
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

        // If we every modified the pasted data inside SetBytesFromDeserialise, this check would need to move
        if (Data.Length != data.Length)
        {
            return false;
        }

        SetBytesFromDeserialise(data);
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