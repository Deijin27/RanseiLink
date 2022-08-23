using System.Runtime.InteropServices;

namespace System.Numerics
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Matrix3x3
    {
        public static readonly Matrix3x3 Zero = new Matrix3x3();
        public static readonly Matrix3x3 Identity = new Matrix3x3() { M11 = 1.0f, M22 = 1.0f, M33 = 1.0f };

        public float M11;
        public float M12;
        public float M13;
        public float M21;
        public float M22;
        public float M23;
        public float M31;
        public float M32;
        public float M33;

        public Matrix3x3(float M11, float M12, float M13,
                        float M21, float M22, float M23,
                        float M31, float M32, float M33)
        {
            this.M11 = M11; this.M12 = M12; this.M13 = M13;
            this.M21 = M21; this.M22 = M22; this.M23 = M23;
            this.M31 = M31; this.M32 = M32; this.M33 = M33;
        }

        public Matrix4x4 As4x4()
        {
            Matrix4x4 result = Matrix4x4.Identity;
            result.M11 = this.M11;
            result.M12 = this.M12;
            result.M13 = this.M13;
            result.M21 = this.M21;
            result.M22 = this.M22;
            result.M23 = this.M23;
            result.M31 = this.M31;
            result.M32 = this.M32;
            result.M33 = this.M33;
            return result;
        }

        public override string ToString()
        {
            return $"Matrix3x3 [[{M11}, {M12}, {M13}], [{M21}, {M22}, {M23}], [{M31}, {M32}, {M33}]]";
        }
    }
}
