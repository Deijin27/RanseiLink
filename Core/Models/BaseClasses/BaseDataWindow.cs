using System;
using System.Text;

namespace Core.Models
{
    public class BaseDataWindow : IDataWindow, IDataWrapper
    {
        public byte[] Data { get; }

        public BaseDataWindow(byte[] data, int length)
        {
            if (data.Length != length)
            {
                throw new ArgumentException($"Byte array passed into {GetType().Name} constructor must have a length of {length}.");
            }
            Data = data;
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

        public string GetPaddedUtf8String(int index, int maxLength)
        {
            return GetUtf8String(index, maxLength).TrimEnd('\x0');
        }

        public void SetPaddedUtf8String(int index, int maxLength, string value)
        {
            if (value.Length > maxLength)
            {
                value = value.Substring(0, maxLength);
            }
            var output = new byte[maxLength];
            Encoding.UTF8.GetBytes(value).CopyTo(output, 0);
            output.CopyTo(Data, index);
        }
    }

}
