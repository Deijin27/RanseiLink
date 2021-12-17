using RanseiLink.Core.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Services.Concrete;

internal class MsgService : IMsgService
{

    public int BlockCount => _blockCount;

    private readonly int _blockCount;
    private readonly string _encryptionKey;
    private readonly uint _encryptionMult1;
    private readonly int _encryptionMult2;

    public MsgService(
        int blockCount = 0x21, // 0x2C for Pokemon Nobunaga Ambition (japanese version?)
        string encryptionKey = "MsgLinker Ver1.00", 
        uint encryptionMult1 = 0xF0F0F0F1, 
        int encryptionMult2 = 0x11)
    {
        _blockCount = blockCount;
        _encryptionKey = encryptionKey;
        _encryptionMult1 = encryptionMult1;
        _encryptionMult2 = encryptionMult2;
    }

    public void ExtractFromMsgDat(string sourceFile, string destinationFolder)
    {
        Directory.CreateDirectory(destinationFolder);

        for (int i = 0; i < _blockCount; i++)
        {
            byte[] block = ExtractBlockFromMsgDat(sourceFile, i);
            block = ApplyEncryption(block);

            string destFile = Path.Combine(destinationFolder, $"block{i}.bin");
            using var stream = File.Create(destFile);
            stream.Write(block);
        }
    }

    public void CreateMsgDat(string sourceFolder, string destinationFile)
    {
        string[] files = Directory.GetFiles(sourceFolder);
        byte[][] blocks = new byte[_blockCount][];
        for (int i = 0; i < _blockCount; i++)
        {
            string currFile = Array.Find(files, name => Path.GetFileNameWithoutExtension(name) == "block" + i.ToString());
            blocks[i] = File.ReadAllBytes(currFile);
            blocks[i] = ApplyEncryption(blocks[i]);
        }

        using var bw = new BinaryWriter(File.Create(destinationFile));

        // Write offset table
        uint offset = (uint)blocks.Length * 8;
        if (offset % 0x800 != 0)  // Padding
            offset += (0x800 - (offset % 0x800));

        for (int i = 0; i < blocks.Length; i++)
        {
            bw.Write(offset);
            bw.Write(blocks[i].Length);

            offset += (uint)blocks[i].Length;
            if (offset % 0x800 != 0)  // Padding
                offset += (0x800 - (offset % 0x800));
        }

        // Padding
        byte[] rem = Array.Empty<byte>();
        if (bw.BaseStream.Position % 0x800 != 0)
            rem = new byte[0x800 - (bw.BaseStream.Position % 0x800)];
        bw.Write(rem);
        bw.Flush();

        // Write blocks
        for (int i = 0; i < blocks.Length; i++)
        {
            bw.Write(blocks[i]);

            // Padding
            rem = Array.Empty<byte>();
            if (bw.BaseStream.Position % 0x800 != 0)
                rem = new byte[0x800 - (bw.BaseStream.Position % 0x800)];
            bw.Write(rem);
            bw.Flush();
        }
    }

    public string[] LoadBlock(string file)
    {
        byte[] blockData = File.ReadAllBytes(file);
        uint entryCount = BitConverter.ToUInt32(blockData, 0);
        string[] textItems = new string[entryCount];
        for (int i = 0; i < entryCount; i++)
        {
            PNAReader reader = new(blockData, i, false);
            textItems[i] = reader.Text;
        }
        return textItems;
    }

    public void SaveBlock(string file, string[] block)
    {
        using var bw = new BinaryWriter(File.Create(file));
        bw.Write(block.Length);

        int offset = block.Length * 4 + 4;
        List<byte> buffer = new();
        for (int i = 0; i < block.Length; i++)
        {
            bw.Write(offset);
            var pw = new PNAWriter(block[i], false);
            offset += pw.Data.Length;
            buffer.AddRange(pw.Data);
        }

        bw.Write(buffer.ToArray());
    }


    public byte[] ExtractBlockFromMsgDat(string file, int blockId)
    {
        using var br = new BinaryReader(File.OpenRead(file));

        br.BaseStream.Position = blockId * 8;

        uint offset = br.ReadUInt32();
        uint size = br.ReadUInt32();

        br.BaseStream.Position = offset;
        byte[] data = br.ReadBytes((int)size);

        return data;
        
    }

    public byte[] ApplyEncryption(byte[] data)
    {
        int size = data.Length;
        int pos = 0;

        while (pos < size)
        {
            long mult = pos * _encryptionMult1;
            int key_offset = (int)(mult >> 32) >> 4;
            mult = key_offset * _encryptionMult2;
            key_offset = (int)mult >> 32;
            key_offset = pos - key_offset;

            byte value = data[pos];
            byte keyv = (byte)_encryptionKey[key_offset];
            value = (byte)(value ^ keyv);
            data[pos] = value;

            pos++;
        }

        return data;
    }
}
