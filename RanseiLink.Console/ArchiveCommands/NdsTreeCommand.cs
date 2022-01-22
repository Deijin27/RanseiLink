using CliFx;
using CliFx.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using CliFx.Infrastructure;
using RanseiLink.Core.Nds;

namespace RanseiLink.Console.ArchiveCommands;

[Command("nds tree", Description = "Print the name table of the nds file to the console.")]
public class NdsTreeCommand : ICommand
{
    [CommandParameter(0, Description = "Path to the nds file.", Name = "source", Converter = typeof(PathConverter))]
    public string Source { get; set; }

    [CommandOption("startOffset", 'o', Description = "Optionally set the start offset of the name table in the file", IsRequired = false)]
    public long? StartOffset { get; set; }

    private static long GetStartOffset(BinaryReader stream, long position)
    {
        stream.BaseStream.Position = position;
        return stream.ReadUInt32();
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        using var stream = new BinaryReader(new FileStream(Source, FileMode.Open, FileAccess.Read));

        long startOffset = StartOffset ?? GetStartOffset(stream, Nds.NdsConfig.NameTableStartOffsetPositon);

        List<NdsNameTable.FileOrFolderName> contents = NdsNameTable.GetRootFolderContents(stream, startOffset);

        PrintContents(contents, stream, "", console, startOffset);

        return default;
    }

    void PrintContents(List<NdsNameTable.FileOrFolderName> contents, BinaryReader stream, string indent, IConsole console, long startOffset)
    {
        var count = 1;
        foreach (var cont in contents)
        {
            string treeStr;
            string newIndent;

            if (count == contents.Count)
            {
                // is last in branch
                treeStr = "└───";
                newIndent = indent + "    ";
            }
            else
            {
                // is not last in branch
                treeStr = "├───";
                newIndent = indent + "│   ";
            }
            count++;

            console.Output.WriteLine(indent + treeStr + Encoding.UTF8.GetString(cont.Name) + (cont.IsFolder ? " (Folder)" : " (File)"));

            if (cont.IsFolder)
            {
                var alloc = NdsNameTable.GetAllocationData(stream, startOffset, cont.ContentsIndexIfFolder);
                var newCont = NdsNameTable.GetContents(stream, startOffset, alloc);
                PrintContents(newCont, stream, newIndent, console, startOffset);
            }
        }
    }


}
