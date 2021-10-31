using System;
using System.Collections.Generic;
using System.Text;

namespace RanseiLink.Core.Models
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

        public uint GetUInt32(int index, int bitCount, int offset)
        {
            throw new NotImplementedException();
        }

        public void SetUInt32(int index, int bitCount, int offset, uint value)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int offset)
        {
            throw new NotImplementedException();
        }

        public void SetByte(int offset, byte value)
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
