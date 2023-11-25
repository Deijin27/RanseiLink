using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;

namespace RanseiLink.Core;

public static class FileUtil
{
    /// <summary>
    /// Gets the path of the desktop directory. Because it's hard to remember which special folder to use
    /// </summary>
    public static string DesktopDirectory => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

    /// <summary>
    /// Replaces any instances of invalid file name characters with underscore
    /// </summary>
    public static string MakeValidFileName(string fileName)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(c, '_');
        }
        return fileName;
    }

    /// <summary>
    /// Replaces any instances of '/' or '\\' with the directory separator char for the current system.
    /// </summary>
    [return: NotNullIfNotNull(nameof(path))]
    public static string? NormalizePath(string? path)
    {
        return path?.Replace('/', Path.DirectorySeparatorChar)
                    .Replace('\\', Path.DirectorySeparatorChar);
    }



    /// <summary>
    /// Create an empty file if it doesn't already exist. Pass the option <paramref name="overwriteExisting"/> to replace an existing file.
    /// </summary>
    public static void CreateEmptyFile(string path, bool overwriteExisting = false)
    {
        if (overwriteExisting || !File.Exists(path))
        {
            File.Create(path).Dispose();
        }
    }

    /// <summary>
    /// Modifies the path of a file or directory until there are no files or directories with that path existing.
    /// </summary>
    public static string MakeUniquePath(string path)
    {
        string testPath = path;
        string root = Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path));
        string ext = Path.GetExtension(path);
        int count = 1;
        while (File.Exists(testPath) || Directory.Exists(testPath))
        {
            testPath = $"{root} [{count++}]{ext}";
        }
        return testPath;
    }

    /// <summary>
    /// Copies every file and directory from the <paramref name="sourcePath"/> to the <paramref name="targetPath"/>, 
    /// including those in sub folders.
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <param name="targetPath"></param>
    public static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }

    /// <summary>
    /// Generates a temporary directory path and creates it
    /// </summary>
    /// <returns>path of created directory</returns>
    public static string GetTemporaryDirectory()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);
        return tempDirectory;
    }

    private static readonly Regex ExistentAlphanumericRegex = new Regex(@"^[a-zA-Z0-9]+$");
    public static bool IsExistentAndAlphaNumeric(string strToCheck)
    {
        return ExistentAlphanumericRegex.IsMatch(strToCheck);
    }
}