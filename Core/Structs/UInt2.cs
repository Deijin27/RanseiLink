
namespace Core.Structs
{
    public readonly struct UInt2
    {
        public UInt2(int value)
        {
            _value = value & 0b_0000_0011;
        }

        readonly int _value;

        public static explicit operator UInt2(int i)
        {
            return new UInt2(i);
        }
        public static explicit operator UInt2(uint i)
        {
            return new UInt2((int)i);
        }

        public static implicit operator byte(UInt2 i)
        {
            return (byte)i._value;
        }

        public static implicit operator int(UInt2 i)
        {
            return i._value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
