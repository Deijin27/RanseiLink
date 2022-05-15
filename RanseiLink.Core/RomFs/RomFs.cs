using System;
using System.IO;
using System.Text;

namespace RanseiLink.Core.RomFs
{
    public class RomFs : IRomFs
    {
        private readonly Stream _underlyingStream;
        private readonly BinaryReader _underlyingStreamReader;
        private readonly BinaryWriter _underlyingStreamWriter;
        private readonly long _nameTableStartOffset;
        private readonly long _fatStartOffset;
        private readonly long _bannerStartOffset;

        private readonly NdsHeader _header;

        public RomFs(string filePath)
        {
            _underlyingStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite);
            _underlyingStreamReader = new BinaryReader(_underlyingStream);
            _underlyingStreamWriter = new BinaryWriter(_underlyingStream);
            _header = new NdsHeader(_underlyingStreamReader);
            _nameTableStartOffset = _header.FileNameTableOffset;
            _fatStartOffset = _header.FileAllocationTableOffset;
            _bannerStartOffset = _header.IconBannerOffset;
        }

        public Banner GetBanner()
        {
            _underlyingStreamReader.BaseStream.Position = _bannerStartOffset;
            return new Banner(_underlyingStreamReader);
        }

        public void SetBanner(Banner banner)
        {
            _underlyingStreamWriter.BaseStream.Position = _bannerStartOffset;
            banner.WriteTo(_underlyingStreamWriter, _underlyingStreamReader);
        }

        private Fat32.Entry GetEntryFromPath(string path)
        {
            uint entryIndex = RomFsNameTable.GetFatEntryIndex(_underlyingStreamReader, _nameTableStartOffset, path);
            return RomFsFileAllocationTable.GetEntry(_underlyingStreamReader, _fatStartOffset, entryIndex);
        }


        public void ExtractCopyOfDirectory(string path, string destinationFolder)
        {
            destinationFolder = Path.Combine(destinationFolder, Path.GetFileName(path));
            var alloc = RomFsNameTable.GetFolderAllocationFromPath(_underlyingStreamReader, _nameTableStartOffset, path);
            ExtractCopyOfDirectoryContentsUsingAllocation(alloc, destinationFolder);
        }


        private void ExtractCopyOfDirectoryContentsUsingAllocation(RomFsNameTable.FolderAllocation alloc, string destinationFolder)
        {
            Directory.CreateDirectory(destinationFolder);
            uint count = alloc.FatTopFileId;
            foreach (var item in RomFsNameTable.GetContents(_underlyingStreamReader, _nameTableStartOffset, alloc))
            {
                string itemName = Encoding.UTF8.GetString(item.Name);
                if (item.IsFolder)
                {
                    ExtractCopyOfDirectoryContentsUsingAllocation(
                        RomFsNameTable.GetAllocationData(_underlyingStreamReader, _nameTableStartOffset, item.ContentsIndexIfFolder),
                        Path.Combine(destinationFolder, itemName)
                        );
                }
                else
                {
                    var entry = RomFsFileAllocationTable.GetEntry(_underlyingStreamReader, _fatStartOffset, count);
                    ExtractCopyOfFileFromEntry(entry, Path.Combine(destinationFolder, itemName));
                }
                count++;
            }
        }



        public void ExtractCopyOfFile(string path, string destinationFolder)
        {
            var entry = GetEntryFromPath(path);
            ExtractCopyOfFileFromEntry(entry, Path.Combine(destinationFolder, Path.GetFileName(path)));
        }

        private void ExtractCopyOfFileFromEntry(Fat32.Entry entry, string destinationFile)
        {
            _underlyingStreamReader.BaseStream.Position = entry.Start;
            byte[] bytes = _underlyingStreamReader.ReadBytes(entry.GetLength());
            File.WriteAllBytes(destinationFile, bytes);
        }

        public void InsertFixedLengthFile(string path, string source)
        {
            var entry = GetEntryFromPath(path);
            _underlyingStreamReader.BaseStream.Position = entry.Start;
            byte[] sourceData = File.ReadAllBytes(source);
            var entryLength = entry.GetLength();
            if (entryLength != sourceData.Length)
            {
                throw new DataMisalignedException($"Data length {sourceData.Length} does not match {entryLength} (when writing '{path}' to '{source}')");
            }
            _underlyingStreamReader.BaseStream.Write(sourceData, 0, entryLength);
        }

        public void InsertVariableLengthFile(string path, string source)
        {
            uint entryIndex = RomFsNameTable.GetFatEntryIndex(_underlyingStreamReader, _nameTableStartOffset, path);
            var originalInsertDestinationEntry = RomFsFileAllocationTable.GetEntry(_underlyingStreamReader, _fatStartOffset, entryIndex);
            byte[] sourceData = File.ReadAllBytes(source);
            var oldEntryLength = originalInsertDestinationEntry.GetLength();
            if (oldEntryLength == sourceData.Length)
            {
                // Is still the same length so just process as fixed length file.
                _underlyingStreamReader.BaseStream.Position = originalInsertDestinationEntry.Start;
                _underlyingStreamReader.BaseStream.Write(sourceData, 0, oldEntryLength);
                return;
            }

            // Create new entry
            var newInsertDestinationEntry = new Fat32.Entry(originalInsertDestinationEntry.Start, originalInsertDestinationEntry.Start + (uint)sourceData.Length);
            int change = newInsertDestinationEntry.GetLength() - oldEntryLength;

            // Update entry value
            RomFsFileAllocationTable.SetEntry(_underlyingStreamWriter, _fatStartOffset, entryIndex, newInsertDestinationEntry);

            // Update entry values after changed one with change
            var currentEntryId = entryIndex + 1;
            var previousEntry = newInsertDestinationEntry;
            var currentEntry = RomFsFileAllocationTable.GetEntry(_underlyingStreamReader, _fatStartOffset, currentEntryId);
            while (!currentEntry.IsDefault)
            {
                uint newStart = (uint)(currentEntry.Start + change);
                uint newEnd = (uint)(currentEntry.End + change);
                RomFsFileAllocationTable.SetEntry(_underlyingStreamWriter, _fatStartOffset, currentEntryId++, new Fat32.Entry(newStart, newEnd));
                previousEntry = currentEntry;
                currentEntry = RomFsFileAllocationTable.GetEntry(_underlyingStreamReader, _fatStartOffset, currentEntryId);
            }

            // Shift data

            // last edited entry is the final non-default entry which will store the end of the data
            byte[] buffer = new byte[previousEntry.End - originalInsertDestinationEntry.End];

            // read the data from the original position
            _underlyingStream.Position = originalInsertDestinationEntry.End;
            _underlyingStream.Read(buffer, 0, buffer.Length);

            // write the data into the new position
            _underlyingStream.Position = newInsertDestinationEntry.End;
            _underlyingStream.Write(buffer, 0, buffer.Length);

            // Write file data in newly created space
            _underlyingStream.Position = newInsertDestinationEntry.Start;
            _underlyingStream.Write(sourceData, 0, sourceData.Length);
        }

        #region Implmentation of IDisposable

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _underlyingStream.Dispose();
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