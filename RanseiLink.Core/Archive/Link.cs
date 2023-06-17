using System;
using System.IO;

namespace RanseiLink.Core.Archive;

/// <summary>
/// Archive format used in Pokemon Conquest used to group related graphic resources. 
/// No consistent file extension. ".G2DR" or ".ALL"
/// </summary>
public static class LINK
{
    public const string FileExtension = ".link";
    public const string MagicNumber = "LINK";
    private const uint _weirdPaddingMarker = 0x89ABCDEF; // maybe an artifact of creators testing if the code is working


    private static string FileIndexToExtension(long fileIndex)
    {
        switch (fileIndex)
        {
            case 0:
                return ".nanr";
            case 1:
                return ".ncgr";
            case 2:
                return ".ncer";
            case 3:
                return ".ncgr";
            case 4:
                return ".nclr";
            case 5:
                return ".nscr";
            default:
                throw new Exception();
        };
    }

    /// <summary>
    /// Unpack a link archive
    /// </summary>
    /// <param name="filePath">Link file to unpack</param>
    /// <param name="destinationFolder">Files are placed in this folder</param>
    /// <param name="detectExt">If true, tries to read magic number and sets it as the file extension of the exported file</param>
    /// <param name="zeroPadLength">Length to padleft with zeros to. 4 -> 0001, 0234</param>
    /// <exception cref="InvalidDataException"/>
    public static void Unpack(string filePath, string? destinationFolder = null, bool detectExt = true, int zeroPadLength = 4)
    {
        using (var br = new BinaryReader(File.OpenRead(filePath)))
        {
            // Confirm correct magic number
            string magic = br.ReadMagicNumber();
            if (magic != MagicNumber)
            {
                throw new InvalidDataException("Correct magic number not detected at start of file. Thus the file is incorret format.");
            }

            // Get destination folder
            if (string.IsNullOrEmpty(destinationFolder))
            {
                destinationFolder = Path.Combine(Path.GetDirectoryName(filePath)!, Path.GetFileNameWithoutExtension(filePath) + "-Unpacked");
            }
            Directory.CreateDirectory(destinationFolder);

            // Get file header info
            long numberOfFiles = br.ReadUInt32();
            long offsetScaler = br.ReadUInt32();
            uint unknown = br.ReadUInt32(); // this may just be padding the header to 16 bytes since it's always zero
            if (unknown != 0)
            {
                var startColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WARNING!!!!!! Unknown value not equal to zero!!!!!!!");
                Console.ForegroundColor = startColor;
            }

            for (long fileIndex = 0; fileIndex < numberOfFiles; fileIndex++)
            {
                // Get offet and length of file
                br.BaseStream.Position = 16 + fileIndex * 8;
                long fileOffset = br.ReadUInt32() * offsetScaler;
                int fileLength = br.ReadInt32();

                // Detect file type if possible
                string ext = "";
                if (numberOfFiles == 6)
                {
                    ext = FileIndexToExtension(fileIndex);
                }
                //if (detectExt && fileLength >= 4)
                //{
                //    br.BaseStream.Position = fileOffset;
                //    var mag = br.ReadMagicNumber();
                //    if (FileUtil.IsExistentAndAlphaNumeric(mag))
                //    {
                //        ext += "." + mag;
                //    }
                //}

                string fileDest = Path.Combine(destinationFolder, fileIndex.ToString().PadLeft(zeroPadLength, '0') + ext);

                br.BaseStream.Position = fileOffset;
                byte[] fileBytes = br.ReadBytes(fileLength);
                File.WriteAllBytes(fileDest, fileBytes);
            } 
        }
    }

    public static void Pack(string[] files, string destinationFile, int offsetScaler = 0x200)
    {
        using (var bw = new BinaryWriter(File.Create(destinationFile))) 
        {

            bw.WriteMagicNumber(MagicNumber);
            bw.Write(files.Length);
            bw.Write(offsetScaler);
            bw.Write(0);
            var offsetStart = bw.BaseStream.Position;
            bw.BaseStream.Seek(files.Length * 8, SeekOrigin.Current);
            int headerPadding = (int)((bw.BaseStream.Position / offsetScaler + 1) * offsetScaler - bw.BaseStream.Position);
            Console.WriteLine(headerPadding);
            bw.Pad(headerPadding);

            long[] offsets = new long[files.Length];
            int[] lengths = new int[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                byte[] buffer = File.ReadAllBytes(files[i]);
                int len = buffer.Length;
                lengths[i] = len;
                offsets[i] = bw.BaseStream.Position;
                bw.Write(buffer);
                int padding = (len / offsetScaler + 1) * offsetScaler - len;
                // I haven't found a file yet that is a good example of what happens at the boundary.
                if (padding > 3)
                {
                    bw.Write(_weirdPaddingMarker);
                    padding -= 4;
                }
                bw.Pad(padding);
            }

            Console.WriteLine(offsets[0]);

            bw.BaseStream.Position = offsetStart;
            for (int i = 0; i < files.Length; i++)
            {
                bw.Write((uint)(offsets[i] / offsetScaler));
                bw.Write(lengths[i]);
            } 
        }
    }

    public static void Pack(string folderPath, string? destinationFile = null, int offsetScaler = 0x200)
    {
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
            destinationFile = Path.Combine(Path.GetDirectoryName(folderPath)!, Path.GetFileNameWithoutExtension(folderPath) + FileExtension);
        }

        string[] files = Directory.GetFiles(folderPath);
        Array.Sort(files);

        Pack(files, destinationFile, offsetScaler);
    }
}