namespace RanseiLink.Core.Graphics;

[System.Diagnostics.DebuggerDisplay("PolygonDisplayCommand: {OpCode}")]
public class PolygonDisplayCommand
{
    public PolygonDisplayOpCode OpCode { get; }
    public int[] Params { get; }

    public PolygonDisplayCommand(PolygonDisplayOpCode opCode)
    {
        OpCode = opCode;
        Params = Array.Empty<int>();
    }

    public PolygonDisplayCommand(PolygonDisplayOpCode opCode, int[] @params)
    {
        OpCode = opCode;
        Params = @params;
    }
}
