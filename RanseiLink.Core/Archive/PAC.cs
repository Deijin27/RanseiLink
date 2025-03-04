﻿namespace RanseiLink.Core.Archive;

public static class PAC
{
    public const string FileExtension = ".pac";

    public enum FileTypeNumber
    {
        NSBMD,
        NSBTX,
        NSBTP,
        NSBCA,
        NSBMA,
        PAT,
        CHAR,
        NSBTA,

        DEFAULT = -1
    }

    public static string FileTypeNumberToExtension(FileTypeNumber fileType)
    {
        return fileType switch
        {
            FileTypeNumber.NSBMD => ".nsbmd",// nitro sdk binary model data
            FileTypeNumber.NSBTX => ".nsbtx",// nitro sdk binary texture
            FileTypeNumber.NSBTP => ".nsbtp",// nitro sdk binary texture pattern
            FileTypeNumber.NSBCA => ".nsbca", // nitro sdk binary character animation
            FileTypeNumber.NSBMA => ".nsbma",// nitro sdk binary material animation
            FileTypeNumber.PAT => ".pat",// in pokemon model pacs this is a stripped down version of nsbtp
            FileTypeNumber.CHAR => ".char",
            FileTypeNumber.NSBTA => ".nsbta",// nitro sdk binary texture animation
            _ => "",
        };
        ;
    }

    public static FileTypeNumber ExtensionToFileTypeNumber(string extension)
    {
        switch (extension)
        {
            case ".bmd":
            case ".bmd0":
            case ".nsbmd":
                return FileTypeNumber.NSBMD;

            case ".btx":
            case ".btx0":
            case ".nsbtx":
                return FileTypeNumber.NSBTX;

            case ".btp":
            case ".btp0":
            case ".nsbtp":
                return FileTypeNumber.NSBTP;

            case ".unknown3": // retained for compatibity since we didn't used to know what this file was
            case ".bca":
            case ".bca0":
            case ".nsbca":
                return FileTypeNumber.NSBCA;

            case ".bma":
            case ".bma0":
            case ".nsbma":
                return FileTypeNumber.NSBMA;

            case ".unknown5": // retained for compatibity since we didn't used to know what this file was
            case ".pat":
                return FileTypeNumber.PAT;

            case ".chr":
            case ".char":
                return FileTypeNumber.CHAR;

            case ".bta":
            case ".bta0":
            case ".nsbta":
                return FileTypeNumber.NSBTA;

            default:
                return FileTypeNumber.DEFAULT;
        };
    }

    /// <summary>
    /// Unpack a PAC archive
    /// </summary>
    /// <param name="filePath">PAC file to unpack</param>
    /// <param name="destinationFolder">Files are placed in this folder</param>
    /// <param name="detectExt">If true, tries to read magic number and sets it as the file extension of the exported file</param>
    /// <param name="zeroPadLength">Length to padleft with zeros to. 4 -> 0001, 0234</param>
    public static void Unpack(string filePath, string? destinationFolder = null, bool detectExt = true, int zeroPadLength = 4)
    {
        using (var br = new BinaryReader(File.OpenRead(filePath)))
        {

            // Get destination folder
            if (string.IsNullOrEmpty(destinationFolder))
            {
                destinationFolder = Path.Combine(Path.GetDirectoryName(filePath)!, Path.GetFileNameWithoutExtension(filePath) + "-Unpacked");
            }
            Directory.CreateDirectory(destinationFolder);

            // header
            uint magicNumber = br.ReadUInt32();
            // seen a couple of different magic numbers, with no difference in the file. So be forgiving here
            // currenly seen: 0x0040E3C4, 0x0040E3BC

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
            int[] fileTypes = new int[numFiles];
            for (int i = 0; i < numFiles; i++)
            {
                fileTypes[i] = br.ReadInt32();
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
                    fileDest += FileTypeNumberToExtension((FileTypeNumber)fileTypes[i]);
                }
                byte[] buffer = br.ReadBytes(length);
                File.WriteAllBytes(fileDest, buffer);
            }
        }
    }

    private static FileTypeNumber[] AutoDetectFileTypeNumbers(string[] files)
    {
        FileTypeNumber[] result = new FileTypeNumber[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            result[i] = ExtensionToFileTypeNumber(Path.GetExtension(files[i]));
        }
        return result;
    }

    public static void Pack(string[] files, string destinationFile, FileTypeNumber[]? fileTypeNumbers = null, uint sharedFileCount = 1, uint magicNumber = 0x0040E3C4)
    {
        if (fileTypeNumbers == null)
        {
            fileTypeNumbers = AutoDetectFileTypeNumbers(files);
        }

        // get them in the correct order
        Array.Sort(fileTypeNumbers, files);

        using (var bw = new BinaryWriter(File.Create(destinationFile)))
        {

            bw.Pad(0x2C);
            bw.Pad(0x20, 0xFF);

            int[] fileOffsets = new int[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                fileOffsets[i] = (int)bw.BaseStream.Position;
                using (var stream = File.OpenRead(files[i]))
                {
                    stream.CopyTo(bw.BaseStream);
                }
            }

            bw.BaseStream.Position = 0;
            bw.Write(magicNumber);
            bw.Write(sharedFileCount);
            bw.Write(files.Length);

            foreach (var fileOffset in fileOffsets)
            {
                bw.Write(fileOffset);
            }

            bw.BaseStream.Position = 0x2C;
            foreach (var fileType in fileTypeNumbers)
            {
                bw.Write((int)fileType);
            }
        }
    }

    public static void Pack(string folderPath, string? destinationFile = null, FileTypeNumber[]? fileTypeNumbers = null, uint sharedFileCount = 1)
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
            destinationFile = Path.Combine(Path.GetDirectoryName(folderPath)!, Path.GetFileNameWithoutExtension(folderPath) + FileExtension);
        }

        string[] files = Directory.GetFiles(folderPath);
        Array.Sort(files);

        Pack(files, destinationFile, fileTypeNumbers, sharedFileCount);
    }
}