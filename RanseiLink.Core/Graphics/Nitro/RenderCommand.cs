#nullable enable
using System;
using System.IO;

namespace RanseiLink.Core.Graphics;

[System.Diagnostics.DebuggerDisplay("RenderCommand: {OpCode}, {Flags}")]
public class RenderCommand
{
    public RenderOpCode OpCode { get; set; }
    public int Flags { get; set; }
    public byte[] Parameters { get; set; }

    public RenderCommand(RenderOpCode opCode, int flags = 0)
    {
        OpCode = opCode;
        Flags = flags;
        Parameters = Array.Empty<byte>();
    }
    public RenderCommand(RenderOpCode opCode, byte[] parameters, int flags = 0)
    {
        OpCode = opCode;
        Flags = flags;
        Parameters = parameters;
    }
    public RenderCommand(BinaryReader br)
    {
        var b = br.ReadByte();

        OpCode = (RenderOpCode)(b & 0xF);
        Flags = b >> 5;
        var paramLength = OpCode.ParamLength(Flags, br);
        if (paramLength != 0)
        {
            Parameters = br.ReadBytes(paramLength);
        }
        else
        {
            Parameters = Array.Empty<byte>();
        }
    }

    public void WriteTo(BinaryWriter bw)
    {
        var b = (int)OpCode | (Flags << 5);
        bw.Write((byte)b);
        if (Parameters != null && Parameters.Length > 0)
        {
            bw.Write(Parameters);
        }
    }
}
