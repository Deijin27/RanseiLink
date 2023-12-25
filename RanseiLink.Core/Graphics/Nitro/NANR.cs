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
        try
        {
            using (var br = new BinaryReader(File.OpenRead(file)))
            {
                return new NANR(br);
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Error loading file '{file}'", e);
        }
    }

    public void Save(string file)
    {
        using (var bw = new BinaryWriter(File.Create(file)))
        {
            WriteTo(bw);
        }
    }

    /// <summary>
    /// This is the default animation used in most places when a cell shouldn't be animated
    /// </summary>
    /// <returns></returns>
    public static NANR Default(int bankCount = 1)
    {
        var nanr = new NANR();
        for (int i = 0; i < bankCount; i++)
        {
            var anim = new ABNK.Anim();
            anim.DataType = 0;
            anim.Unknown1 = 1;
            anim.Unknown2 = 2;
            anim.Unknown3 = 0;
            anim.Frames.Add(new ABNK.Frame() { CellBank = (ushort)i, Duration = 4 });
            nanr.Labels.Names.Add($"CellAnime{i}");
            nanr.AnimationBanks.Banks.Add(anim);
        }
        nanr.Unknown.Unknown = 0;
        
        return nanr;
    }

    public NANR()
    {
        AnimationBanks = new();
        Labels = new();
        Unknown = new();
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

    private struct SubHeader
    {
        public const int Length = 2 + 2 + 4 + 4 + 4 + 8;

        public ushort BankCount;
        public ushort FrameCount;
        public uint BlockOffset_Anim;
        public uint BlockOffset_Frames;
        public uint BlockOffset_Cells;
        public uint BlockOffset_Unknown;
        public uint BlockOffset_UAAT;

        public SubHeader(BinaryReader br)
        {
            BankCount = br.ReadUInt16();
            FrameCount = br.ReadUInt16();
            BlockOffset_Anim = br.ReadUInt32();
            BlockOffset_Frames = br.ReadUInt32();
            BlockOffset_Cells = br.ReadUInt32();
            BlockOffset_Unknown = br.ReadUInt32();
            if (BlockOffset_Unknown != 0)
            {
                throw new Exception($"It isn't always zero !!! 0_0 (0x{BlockOffset_Unknown:X})");
            }
            BlockOffset_UAAT = br.ReadUInt32(); // 00_05_parts_title_lo-Unpacked> ransei nanr info 0000.nanr
        }

        public void WriteTo(BinaryWriter bw)
        {
            bw.Write(BankCount);
            bw.Write(FrameCount);
            bw.Write(BlockOffset_Anim);
            bw.Write(BlockOffset_Frames);
            bw.Write(BlockOffset_Cells);
            bw.Write(BlockOffset_Unknown);
            bw.Write(BlockOffset_UAAT);
        }
    }

    public ABNK()
    {
        Banks = new();
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

        if (subHeader.BlockOffset_UAAT != 0)
        {
            throw new NotImplementedException("UAAT section");
        }

        Banks = new List<Anim>();
        int tframes = 0;
        for (int i = 0; i < subHeader.BankCount; i++)
        {
            br.BaseStream.Position = initOffset + NitroChunkHeader.Length + subHeader.BlockOffset_Anim + i * Anim.DataLength;
            var anim = new Anim();
            Banks.Add(anim);
            var numFrames = br.ReadInt32();
            tframes += numFrames;
            anim.DataType = br.ReadUInt16();
            anim.Unknown1 = br.ReadUInt16();
            anim.Unknown2 = br.ReadUInt16();
            anim.Unknown3 = br.ReadUInt16();
            var offsetFrame = br.ReadUInt32();

            for (var j = 0; j < numFrames; j++)
            {
                br.BaseStream.Position = initOffset + NitroChunkHeader.Length + subHeader.BlockOffset_Frames + offsetFrame + j * Frame.DataLength_Frame;
                var frame = new Frame();
                anim.Frames.Add(frame);
                var frameOffsetData = br.ReadUInt32();
                frame.Duration = br.ReadUInt16();
                // Note: BEEF usually indicates dead memory
                var beef = br.ReadUInt16();
                if (beef != 0xBEEF)
                {
                    throw new Exception("It's not always BEEF!!!!");
                }
                // NB: two frames sometimes use the same numCell offset, so they're shared to save space, no dupes
                br.BaseStream.Position = initOffset + NitroChunkHeader.Length + subHeader.BlockOffset_Cells + frameOffsetData;
                frame.CellBank = br.ReadUInt16();
            }
        }
        if (tframes != subHeader.FrameCount)
        {
            throw new Exception("The frame count in the header does not match the number of frames specified in the animation banks");
        }
        br.BaseStream.Position = initOffset + header.ChunkLength;
    }

    public void WriteTo(BinaryWriter bw)
    {
        var initOffset = bw.BaseStream.Position;

        var header = new NitroChunkHeader
        {
            MagicNumber = MagicNumber
        };

        var postNitroHeaderOffset = initOffset + NitroChunkHeader.Length;

        var subHeader = new SubHeader
        {
            BankCount = (ushort)Banks.Count,
            FrameCount = (ushort)Banks.Sum(x => x.Frames.Count),
            BlockOffset_Anim = SubHeader.Length
        };

        // skip headers for now
        bw.Pad(NitroChunkHeader.Length + SubHeader.Length);

        // Write animation banks

        int cumulativeFrames = 0;
        for (int i = 0; i < Banks.Count; i++)
        {
            var anim = Banks[i];
            bw.Write((uint)anim.Frames.Count);
            bw.Write(anim.DataType);
            bw.Write(anim.Unknown1);
            bw.Write(anim.Unknown2);
            bw.Write(anim.Unknown3);
            bw.Write(Frame.DataLength_Frame * cumulativeFrames);
            cumulativeFrames += anim.Frames.Count;
        }

        // Write Frames

        subHeader.BlockOffset_Frames = (uint)(bw.BaseStream.Position - postNitroHeaderOffset);

        var distinctCells = new List<ushort>();
        for (int i = 0; i < Banks.Count; i++)
        {
            var anim = Banks[i];
            for (int j = 0; j < anim.Frames.Count; j++)
            {
                var frame = anim.Frames[j];
                // the cell values are not duplicated
                var index = distinctCells.IndexOf(frame.CellBank);
                if (index == -1)
                {
                    index = distinctCells.Count;
                    distinctCells.Add(frame.CellBank);
                }
                bw.Write(Frame.DataLength_Cell * index);
                bw.Write(frame.Duration);
                bw.Write((ushort)0xBEEF);
            }
        }

        // write cells

        subHeader.BlockOffset_Cells = (uint)(bw.BaseStream.Position - postNitroHeaderOffset);

        // make it disisible by 4?
        if (distinctCells.Count % 2 != 0)
        {
            distinctCells.Add(0xCCCC);
        }
        foreach (var cell in distinctCells) 
        {
            bw.Write(cell);
        }
        
        // write headers
        var end = bw.BaseStream.Position;
        header.ChunkLength = (uint)(end - initOffset);
        bw.BaseStream.Position = initOffset;
        header.WriteTo(bw);
        subHeader.WriteTo(bw);
        bw.BaseStream.Position = end;
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
        /// <summary>
        /// Number of frames this is shown (60fps)
        /// </summary>
        public ushort Duration { get; set; }
        /// <summary>
        /// Index of cell bank containing the cell image data to be drawn during this keyframe
        /// </summary>
        public ushort CellBank { get; set; }
    }
}
