using System;
using System.Collections.Generic;
using System.Text;
using Core.Enums;
using Core.Models.Interfaces;

namespace Core.Models
{
    public class Item : BaseDataWindow, IItem
    {
        public const int DataLength = 0x24;
        public Item(byte[] data) : base(data, DataLength)
        {
        }

        public Item() : base(new byte[DataLength], DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 21);
            set => SetPaddedUtf8String(0, 21, value);
        }

        public IItem Clone()
        {
            return new Item((byte[])Data.Clone());
        }
    }
}
