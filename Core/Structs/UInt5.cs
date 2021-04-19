
namespace Core.Structs
{
    public readonly struct UInt5
    {
        public UInt5(int value)
        {
            _value = value & 0b_0001_1111;
        }

        readonly int _value;

        public static explicit operator UInt5(int i)
        {
            return new UInt5(i);
        }

        public static implicit operator byte(UInt5 i)
        {
            return (byte)i._value;
        }

        public static implicit operator int(UInt5 i)
        {
            return i._value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
