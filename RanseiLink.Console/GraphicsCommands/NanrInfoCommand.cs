using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            new XElement("TFrames", nanr.AnimationBanks.TFrames),
            new XElement("Banks", nanr.AnimationBanks.Banks.Select(bank =>
                new XElement("Bank",
                    new XElement("DataType", bank.DataType),
                    new XElement("Unknown1", bank.Unknown1),
                    new XElement("Unknown2", bank.Unknown2),
                    new XElement("Unknown3", bank.Unknown3),
                    new XElement("Frames", bank.Frames.Select(frame =>
                        new XElement("Frame", 
                            new XElement("Unknown1", frame.Unknown1),
                            new XElement("NumCell", frame.NumCell)
            )))))));

        console.Output.WriteLine(el.ToString());

        return default;
    }
}
