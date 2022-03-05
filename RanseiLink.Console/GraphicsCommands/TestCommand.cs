using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("test")]
public class TestCommand : BaseCommand
{
    public TestCommand(IServiceContainer container) : base(container) { }
    public TestCommand() : base() { }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var eve = new EVE(@"C:\Users\Mia\Desktop\00000064.eve");

        console.Output.WriteLine("Complete!");

        return default;
    }
}

[Command("eve unpack")]
public class EveUnpackCommand : BaseCommand
{
    public EveUnpackCommand(IServiceContainer container) : base(container) { }
    public EveUnpackCommand() : base() { }

    [CommandParameter(0, Description = "Path of eve file.", Name = "eveFile")]
    public string FilePath { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var eve = new EVE(FilePath);

        string outFolder = FileUtil.MakeUniquePath(Path.Combine(Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath)));
        Directory.CreateDirectory(outFolder);


        {
            string folderA = Path.Combine(outFolder, "A");
            Directory.CreateDirectory(folderA);
            int groupCount = 0;
            foreach (var eventGroup in eve.EventGroupsA)
            {
                string eventGroupFolder = Path.Combine(folderA, groupCount++.ToString().PadLeft(4, '0'));
                Directory.CreateDirectory(eventGroupFolder);
                int eventCount = 0;
                foreach (var e in eventGroup)
                {
                    string eventFile = Path.Combine(eventGroupFolder, eventCount++.ToString().PadLeft(4, '0'));
                    File.WriteAllBytes(eventFile, e.AllData);
                }
            }
        }

        {
            string folderB = Path.Combine(outFolder, "B");
            Directory.CreateDirectory(folderB);
            int groupCount = 0;
            foreach (var eventGroup in eve.EventGroupsA)
            {
                string eventGroupFolder = Path.Combine(folderB, groupCount++.ToString().PadLeft(4, '0'));
                Directory.CreateDirectory(eventGroupFolder);
                int eventCount = 0;
                foreach (var e in eventGroup)
                {
                    string eventFile = Path.Combine(eventGroupFolder, eventCount++.ToString().PadLeft(4, '0'));
                    File.WriteAllBytes(eventFile, e.AllData);
                }
            }
        }

        console.Output.WriteLine("Complete!");

        return default;
    }


}