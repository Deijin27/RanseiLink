
namespace Core.Models
{
    public interface IDataWindow
    {
        byte GetByte(int offset);
        void SetByte(int offset, byte value);
        uint GetUInt32(int index, int bitCount, int offset);
        void SetUInt32(int index, int bitCount, int offset, uint value);
        string GetUtf8String(int index, int length);

        string GetPaddedUtf8String(int index, int maxLength);

        void SetPaddedUtf8String(int index, int maxLength, string value);

        //byte GetOverflowByte(int startByte, int bitInStartByte);

        //void SetOverflowByte(int startByte, int bitInStartByte, byte value);
    }
}