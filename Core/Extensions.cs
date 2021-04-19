using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Core
{
    internal static class Extensions
    {
        internal static string ReadMagicNumber(this BinaryReader br)
        {
            return new string(br.ReadChars(4));
        }
    }
}