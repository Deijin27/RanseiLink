using System;
using System.IO;
using System.Text.RegularExpressions;

namespace RanseiLink.Core
{
    public static class FileUtil
    {
        public static string DesktopDirectory { get; } = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        public static string MakeValidFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        public static string NormalizePath(string path)
        {
            return path?.Replace('/', Path.DirectorySeparatorChar)
                        .Replace('\\', Path.DirectorySeparatorChar);
        }

        public static string MakeUniquePath(string path)
        {
            string testPath = path;
            string root = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
            string ext = Path.GetExtension(path);
            int count = 1;
            while (File.Exists(testPath) || Directory.Exists(testPath))
            {
                testPath = $"{root} [{count++}]{ext}";
            }
            return testPath;
        }

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
}