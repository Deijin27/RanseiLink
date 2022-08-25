using RanseiLink.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace RanseiLink.Core.Graphics
{
    public class NSMDL
    {
        public const string MagicNumber = "MDL0";

        public List<Model> Models { get; set; } = new List<Model>();

        public NSMDL()
        {
            
        }

        public NSMDL(BinaryReader br)
        {
            var header = new GenericChunkHeader(br);
            if (header.MagicNumber != MagicNumber)
            {
                throw new InvalidDataException($"Unexpected magic number in file header '{header.MagicNumber}' (expected: {MagicNumber})");
            }

            var mdlRadixDict = new RadixDict<OffsetRadixData>(br);

            foreach (string name in mdlRadixDict.Names)
            {
                Models.Add(new Model(br) { Name = name });
            }
        }

        public void WriteTo(BinaryWriter bw)
        {
            var initOffset = bw.BaseStream.Position;

            var header = new GenericChunkHeader
            {
                MagicNumber = MagicNumber,
            };

            var radix = new RadixDict<OffsetRadixData>();

            // skip header and radix for now
            bw.Pad(GenericChunkHeader.Length + RadixDict<OffsetRadixData>.CalculateLength(Models.Count));

            // write models
            foreach (var model in Models)
            {
                radix.Data.Add(new OffsetRadixData { Offset = (uint)(bw.BaseStream.Position - initOffset) });
                model.WriteTo(bw);
                radix.Names.Add(model.Name);
            }

            // write header and radix
            var endOffset = bw.BaseStream.Position;
            header.ChunkLength = (uint)(endOffset - initOffset);
            bw.BaseStream.Position = initOffset;
            header.WriteTo(bw);
            radix.WriteTo(bw);
            bw.BaseStream.Position = endOffset;
        }

        public class Model
        {
            #region Properties

            public string Name { get; set; }
            public ModelInfo MdlInfo { get; set; }
            public List<PolymeshData> Polymeshes { get; set; } = new List<PolymeshData>();
            public List<RenderCommand> RenderCommands { get; set; } = new List<RenderCommand>();
            public List<Material> Materials { get; set; } = new List<Material>();
            public List<Polygon> Polygons { get; set; } = new List<Polygon>();

            #endregion

            #region ReadWrite

            public Model()
            {

            }

            public Model(BinaryReader br)
            {
                var initPos = br.BaseStream.Position;
                var header = new Header(br);

                MdlInfo = new ModelInfo(br);

                // polymeshes
                ReadPolymeshes(br);

                // commands
                if (br.BaseStream.Position != header.OffsetRenderCommands + initPos)
                {
                    throw new Exception("offset wrong : bone commands");
                }
                ReadRenderCommands(br);

                // materials
                if (br.BaseStream.Position != header.OffsetMaterials + initPos)
                {
                    throw new Exception("offset wrong : materials");
                }
                ReadMaterials(br);

                // polygons
                if (br.BaseStream.Position != header.OffsetPolygons + initPos)
                {
                    throw new Exception("offset wrong : meshes");
                }
                ReadPolygons(br);

                br.BaseStream.Position = initPos + header.TotalSize;
            }

            public void WriteTo(BinaryWriter bw)
            {
                var initOffset = bw.BaseStream.Position;
                Header header = new Header();

                // skip header for now
                bw.Pad(Header.Length);

                // Write contents
                MdlInfo.WriteTo(bw);
                WritePolymeshes(bw);
                header.OffsetRenderCommands = (uint)(bw.BaseStream.Position - initOffset);
                WriteRenderCommands(bw);
                header.OffsetMaterials = (uint)(bw.BaseStream.Position - initOffset);
                WriteMaterials(bw);
                header.OffsetPolygons = (uint)(bw.BaseStream.Position - initOffset);
                WritePolygons(bw);
                header.OffsetEvpMatrix = (uint)(bw.BaseStream.Position - initOffset);

                var endOffset = bw.BaseStream.Position;
                header.TotalSize = (uint)(endOffset - initOffset);
                bw.BaseStream.Position = initOffset;
                header.WriteTo(bw);
                bw.BaseStream.Position = endOffset;

            }

            #endregion

            #region Header

            private struct Header
            {
                public const int Length = 20;

                public uint TotalSize;
                public uint OffsetRenderCommands;
                public uint OffsetMaterials;
                public uint OffsetPolygons;
                public uint OffsetEvpMatrix;

                public Header(BinaryReader br)
                {
                    TotalSize = br.ReadUInt32();
                    OffsetRenderCommands = br.ReadUInt32();
                    OffsetMaterials = br.ReadUInt32();
                    OffsetPolygons = br.ReadUInt32();
                    OffsetEvpMatrix = br.ReadUInt32();
                }

                public void WriteTo(BinaryWriter bw)
                {
                    bw.Write(TotalSize);
                    bw.Write(OffsetRenderCommands);
                    bw.Write(OffsetMaterials);
                    bw.Write(OffsetPolygons);
                    bw.Write(OffsetEvpMatrix);
                }
            }

            public struct ModelInfo
            {
                public byte SbcType;
                public byte ScalingRule;
                public byte TexMtxMode;
                public byte NumBone;
                public byte NumMat;
                public byte NumMesh;
                public byte FirstUnusedMtxStackId;
                public byte Unknown;
                public float UpScale;
                public float DownScale;
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
                public uint BoxUpScale;
                public uint BoxDownScale;

                public ModelInfo(BinaryReader br)
                {
                    SbcType = br.ReadByte();
                    ScalingRule = br.ReadByte();
                    TexMtxMode = br.ReadByte();
                    NumBone = br.ReadByte();
                    NumMat = br.ReadByte();
                    NumMesh = br.ReadByte();
                    FirstUnusedMtxStackId = br.ReadByte();
                    Unknown = br.ReadByte();
                    UpScale = FixedPoint.Fix_1_19_12(br.ReadInt32());
                    DownScale = FixedPoint.Fix_1_19_12(br.ReadInt32());
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
                    BoxUpScale = br.ReadUInt32();
                    BoxDownScale = br.ReadUInt32();
                }

                public void WriteTo(BinaryWriter bw)
                {
                    bw.Write(SbcType);
                    bw.Write(ScalingRule);
                    bw.Write(TexMtxMode);
                    bw.Write(NumBone);
                    bw.Write(NumMat);
                    bw.Write(NumMesh);
                    bw.Write(FirstUnusedMtxStackId);
                    bw.Write(Unknown);
                    bw.Write(FixedPoint.InverseFix_1_19_12(UpScale));
                    bw.Write(FixedPoint.InverseFix_1_19_12(DownScale));
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
                    bw.Write(BoxUpScale);
                    bw.Write(BoxDownScale);
                }
            }

            #endregion

            #region Polymeshes

            public class PolymeshData
            {
                [Flags]
                public enum TransFlag : ushort
                {
                    Translate = 1,
                    Rotate = 2,
                    Scale = 4,
                    Pivot = 8,
                }
                public string Name;

                public TransFlag Flag;

                public Vector3 Translation;
                public Matrix3x3 Rotate = Matrix3x3.Identity;
                public Vector3 Scale;

                // matrix parameters, may not be necessary in the future
                public bool HasRotate;
                public float A;
                public float B;
                public int Select;
                public int Neg;

                // pre-calculated trs matrix
                public Matrix4x4 TRSMatrix;

                static Matrix3x3 CalcPivotMtx(int select, int neg, float a, float b)
                {
                    float o = (neg & 1) == 0 ? 1 : -1;
                    float c = (neg >> 1 & 1) == 0 ? b : -b;
                    float d = (neg >> 2 & 1) == 0 ? a : -a;

                    switch (select)
                    {
                        case 0: return new Matrix3x3(o, 0, 0, 0, a, b, 0, c, d);
                        case 1: return new Matrix3x3(0, o, 0, a, 0, b, c, 0, d);
                        case 2: return new Matrix3x3(0, 0, o, a, b, 0, c, d, 0);

                        case 3: return new Matrix3x3(0, a, b, o, 0, 0, 0, c, d);
                        case 4: return new Matrix3x3(a, 0, b, 0, o, 0, c, 0, d);
                        case 5: return new Matrix3x3(a, b, 0, 0, 0, o, c, d, 0);

                        case 6: return new Matrix3x3(0, a, b, 0, c, d, o, 0, 0);
                        case 7: return new Matrix3x3(a, 0, b, c, 0, d, 0, o, 0);
                        case 8: return new Matrix3x3(a, b, 0, c, d, 0, 0, 0, o);

                        default: throw new Exception($"Unexepeced select value in {nameof(CalcPivotMtx)}");
                    }
                }

                public PolymeshData(BinaryReader br)
                {
                    var f = br.ReadUInt16();
                    Flag = (TransFlag)((~f) & 0b1111); // inverted because they use 0 as true

                    var m0 = br.ReadUInt16(); // part of rotation matrix stuff

                    if (Flag.HasFlag(TransFlag.Translate))
                    {
                        Translation = new Vector3(
                            FixedPoint.Fix_1_19_12(br.ReadInt32()),
                            FixedPoint.Fix_1_19_12(br.ReadInt32()),
                            FixedPoint.Fix_1_19_12(br.ReadInt32())
                        );
                    }

                    bool hasRotate = false;
                    if (!Flag.HasFlag(TransFlag.Pivot))
                    {
                        if (!Flag.HasFlag(TransFlag.Rotate))
                        {
                            throw new Exception("Pivot can exist without rotate!!!!!!!!!!!!!");
                        }
                        hasRotate = true;
                        A = FixedPoint.Fix_1_3_12(br.ReadUInt16());
                        B = FixedPoint.Fix_1_3_12(br.ReadUInt16());
                        Select = f >> 4 & 0b1111;
                        Neg = f >> 8 & 0b111;
                        Rotate = CalcPivotMtx(Select, Neg, A, B);

                    }
                    else if (Flag.HasFlag(TransFlag.Rotate))
                    {
                        hasRotate = true;
                        Rotate = new Matrix3x3(
                            FixedPoint.Fix_1_3_12(m0),
                            FixedPoint.Fix_1_3_12(br.ReadUInt16()),
                            FixedPoint.Fix_1_3_12(br.ReadUInt16()),

                            FixedPoint.Fix_1_3_12(br.ReadUInt16()),
                            FixedPoint.Fix_1_3_12(br.ReadUInt16()),
                            FixedPoint.Fix_1_3_12(br.ReadUInt16()),

                            FixedPoint.Fix_1_3_12(br.ReadUInt16()),
                            FixedPoint.Fix_1_3_12(br.ReadUInt16()),
                            FixedPoint.Fix_1_3_12(br.ReadUInt16())
                            );
                    }

                    if (Flag.HasFlag(TransFlag.Scale))
                    {
                        Scale = new Vector3(
                            FixedPoint.Fix_1_19_12(br.ReadInt32()),
                            FixedPoint.Fix_1_19_12(br.ReadInt32()),
                            FixedPoint.Fix_1_19_12(br.ReadInt32())
                        );
                        var invSx = FixedPoint.Fix_1_19_12(br.ReadInt32());
                        var invSy = FixedPoint.Fix_1_19_12(br.ReadInt32());
                        var invSz = FixedPoint.Fix_1_19_12(br.ReadInt32());
                    }

                    TRSMatrix = Matrix4x4.Identity;
                    if (Flag.HasFlag(TransFlag.Scale))
                    {
                        TRSMatrix = Matrix4x4.CreateScale(Scale);
                    }
                    if (hasRotate)
                    {
                        HasRotate = true;
                        TRSMatrix *= Rotate.As4x4();
                    }
                    if (Flag.HasFlag(TransFlag.Translate))
                    {
                        TRSMatrix *= Matrix4x4.CreateTranslation(Translation);
                    }
                }

                public void WriteTo(BinaryWriter bw)
                {
                    // calculate f and m0
                    int f = (~(int)Flag) & 0b1111
                          | 0b11111 << 11 // this is always there
                          | Select << 4 // default 0
                          | Neg << 8; // default 0

                    bw.Write((ushort)f);
                    bw.Write((ushort)FixedPoint.InverseFix_1_3_12(Rotate.M11)); // take the value from identity matrix as default, that is 1f=0x1000

                    // decide whether to write parameters for translate, pivot, and rotate
                    if (Flag.HasFlag(TransFlag.Translate))
                    {
                        bw.Write(FixedPoint.InverseFix_1_19_12(Translation.X));
                        bw.Write(FixedPoint.InverseFix_1_19_12(Translation.Y));
                        bw.Write(FixedPoint.InverseFix_1_19_12(Translation.Z));
                    }

                    if (!Flag.HasFlag(TransFlag.Pivot))
                    {
                        bw.Write((ushort)FixedPoint.InverseFix_1_3_12(A));
                        bw.Write((ushort)FixedPoint.InverseFix_1_3_12(B));

                    }
                    else if (Flag.HasFlag(TransFlag.Rotate))
                    {
                        bw.Write((ushort)FixedPoint.InverseFix_1_3_12(Rotate.M12));
                        bw.Write((ushort)FixedPoint.InverseFix_1_3_12(Rotate.M13));

                        bw.Write((ushort)FixedPoint.InverseFix_1_3_12(Rotate.M21));
                        bw.Write((ushort)FixedPoint.InverseFix_1_3_12(Rotate.M22));
                        bw.Write((ushort)FixedPoint.InverseFix_1_3_12(Rotate.M23));

                        bw.Write((ushort)FixedPoint.InverseFix_1_3_12(Rotate.M31));
                        bw.Write((ushort)FixedPoint.InverseFix_1_3_12(Rotate.M32));
                        bw.Write((ushort)FixedPoint.InverseFix_1_3_12(Rotate.M33));
                    }

                    if (Flag.HasFlag(TransFlag.Scale))
                    {
                        bw.Write(FixedPoint.InverseFix_1_19_12(Scale.X));
                        bw.Write(FixedPoint.InverseFix_1_19_12(Scale.Y));
                        bw.Write(FixedPoint.InverseFix_1_19_12(Scale.Z));
                        bw.Write(FixedPoint.InverseFix_1_19_12(1 / Scale.X));
                        bw.Write(FixedPoint.InverseFix_1_19_12(1 / Scale.Y));
                        bw.Write(FixedPoint.InverseFix_1_19_12(1 / Scale.Z));
                    }

                }
            }

            private void ReadPolymeshes(BinaryReader br)
            {
                var nodeRadixDict = new RadixDict<OffsetRadixData>(br);
                for (int i = 0; i < nodeRadixDict.Names.Count; i++)
                {
                    var d = new PolymeshData(br);
                    d.Name = nodeRadixDict.Names[i];

                    Polymeshes.Add(d);
                }
            }

            private void WritePolymeshes(BinaryWriter bw)
            {
                var initPos = bw.BaseStream.Position;

                // skip radix dict for now
                bw.Pad(RadixDict<OffsetRadixData>.CalculateLength(Polymeshes.Count));

                var plymshRadixDict = new RadixDict<OffsetRadixData>();
                foreach (var plymsh in Polymeshes)
                {
                    plymshRadixDict.Names.Add(plymsh.Name);
                    plymshRadixDict.Data.Add(new OffsetRadixData { Offset = (uint)(bw.BaseStream.Position - initPos) });
                    plymsh.WriteTo(bw);
                }

                // write radix dict
                var endPos = bw.BaseStream.Position;
                bw.BaseStream.Position = initPos;
                plymshRadixDict.WriteTo(bw);
                bw.BaseStream.Position = endPos;

            }
            
            #endregion

            #region Render Commands

            private void ReadRenderCommands(BinaryReader br)
            {
                var initPos = br.BaseStream.Position;
                RenderCommand command;
                do
                {
                    command = new RenderCommand(br);
                    RenderCommands.Add(command);
                }
                while (command.OpCode != RenderOpCode.END);

                // padding to 32bit
                var lengthMod4 = (br.BaseStream.Position - initPos) % 4;
                if (lengthMod4 != 0)
                {
                    br.Skip(4 - (int)lengthMod4);
                }
            }

            private void WriteRenderCommands(BinaryWriter bw)
            {
                var initPos = bw.BaseStream.Position;

                foreach (var command in RenderCommands)
                {
                    command.WriteTo(bw);
                }

                // padding to 32bit
                var lengthMod4 = (bw.BaseStream.Position - initPos) % 4;
                if (lengthMod4 != 0)
                {
                    bw.Pad(4 - (int)lengthMod4);
                }
            }

            #endregion

            #region Materials

            private class ToMatRadixData : IRadixData
            {
                public ushort Offset;
                public byte NumMat;
                public byte Bound;

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


            public class Material
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
                public Rgb15 Diffuse;
                public bool DiffuseIsDefaultVertexColor;
                public Rgb15 Ambient;
                public Rgb15 Specular;
                public bool EnableShininessTable;
                public Rgb15 Emission;
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
                    var size = br.ReadUInt16();
                    var diffAmb = br.ReadInt32();
                    Diffuse = Rgb15.From((ushort)(diffAmb & 0x7FFF));
                    DiffuseIsDefaultVertexColor = (diffAmb >> 15 & 1) == 1;
                    Ambient = Rgb15.From((ushort)(diffAmb >> 16 & 0x7FFF));
                    var specEmi = br.ReadInt32();
                    Specular = Rgb15.From((ushort)(specEmi & 0x7FFF));
                    EnableShininessTable = (specEmi >> 15 & 1) == 1;
                    Emission = Rgb15.From((ushort)(specEmi >> 16 & 0x7FFF));
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

                public void WriteTo(BinaryWriter bw)
                {
                    var initOffset = bw.BaseStream.Position;

                    // skip header
                    bw.Pad(4);

                    // write data
                    int diffAmb =
                        Diffuse.ToUInt16()
                        | (DiffuseIsDefaultVertexColor ? 1 : 0) << 15
                        | Ambient.ToUInt16() << 16;
                    bw.Write(diffAmb);

                    int speEmi =
                        Specular.ToUInt16()
                        | (EnableShininessTable ? 1 : 0) << 15
                        | Emission.ToUInt16() << 16;
                    bw.Write(speEmi);

                    bw.Write(PolyAttr);
                    bw.Write(PolyAttrMask);
                    bw.Write(TexImageParam);
                    bw.Write(TexImageParamMask);
                    bw.Write(TexPaletteBase);
                    ushort flagVal = (ushort)~Flag;
                    bw.Write(flagVal);
                    bw.Write(OrigWidth);
                    bw.Write(OrigHeight);

                    bw.Write(MagWidth);
                    bw.Write(MagHeight);

                    if (Flag.HasFlag(MATFLAG.TEXMTX_SCALEONE))
                    {
                        bw.Write(ScaleS);
                        bw.Write(ScaleT);
                    }
                    if (Flag.HasFlag(MATFLAG.TEXMTX_ROTZERO))
                    {
                        bw.Write(RotSin);
                        bw.Write(RotCos);
                    }
                    if (Flag.HasFlag(MATFLAG.TEXMTX_TRANSZERO))
                    {
                        bw.Write(TransS);
                        bw.Write(TransT);
                    }
                    if (!Flag.HasFlag(MATFLAG.EFFECTMTX)) // this check is inverse
                    {
                        for (int i = 0; i < 0x10; i++)
                        {
                            bw.Write(EffectMtx[i]);
                        }
                    }

                    // write header
                    var endOffset = bw.BaseStream.Position;
                    bw.BaseStream.Position = initOffset;
                    bw.Write(ItemTag);
                    ushort size = (ushort)(endOffset - initOffset);
                    bw.Write(size);
                    bw.BaseStream.Position = endOffset;

                }

                public string Name { get; set; }
                public string Texture { get; set; }
                public string Palette { get; set; }
            }

            private void ReadMaterials(BinaryReader br)
            {
                ushort offsetDictTextToMatList = br.ReadUInt16();
                ushort offsetDictPalToMatList = br.ReadUInt16();
                var materialRadixDict = new RadixDict<OffsetRadixData>(br);
                var texToMatDict = new RadixDict<ToMatRadixData>(br);
                var palToMatDict = new RadixDict<ToMatRadixData>(br);

                var materialTextures = new string[materialRadixDict.Names.Count];
                for (int i = 0; i < texToMatDict.Names.Count; i++)
                {
                    var data = texToMatDict.Data[i];
                    for (int n = 0; n < data.NumMat; n++)
                    {
                        int matIdx = br.ReadByte();
                        materialTextures[matIdx] = texToMatDict.Names[i];
                    }
                }

                var materialPalettes = new string[materialRadixDict.Names.Count];
                for (int i = 0; i < palToMatDict.Names.Count; i++)
                {
                    var data = palToMatDict.Data[i];
                    for (int n = 0; n < data.NumMat; n++)
                    {
                        int matIdx = br.ReadByte();
                        materialPalettes[matIdx] = palToMatDict.Names[i];
                    }
                }

                // this padding is seen in map5. Probably just for 32bit alignment
                if ((2 * materialRadixDict.Names.Count) % 4 != 0)
                {
                    br.Skip(4 - (2 * materialRadixDict.Names.Count) % 4);
                }

                for (int i = 0; i < materialRadixDict.Names.Count; i++)
                {
                    var m = new Material(br);
                    m.Texture = materialTextures[i];
                    m.Palette = materialPalettes[i];
                    m.Name = materialRadixDict.Names[i];
                    Materials.Add(m);
                }
            }

            private void WriteMaterials(BinaryWriter bw)
            {
                var initOffset = bw.BaseStream.Position;

                // preprare radix dicts and calculate material-texture-palette links
                var materialRadixDict = new RadixDict<OffsetRadixData>();
                var texToMatDict = new RadixDict<ToMatRadixData>();
                var palToMatDict = new RadixDict<ToMatRadixData>();
                var textureMaterials = new List<List<int>>();
                var palMaterials = new List<List<int>>();
                for (int mtlIdx = 0; mtlIdx < Materials.Count; mtlIdx++)
                {
                    var m = Materials[mtlIdx];

                    // prepare material radix dict
                    materialRadixDict.Names.Add(m.Name);
                    materialRadixDict.Data.Add(new OffsetRadixData());

                    // prepare texture radix dict, and link to materials
                    var texIdx = texToMatDict.Names.IndexOf(m.Texture);
                    if (texIdx == -1)
                    {
                        texToMatDict.Names.Add(m.Texture);
                        texToMatDict.Data.Add(new ToMatRadixData());
                        textureMaterials.Add(new List<int> { mtlIdx });
                    }
                    else
                    {
                        textureMaterials[texIdx].Add(mtlIdx);
                    }

                    // prepare palette radix dict, and link to materials
                    var palIdx = palToMatDict.Names.IndexOf(m.Palette);
                    if (palIdx == -1)
                    {
                        palToMatDict.Names.Add(m.Palette);
                        palToMatDict.Data.Add(new ToMatRadixData());
                        palMaterials.Add(new List<int> { mtlIdx });
                    }
                    else
                    {
                        palMaterials[palIdx].Add(mtlIdx);
                    }
                }

                // skip header and radix dicts for now
                ushort offsetMatDict = 4;
                ushort offsetTexToMatDict = (ushort)(offsetMatDict + RadixDict<OffsetRadixData>.CalculateLength(materialRadixDict.Names.Count));
                ushort offsetPalToMatDict = (ushort)(offsetTexToMatDict + RadixDict<ToMatRadixData>.CalculateLength(texToMatDict.Names.Count));
                int lenPalDict = RadixDict<ToMatRadixData>.CalculateLength(texToMatDict.Names.Count);
                bw.Pad(offsetPalToMatDict + lenPalDict);

                // write the list of texture-material and pal-material and store the offsets and counts in the dict data
                for (int i = 0; i < texToMatDict.Data.Count; i++)
                {
                    var texMats = textureMaterials[i];
                    var dictData = texToMatDict.Data[i];
                    dictData.Offset = (ushort)(bw.BaseStream.Position - initOffset);
                    dictData.NumMat = (byte)texMats.Count;
                    foreach (int matIdx in texMats)
                    {
                        bw.Write((byte)matIdx);
                    }
                }
                for (int i = 0; i < palToMatDict.Data.Count; i++)
                {
                    var palMats = palMaterials[i];
                    var dictData = palToMatDict.Data[i];
                    dictData.Offset = (ushort)(bw.BaseStream.Position - initOffset);
                    dictData.NumMat = (byte)palMats.Count;
                    foreach (int matIdx in palMats)
                    {
                        bw.Write((byte)matIdx);
                    }
                }

                // the 32bit alignment
                if ((2 * Materials.Count) % 4 != 0)
                {
                    bw.Pad(4 - (2 * Materials.Count) % 4);
                }

                // write material data and record offsets
                for (int i = 0; i < Materials.Count; i++)
                {
                    materialRadixDict.Data[i].Offset = (uint)(bw.BaseStream.Position - initOffset);
                    Materials[i].WriteTo(bw);
                }

                // return to start to write header
                var endOffset = bw.BaseStream.Position;
                bw.BaseStream.Position = initOffset;

                // write radix dict offsets
                bw.Write(offsetTexToMatDict);
                bw.Write(offsetPalToMatDict);

                // write radix dicts
                materialRadixDict.WriteTo(bw);
                texToMatDict.WriteTo(bw);
                palToMatDict.WriteTo(bw);

                // return to end
                bw.BaseStream.Position = endOffset;
            }
            
            #endregion

            #region Polygons

            [Flags]
            public enum SHPFLAG : uint
            {
                USE_NORMAL = 1,
                USE_COLOR = 2,
                USE_TEXCOORD = 4,
                USE_RESTOREMTX = 8,
            }

            private struct PolygonInfo
            {
                public ushort ItemTag;
                public const ushort Length = 16;
                public SHPFLAG Flag;
                public uint CommandsOffset;
                public int CommandsLength;

                public PolygonInfo(BinaryReader br)
                {
                    ItemTag = br.ReadUInt16();
                    var length = br.ReadUInt16();
                    Flag = (SHPFLAG)br.ReadUInt32();
                    CommandsOffset = br.ReadUInt32();
                    CommandsLength = br.ReadInt32();
                }

                public void WriteTo(BinaryWriter bw)
                {
                    bw.Write(ItemTag);
                    bw.Write(Length);
                    bw.Write((uint)Flag);
                    bw.Write(CommandsOffset);
                    bw.Write(CommandsLength);
                }
            }

            public class Polygon
            {
                public ushort ItemTag { get; set; }
                public SHPFLAG Flag { get; set; }
                public string Name { get; set; }
                public List<PolygonDisplayCommand> Commands { get; set; } = new List<PolygonDisplayCommand>();
            }

            private void ReadPolygons(BinaryReader br)
            {
                var polygonRadixDict = new RadixDict<OffsetRadixData>(br);
                PolygonInfo[] polyInfos = new PolygonInfo[polygonRadixDict.Names.Count];
                for (int i = 0; i < polyInfos.Length; i++)
                {
                    polyInfos[i] = new PolygonInfo(br);
                }

                for (int i = 0; i < polyInfos.Length; i++)
                {
                    var polyInfo = polyInfos[i];
                    var endPos = br.BaseStream.Position + polyInfo.CommandsLength;
                    var poly = new Polygon { Name = polygonRadixDict.Names[i], ItemTag = polyInfo.ItemTag, Flag = polyInfo.Flag };
                    while (br.BaseStream.Position < endPos)
                    {
                        var commandIds = br.ReadBytes(4).Select(x => (MeshDisplayOpCode)x).ToArray();
                        foreach (var id in commandIds)
                        {
                            var numPrms = id.ParamLength();
                            var prms = new int[numPrms];
                            for (int n = 0; n < numPrms; n++)
                            {
                                prms[n] = br.ReadInt32();
                            }
                            poly.Commands.Add(new PolygonDisplayCommand { OpCode = id, Params = prms });
                        }
                    }
                    br.BaseStream.Position = endPos; // ensure end pos
                    Polygons.Add(poly);
                }
            }

            private void WritePolygons(BinaryWriter bw)
            {
                var initOffset = bw.BaseStream.Position;

                // skip radix dict and mesh info for now
                bw.Pad(RadixDict<OffsetRadixData>.CalculateLength(Polygons.Count));
                var polyInfoOffset = bw.BaseStream.Position;
                bw.Pad(PolygonInfo.Length * Polygons.Count);

                var polygonRadixDict = new RadixDict<OffsetRadixData>();
                PolygonInfo[] polyInfos = new PolygonInfo[Polygons.Count];

                // write polygon commands and store offsets and lengths in meshinfo, and store names in radix dict
                for (int i = 0; i < Polygons.Count; i++)
                {
                    long currentPolyInfoOffset = polyInfoOffset + i * PolygonInfo.Length;
                    Polygon poly = Polygons[i];

                    polygonRadixDict.Names.Add(poly.Name);

                    var polyInfo = new PolygonInfo
                    {
                        ItemTag = poly.ItemTag,
                        Flag = poly.Flag,
                        CommandsOffset = (uint)(bw.BaseStream.Position - currentPolyInfoOffset)
                    };

                    // write commands in sets of 4
                    for (int cmdIdx = 0; cmdIdx < poly.Commands.Count; cmdIdx += 4)
                    {
                        var commandSet = new PolygonDisplayCommand[] { poly.Commands[cmdIdx], poly.Commands[cmdIdx + 1], poly.Commands[cmdIdx + 2], poly.Commands[cmdIdx + 3] };
                        foreach (var c in commandSet)
                        {
                            bw.Write((byte)c.OpCode);
                        }
                        foreach (var c in commandSet)
                        {
                            for (int paramIdx = 0; paramIdx < c.OpCode.ParamLength(); paramIdx++)
                            {
                                bw.Write(c.Params[paramIdx]);
                            }
                        }
                    }

                    polyInfo.CommandsLength = (int)(bw.BaseStream.Position - currentPolyInfoOffset - polyInfo.CommandsOffset);
                    polyInfos[i] = polyInfo;
                }

                // remember end offset
                var endOffset = bw.BaseStream.Position;

                // write polyinfo and store offsets in radix dict data
                bw.BaseStream.Position = polyInfoOffset;
                for (int i = 0; i < polyInfos.Length; i++)
                {
                    PolygonInfo polyInfo = polyInfos[i];
                    polygonRadixDict.Data.Add(new OffsetRadixData { Offset = (uint)(bw.BaseStream.Position - initOffset) });
                    polyInfo.WriteTo(bw);
                }

                // write radix dict
                bw.BaseStream.Position = initOffset;
                polygonRadixDict.WriteTo(bw);

                // return to end
                bw.BaseStream.Position = endOffset;
            }
            
            #endregion
        }
    }

    public class RenderCommand
    {
        public RenderOpCode OpCode { get; set; }
        public int Flags { get; set; }
        public byte[] Parameters { get; set; }

        public RenderCommand(BinaryReader br)
        {
            var b = br.ReadByte();

            OpCode = (RenderOpCode)(b & 0xF);
            Flags = b >> 5;
            var paramLength = OpCode.ParamLength(Flags, br);
            if (paramLength != 0)
            {
                Parameters = br.ReadBytes(paramLength);
            }
        }

        public void WriteTo(BinaryWriter bw)
        {
            var b = (int)OpCode | (Flags << 5);
            bw.Write((byte)b);
            if (Parameters != null)
            {
                bw.Write(Parameters);
            }
        }
    }

    public class PolygonDisplayCommand
    {
        public MeshDisplayOpCode OpCode { get; set; }
        public int[] Params { get; set; }
    }

}
