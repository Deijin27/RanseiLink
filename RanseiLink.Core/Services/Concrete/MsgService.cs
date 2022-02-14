using RanseiLink.Core.Resources;
using RanseiLink.Core.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RanseiLink.Core.Services.Concrete;
public class MsgService : IMsgService
{
    public int BlockCount => _blockCount;

    private readonly int _blockCount;

    private readonly byte[] _encryptionKey;

    public MsgService(
        int blockCount = 0x21, // 0x2C for Pokemon Nobunaga Ambition (japanese version?)
        string encryptionKey = "MsgLinker Ver1.00")
    {
        _blockCount = blockCount;
        _encryptionKey = Encoding.ASCII.GetBytes(encryptionKey);
    }

    public void ExtractFromMsgDat(string sourceFile, string destinationFolder)
    {
        Directory.CreateDirectory(destinationFolder);

        for (int i = 0; i < _blockCount; i++)
        {
            byte[] block = ExtractBlockFromMsgDat(sourceFile, i);
            ApplyEncryption(block);

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
            ApplyEncryption(blocks[i]);
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

    public List<Message> LoadBlock(string file)
    {
        using var br = new BinaryReader(File.OpenRead(file));
        var pnaReader = new PnaTextReader(br);
        uint entryCount = br.ReadUInt32();
        List<Message> textItems = new();
        for (int i = 0; i < entryCount; i++)
        {
            br.BaseStream.Position = (i + 1) * 4;
            uint offset = br.ReadUInt32();
            uint endOffset;
            if (i + 1 == entryCount)
            {
                endOffset = (uint)br.BaseStream.Length;
            }
            else
            {
                endOffset = br.ReadUInt32();
            }
            br.BaseStream.Position = offset;
            int elementId = 0;
            while (br.BaseStream.Position < endOffset)
            {
                Message msg = pnaReader.ReadMessage();
                foreach (var (key, value) in CharacterTableResource.LoadTable)
                {
                    msg.Text = msg.Text.Replace(key, value);
                }
                msg.GroupId = i;
                msg.ElementId = elementId++;
                textItems.Add(msg);
            }
        }

        return textItems;
    }

    public void SaveBlock(string file, List<Message> block)
    {
        using var bw = new BinaryWriter(File.Create(file));
        var pnaWriter = new PnaTextWriter(bw);
        // Group the messages by groupId so we can generate the table
        var msgByGroup = block.GroupBy(m => m.GroupId).ToArray();

        // Write number and empty offset table
        bw.Write(msgByGroup.Length);
        for (int i = 0; i < 4 * msgByGroup.Length; i++)
        {
            bw.Write((byte)0);
        }

        // Write every message with its offset
        bool multiElements = false;
        for (int i = 0; i < msgByGroup.Length; i++)
        {
            var currentPos = bw.BaseStream.Position;
            bw.BaseStream.Position = 4 + (i * 4);
            bw.Write((uint)bw.BaseStream.Length);
            bw.BaseStream.Position = currentPos;

            // Write all the elements of the message
            foreach (var msg in msgByGroup[i])
            {
                if (msg.Context.Contains("text-if"))
                {
                    multiElements = true;
                }

                foreach (var (key, value) in CharacterTableResource.SaveTable)
                {
                    msg.Text = msg.Text.Replace(key, value);
                }
                pnaWriter.WriteMessage(msg, multiElements);
            }
        }
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

    public void ApplyEncryption(byte[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= _encryptionKey[i % _encryptionKey.Length];
        }
    }

    

    public string LoadName(byte[] nameData)
    {
        return NameLoader.LoadName(nameData);
    }

    public byte[] SaveName(string name)
    {
        return NameLoader.SaveName(name);
    }
}
