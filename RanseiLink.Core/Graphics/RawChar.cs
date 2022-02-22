
namespace RanseiLink.Core.Graphics;

public static class RawChar
{
    public static byte[] Decompress(byte[] data)
    {
        byte[] res = new byte[data.Length * 2];

        for (int i = 0; i < data.Length; i++)
        {
            int idx = i * 2;
            byte val = data[i];
            res[idx] = (byte)(val & 0b1111);
            res[idx + 1] = (byte)(val >> 4);
        }
        return res;
    }

    public static byte[] Compress(byte[] data)
    {
        byte[] res = new byte[data.Length / 2];

        for (int i = 0; i < data.Length / 2; i++)
        {
            res[i] = (byte)((data[i*2] & 0b1111) | (data[i*2 + 1] << 4));
        }
        return res;
    }
}
