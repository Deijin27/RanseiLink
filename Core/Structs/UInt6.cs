
namespace Core.Structs
{
    public readonly struct UInt6
    {
        public UInt6(int value)
        {
            _value = value & 0b_0011_1111;
        }

        readonly int _value;

        public static explicit operator UInt6(int i)
        {
            return new UInt6(i);
        }

        public static implicit operator byte(UInt6 i)
        {
            return (byte)i._value;
        }

        public static implicit operator int(UInt6 i)
        {
            return i._value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
