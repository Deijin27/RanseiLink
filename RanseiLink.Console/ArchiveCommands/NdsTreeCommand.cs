using System.Text;
using RanseiLink.Core.RomFs;

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

        long startOffset = StartOffset ?? GetStartOffset(stream, 0x40);

        List<RomFsNameTable.FileOrFolderName> contents = RomFsNameTable.GetRootFolderContents(stream, startOffset);

        PrintContents(contents, stream, "", console, startOffset);

        return default;
    }

    void PrintContents(List<RomFsNameTable.FileOrFolderName> contents, BinaryReader stream, string indent, IConsole console, long startOffset)
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

            console.WriteLine(indent + treeStr + Encoding.UTF8.GetString(cont.Name) + (cont.IsFolder ? " (Folder)" : " (File)"));

            if (cont.IsFolder)
            {
                var alloc = RomFsNameTable.GetAllocationData(stream, startOffset, cont.ContentsIndexIfFolder);
                var newCont = RomFsNameTable.GetContents(stream, startOffset, alloc);
                PrintContents(newCont, stream, newIndent, console, startOffset);
            }
        }
    }


}
