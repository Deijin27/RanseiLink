using Core.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class SubDataWindow : IDataWindow
    {
        protected SubDataWindow(IDataWindow parent, int offset)
        {
            Parent = parent;
            Offset = offset;
        }

        readonly IDataWindow Parent;
        readonly int Offset;

        public byte GetByte(int index) => Parent.GetByte(Offset + index);
        public void SetByte(int index, byte value) => Parent.SetByte(Offset + index, value);

        public ushort GetUInt16(int index) => Parent.GetUInt16(Offset + index);
        public void SetUInt16(int index, ushort value) => Parent.SetUInt16(Offset + index, value);

        public bool GetBit(int byteOffset, int bitOffsetIntoByte)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int index)
        {
            throw new NotImplementedException();
        }

        public sbyte GetSByte(int index)
        {
            throw new NotImplementedException();
        }

        public UInt2 GetUInt2(int byteOffset, int startOffsetIntoByte)
        {
            throw new NotImplementedException();
        }

        public uint GetUInt32(int index)
        {
            throw new NotImplementedException();
        }

        public UInt4 GetUInt4(int byteOffset, int startOffsetIntoByte)
        {
            throw new NotImplementedException();
        }

        public ulong GetUInt64(int index)
        {
            throw new NotImplementedException();
        }

        public void SetBit(int byteOffset, int bitOffsetIntoByte, bool bitValue)
        {
            throw new NotImplementedException();
        }

        public void SetInt16(int index, short value)
        {
            throw new NotImplementedException();
        }

        public void SetSByte(int index, sbyte value)
        {
            throw new NotImplementedException();
        }

        public void SetUInt2(int byteOffset, int startOffsetIntoByte, UInt2 twoBitValue)
        {
            throw new NotImplementedException();
        }

        public void SetUInt32(int index, uint value)
        {
            throw new NotImplementedException();
        }

        public void SetUInt4(int byteOffset, int startOffsetIntoByte, UInt4 fourBitValue)
        {
            throw new NotImplementedException();
        }

        public void SetUInt64(int index, ulong value)
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

        //public byte GetOverflowByte(int startByte, int bitInStartByte)
        //{
        //    throw new NotImplementedException();
        //}

        //public void SetOverflowByte(int startByte, int bitInStartByte, byte value)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
