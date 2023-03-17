#nullable enable
namespace RanseiLink.Core.RomFs;

public static class Checksum
{
	public class CRC16
	{
    public readonly ushort[] table = new ushort[256];

    public CRC16(ushort polynomial = IBMReversedPolynomial)
    {
        ushort value;
        ushort temp;
        for (ushort i = 0; i < table.Length; ++i)
        {
            value = 0;
            temp = i;
            for (byte j = 0; j < 8; ++j)
            {
                if (((value ^ temp) & 0x0001) != 0)
                {
                    value = (ushort)((value >> 1) ^ polynomial);
                }
                else
                {
                    value >>= 1;
                }
                temp >>= 1;
            }
            table[i] = value;
        }
    }

    public ushort Calculate(byte[] bytes, uint init = 0xFFFF)
		{
			uint crc = init;

			for (int i = 0; i < bytes.Length; i++)
			{
				crc = (crc >> 8) ^ table[(crc ^ bytes[i]) & 0xFF];
			}

			return (ushort)crc;
		}

		public const ushort IBMReversedPolynomial = 0xA001;
	}
}
