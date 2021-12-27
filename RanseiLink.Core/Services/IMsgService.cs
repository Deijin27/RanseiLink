
using RanseiLink.Core.Text;
using System.Collections.Generic;

namespace RanseiLink.Core.Services;

public interface IMsgService
{
    int BlockCount { get; }

    string LoadName(byte[] nameData);

    byte[] SaveName(string name);

    /// <summary>
    /// Load single block from file
    /// </summary>
    /// <param name="file">File containing block data</param>
    /// <returns>Loaded block</returns>
    List<Message> LoadBlock(string file);

    /// <summary>
    /// Save single block to new file. Overrides existing file in location.
    /// </summary>
    /// <param name="file">Path of file to write to.</param>
    /// <param name="block">Block to write to file</param>
    void SaveBlock(string file, List<Message> block);

    /// <summary>
    /// Decrypt MSG.DAT blocks and export them into folder as separate files
    /// </summary>
    /// <param name="sourceFile">MSG.DAT file</param>
    /// <param name="destinationFolder">Folder to export blocks into</param>
    void ExtractFromMsgDat(string sourceFile, string destinationFolder);

    /// <summary>
    /// Encrypt MSG.DAT blocks and import them into a new MSG.DAT file.
    /// </summary>
    /// <param name="sourceFolder">Folder containing blocks to import</param>
    /// <param name="destinationFile">MSG.DAT file (will be overwritten)</param>
    void CreateMsgDat(string sourceFolder, string destinationFile);

    /// <summary>
    /// Extract a block from a MSG.DAT file without applying any encryption.
    /// </summary>
    /// <param name="file">MSG.DAT file</param>
    /// <param name="blockId">Id of block to extract</param>
    /// <returns>Block data</returns>
    byte[] ExtractBlockFromMsgDat(string file, int blockId);

    /// <summary>
    /// Apply symmetric encryption to data
    /// </summary>
    /// <param name="data">Block data</param>
    void ApplyEncryption(byte[] data);
    
}
