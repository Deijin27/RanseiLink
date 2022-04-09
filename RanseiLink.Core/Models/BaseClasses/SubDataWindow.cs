using System;
using System.Collections.Generic;
using System.Text;

namespace RanseiLink.Core.Models;

public class SubDataWindow : IDataWindow
{
    protected SubDataWindow(IDataWindow parent, int offset)
    {
        Parent = parent;
        Offset = offset;
    }

    readonly IDataWindow Parent;
    readonly int Offset;

    public byte GetByte(int offset)
    {
        throw new NotImplementedException();
    }

    public void SetByte(int offset, byte value)
    {
        throw new NotImplementedException();
    }

    public int GetInt(int index, int offset, int bitCount)
    {
        throw new NotImplementedException();
    }

    public void SetInt(int index, int offset, int bitCount, int value)
    {
        throw new NotImplementedException();
    }

    public string GetUtf8String(int index, int length)
    {
        throw new NotImplementedException();
    }

    public string GetPaddedUtf8String(int index, int maxLength)
    {
        throw new NotImplementedException();
    }

    public void SetPaddedUtf8String(int index, int maxLength, string value)
    {
        throw new NotImplementedException();
    }
}
