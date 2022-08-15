using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RanseiLink.Core.Graphics
{
    public class MDL0
    {
        public const string MagicNumber = "MDL0";

        public List<Model> Models { get; set; }

        public MDL0()
        {
            
        }

        public MDL0(BinaryReader br)
        {
            var initOffset = br.BaseStream.Position;
            var header = new GenericChunkHeader(br);
            if (header.MagicNumber != MagicNumber)
            {
                throw new InvalidDataException($"Unexpected magic number in file header '{header.MagicNumber}' (expected: {MagicNumber})");
            }

            var mdlRadixDict = new RadixDict<OffsetRadixData>();

            foreach (string name in mdlRadixDict.Names)
            {
                Models.Add(new Model(br) { Name = name });
            }
        }

        public void WriteTo(BinaryWriter bw)
        {

        }

        public class Model
        {
            private struct Header
            {
                public uint TotalSize;
                public uint OffsetSbc;
                public uint OffsetMaterial;
                public uint OffsetShape;
                public uint OffsetEvpMatrix;

                public Header(BinaryReader br)
                {
                    TotalSize = br.ReadUInt32();
                    OffsetSbc = br.ReadUInt32();
                    OffsetMaterial = br.ReadUInt32();
                    OffsetShape = br.ReadUInt32();
                    OffsetEvpMatrix = br.ReadUInt32();
                }

                public void WriteTo(BinaryWriter bw)
                {
                    bw.Write(TotalSize);
                    bw.Write(OffsetSbc);
                    bw.Write(OffsetMaterial);
                    bw.Write(OffsetShape);
                    bw.Write(OffsetEvpMatrix);
                }
            }

            private struct ModelInfo
            {
                public byte SbcType;
                public byte ScalingRule;
                public byte TexMtxMode;
                public byte NumNode;
                public byte NumMat;
                public byte NumShp;
                public byte FirstUnusedMtxStackId;
                public byte Unknown;
                public uint PosScale;
                public uint InvPosScale;
                public ushort NumVertex;
                public ushort NumPolygon;
                public ushort NumTriangle;
                public ushort NumQuad;
                public ushort BoxX; // float2
                public ushort BoxY; // float2
                public ushort BoxZ; // float2
                public ushort BoxW; // float2
                public ushort BoxH; // float2
                public ushort BoxD; // float2
                public uint BoxPosScale;
                public uint BoxInvPosScale;

                public ModelInfo(BinaryReader br)
                {
                    SbcType = br.ReadByte();
                    ScalingRule = br.ReadByte();
                    TexMtxMode = br.ReadByte();
                    NumNode = br.ReadByte();
                    NumMat = br.ReadByte();
                    NumShp = br.ReadByte();
                    FirstUnusedMtxStackId = br.ReadByte();
                    Unknown = br.ReadByte();
                    PosScale = br.ReadUInt32();
                    InvPosScale = br.ReadUInt32();
                    NumVertex = br.ReadUInt16();
                    NumPolygon = br.ReadUInt16();
                    NumTriangle = br.ReadUInt16();
                    NumQuad = br.ReadUInt16();
                    BoxX = br.ReadUInt16();
                    BoxY = br.ReadUInt16();
                    BoxZ = br.ReadUInt16();
                    BoxW = br.ReadUInt16();
                    BoxH = br.ReadUInt16();
                    BoxD = br.ReadUInt16();
                    BoxPosScale = br.ReadUInt32();
                    BoxInvPosScale = br.ReadUInt32();
                }

                public void WriteTo(BinaryWriter bw)
                {
                    bw.Write(SbcType);
                    bw.Write(ScalingRule);
                    bw.Write(TexMtxMode);
                    bw.Write(NumNode);
                    bw.Write(NumMat);
                    bw.Write(NumShp);
                    bw.Write(FirstUnusedMtxStackId);
                    bw.Write(Unknown);
                    bw.Write(PosScale);
                    bw.Write(InvPosScale);
                    bw.Write(NumVertex);
                    bw.Write(NumPolygon);
                    bw.Write(NumTriangle);
                    bw.Write(NumQuad);
                    bw.Write(BoxX);
                    bw.Write(BoxY);
                    bw.Write(BoxZ);
                    bw.Write(BoxW);
                    bw.Write(BoxH);
                    bw.Write(BoxD);
                    bw.Write(BoxPosScale);
                    bw.Write(BoxInvPosScale);
                }
            }

            private struct BoneCommand
            {
                public enum Id : byte
                {
                    Unknown0,
                    End,
                    Visibility,
                    PolygonStack,
                    MaterialPoly,
                    Unknown5,
                    ObjectParent,

                }

                public byte Command;
                public byte Size;
                public byte[] Parameters;
            }

            private class NodeData
            {
                [Flags]
                enum TransFlag : ushort
                {
                    T = 1,
                    R = 2,
                    S = 4,
                    P = 8,
                }

                TransFlag Flag;
                ushort Unknown;

                uint Tx;
                uint Ty;
                uint Tz;

                uint Sx;
                uint Sy;
                uint Sz;
                uint InvSx;
                uint InvSy;
                uint InvSz;

                ushort Rot1;
                ushort Rot2;
                ushort Rot3;
                ushort Rot4;
                ushort Rot5;
                ushort Rot6;
                ushort Rot7;
                ushort Rot8;

                ushort A;
                ushort B;

                public NodeData(BinaryReader br)
                {
                    Flag = (TransFlag)(~br.ReadUInt16()); // inverted because they use 0 as true
                    Unknown = br.ReadUInt16();

                    if (Flag.HasFlag(TransFlag.T))
                    {
                        Tx = br.ReadUInt32();
                        Ty = br.ReadUInt32();
                        Tz = br.ReadUInt32();
                    }
                    if (Flag.HasFlag(TransFlag.R))
                    {
                        if (Flag.HasFlag(TransFlag.P))
                        {
                            Rot1 = br.ReadUInt16();
                            Rot2 = br.ReadUInt16();
                            Rot3 = br.ReadUInt16();
                            Rot4 = br.ReadUInt16();
                            Rot5 = br.ReadUInt16();
                            Rot6 = br.ReadUInt16();
                            Rot7 = br.ReadUInt16();
                            Rot8 = br.ReadUInt16();
                        }
                        else
                        {
                            A = br.ReadUInt16();
                            B = br.ReadUInt16();
                        }
                    }
                    if (Flag.HasFlag(TransFlag.S))
                    {
                        Sx = br.ReadUInt32();
                        Sy = br.ReadUInt32();
                        Sz = br.ReadUInt32();
                        InvSx = br.ReadUInt32();
                        InvSy = br.ReadUInt32();
                        InvSz = br.ReadUInt32();
                    }

                }

                public void WriteTo(BinaryWriter bw)
                {
                    bw.Write(~(ushort)Flag);
                    bw.Write(Unknown);

                    if (Flag.HasFlag(TransFlag.T))
                    {
                        bw.Write(Tx);
                        bw.Write(Ty);
                        bw.Write(Tz);
                    }
                    if (Flag.HasFlag(TransFlag.R))
                    {
                        if (Flag.HasFlag(TransFlag.P))
                        {
                            bw.Write(Rot1);
                            bw.Write(Rot2);
                            bw.Write(Rot3);
                            bw.Write(Rot4);
                            bw.Write(Rot5);
                            bw.Write(Rot6);
                            bw.Write(Rot7);
                            bw.Write(Rot8);
                        }
                        else
                        {
                            bw.Write(A);
                            bw.Write(B);
                        }
                    }
                    if (Flag.HasFlag(TransFlag.S))
                    {
                        bw.Write(Sx);
                        bw.Write(Sy);
                        bw.Write(Sz);
                        bw.Write(InvSx);
                        bw.Write(InvSy);
                        bw.Write(InvSz);
                    }

                }
            }

            public string Name { get; set; }

            public Model()
            {

            }

            private class ToMatRadixData : IRadixData
            {
                ushort Offset;
                byte NumMat;
                byte Bound;

                public ushort Length => 4;

                public void ReadFrom(BinaryReader br)
                {
                    Offset = br.ReadUInt16();
                    NumMat = br.ReadByte();
                    Bound = br.ReadByte();
                }

                public void WriteTo(BinaryWriter bw)
                {
                    bw.Write(Offset);
                    bw.Write(NumMat);
                    bw.Write(Bound);
                }
            }

            public Model(BinaryReader br)
            {
                var header = new Header(br);

                var mdlInfo = new ModelInfo(br);

                // nodes
                var nodeRadixDict = new RadixDict<OffsetRadixData>();
                NodeData[] nodeData = new NodeData[nodeRadixDict.Names.Count];
                for (int i = 0; i < nodeRadixDict.Names.Count; i++)
                {
                    nodeData[i] = new NodeData(br);
                }

                // sbc
                byte[] sbc = br.ReadBytes((int)(header.OffsetMaterial - header.OffsetSbc));

                // materials
                var materials = new MaterialSet(br);

                // shapes
                var shapes = new ShapeSet(br);
            }

            private class MaterialSet
            {
                private class Material
                {
                    [Flags]
                    public enum MATFLAG : ushort
                    {
                        TEXMTX_USE = 1,
                        TEXMTX_SCALEONE = 2,
                        TEXMTX_ROTZERO = 4,
                        TEXMTX_TRANSZERO = 8,
                        ORIGWH_SAME = 16, // 0x0010
                        WIREFRAME = 32, // 0x0020
                        DIFFUSE = 64, // 0x0040
                        AMBIENT = 128, // 0x0080
                        VTXCOLOR = 256, // 0x0100
                        SPECULAR = 512, // 0x0200
                        EMISSION = 1024, // 0x0400
                        SHININESS = 2048, // 0x0800
                        TEXPLTTBASE = 4096, // 0x1000
                        EFFECTMTX = 8192, // 0x2000
                    }

                    public ushort ItemTag;
                    public ushort Size;
                    public uint DiffuseAmb;
                    public uint SpecularEmission;
                    public uint PolyAttr;
                    public uint PolyAttrMask;
                    public uint TexImageParam;
                    public uint TexImageParamMask;
                    public ushort TexPaletteBase;
                    public MATFLAG Flag;
                    public ushort OrigWidth;
                    public ushort OrigHeight;

                    public uint MagWidth;
                    public uint MagHeight;
                    public uint ScaleS;
                    public uint ScaleT;
                    public ushort RotSin;
                    public ushort RotCos;
                    public uint TransS;
                    public uint TransT;
                    public uint[] EffectMtx;

                    public Material(BinaryReader br)
                    {
                        ItemTag = br.ReadUInt16();  
                        Size = br.ReadUInt16();
                        DiffuseAmb = br.ReadUInt32();
                        SpecularEmission = br.ReadUInt32();
                        PolyAttr = br.ReadUInt32();
                        PolyAttrMask = br.ReadUInt32();
                        TexImageParam = br.ReadUInt32();
                        TexImageParamMask = br.ReadUInt32();
                        TexPaletteBase = br.ReadUInt16();
                        Flag = (MATFLAG)(~br.ReadUInt16()); // inverted
                        OrigWidth = br.ReadUInt16();
                        OrigHeight = br.ReadUInt16();

                        MagWidth = br.ReadUInt32();
                        MagHeight = br.ReadUInt32();

                        if (Flag.HasFlag(MATFLAG.TEXMTX_SCALEONE))
                        {
                            ScaleS = br.ReadUInt32();
                            ScaleT = br.ReadUInt32();
                        }
                        if (Flag.HasFlag(MATFLAG.TEXMTX_ROTZERO))
                        {
                            RotSin = br.ReadUInt16();
                            RotCos = br.ReadUInt16();
                        }
                        if (Flag.HasFlag(MATFLAG.TEXMTX_TRANSZERO))
                        {
                            TransS = br.ReadUInt32();
                            TransT = br.ReadUInt32();
                        }
                        if (!Flag.HasFlag(MATFLAG.EFFECTMTX)) // this check is inverse
                        {
                            EffectMtx = new uint[0x10];
                            for (int i = 0; i < 0x10; i++)
                            {
                                EffectMtx[i] = br.ReadUInt32();
                            }
                        }
                    }
                }

                public MaterialSet(BinaryReader br)
                {
                    ushort offsetDictTextToMatList = br.ReadUInt16();
                    ushort offsetDictPalToMatList = br.ReadUInt16();
                    var materialRadixDict = new RadixDict<OffsetRadixData>();
                    var texToMatDict = new RadixDict<ToMatRadixData>();
                    var palToMatDict = new RadixDict<ToMatRadixData>();

                    var texMaterialIndex = new byte[texToMatDict.Names.Count];
                    for (int i = 0; i < texToMatDict.Names.Count; i++)
                    {
                        texMaterialIndex[i] = br.ReadByte();
                    }

                    var palMaterialIndex = new byte[palToMatDict.Names.Count];
                    for (int i = 0; i < palToMatDict.Names.Count; i++)
                    {
                        palMaterialIndex[i] = br.ReadByte();
                    }

                    var materials = new Material[materialRadixDict.Names.Count];
                    for (int i = 0; i < materialRadixDict.Names.Count; i++)
                    {
                        materials[i] = new Material(br);
                    }
                }

                public void WriteTo(BinaryWriter bw)
                {
                    throw new NotImplementedException();
                }
            }

            private class ShapeSet
            {
                private struct Shape
                {
                    [Flags]
                    public enum SHPFLAG : uint
                    {
                        USE_NORMAL = 1,
                        USE_COLOR = 2,
                        USE_TEXCOORD = 4,
                        USE_RESTOREMTX = 8,
                    }

                    public ushort ItemTag;
                    public ushort Size;
                    public SHPFLAG Flag;
                    public uint OffsetDL;
                    public int SizeDL;

                    public Shape(BinaryReader br)
                    {
                        ItemTag = br.ReadUInt16();
                        Size = br.ReadUInt16();
                        Flag = (SHPFLAG)br.ReadUInt32();
                        OffsetDL = br.ReadUInt32();
                        SizeDL = br.ReadInt32();
                    }

                    public void WriteTo(BinaryWriter bw)
                    {
                        bw.Write(ItemTag);
                        bw.Write(Size);
                        bw.Write((uint)Flag);
                        bw.Write(OffsetDL);
                        bw.Write(SizeDL);
                    }
                }

                public ShapeSet(BinaryReader br)
                {
                    var shapeRadixDict = new RadixDict<OffsetRadixData>();
                    Shape[] shapes = new Shape[shapeRadixDict.Names.Count];
                    for (int i = 0; i < shapes.Length; i++)
                    {
                        shapes[i] = new Shape(br);
                    }

                    List<byte[]> dls = new List<byte[]>();
                    for (int i = 0; i < shapes.Length; i++)
                    {
                        dls.Add(br.ReadBytes(shapes[i].SizeDL));
                    }
                }

                public void WriteTo(BinaryWriter bw)
                {
                    throw new NotImplementedException();
                }
            }
        }

        
    }
}
