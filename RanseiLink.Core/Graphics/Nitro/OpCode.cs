namespace RanseiLink.Core.Graphics;

public enum RenderOpCode
{
    NOP = 0x0,
    END = 0x1,
    VISIBILITY = 0x2,
    MTX_RESTORE = 0x3,
    BIND_MATERIAL = 0x4,
    DRAW_MESH = 0x5,
    MTX_MULT = 0x6,
    UNKNOWN_7 = 0x7,
    UNKNOWN_8 = 0x8,
    SKIN = 0x9,
    UNKNOWN_10 = 0xA,
    MTX_SCALE = 0xB,
    UNKNOWN_12 = 0xC,
    UNKNOWN_13 = 0xD,
}

public enum PolygonDisplayOpCode : byte
{
    NOP = 0x00,
    MTX_MODE = 0x10,
    MTX_PUSH = 0x11,
    MTX_POP = 0x12,
    MTX_STORE = 0x13,
    MTX_RESTORE = 0x14,
    MTX_IDENTITY = 0x15,
    MTX_LOAD_4x4 = 0x16,
    MTX_LOAD_4x3 = 0x17,
    MTX_MULT_4x4 = 0x18,
    MTX_MULT_4x3 = 0x19,
    MTX_MULT_3x3 = 0x1A,
    MTX_SCALE = 0x1B,
    MTX_TRANS = 0x1C,
    COLOR = 0x20,
    NORMAL = 0x21,
    TEXCOORD = 0x22,
    VTX_16 = 0x23,
    VTX_10 = 0x24,
    VTX_XY = 0x25,
    VTX_XZ = 0x26,
    VTX_YZ = 0x27,
    VTX_DIFF = 0x28,
    POLYGON_ATTR = 0x29,
    TEXIMAGE_PARAM = 0x2A,
    PLTT_BASE = 0x2B,
    DIF_AMB = 0x30,
    SPE_EMI = 0x31,
    LIGHT_VECTOR = 0x32,
    LIGHT_COLOR = 0x33,
    SHININESS = 0x34,
    BEGIN_VTXS = 0x40,
    END_VTXS = 0x41,
    SWAP_BUFFERS = 0x50,
    VIEWPORT = 0x60,
    BOX_TEST = 0x70,
    POS_TEST = 0x71,
    VEC_TEST = 0x72,
}

public static class OpCodeExtensions
{
    public static int ParamLength(this PolygonDisplayOpCode command)
    {
        return command switch
        {
            PolygonDisplayOpCode.NOP => 0,
            PolygonDisplayOpCode.MTX_MODE => 1,
            PolygonDisplayOpCode.MTX_PUSH => 0,
            PolygonDisplayOpCode.MTX_POP => 1,
            PolygonDisplayOpCode.MTX_STORE => 1,
            PolygonDisplayOpCode.MTX_RESTORE => 1,
            PolygonDisplayOpCode.MTX_IDENTITY => 0,
            PolygonDisplayOpCode.MTX_LOAD_4x4 => 16,
            PolygonDisplayOpCode.MTX_LOAD_4x3 => 12,
            PolygonDisplayOpCode.MTX_MULT_4x4 => 16,
            PolygonDisplayOpCode.MTX_MULT_4x3 => 12,
            PolygonDisplayOpCode.MTX_MULT_3x3 => 9,
            PolygonDisplayOpCode.MTX_SCALE => 3,
            PolygonDisplayOpCode.MTX_TRANS => 3,
            PolygonDisplayOpCode.COLOR => 1,
            PolygonDisplayOpCode.NORMAL => 1,
            PolygonDisplayOpCode.TEXCOORD => 1,
            PolygonDisplayOpCode.VTX_16 => 2,
            PolygonDisplayOpCode.VTX_10 => 1,
            PolygonDisplayOpCode.VTX_XY => 1,
            PolygonDisplayOpCode.VTX_XZ => 1,
            PolygonDisplayOpCode.VTX_YZ => 1,
            PolygonDisplayOpCode.VTX_DIFF => 1,
            PolygonDisplayOpCode.POLYGON_ATTR => 1,
            PolygonDisplayOpCode.TEXIMAGE_PARAM => 1,
            PolygonDisplayOpCode.PLTT_BASE => 1,
            PolygonDisplayOpCode.DIF_AMB => 1,
            PolygonDisplayOpCode.SPE_EMI => 1,
            PolygonDisplayOpCode.LIGHT_VECTOR => 1,
            PolygonDisplayOpCode.LIGHT_COLOR => 1,
            PolygonDisplayOpCode.SHININESS => 32,
            PolygonDisplayOpCode.BEGIN_VTXS => 1,
            PolygonDisplayOpCode.END_VTXS => 0,
            PolygonDisplayOpCode.SWAP_BUFFERS => 1,
            PolygonDisplayOpCode.VIEWPORT => 1,
            PolygonDisplayOpCode.BOX_TEST => 3,
            PolygonDisplayOpCode.POS_TEST => 2,
            PolygonDisplayOpCode.VEC_TEST => 1,
            _ => 0,
        };
    }

    public static int ParamLength(this RenderOpCode command, int flag, BinaryReader? br = null)
    {
        switch (command)
        {
            case RenderOpCode.NOP: return 0;
            case RenderOpCode.END: return 0;
            case RenderOpCode.VISIBILITY: return 2;
            case RenderOpCode.MTX_RESTORE: return 1;
            case RenderOpCode.BIND_MATERIAL: return 1;
            case RenderOpCode.DRAW_MESH: return 1;

            case RenderOpCode.MTX_MULT:
                return flag switch
                {
                    0 => 3,
                    1 => 4,
                    2 => 4,
                    3 => 5,
                    _ => -1,
                };
            case RenderOpCode.UNKNOWN_7: return 1;
            case RenderOpCode.UNKNOWN_8: return 1;

            case RenderOpCode.SKIN:
                if (br == null)
                {
                    return -1;
                }
                br.ReadByte();
                var count = br.ReadByte();
                br.BaseStream.Position = br.BaseStream.Position - 2;
                return 2 + count * 3;

            case RenderOpCode.UNKNOWN_10: return 0;
            case RenderOpCode.MTX_SCALE: return 0;
            case RenderOpCode.UNKNOWN_12: return 2;
            case RenderOpCode.UNKNOWN_13: return 2;

            default: return -1;
        }
    }
}
