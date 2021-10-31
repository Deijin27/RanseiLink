using System.IO;

namespace RanseiLink.Core
{
    public static class FileUtil
    {
        public static string MakeValidFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
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
    }
}
