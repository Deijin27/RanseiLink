using RanseiLink.Core.Graphics;
using System.Xml.Linq;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nanr info", Description = "Print out informational content of Nitro Animation Resource")]
public class NanrInfoCommand : ICommand
{
    [CommandParameter(0, Description = "Path of nanr file", Name = "filePath")]
    public string FilePath { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var nanr = NANR.Load(FilePath);

        var el = new XElement("AnimationBank",
            new XElement("Banks", nanr.AnimationBanks.Banks.Select((bank, i) =>
                new XElement("Bank",
                    new XAttribute("Name", nanr.Labels.Names[i]),
                    new XElement("DataType", bank.DataType),
                    new XElement("Unknown1", bank.Unknown1),
                    new XElement("Unknown2", bank.Unknown2),
                    new XElement("Unknown3", bank.Unknown3),
                    new XElement("Frames", bank.Frames.Select(frame =>
                        new XElement("Frame",
                            new XAttribute("Cluster", frame.Cluster),
                            new XAttribute("Duration", frame.Duration)
                            
                )))))),
            new XElement("Labels", nanr.Labels.Names.Select(name => new XElement("Name", name)))
            );

        console.Output.WriteLine(el.ToString());

        return default;
    }
}
