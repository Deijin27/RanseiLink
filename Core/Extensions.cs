using System;
using System.Collections.Generic;
using System.IO;

namespace Core
{
    public static class Extensions
    {
        internal static string ReadMagicNumber(this BinaryReader br)
        {
            return new string(br.ReadChars(4));
        }

        public static T Choice<T>(this Random random, T[] collection)
        {
            return collection[random.Next(0, collection.Length)];
        }
        public static T Choice<T>(this Random random, IList<T> collection)
        {
            return collection[random.Next(0, collection.Count)];
        }
    }
}