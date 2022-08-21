using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RanseiLink.Core.Graphics
{

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

    public enum MeshDisplayOpCode : byte
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
        public static int ParamLength(this MeshDisplayOpCode command)
        {
            switch (command)
            {
                case MeshDisplayOpCode.NOP: return 0;

                case MeshDisplayOpCode.MTX_MODE: return 1;
                case MeshDisplayOpCode.MTX_PUSH: return 0;
                case MeshDisplayOpCode.MTX_POP: return 1;
                case MeshDisplayOpCode.MTX_STORE: return 1;
                case MeshDisplayOpCode.MTX_RESTORE: return 1;
                case MeshDisplayOpCode.MTX_IDENTITY: return 0;
                case MeshDisplayOpCode.MTX_LOAD_4x4: return 16;
                case MeshDisplayOpCode.MTX_LOAD_4x3: return 12;
                case MeshDisplayOpCode.MTX_MULT_4x4: return 16;
                case MeshDisplayOpCode.MTX_MULT_4x3: return 12;
                case MeshDisplayOpCode.MTX_MULT_3x3: return 9;
                case MeshDisplayOpCode.MTX_SCALE: return 3;
                case MeshDisplayOpCode.MTX_TRANS: return 3;

                case MeshDisplayOpCode.COLOR: return 1;
                case MeshDisplayOpCode.NORMAL: return 1;
                case MeshDisplayOpCode.TEXCOORD: return 1;
                case MeshDisplayOpCode.VTX_16: return 2;
                case MeshDisplayOpCode.VTX_10: return 1;
                case MeshDisplayOpCode.VTX_XY: return 1;
                case MeshDisplayOpCode.VTX_XZ: return 1;
                case MeshDisplayOpCode.VTX_YZ: return 1;
                case MeshDisplayOpCode.VTX_DIFF: return 1;

                case MeshDisplayOpCode.POLYGON_ATTR: return 1;
                case MeshDisplayOpCode.TEXIMAGE_PARAM: return 1;
                case MeshDisplayOpCode.PLTT_BASE: return 1;

                case MeshDisplayOpCode.DIF_AMB: return 1;
                case MeshDisplayOpCode.SPE_EMI: return 1;
                case MeshDisplayOpCode.LIGHT_VECTOR: return 1;
                case MeshDisplayOpCode.LIGHT_COLOR: return 1;
                case MeshDisplayOpCode.SHININESS: return 32;

                case MeshDisplayOpCode.BEGIN_VTXS: return 1;
                case MeshDisplayOpCode.END_VTXS: return 0;

                case MeshDisplayOpCode.SWAP_BUFFERS: return 1;

                case MeshDisplayOpCode.VIEWPORT: return 1;

                case MeshDisplayOpCode.BOX_TEST: return 3;
                case MeshDisplayOpCode.POS_TEST: return 2;
                case MeshDisplayOpCode.VEC_TEST: return 1;

                default: return 0;
            }
        }

        public static int ParamLength(this RenderOpCode command, int flag, BinaryReader br = null)
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
                    switch (flag)
                    {
                        case 0: return 3;
                        case 1: return 4;
                        case 2: return 4;
                        case 3: return 5;
                        default: return -1;
                    }

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
}
