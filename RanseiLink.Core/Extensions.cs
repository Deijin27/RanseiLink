using System;
using System.IO;
using System.Text;

namespace RanseiLink.Core
{
    public static class Extensions
    {
        internal static string ReadMagicNumber(this BinaryReader br)
        {
            return Encoding.UTF8.GetString(br.ReadBytes(4));
        }

        internal static void WriteMagicNumber(this BinaryWriter bw, string magicNumber)
        {
            bw.Write(Encoding.UTF8.GetBytes(magicNumber));
        }

        /// <summary>
        /// Write zero padding
        /// </summary>
        internal static void Pad(this BinaryWriter bw, int count)
        {
            bw.Write(new byte[count]);
        }

        internal static void Pad(this BinaryWriter bw, int count, byte padByte)
        {
            for (int i = 0; i < count; i++)
            {
                bw.Write(padByte);
            }
        }

        internal static void Skip(this BinaryReader br, int count)
        {
            br.BaseStream.Seek(count, SeekOrigin.Current);
        }

        public static ushort[] ToUInt16Array(this byte[] arr)
        {
            ushort[] shorts = new ushort[arr.Length / 2];
            Buffer.BlockCopy(arr, 0, shorts, 0, arr.Length);
            return shorts;
        }

        public static byte[] ToByteArray(this ushort[] arr)
        {
            byte[] bytes = new byte[arr.Length * 2];
            Buffer.BlockCopy(arr, 0, bytes, 0, arr.Length * 2);
            return bytes;
        }

        public static string Reverse(this string str)
        {
            var chars = str.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }
    }
}