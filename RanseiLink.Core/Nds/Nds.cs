using System;
using System.IO;
using System.Text;

namespace RanseiLink.Core.Nds;

public class Nds : INds
{
    private readonly Stream _underlyingStream;
    private readonly BinaryReader _underlyingStreamReader;
    private readonly BinaryWriter _underlyingStreamWriter;
    private readonly long _nameTableStartOffset;
    private readonly long _fatStartOffset;
    public Nds(string filePath)
    {
        _underlyingStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite);
        _underlyingStreamReader = new BinaryReader(_underlyingStream);
        _underlyingStreamWriter = new BinaryWriter(_underlyingStream);
        _nameTableStartOffset = NdsNameTable.GetStartOffset(_underlyingStreamReader);
        _fatStartOffset = NdsFileAllocationTable.GetStartOffset(_underlyingStreamReader);
    }

    private Fat32.Entry GetEntryFromPath(string path)
    {
        uint entryIndex = NdsNameTable.GetFatEntryIndex(_underlyingStreamReader, _nameTableStartOffset, path);
        return NdsFileAllocationTable.GetEntry(_underlyingStreamReader, _fatStartOffset, entryIndex);
    }

    /// <summary>
    /// Extract from the NDS file system a copy of directory and contents to destination on computer
    /// </summary>
    /// <param name="path">Path of directory within the nds file system</param>
    /// <param name="destinationFolder">Folder in computer file system to place the extracted folder</param>
    public void ExtractCopyOfDirectory(string path, string destinationFolder)
    {
        destinationFolder = Path.Combine(destinationFolder, Path.GetFileName(path));
        var alloc = NdsNameTable.GetFolderAllocationFromPath(_underlyingStreamReader, _nameTableStartOffset, path);
        ExtractCopyOfDirectoryContentsUsingAllocation(alloc, destinationFolder);
    }


    private void ExtractCopyOfDirectoryContentsUsingAllocation(NdsNameTable.FolderAllocation alloc, string destinationFolder)
    {
        Directory.CreateDirectory(destinationFolder);
        uint count = alloc.FatTopFileId;
        foreach (var item in NdsNameTable.GetContents(_underlyingStreamReader, _nameTableStartOffset, alloc))
        {
            string itemName = Encoding.UTF8.GetString(item.Name);
            if (item.IsFolder)
            {
                ExtractCopyOfDirectoryContentsUsingAllocation(
                    NdsNameTable.GetAllocationData(_underlyingStreamReader, _nameTableStartOffset, item.ContentsIndexIfFolder),
                    Path.Combine(destinationFolder, itemName)
                    );
            }
            else
            {
                var entry = NdsFileAllocationTable.GetEntry(_underlyingStreamReader, _fatStartOffset, count);
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
        uint entryIndex = NdsNameTable.GetFatEntryIndex(_underlyingStreamReader, _nameTableStartOffset, path);
        var originalInsertDestinationEntry = NdsFileAllocationTable.GetEntry(_underlyingStreamReader, _fatStartOffset, entryIndex);
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
        NdsFileAllocationTable.SetEntry(_underlyingStreamWriter, _fatStartOffset, entryIndex, newInsertDestinationEntry);

        // Update entry values after changed one with change
        var currentEntryId = entryIndex + 1;
        var previousEntry = newInsertDestinationEntry;
        var currentEntry = NdsFileAllocationTable.GetEntry(_underlyingStreamReader, _fatStartOffset, currentEntryId);
        while (!currentEntry.IsDefault)
        {
            uint newStart = (uint)(currentEntry.Start + change);
            uint newEnd = (uint)(currentEntry.End + change);
            NdsFileAllocationTable.SetEntry(_underlyingStreamWriter, _fatStartOffset, currentEntryId++, new Fat32.Entry(newStart, newEnd));
            previousEntry = currentEntry;
            currentEntry = NdsFileAllocationTable.GetEntry(_underlyingStreamReader, _fatStartOffset, currentEntryId);
        }

        // Shift data

        // last edited entry is the final non-default entry which will store the end of the data
        byte[] buffer = new byte[previousEntry.End - originalInsertDestinationEntry.End];

        // read the data from the original position
        _underlyingStream.Position = originalInsertDestinationEntry.End;
        _underlyingStream.Read(buffer, 0, buffer.Length);

        // write the data into the new position
        _underlyingStream.Position = newInsertDestinationEntry.End;
        _underlyingStream.Write(buffer);

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
