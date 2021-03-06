using System;
using System.IO;

namespace RanseiLink.Core.Archive
{
    public static class PAC
    {
        public const string FileExtension = ".pac";
        const uint MagicNumber = 0x0040E3C4;

        private static string FileTypeNumberToExtension(uint fileType)
        {
            switch (fileType)
            {
                case 0: return ".bmd0";
                case 1: return ".btx0";
                case 2: return ".btp0";
                case 4: return ".bma0";
                case 5: return ".unknown5";
                case 6: return ".char";
                case 7: return ".bta0";
                default: return "";
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
            using (var br = new BinaryReader(File.OpenRead(filePath)))
            {

                // Get destination folder
                if (string.IsNullOrEmpty(destinationFolder))
                {
                    destinationFolder = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "-Unpacked");
                }
                Directory.CreateDirectory(destinationFolder);

                // header
                uint magicNumber = br.ReadUInt32();
                if (magicNumber != MagicNumber)
                {
                    throw new Exception($"Unexpected magic number in PAC file {filePath}");
                }

                // If not 0, this is a variant of another, with one of the files shared with that base
                // i.e. a file is omitted from this because it would be the same as the base
                uint sharedFileCount = br.ReadUInt32();

                uint numFiles = br.ReadUInt32();

                // file offsets
                int[] fileOffsets = new int[numFiles + 1];
                for (uint i = 0; i < numFiles; i++)
                {
                    fileOffsets[i] = br.ReadInt32();
                }
                fileOffsets[fileOffsets.Length - 1] = (int)br.BaseStream.Length;

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
                        i.ToString().PadLeft(zeroPadLength, '0'));
                    if (detectExt)
                    {
                        fileDest += FileTypeNumberToExtension(fileTypes[i]);
                    }
                    byte[] buffer = br.ReadBytes(length);
                    File.WriteAllBytes(fileDest, buffer);
                }
            }
        }

        public static void Pack(string[] files, int[] fileTypeNumbers, string destinationFile, uint sharedFileCount = 1)
        {
            using (var bw = new BinaryWriter(File.Create(destinationFile)))
            {

                bw.Pad(0x2C);
                bw.Pad(0x20, 0xFF);

                int[] fileOffsets = new int[files.Length];

                for (int i = 0; i < files.Length; i++)
                {
                    fileOffsets[i] = (int)bw.BaseStream.Position;
                    byte[] buffer = File.ReadAllBytes(files[i]);
                    bw.Write(buffer);
                }

                bw.BaseStream.Position = 0;
                bw.Write(MagicNumber);
                bw.Write(sharedFileCount);
                bw.Write(files.Length);

                foreach (var fileOffset in fileOffsets)
                {
                    bw.Write(fileOffset);
                }

                bw.BaseStream.Position = 0x2C;
                foreach (var fileType in fileTypeNumbers)
                {
                    bw.Write(fileType);
                }
            }
        }

        public static void Pack(string folderPath, int[] fileTypeNumbers, string destinationFile = null, uint sharedFileCount = 1)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException(folderPath);
            }

            if (Directory.GetDirectories(folderPath).Length != 0)
            {
                throw new Exception($"Cannot create a pac archive from a directory that contains sub directories ({folderPath})");
            }

            // Get destination folder
            if (string.IsNullOrEmpty(destinationFile))
            {
                destinationFile = Path.Combine(Path.GetDirectoryName(folderPath), Path.GetFileNameWithoutExtension(folderPath) + FileExtension);
            }

            string[] files = Directory.GetFiles(folderPath);
            Array.Sort(files);

            Pack(files, fileTypeNumbers, destinationFile, sharedFileCount);
        }
    }
}