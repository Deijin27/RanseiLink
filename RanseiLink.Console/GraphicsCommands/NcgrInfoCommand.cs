using RanseiLink.Core.Graphics;
using System.Xml.Linq;

namespace RanseiLink.Console.GraphicsCommands;

[Command("ncgr info", Description = "Print out informational content of Nitro Character Graphic Resource")]
public class NcgrInfoCommand : ICommand
{
    [CommandParameter(0, Description = "Path of ncgr file", Name = "filePath")]
    public string FilePath { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var ncgr = NCGR.Load(FilePath);
        var c = ncgr.Pixels;
        var el = new XElement("NCGR",
            new XElement("CHAR",
                new XElement("IsTiled", c.IsTiled),
                new XElement("Format", c.Format),
                new XElement("TilesPerRow", c.TilesPerRow),
                new XElement("TilesPerColumn", c.TilesPerColumn),
                new XElement("Unknown1", c.Unknown1),
                new XElement("Unknown2", c.Unknown2)
                ));

        console.Output.WriteLine(el.ToString());

        return default;
    }
}
