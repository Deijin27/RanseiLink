using RanseiLink.Core.Util;
using System.Numerics;

namespace RanseiLink.Core.Graphics
{
    public static class CommandGenerator
    {
        // Atomize Parameterless
        private static readonly PolygonDisplayCommand _NOP = new PolygonDisplayCommand { OpCode = PolygonDisplayOpCode.NOP };
        private static readonly PolygonDisplayCommand _END_VTXS = new PolygonDisplayCommand { OpCode = PolygonDisplayOpCode.END_VTXS };

        public static PolygonDisplayCommand NOP()
        {
            return _NOP;
        }

        public static PolygonDisplayCommand MTX_STORE(int stackIndex)
        {
            return new PolygonDisplayCommand
            {
                OpCode = PolygonDisplayOpCode.MTX_STORE,
                Params = new int[] { stackIndex }
            };
        }

        public static PolygonDisplayCommand MTX_RESTORE(int stackIndex)
        {
            return new PolygonDisplayCommand
            {
                OpCode = PolygonDisplayOpCode.MTX_RESTORE,
                Params = new int[] { stackIndex }
            };
        }

        public static PolygonDisplayCommand MTX_SCALE(Vector3 scale)
        {
            return new PolygonDisplayCommand
            {
                OpCode = PolygonDisplayOpCode.MTX_SCALE,
                Params = new int[] { FixedPoint.InverseFix_1_19_12(scale.X), FixedPoint.InverseFix_1_19_12(scale.Y), FixedPoint.InverseFix_1_19_12(scale.Z) }
            };
        }

        public static PolygonDisplayCommand MTX_TRANS(Vector3 trans)
        {
            return new PolygonDisplayCommand
            {
                OpCode = PolygonDisplayOpCode.MTX_TRANS,
                Params = new int[] { FixedPoint.InverseFix_1_19_12(trans.X), FixedPoint.InverseFix_1_19_12(trans.Y), FixedPoint.InverseFix_1_19_12(trans.Z) }
            };
        }

        public static PolygonDisplayCommand NORMAL(Vector3 normal)
        {
            return new PolygonDisplayCommand
            {
                OpCode = PolygonDisplayOpCode.NORMAL,
                Params = new int[] { FixedPoint.InverseFix(normal.X, 1, 0, 9) | FixedPoint.InverseFix(normal.Y, 1, 0, 9) << 10 | FixedPoint.InverseFix(normal.Z, 1, 0, 9) << 20 }
            };
        }

        public static PolygonDisplayCommand TEXCOORD(Vector2 texCoord)
        {
            return new PolygonDisplayCommand
            {
                OpCode = PolygonDisplayOpCode.TEXCOORD,
                Params = new int[] { FixedPoint.InverseFix(texCoord.X, 1, 11, 4) | FixedPoint.InverseFix(texCoord.Y, 1, 11, 4) << 16 }
            };
        }

        public static PolygonDisplayCommand VTX_16(Vector3 vertex)
        {
            return new PolygonDisplayCommand
            {
                OpCode = PolygonDisplayOpCode.VTX_16,
                Params = new[] { FixedPoint.InverseFix_1_3_12(vertex.X) | FixedPoint.InverseFix_1_3_12(vertex.Y) << 16, FixedPoint.InverseFix_1_3_12(vertex.Z) }
            };
        }

        public static PolygonDisplayCommand VTX_10(Vector3 vertex)
        {
            return new PolygonDisplayCommand
            {
                OpCode = PolygonDisplayOpCode.VTX_10,
                Params = new int[] { FixedPoint.InverseFix(vertex.X, 1, 3, 6) | FixedPoint.InverseFix(vertex.Y, 1, 3, 6) << 10 | FixedPoint.InverseFix(vertex.Z, 1, 3, 6) << 20 }
            };
        }

        public static PolygonDisplayCommand VTX_XY(Vector3 vertex)
        {
            return new PolygonDisplayCommand
            {
                OpCode = PolygonDisplayOpCode.VTX_XY,
                Params = new int[] { FixedPoint.InverseFix_1_3_12(vertex.X) | FixedPoint.InverseFix_1_3_12(vertex.Y) << 16 }
            };
        }

        public static PolygonDisplayCommand VTX_XZ(Vector3 vertex)
        {
            return new PolygonDisplayCommand
            {
                OpCode = PolygonDisplayOpCode.VTX_XZ,
                Params = new int[] { FixedPoint.InverseFix_1_3_12(vertex.X) | FixedPoint.InverseFix_1_3_12(vertex.Z) << 16 }
            };
        }

        public static PolygonDisplayCommand VTX_YZ(Vector3 vertex)
        {
            return new PolygonDisplayCommand
            {
                OpCode = PolygonDisplayOpCode.VTX_YZ,
                Params = new int[] { FixedPoint.InverseFix_1_3_12(vertex.Y) | FixedPoint.InverseFix_1_3_12(vertex.Z) << 16 }
            };
        }

        public static PolygonDisplayCommand VTX_DIFF(Vector3 diff)
        {
            return new PolygonDisplayCommand
            {
                OpCode = PolygonDisplayOpCode.VTX_DIFF,
                Params = new int[] { FixedPoint.InverseFix(diff.X, 1, 0, 9) | FixedPoint.InverseFix(diff.Y, 1, 0, 9) << 10 | FixedPoint.InverseFix(diff.Z, 1, 0, 9) << 20 }
            };
        }

        public static PolygonDisplayCommand BEGIN_VTXS(PolygonType polyType)
        {
            return new PolygonDisplayCommand
            {
                OpCode = PolygonDisplayOpCode.BEGIN_VTXS,
                Params = new int[] { (int)polyType }
            };
        }

        public static PolygonDisplayCommand END_VTXS()
        {
            return _END_VTXS;
        }
    }
}