using System.IO;

namespace RanseiLink.Core.Archive;

public static class PAC
{
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
        uint unknown2 = br.ReadUInt32();
        uint numFiles = br.ReadUInt32();

        // file offsets
        int[] fileOffsets = new int[numFiles + 1];
        for (uint i = 0; i < numFiles; i++)
        {
            fileOffsets[i] = br.ReadInt32();
        }
        fileOffsets[^1] = (int)br.BaseStream.Length;

        // files
        for (int i = 0; i < numFiles; i++)
        {
            int offset = fileOffsets[i];
            int length = fileOffsets[i + 1] - offset;
            
            // Detect file type if possible
            string ext = "";
            if (detectExt && length >= 4)
            {
                br.BaseStream.Position = offset;
                var mag = br.ReadMagicNumber();
                if (FileUtil.IsExistentAndAlphaNumeric(mag))
                {
                    ext += "." + mag;
                }
                
            }
            br.BaseStream.Position = offset;
            string fileDest = Path.Combine(destinationFolder, i.ToString().PadLeft(zeroPadLength, '0') + ext);
            byte[] buffer = br.ReadBytes(length);
            File.WriteAllBytes(fileDest, buffer);
        }
    }
}
