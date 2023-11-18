using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nclr info", Description = "Print out informational content of Nitro Color Resource")]
public class NclrInfoCommand : ICommand
{
    [CommandParameter(0, Description = "Path of nclr file", Name = "filePath")]
    public string FilePath { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var ncgr = NCLR.Load(FilePath);
        var c = ncgr.PaletteCollectionMap.Palettes ?? new List<ushort> { 0 };
        var p = ncgr.Palettes;

        var paletteLen = p.Format switch
        {
            TexFormat.Pltt16 => 16,
            TexFormat.Pltt256 => 256,
            _ => throw new System.NotImplementedException()
        };
        var palettes = c.Select(x => p.Palette[(paletteLen * x)..(paletteLen * x + paletteLen)]).ToArray();

        var el = new XElement("NCLR");

        if (ncgr.PaletteCollectionMap != null)
        {
            el.Add(new XElement("PCMP", string.Join(", ", c)));
        }
        el.Add(new XElement("PLTT",
            new XAttribute("Format", p.Format),
            new XAttribute("Count", p.Palette.Length),
                palettes.Select(pal => 
                    new XElement("Palette", pal.Select(clr => 
                        new XElement("Color", 
                            new XAttribute("Rgb15", clr), 
                            new XAttribute("Rgba32", PaletteUtil.To32BitColor(clr))))
            ))));

        console.Output.WriteLine(el.ToString());





        return default;
    }
}
