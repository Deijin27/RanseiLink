﻿using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Graphics;

/// <summary>
/// Nitro Animation Resource
/// </summary>
public class NANR
{
    public const string MagicNumber = "RNAN";
    public static readonly string[] FileExtensions = new[] { ".nanr", ".rnan" };

    public ushort Version { get; set; } = 0x100;
    public ABNK AnimationBanks { get; set; }
    public LABL Labels { get; set; }
    public UEXT Unknown { get; set; }

    public static NANR Load(string file)
    {
        using (var br = new BinaryReader(File.OpenRead(file)))
        {
            return new NANR(br);
        }
    }

    public void Save(string file)
    {
        using (var bw = new BinaryWriter(File.Create(file)))
        {
            WriteTo(bw);
        }
    }

    public NANR(BinaryReader br)
    {
        long initOffset = br.BaseStream.Position;

        // first a typical file header
        var header = new NitroFileHeader(br);
        Version = header.Version;

        if (header.MagicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number in file header '{header.MagicNumber}' at offset 0x{initOffset:X} (expected: {MagicNumber})");
        }

        if (header.ChunkCount != 3)
        {
            throw new Exception("Unexpected chunk count of NCER");
        }

        // read 
        AnimationBanks = new ABNK(br);
        Labels = new LABL(br);
        Unknown = new UEXT(br);

        br.BaseStream.Position = initOffset + header.FileLength;
    }

    public void WriteTo(BinaryWriter bw)
    {
        var initOffset = bw.BaseStream.Position;
        var header = new NitroFileHeader
        {
            MagicNumber = MagicNumber,
            ByteOrderMarker = 0xFEFF,
            Version = Version,
            ChunkCount = 3,
            HeaderLength = 0x10
        };

        bw.Pad(header.HeaderLength);

        AnimationBanks.WriteTo(bw);
        Labels.WriteTo(bw);
        Unknown.WriteTo(bw);

        var endOffset = bw.BaseStream.Position;
        header.FileLength = (uint)(endOffset - initOffset);
        bw.BaseStream.Position = initOffset;
        header.WriteTo(bw);
    }

    
}

/// <summary>
/// Animation Bank
/// </summary>
public class ABNK
{
    public const string MagicNumber = "KNBA";

    public List<Anim> Banks { get; set; }
    public ushort TFrames { get; set; }

    private struct SubHeader
    {
        public const int Length = 2 + 2 + 4 + 4 + 4 + 8;

        public ushort BankCount;
        public ushort TFrames;
        public uint BlockOffset_Anim;
        public uint BlockOffset_Frames;
        public uint BlockOffset_Cells;

        public SubHeader(BinaryReader br)
        {
            BankCount = br.ReadUInt16();
            TFrames = br.ReadUInt16();
            BlockOffset_Anim = br.ReadUInt32();
            BlockOffset_Frames = br.ReadUInt32();
            BlockOffset_Cells = br.ReadUInt32();
            var reserved = br.ReadUInt64();
            if (reserved != 0)
            {
                throw new Exception("It isn't always zero !!! 0_0");
            }
        }

        public void WriteTo(BinaryWriter bw)
        {
            bw.Write(BankCount);
            bw.Write(TFrames);
            bw.Write(BlockOffset_Anim);
            bw.Write(BlockOffset_Frames);
            bw.Write(BlockOffset_Cells);
            bw.Write(0UL);
        }
    }

    public ABNK(BinaryReader br)
    {
        var initOffset = br.BaseStream.Position;
        var header = new NitroChunkHeader(br);

        if (header.MagicNumber != MagicNumber)
        {
            throw new InvalidDataException($"Unexpected magic number in file header '{header.MagicNumber}' at offset 0x{initOffset:X} (expected: {MagicNumber})");
        }

        var subHeader = new SubHeader(br);
        TFrames = subHeader.TFrames;

        Banks = new List<Anim>();

        for (int i = 0; i < subHeader.BankCount; i++)
        {
            br.BaseStream.Position = initOffset + NitroChunkHeader.Length + subHeader.BlockOffset_Anim + i * Anim.DataLength; // maybe not needed
            var anim = new Anim();
            Banks.Add(anim);
            var numFrames = br.ReadUInt32();
            anim.DataType = br.ReadUInt16();
            anim.Unknown1 = br.ReadUInt16();
            anim.Unknown2 = br.ReadUInt16();
            anim.Unknown3 = br.ReadUInt16();
            var offsetFrame = br.ReadUInt32();

            for (int j = 0; j < numFrames; j++)
            {
                br.BaseStream.Position = initOffset + NitroChunkHeader.Length + subHeader.BlockOffset_Frames + offsetFrame + j * Frame.DataLength_Frame;
                var frame = new Frame();
                anim.Frames.Add(frame);
                var frameOffsetData = br.ReadUInt32();
                frame.Unknown1 = br.ReadUInt16();
                // Note: BEEF usually indicates dead memory
                var beef = br.ReadUInt16();
                if (beef != 0xBEEF)
                {
                    throw new Exception("It's not always BEEF!!!!");
                }
                // NB: two frames sometimes use the same numCell offset, so they're shared to save space, no dupes
                br.BaseStream.Position = initOffset + NitroChunkHeader.Length + subHeader.BlockOffset_Cells + frameOffsetData;
                frame.NumCell = br.ReadUInt16();
            }
        }

        
    }

    public void WriteTo(BinaryWriter bw)
    {
        var initOffset = bw.BaseStream.Position;

        var header = new NitroChunkHeader
        {
            MagicNumber = MagicNumber
        };

        var subHeader = new SubHeader
        {
            BankCount = (ushort)Banks.Count,
            TFrames = TFrames,
            BlockOffset_Anim = SubHeader.Length
        };

        subHeader.BlockOffset_Frames = (uint)(subHeader.BlockOffset_Anim + Anim.DataLength * Banks.Count);
        var numFramesTotal = Banks.Sum(x => x.Frames.Count);
        subHeader.BlockOffset_Cells = (uint)(subHeader.BlockOffset_Frames + Frame.DataLength_Frame * numFramesTotal);

        header.ChunkLength = (uint)(subHeader.BlockOffset_Cells + Frame.DataLength_Cell * numFramesTotal);

        header.WriteTo(bw);
        subHeader.WriteTo(bw);

        uint cumulativeFrames = 0;
        for (int i = 0; i < Banks.Count; i++)
        {
            var anim = Banks[i];
            bw.Write((uint)anim.Frames.Count);
            bw.Write(anim.DataType);
            bw.Write(anim.Unknown1);
            bw.Write(anim.Unknown2);
            bw.Write(anim.Unknown3);
            bw.Write(Frame.DataLength_Frame * cumulativeFrames);
            cumulativeFrames += (uint)anim.Frames.Count;
        }

        cumulativeFrames = 0;
        for (int i = 0; i < Banks.Count; i++)
        {
            var anim = Banks[i];
            for (int j = 0; j < anim.Frames.Count; j++)
            {
                var frame = anim.Frames[i];
                bw.Write(Frame.DataLength_Cell * cumulativeFrames);
                bw.Write(frame.Unknown1);
                bw.Write((ushort)0xBEEF);
                cumulativeFrames++;
            }
        }

        for (int i = 0; i < Banks.Count; i++)
        {
            var anim = Banks[i];
            for (int j = 0; j < anim.Frames.Count; j++)
            {
                var frame = anim.Frames[i];
                bw.Write(frame.NumCell);
            }
        }
    }

    public class Anim
    {
        public const int DataLength = 0x10;
        public ushort DataType { get; set; }
        public ushort Unknown1 { get; set; }
        public ushort Unknown2 { get; set; }
        public ushort Unknown3 { get; set; }
        public List<Frame> Frames { get; set; } = new List<Frame>();
     }

    public class Frame
    {
        public const int DataLength_Frame = 0x8;
        public const int DataLength_Cell = 0x2;
        public ushort Unknown1 { get; set; }

        public ushort NumCell { get; set; }
    }
}