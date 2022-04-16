
namespace RanseiLink.Core.Graphics 
{
    public struct Rgb15
    {
        public int R;
        public int G;
        public int B;
        public Rgb15(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }
        private const int _mask = 0b11111;

        public static Rgb15 From(ushort value)
        {
            return new Rgb15
            (
                value & _mask, 
                (value >> 5) & _mask, 
                (value >> 10) & _mask
            );
        }

        public ushort ToUInt16()
        {
            return (ushort)(R | G << 5 | B << 10);
        }
    }
}