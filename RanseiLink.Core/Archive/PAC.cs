using System;
using System.IO;

namespace RanseiLink.Core.Archive;

public static class PAC
{
    public const string FileExtension = ".pac";
    const uint MagicNumber = 0x0040E3C4;
    const uint Version = 1;

    private static string FileTypeNumberToExtension(uint fileType)
    {
        return fileType switch
        {
            0 => ".bmd0",
            1 => ".btx0",
            2 => ".btp0",
            4 => ".bma0",
            5 => ".unknown5",
            6 => ".char",
            7 => ".bta0",
            _ => ""
        };
    }

    /// <summary>
    /// Unpack a PAC archive
    /// </summary>
    /// <param name="filePath">PAC file to unpack</param>
    /// <param name="destinationFolder">Files are placed in this folder</param>
    /// <param name="detectExt">If true, tries to read magic number and sets it as the file extension of the exported file</param>
    /// <param name="zeroPadLength">Length to padleft with zeros to. 4 -> 0001, 0234</param>
    public static void Unpack(string filePath, string destinationFolder = null, bool detectExt = true, int zeroPadLength = 4)
    {
        using var br = new BinaryReader(File.OpenRead(filePath));

        // Get destination folder
        if (string.IsNullOrEmpty(destinationFolder))
        {
            destinationFolder = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "-Unpacked");
        }
        Directory.CreateDirectory(destinationFolder);

        // header
        uint unknown1 = br.ReadUInt32();
        if (unknown1 != MagicNumber)
        {
            throw new Exception($"Unexpected informatin in PAC file {filePath}");
        }
        uint unknown2 = br.ReadUInt32();
        if (unknown2 != Version)
        {
            throw new Exception($"Unexpected informatin in PAC file {filePath}");
        }
        uint numFiles = br.ReadUInt32();

        // file offsets
        int[] fileOffsets = new int[numFiles + 1];
        for (uint i = 0; i < numFiles; i++)
        {
            fileOffsets[i] = br.ReadInt32();
        }
        fileOffsets[^1] = (int)br.BaseStream.Length;

        br.BaseStream.Seek(0x2C, SeekOrigin.Begin);
        uint[] fileTypes = new uint[numFiles];
        for (int i = 0; i < numFiles; i++)
        {
            fileTypes[i] = br.ReadUInt32();
        }

        // files
        for (int i = 0; i < numFiles; i++)
        {
            int offset = fileOffsets[i];
            int length = fileOffsets[i + 1] - offset;
            
            br.BaseStream.Position = offset;
            string fileDest = Path.Combine(destinationFolder, 
                i.ToString().PadLeft(zeroPadLength, '0') 
                + FileTypeNumberToExtension(fileTypes[i])
                );
            byte[] buffer = br.ReadBytes(length);
            File.WriteAllBytes(fileDest, buffer);
        }
    }

    public static void Pack(string folderPath, string destinationFile = null, int offsetScaler = 0x200)
    {
        throw new NotImplementedException();
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException(folderPath);
        }

        if (Directory.GetDirectories(folderPath).Length != 0)
        {
            throw new Exception($"Cannot create a link archive from a directory that contains sub directories ({folderPath})");
        }

        // Get destination folder
        if (string.IsNullOrEmpty(destinationFile))
        {
            destinationFile = Path.Combine(Path.GetDirectoryName(folderPath), Path.GetFileNameWithoutExtension(folderPath) + FileExtension);
        }

        using var bw = new BinaryWriter(File.Create(destinationFile));
        string[] files = Directory.GetFiles(folderPath);

        bw.Write(MagicNumber);
        bw.Write(Version);
        bw.Write(files.Length);
        bw.Write(offsetScaler);
        var offsetStart = bw.BaseStream.Position;
        bw.BaseStream.Seek(files.Length * 8, SeekOrigin.Current);

        int[] lengths = new int[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            byte[] buffer = File.ReadAllBytes(files[i]);
            int len = buffer.Length;
            lengths[i] = len;
            bw.Write(buffer);
            int padding = (len / offsetScaler + 1) * offsetScaler;
            // I haven't found a file yet that is a good example of what happens at the boundary.
            if (padding > 3)
            {
                padding -= 4;
            }
            bw.Pad(padding);

        }
    }
}
