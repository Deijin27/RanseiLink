using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    /// <summary>
    /// Tokusei
    /// </summary>
    public class Ability : BaseDataWindow
    {
        public const int DataLength = 0x14;
        public Ability(byte[] data) : base(data, DataLength) { }

        public string Name
        {
            get => GetPaddedUtf8String(0, 0x0C);
            set => SetPaddedUtf8String(0, 0x0C, value);
        }
    }
}
