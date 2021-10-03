using System;
using System.IO;
using System.Text;

namespace Core.Nds
{
    public class Nds : INds
    {
        private readonly BinaryReader UnderlyingStream;
        private readonly long nameTableStartOffset;
        private readonly long fatStartOffset;
        public Nds(string filePath)
        {
            UnderlyingStream = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.ReadWrite));
            nameTableStartOffset = NdsNameTable.GetStartOffset(UnderlyingStream);
            fatStartOffset = NdsFileAllocationTable.GetStartOffset(UnderlyingStream);
        }

        private Fat32.Entry GetEntryFromPath(string path)
        {
            uint entryIndex = NdsNameTable.GetFatEntryIndex(UnderlyingStream, nameTableStartOffset, path);
            return NdsFileAllocationTable.GetEntry(UnderlyingStream, fatStartOffset, entryIndex);
        }

        /// <summary>
        /// Extract from the NDS file system a copy of directory and contents to destination on computer
        /// </summary>
        /// <param name="path">Path of directory within the nds file system</param>
        /// <param name="destinationFolder">Folder in computer file system to place the extracted folder</param>
        public void ExtractCopyOfDirectory(string path, string destinationFolder)
        {
            destinationFolder = Path.Combine(destinationFolder, Path.GetFileName(path));
            var alloc = NdsNameTable.GetFolderAllocationFromPath(UnderlyingStream, nameTableStartOffset, path);
            ExtractCopyOfDirectoryContentsUsingAllocation(alloc, destinationFolder);
        }


        private void ExtractCopyOfDirectoryContentsUsingAllocation(NdsNameTable.FolderAllocation alloc, string destinationFolder)
        {
            Directory.CreateDirectory(destinationFolder);
            uint count = alloc.FatTopFileId;
            foreach (var item in NdsNameTable.GetContents(UnderlyingStream, nameTableStartOffset, alloc))
            {
                string itemName = Encoding.UTF8.GetString(item.Name);
                if (item.IsFolder)
                {
                    ExtractCopyOfDirectoryContentsUsingAllocation(
                        NdsNameTable.GetAllocationData(UnderlyingStream, nameTableStartOffset, item.ContentsIndexIfFolder),
                        Path.Combine(destinationFolder, itemName)
                        );
                }
                else
                {
                    var entry = NdsFileAllocationTable.GetEntry(UnderlyingStream, fatStartOffset, count);
                    ExtractCopyOfFileFromEntry(entry, Path.Combine(destinationFolder, itemName));
                }
                count++;
            }
        }


        /// <summary>
        /// Make a copy of a file within the nds file system to the destination on the computer file system.
        /// </summary>
        /// <param name="path">path to file within Nds file system</param>
        /// <param name="destination">destination file on computer</param>
        public void ExtractCopyOfFile(string path, string destinationFolder)
        {
            var entry = GetEntryFromPath(path);
            ExtractCopyOfFileFromEntry(entry, Path.Combine(destinationFolder, Path.GetFileName(path)));
        }

        private void ExtractCopyOfFileFromEntry(Fat32.Entry entry, string destinationFile)
        {
            UnderlyingStream.BaseStream.Position = entry.Start;
            byte[] bytes = UnderlyingStream.ReadBytes(entry.GetLength());
            File.WriteAllBytes(destinationFile, bytes);
        }

        public void InsertFixedLengthFile(string path, string source)
        {
            var entry = GetEntryFromPath(path);
            UnderlyingStream.BaseStream.Position = entry.Start;
            byte[] sourceData = File.ReadAllBytes(source);
            var entryLength = entry.GetLength();
            if (entryLength != sourceData.Length)
            {
                throw new DataMisalignedException($"Data length {sourceData.Length} does not match {entryLength} (when writing '{path}' to '{source}')");
            }
            UnderlyingStream.BaseStream.Write(sourceData, 0, entryLength);
        }

        #region Implmentation of IDisposable

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    UnderlyingStream.Dispose();
                }

                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
