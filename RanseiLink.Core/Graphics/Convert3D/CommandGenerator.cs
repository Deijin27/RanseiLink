using RanseiLink.Core.Util;
using System.Numerics;

namespace RanseiLink.Core.Graphics;

public static class CommandGenerator
{
    // Atomize Parameterless
    private static readonly PolygonDisplayCommand _NOP = new PolygonDisplayCommand(PolygonDisplayOpCode.NOP);
    private static readonly PolygonDisplayCommand _END_VTXS = new PolygonDisplayCommand(PolygonDisplayOpCode.END_VTXS);

    public static PolygonDisplayCommand NOP()
    {
        return _NOP;
    }

    public static PolygonDisplayCommand MTX_STORE(int stackIndex)
    {
        return new PolygonDisplayCommand
        (
            opCode: PolygonDisplayOpCode.MTX_STORE,
            @params: new int[] { stackIndex }
        );
    }

    public static PolygonDisplayCommand MTX_RESTORE(int stackIndex)
    {
        return new PolygonDisplayCommand
        (
            opCode: PolygonDisplayOpCode.MTX_RESTORE,
            @params: new int[] { stackIndex }
        );
    }

    public static PolygonDisplayCommand MTX_SCALE(Vector3 scale)
    {
        return new PolygonDisplayCommand
        (
            opCode: PolygonDisplayOpCode.MTX_SCALE,
            @params: new int[] { FixedPoint.InverseFix_1_19_12(scale.X), FixedPoint.InverseFix_1_19_12(scale.Y), FixedPoint.InverseFix_1_19_12(scale.Z) }
        );
    }

    public static PolygonDisplayCommand MTX_TRANS(Vector3 trans)
    {
        return new PolygonDisplayCommand
        (
            opCode: PolygonDisplayOpCode.MTX_TRANS,
            @params: new int[] { FixedPoint.InverseFix_1_19_12(trans.X), FixedPoint.InverseFix_1_19_12(trans.Y), FixedPoint.InverseFix_1_19_12(trans.Z) }
        );
    }

    public static PolygonDisplayCommand NORMAL(Vector3 normal)
    {
        return new PolygonDisplayCommand
        (
            opCode: PolygonDisplayOpCode.NORMAL,
            @params: new int[] { FixedPoint.InverseFix(normal.X, 1, 0, 9) | FixedPoint.InverseFix(normal.Y, 1, 0, 9) << 10 | FixedPoint.InverseFix(normal.Z, 1, 0, 9) << 20 }
        );
    }

    public static PolygonDisplayCommand TEXCOORD(Vector2 texCoord)
    {
        return new PolygonDisplayCommand
        (
            opCode: PolygonDisplayOpCode.TEXCOORD,
            @params: new int[] { FixedPoint.InverseFix(texCoord.X, 1, 11, 4) | FixedPoint.InverseFix(texCoord.Y, 1, 11, 4) << 16 }
        );
    }

    public static PolygonDisplayCommand VTX_16(Vector3 vertex)
    {
        return new PolygonDisplayCommand
        (
            opCode: PolygonDisplayOpCode.VTX_16,
            @params: new[] { FixedPoint.InverseFix_1_3_12(vertex.X) | FixedPoint.InverseFix_1_3_12(vertex.Y) << 16, FixedPoint.InverseFix_1_3_12(vertex.Z) }
        );
    }

    public static PolygonDisplayCommand VTX_10(Vector3 vertex)
    {
        return new PolygonDisplayCommand
        (
            opCode: PolygonDisplayOpCode.VTX_10,
            @params: new int[] { FixedPoint.InverseFix(vertex.X, 1, 3, 6) | FixedPoint.InverseFix(vertex.Y, 1, 3, 6) << 10 | FixedPoint.InverseFix(vertex.Z, 1, 3, 6) << 20 }
        );
    }

    public static PolygonDisplayCommand VTX_XY(Vector3 vertex)
    {
        return new PolygonDisplayCommand
        (
            opCode: PolygonDisplayOpCode.VTX_XY,
            @params: new int[] { FixedPoint.InverseFix_1_3_12(vertex.X) | FixedPoint.InverseFix_1_3_12(vertex.Y) << 16 }
        );
    }

    public static PolygonDisplayCommand VTX_XZ(Vector3 vertex)
    {
        return new PolygonDisplayCommand
        (
            opCode: PolygonDisplayOpCode.VTX_XZ,
            @params: new int[] { FixedPoint.InverseFix_1_3_12(vertex.X) | FixedPoint.InverseFix_1_3_12(vertex.Z) << 16 }
        );
    }

    public static PolygonDisplayCommand VTX_YZ(Vector3 vertex)
    {
        return new PolygonDisplayCommand
        (
            opCode: PolygonDisplayOpCode.VTX_YZ,
            @params: new int[] { FixedPoint.InverseFix_1_3_12(vertex.Y) | FixedPoint.InverseFix_1_3_12(vertex.Z) << 16 }
        );
    }

    public static PolygonDisplayCommand VTX_DIFF(Vector3 diff)
    {
        return new PolygonDisplayCommand
        (
            opCode: PolygonDisplayOpCode.VTX_DIFF,
            @params: new int[] { FixedPoint.InverseFix(diff.X, 1, 0, 9) | FixedPoint.InverseFix(diff.Y, 1, 0, 9) << 10 | FixedPoint.InverseFix(diff.Z, 1, 0, 9) << 20 }
        );
    }

    public static PolygonDisplayCommand BEGIN_VTXS(PolygonType polyType)
    {
        return new PolygonDisplayCommand
        (
            opCode: PolygonDisplayOpCode.BEGIN_VTXS,
            @params: new int[] { (int)polyType }
        );
    }

    public static PolygonDisplayCommand END_VTXS()
    {
        return _END_VTXS;
    }
}