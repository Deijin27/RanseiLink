using Core.Structs;

namespace Core.Models
{
    public interface IDataWindow
    {
        bool GetBit(int byteOffset, int bitOffsetIntoByte);
        byte GetByte(int index);
        short GetInt16(int index);
        sbyte GetSByte(int index);
        ushort GetUInt16(int index);
        UInt2 GetUInt2(int byteOffset, int startOffsetIntoByte);
        uint GetUInt32(int index);
        UInt4 GetUInt4(int byteOffset, int startOffsetIntoByte);
        ulong GetUInt64(int index);
        void SetBit(int byteOffset, int bitOffsetIntoByte, bool bitValue);
        void SetByte(int index, byte value);
        void SetInt16(int index, short value);
        void SetSByte(int index, sbyte value);
        void SetUInt16(int index, ushort value);
        void SetUInt2(int byteOffset, int startOffsetIntoByte, UInt2 twoBitValue);
        void SetUInt32(int index, uint value);
        void SetUInt4(int byteOffset, int startOffsetIntoByte, UInt4 fourBitValue);
        void SetUInt64(int index, ulong value);
        string GetUtf8String(int index, int length);

        string GetPaddedUtf8String(int index, int maxLength);

        void SetPaddedUtf8String(int index, int maxLength, string value);

        //byte GetOverflowByte(int startByte, int bitInStartByte);

        //void SetOverflowByte(int startByte, int bitInStartByte, byte value);
    }
}