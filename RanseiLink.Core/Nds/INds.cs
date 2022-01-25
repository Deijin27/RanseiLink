using System;

namespace RanseiLink.Core.Nds;

public interface INds : IDisposable
{
    /// <summary>
    /// Extract from the NDS file system a copy of directory and contents to destination on computer
    /// </summary>
    /// <param name="path">Path of directory within the nds file system</param>
    /// <param name="destinationFolder">Folder in computer file system to place the extracted folder</param>
    void ExtractCopyOfDirectory(string path, string destinationFolder);

    /// <summary>
    /// Make a copy of a file within the nds file system to the destination on the computer file system.
    /// </summary>
    /// <param name="path">path to file within Nds file system</param>
    /// <param name="destination">destination file on computer</param>
    void ExtractCopyOfFile(string path, string destinationFolder);
    void InsertFixedLengthFile(string path, string source);
    void InsertVariableLengthFile(string path, string source);
}
