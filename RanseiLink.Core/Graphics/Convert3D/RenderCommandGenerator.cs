namespace RanseiLink.Core.Graphics
{
    public static class RenderCommandGenerator
    {
        public static RenderCommand NOP()
        {
            return new RenderCommand
            {
                OpCode = RenderOpCode.NOP
            };
        }

        public static RenderCommand END()
        {
            return new RenderCommand
            {
                OpCode = RenderOpCode.END
            };
        }

        public static RenderCommand VISIBILITY(int polymeshId, bool isVisible)
        {
            return new RenderCommand
            {
                OpCode = RenderOpCode.VISIBILITY,
                Parameters = new byte[] { (byte)polymeshId, (byte)(isVisible ? 1 : 0) }
            };
        }
    
        public static RenderCommand MTX_RESTORE(int stackIndex)
        {
            return new RenderCommand
            {
                OpCode = RenderOpCode.MTX_RESTORE,
                Parameters = new byte[] { (byte)stackIndex }
            };
        }

        public static RenderCommand BIND_MATERIAL(int materialIndex)
        {
            return new RenderCommand
            {
                OpCode = RenderOpCode.BIND_MATERIAL,
                Parameters = new byte[] { (byte)materialIndex }
            };
        }
    
        public static RenderCommand DRAW_MESH(int polygonIndex)
        {
            return new RenderCommand
            {
                OpCode = RenderOpCode.DRAW_MESH,
                Parameters = new byte[] { (byte)polygonIndex }
            };
        }

        public static RenderCommand MTX_MULT(int polymeshId, int parentId, int unknown)
        {
            return new RenderCommand
            {
                OpCode = RenderOpCode.MTX_MULT,
                Flags = 0,
                Parameters = new byte[] { (byte)polymeshId, (byte)parentId, (byte)unknown }
            };
        }

        public static RenderCommand MTX_MULT_STORE(int polymeshId, int parentId, int unknown, int storeIndex)
        {
            return new RenderCommand
            {
                OpCode = RenderOpCode.MTX_MULT,
                Flags = 1,
                Parameters = new byte[]  { (byte)polymeshId, (byte)parentId, (byte)unknown, (byte)storeIndex }
            };
        }

        public static RenderCommand MTX_MULT_RESTORE(int polymeshId, int parentId, int unknown, int restoreIndex)
        {
            return new RenderCommand
            {
                OpCode = RenderOpCode.MTX_MULT,
                Flags = 2,
                Parameters = new byte[] { (byte)polymeshId, (byte)parentId, (byte)unknown, (byte)restoreIndex }
            };
        }

        public static RenderCommand MTX_MULT_STORE_RESTORE(int polymeshId, int parentId, int unknown, int storeIndex, int restoreIndex)
        {
            return new RenderCommand
            {
                OpCode = RenderOpCode.MTX_MULT,
                Flags = 3,
                Parameters = new byte[] { (byte)polymeshId, (byte)parentId, (byte)unknown, (byte)storeIndex, (byte)restoreIndex }
            };
        }

        public static RenderCommand MTX_SCALE_DOWN()
        {
            return new RenderCommand
            {
                OpCode = RenderOpCode.MTX_SCALE,
                Flags = 1
            };
        }

        public static RenderCommand MTX_SCALE_UP()
        {
            return new RenderCommand
            {
                OpCode = RenderOpCode.MTX_SCALE
            };
        }

        public static RenderCommand UNKNOWN_7(int polymeshId)
        {
            return new RenderCommand
            {
                OpCode = RenderOpCode.UNKNOWN_7,
                Parameters = new byte[] { (byte)polymeshId }
            };
        }
    }

}