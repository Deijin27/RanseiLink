using RanseiLink.Core.Resources;
using RanseiLink.Core.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RanseiLink.Core.Services.Concrete
{
    public class MsgService : IMsgService
    {
        private readonly byte[] _encryptionKey;

        public MsgService(
            //int blockCount = 0x21, // 0x2C for Pokemon Nobunaga Ambition (japanese version?)
            string encryptionKey = "MsgLinker Ver1.00")
        {
            _encryptionKey = Encoding.ASCII.GetBytes(encryptionKey);
        }

        public void ExtractFromMsgDat(string sourceFile, string destinationFolder)
        {
            Directory.CreateDirectory(destinationFolder);

            using (var br = new BinaryReader(File.OpenRead(sourceFile)))
            {
                int i = 0;
                while (true)
                {
                    uint offset = br.ReadUInt32();

                    if (offset == 0)
                    {
                        break;
                    }
                    int size = br.ReadInt32();

                    var pos = br.BaseStream.Position;
                    br.BaseStream.Position = offset;
                    byte[] block = br.ReadBytes(size);
                    br.BaseStream.Position = pos;

                    ApplyEncryption(block);

                    string destFile = Path.Combine(destinationFolder, $"block{i++}.bin");
                    using (var stream = File.Create(destFile))
                    {
                        stream.Write(block, 0, block.Length);
                    }
                }
            }
        }

        public void CreateMsgDat(string sourceFolder, string destinationFile)
        {
            string[] files = Directory.GetFiles(sourceFolder).OrderBy(x => int.Parse(Path.GetFileNameWithoutExtension(x).Substring(5))).ToArray();

            List<byte[]> blocks = new List<byte[]>();
            foreach (var file in files)
            {
                var block = File.ReadAllBytes(file);
                ApplyEncryption(block);
                blocks.Add(block);
            }

            using (var bw = new BinaryWriter(File.Create(destinationFile)))
            {
                // Write offset table
                uint offset = (uint)blocks.Count * 8;
                if (offset % 0x800 != 0)  // Padding
                    offset += (0x800 - (offset % 0x800));

                for (int i = 0; i < blocks.Count; i++)
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

                // Write blocks
                for (int i = 0; i < blocks.Count; i++)
                {
                    bw.Write(blocks[i]);

                    // Padding
                    rem = Array.Empty<byte>();
                    if (bw.BaseStream.Position % 0x800 != 0)
                        rem = new byte[0x800 - (bw.BaseStream.Position % 0x800)];
                    bw.Write(rem);
                }
            }
        }

        public List<Message> LoadBlock(string file)
        {
            using (var br = new BinaryReader(File.OpenRead(file)))
            {
                var pnaReader = new PnaTextReader(br);
                uint entryCount = br.ReadUInt32();
                List<Message> textItems = new List<Message>();
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
                        foreach (var kvp in CharacterTableResource.LoadTable)
                        {
                            msg.Text = msg.Text.Replace(kvp.Key, kvp.Value);
                        }
                        msg.GroupId = i;
                        msg.ElementId = elementId++;
                        textItems.Add(msg);
                    }
                }

                return textItems; 
            }
        }

        public void SaveBlock(string file, List<Message> block)
        {
            using (var bw = new BinaryWriter(File.Create(file)))
            {
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

                        foreach (var kvp in CharacterTableResource.SaveTable)
                        {
                            msg.Text = msg.Text.Replace(kvp.Key, kvp.Value);
                        }
                        pnaWriter.WriteMessage(msg, multiElements);
                    }
                }
            } 
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
}