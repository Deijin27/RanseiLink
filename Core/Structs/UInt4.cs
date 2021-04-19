
namespace Core.Structs
{
    public readonly struct UInt4
    {
        public UInt4(int value)
        {
            _value = value & 0b_0000_1111;
        }

        readonly int _value;

        public static explicit operator UInt4(int i)
        {
            return new UInt4(i);
        }

        public static implicit operator byte(UInt4 i)
        {
            return (byte)i._value;
        }

        public static implicit operator int(UInt4 i)
        {
            return i._value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
