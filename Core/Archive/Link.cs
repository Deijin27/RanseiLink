using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Core.Archive
{
    /// <summary>
    /// Archive format used in Pokemon Conquest used to group related graphic resources. Extension is ".G2GR"
    /// </summary>
    public static class Link
    {
        public static void Unpack(string filePath, string destinationFolder = null)
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
                    if (fileLength >= 4)
                    {
                        br.BaseStream.Position = fileOffset;
                        var mag = br.ReadMagicNumber();
                        if (IsExistentAndAlphaNumeric(mag))
                        {
                            ext += "." + mag;
                        }
                    }

                    string fileDest = Path.Combine(destinationFolder, fileIndex.ToString().PadLeft(4, '0') + ext);

                    br.BaseStream.Position = fileOffset;
                    byte[] fileBytes = br.ReadBytes(fileLength);
                    Console.WriteLine(fileDest);
                    File.WriteAllBytes(fileDest, fileBytes);
                }
            }
        }


        static bool IsExistentAndAlphaNumeric(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9]+$");
            return rg.IsMatch(strToCheck);
        }
    }
}
