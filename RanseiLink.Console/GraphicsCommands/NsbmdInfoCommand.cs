using RanseiLink.Core.Graphics;
using System.Xml.Linq;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nsbmd info", Description = "Print out informational content of NSBMD")]
public class NsbmdInfoCommand : ICommand
{
    [CommandParameter(0, Description = "Path of nsbmd file", Name = "filePath")]
    public string FilePath { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var nsbmd = new NSBMD(FilePath);
        var models = nsbmd.Model;

        var outerEl = new XElement("NSBMD");
        var el = new XElement("NSMDL");
        outerEl.Add(el);

        el.Add(new XAttribute("ModelCount", models.Models.Count));

        foreach (var model in models.Models)
        {
            var modelEl = new XElement("Model");
            el.Add(modelEl);

            modelEl.Add(new XAttribute("Name", model.Name));
            modelEl.Add(model.MdlInfo.Serialise());


            var commands = new XElement("RenderCommands");
            modelEl.Add(commands);
            foreach (var command in model.RenderCommands)
            {
                var cmdEl = new XElement(command.OpCode.ToString());
                string flagsText;
                if (command.OpCode == RenderOpCode.MTX_MULT)
                {
                    flagsText = command.Flags switch
                    {
                        0 => "MULT",
                        1 => "STORE",
                        2 => "RESTORE",
                        3 => "STORE RESTORE",
                        _ => command.Flags.ToString()
                    };
                }
                else if (command.OpCode == RenderOpCode.MTX_SCALE)
                {
                    flagsText = command.Flags switch
                    {
                        0 => "UP",
                        1 => "DOWN",
                        _ => command.Flags.ToString()
                    };
                }
                else
                {
                    flagsText = command.Flags.ToString();
                }
                cmdEl.Add(new XAttribute("Flags", flagsText));
                cmdEl.Add(new XAttribute("Params", string.Join(",", command.Parameters)));
                commands.Add(cmdEl);
            }
            


            var polymeshes = new XElement("Polymeshes");
            modelEl.Add(polymeshes);
            foreach (var polymesh in model.Polymeshes)
            {
                polymeshes.Add(new XElement("Polymesh", new XAttribute("Name", polymesh.Name)));
            }

            var polygons = new XElement("Polygons");
            modelEl.Add(polygons);
            foreach (var polygon in model.Polygons)
            {
                polygons.Add(new XElement("Polygon", new XAttribute("Name", polygon.Name)));
            }
        }

        console.Output.WriteLine(el.ToString());

        return default;
    }
}
