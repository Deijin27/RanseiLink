using System;

namespace RanseiLink.Core.RomFs;

public interface IRomFs : IDisposable
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
    void ExtractCopyOfFile(string path, string destination);

    /// <summary>
    /// Write the data of a file into the nds file system. 
    /// Exception if the length of the provided file does not match the length of the current file in the file system.
    /// </summary>
    /// <param name="path">path to file within Nds file system</param>
    /// <param name="source">source file on computer</param>
    void InsertFixedLengthFile(string path, string source);

    /// <summary>
    /// Write the data of a file into the nds file system.
    /// </summary>
    /// <param name="path">path to file within Nds file system</param>
    /// <param name="source">source file on computer</param>
    void InsertVariableLengthFile(string path, string source);
}
