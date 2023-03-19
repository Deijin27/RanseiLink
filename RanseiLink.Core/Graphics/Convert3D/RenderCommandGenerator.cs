namespace RanseiLink.Core.Graphics
{
    public static class RenderCommandGenerator
    {
        public static RenderCommand NOP()
        {
            return new RenderCommand
            (
                opCode: RenderOpCode.NOP
            );
        }

        public static RenderCommand END()
        {
            return new RenderCommand
            (
                opCode: RenderOpCode.END
            );
        }

        public static RenderCommand VISIBILITY(int polymeshId, bool isVisible)
        {
            return new RenderCommand
            (
                opCode: RenderOpCode.VISIBILITY,
                parameters: new byte[] { (byte)polymeshId, (byte)(isVisible ? 1 : 0) }
            );
        }
    
        public static RenderCommand MTX_RESTORE(int stackIndex)
        {
            return new RenderCommand
            (
                opCode: RenderOpCode.MTX_RESTORE,
                parameters: new byte[] { (byte)stackIndex }
            );
        }

        public static RenderCommand BIND_MATERIAL(int materialIndex)
        {
            return new RenderCommand
            (
                opCode: RenderOpCode.BIND_MATERIAL,
                parameters: new byte[] { (byte)materialIndex }
            );
        }
    
        public static RenderCommand DRAW_MESH(int polygonIndex)
        {
            return new RenderCommand
            (
                opCode: RenderOpCode.DRAW_MESH,
                parameters: new byte[] { (byte)polygonIndex }
            );
        }

        public static RenderCommand MTX_MULT(int polymeshId, int parentId, int unknown)
        {
            return new RenderCommand
            (
                opCode: RenderOpCode.MTX_MULT,
                flags: 0,
                parameters: new byte[] { (byte)polymeshId, (byte)parentId, (byte)unknown }
            );
        }

        public static RenderCommand MTX_MULT_STORE(int polymeshId, int parentId, int unknown, int storeIndex)
        {
            return new RenderCommand
            (
                opCode: RenderOpCode.MTX_MULT,
                flags: 1,
                parameters: new byte[]  { (byte)polymeshId, (byte)parentId, (byte)unknown, (byte)storeIndex }
            );
        }

        public static RenderCommand MTX_MULT_RESTORE(int polymeshId, int parentId, int unknown, int restoreIndex)
        {
            return new RenderCommand
            (
                opCode: RenderOpCode.MTX_MULT,
                flags: 2,
                parameters: new byte[] { (byte)polymeshId, (byte)parentId, (byte)unknown, (byte)restoreIndex }
            );
        }

        public static RenderCommand MTX_MULT_STORE_RESTORE(int polymeshId, int parentId, int unknown, int storeIndex, int restoreIndex)
        {
            return new RenderCommand
            (
                opCode: RenderOpCode.MTX_MULT,
                flags: 3,
                parameters: new byte[] { (byte)polymeshId, (byte)parentId, (byte)unknown, (byte)storeIndex, (byte)restoreIndex }
            );
        }

        public static RenderCommand MTX_SCALE_DOWN()
        {
            return new RenderCommand
            (
                opCode: RenderOpCode.MTX_SCALE,
                flags: 1
            );
        }

        public static RenderCommand MTX_SCALE_UP()
        {
            return new RenderCommand
            (
                opCode: RenderOpCode.MTX_SCALE
            );
        }

        public static RenderCommand UNKNOWN_7(int polymeshId)
        {
            return new RenderCommand
            (
                opCode: RenderOpCode.UNKNOWN_7,
                parameters: new byte[] { (byte)polymeshId }
            );
        }
    }

}