using System;
using System.IO;

namespace RanseiLink.Core.Archive;

/// <summary>
/// Archive format used in Pokemon Conquest used to group related graphic resources. 
/// No consistent file extension. ".G2GR" or ".ALL"
/// </summary>
public static class Link
{
    /// <summary>
    /// Unpack a link archive
    /// </summary>
    /// <param name="filePath">Link file to unpack</param>
    /// <param name="destinationFolder">Files are placed in this folder</param>
    /// <param name="detectExt">If true, tries to read magic number and sets it as the file extension of the exported file</param>
    /// <param name="zeroPadLength">Length to padleft with zeros to. 4 -> 0001, 0234</param>
    public static void Unpack(string filePath, string destinationFolder = null, bool detectExt = true, int zeroPadLength = 4)
    {
        using (var br = new BinaryReader(File.OpenRead(filePath)))
        {
            // Confirm correct magic number
            string magic = br.ReadMagicNumber();
            if (magic != "LINK")
            {
                throw new Exception("Correct magic number not detected at start of file. Thus the file is incorret format.");
            }

            // Get destination folder
            if (string.IsNullOrEmpty(destinationFolder))
            {
                destinationFolder = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "-Unpacked");
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
                int fileLength = br.ReadInt32(); // i have to read as this since br.ReadBytes doesnt accept uint

                // Detect file type if possible
                string ext = "";
                if (detectExt && fileLength >= 4)
                {
                    br.BaseStream.Position = fileOffset;
                    var mag = br.ReadMagicNumber();
                    if (FileUtil.IsExistentAndAlphaNumeric(mag))
                    {
                        ext += "." + mag;
                    }
                }
                
                string fileDest = Path.Combine(destinationFolder, fileIndex.ToString().PadLeft(zeroPadLength, '0') + ext);

                br.BaseStream.Position = fileOffset;
                byte[] fileBytes = br.ReadBytes(fileLength);
                File.WriteAllBytes(fileDest, fileBytes);
            }
        }
    }
}
