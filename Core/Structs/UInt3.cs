
namespace Core.Structs
{
    public readonly struct UInt3
    {
        public UInt3(int value)
        {
            _value = value & 0b_0000_0111;
        }

        readonly int _value;

        public static explicit operator UInt3(int i)
        {
            return new UInt3(i);
        }
        public static explicit operator UInt3(uint i)
        {
            return new UInt3((int)i);
        }

        public static implicit operator byte(UInt3 i)
        {
            return (byte)i._value;
        }

        public static implicit operator int(UInt3 i)
        {
            return i._value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
