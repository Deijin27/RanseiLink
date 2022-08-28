using RanseiLink.Core.Util;
using System.Numerics;

namespace RanseiLink.Core.Graphics
{
    public static class CommandParamExtractor
    {
        public static int MTX_STORE(PolygonDisplayCommand command)
        {
            return command.Params[0];
        }

        public static int MTX_RESTORE(PolygonDisplayCommand command)
        {
            return command.Params[0];
        }
    
        public static Vector3 MTX_SCALE(PolygonDisplayCommand command)
        {
            return new Vector3(
                FixedPoint.Fix_1_19_12(command.Params[0]), 
                FixedPoint.Fix_1_19_12(command.Params[1]), 
                FixedPoint.Fix_1_19_12(command.Params[2])
                );
        }

        public static Vector3 MTX_TRANS(PolygonDisplayCommand command)
        {
            return new Vector3(
                FixedPoint.Fix_1_19_12(command.Params[0]), 
                FixedPoint.Fix_1_19_12(command.Params[1]), 
                FixedPoint.Fix_1_19_12(command.Params[2])
                );
        }
    
        public static Vector3 NORMAL(PolygonDisplayCommand command)
        {
            return new Vector3(
                FixedPoint.Fix(command.Params[0] & 0x3FF, 1, 0, 9), 
                FixedPoint.Fix((command.Params[0] >> 10) & 0x3FF, 1, 0, 9), 
                FixedPoint.Fix((command.Params[0] >> 20) & 0x3FF, 1, 0, 9)
                );
        }
    
        public static Vector2 TEXCOORD(PolygonDisplayCommand command)
        {
            return new Vector2(
                FixedPoint.Fix(command.Params[0] & 0xFFFF, 1, 11, 4),
                FixedPoint.Fix(command.Params[0] >> 16, 1, 11, 4)
                );
        }
    
        public static Vector3 VTX_16(PolygonDisplayCommand command)
        {
            return new Vector3(
                FixedPoint.Fix_1_3_12(command.Params[0] & 0xFFFF),
                FixedPoint.Fix_1_3_12(command.Params[0] >> 16),
                FixedPoint.Fix_1_3_12(command.Params[1] & 0xFFFF)
            );
        }
    
        public static Vector3 VTX_10(PolygonDisplayCommand command)
        {
            return new Vector3(
                FixedPoint.Fix(command.Params[0] & 0x3FF, 1, 3, 6),
                FixedPoint.Fix(command.Params[0] >> 10 & 0x3FF, 1, 3, 6),
                FixedPoint.Fix(command.Params[0] >> 20 & 0x3FF, 1, 3, 6)
            );
        }
    
        public static Vector3 VTX_XY(PolygonDisplayCommand command)
        {
            return new Vector3(
                FixedPoint.Fix_1_3_12(command.Params[0] & 0xFFFF),
                FixedPoint.Fix_1_3_12(command.Params[0] >> 16),
                0
                );
        }

        public static Vector3 VTX_XZ(PolygonDisplayCommand command)
        {
            return new Vector3(
                FixedPoint.Fix_1_3_12(command.Params[0] & 0xFFFF),
                0,
                FixedPoint.Fix_1_3_12(command.Params[0] >> 16)
                );
        }

        public static Vector3 VTX_YZ(PolygonDisplayCommand command)
        {
            return new Vector3(
                0,
                FixedPoint.Fix_1_3_12(command.Params[0] & 0xFFFF),
                FixedPoint.Fix_1_3_12(command.Params[0] >> 16)
                );
        }
    
        public static Vector3 VTX_DIFF(PolygonDisplayCommand command)
        {
            return new Vector3(
                FixedPoint.Fix(command.Params[0] & 0x3FF, 1, 0, 9),
                FixedPoint.Fix(command.Params[0] >> 10 & 0x3FF, 1, 0, 9),
                FixedPoint.Fix(command.Params[0] >> 20 & 0x3FF, 1, 0, 9)
                );
        }
        
        public static PolygonType BEGIN_VTXS(PolygonDisplayCommand command)
        {
            return (PolygonType)command.Params[0];
        }
    }
}