
namespace Core.Structs
{
    public readonly struct UInt7
    {
        public UInt7(int value)
        {
            _value = value & 0b_0111_1111;
        }

        readonly int _value;

        public static explicit operator UInt7(int i)
        {
            return new UInt7(i);
        }

        public static implicit operator byte(UInt7 i)
        {
            return (byte)i._value;
        }

        public static implicit operator int(UInt7 i)
        {
            return i._value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
