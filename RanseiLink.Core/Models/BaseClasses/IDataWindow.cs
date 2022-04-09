
namespace RanseiLink.Core.Models;

public interface IDataWindow
{
    byte GetByte(int offset);
    void SetByte(int offset, byte value);
    int GetInt(int index, int offset, int bitCount);
    void SetInt(int index, int offset, int bitCount, int value);
    string GetUtf8String(int index, int length);

    string GetPaddedUtf8String(int index, int maxLength);

    void SetPaddedUtf8String(int index, int maxLength, string value);

    //byte GetOverflowByte(int startByte, int bitInStartByte);

    //void SetOverflowByte(int startByte, int bitInStartByte, byte value);
}
