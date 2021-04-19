using Core.Structs;
using System;
using System.Text;

namespace Core.Models
{
    public class BaseDataWindow : IDataWindow
    {
        public readonly byte[] Data;

        public BaseDataWindow(byte[] data, int length)
        {
            if (data.Length != length)
            {
                throw new ArgumentException($"Byte array passed into {GetType().Name} constructor must have a length of {length}.");
            }
            Data = data;
        }

        public byte GetByte(int index) => Data[index];
        public void SetByte(int index, byte value) => Data[index] = value;

        public sbyte GetSByte(int index) => (sbyte)Data[index];
        public void SetSByte(int index, sbyte value) => Data[index] = (byte)value;

        public ushort GetUInt16(int index) => BitConverter.ToUInt16(Data, index);
        public void SetUInt16(int index, ushort value) => BitConverter.GetBytes(value).CopyTo(Data, index);

        public short GetInt16(int index) => BitConverter.ToInt16(Data, index);
        public void SetInt16(int index, short value) => BitConverter.GetBytes(value).CopyTo(Data, index);

        public uint GetUInt32(int index) => BitConverter.ToUInt32(Data, index);
        public void SetUInt32(int index, uint value) => BitConverter.GetBytes(value).CopyTo(Data, index);

        public ulong GetUInt64(int index) => BitConverter.ToUInt64(Data, index);
        public void SetUInt64(int index, ulong value) => BitConverter.GetBytes(value).CopyTo(Data, index);

        public UInt4 GetUInt4(int byteOffset, int startOffsetIntoByte) => new UInt4(Data[byteOffset] >> startOffsetIntoByte);
        public void SetUInt4(int byteOffset, int startOffsetIntoByte, UInt4 fourBitValue)
        {
            Data[byteOffset] = (byte)((Data[byteOffset] & ~(0b1111 << startOffsetIntoByte)) | (fourBitValue << startOffsetIntoByte));
        }

        public UInt2 GetUInt2(int byteOffset, int startOffsetIntoByte) => new UInt2(Data[byteOffset] >> startOffsetIntoByte);
        public void SetUInt2(int byteOffset, int startOffsetIntoByte, UInt2 twoBitValue)
        {
            Data[byteOffset] = (byte)((Data[byteOffset] & ~(0b11 << startOffsetIntoByte)) | (twoBitValue << startOffsetIntoByte));
        }

        public bool GetBit(int byteOffset, int bitOffsetIntoByte) => ((Data[byteOffset] >> bitOffsetIntoByte) & 0b1) == 0b1;
        public void SetBit(int byteOffset, int bitOffsetIntoByte, bool bitValue)
        {
            int toSet = 0b1 << bitOffsetIntoByte;
            Data[byteOffset] = (byte)((Data[byteOffset] & ~toSet) | (bitValue ? toSet : 0b0));
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
                throw new ArgumentException("String is too long");
            }
            var output = new byte[maxLength];
            Encoding.UTF8.GetBytes(value).CopyTo(output, 0);
            output.CopyTo(Data, index);
        }


        ///// <summary>
        ///// Get byte that runs across two bytes in the data byte array. The lower part of the number is in the first, and the upper part in the second.
        ///// 
        ///// e.g. for two consecutive bytes 101110_01, 011010_10, numOfBitsInStartByte=6, we get 10_101110
        ///// </summary>
        ///// <param name="startByte">The offset of the first byte in the data to contain part of the number. This contains the lower
        ///// part of the number, while the byte after this contains the upper part of the number.</param>
        ///// <param name="numOfBitsInStartByte">The number of bits out of the 8 that make up the byte that are contained within the first byte (that is, startByte)</param>
        ///// <returns></returns>
        //public byte GetOverflowByte(int startByte, int numOfBitsInStartByte)
        //{
        //    return (byte)((Data[startByte] >> (8 - numOfBitsInStartByte)) | (Data[startByte + 1] << numOfBitsInStartByte));
        //}

        //public void SetOverflowByte(int startByte, int numOfBitsInStartByte, byte value)
        //{
        //    // mask out necessary bits in current values
        //    int byte0 = Data[startByte] & (0xFF >> (8 - numOfBitsInStartByte));
        //    int byte1 = Data[startByte + 1] & (0xFF << numOfBitsInStartByte);

        //    // shift new value to positions
        //    int val0 = value << numOfBitsInStartByte;
        //    int val1 = value >> (8 - numOfBitsInStartByte);

        //    // or together and set
        //    Data[startByte + 1] = (byte)(byte0 | val0);
        //    Data[startByte] = (byte)(byte1 | val1);
        //}
    }

}
