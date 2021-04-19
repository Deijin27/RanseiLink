
namespace Core.Structs
{
    public readonly struct UInt9
    {
        public UInt9(int value)
        {
            _value = value & 0b_1_1111_1111;
        }

        readonly int _value;

        public static explicit operator UInt9(int i)
        {
            return new UInt9(i);
        }

        public static explicit operator UInt9(uint i)
        {
            return new UInt9((ushort)i);
        }

        public static explicit operator UInt9(ushort i)
        {
            return new UInt9(i);
        }

        public static implicit operator byte(UInt9 i)
        {
            return (byte)i._value;
        }

        public static implicit operator int(UInt9 i)
        {
            return i._value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
