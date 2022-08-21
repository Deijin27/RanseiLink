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
            var initOffset = br.BaseStream.Position;
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

        }

        public class Model
        {
            private struct Header
            {
                public uint TotalSize;
                public uint OffsetBoneCommands;
                public uint OffsetMaterial;
                public uint OffsetMeshes;
                public uint OffsetEvpMatrix;

                public Header(BinaryReader br)
                {
                    TotalSize = br.ReadUInt32();
                    OffsetBoneCommands = br.ReadUInt32();
                    OffsetMaterial = br.ReadUInt32();
                    OffsetMeshes = br.ReadUInt32();
                    OffsetEvpMatrix = br.ReadUInt32();
                }

                public void WriteTo(BinaryWriter bw)
                {
                    bw.Write(TotalSize);
                    bw.Write(OffsetBoneCommands);
                    bw.Write(OffsetMaterial);
                    bw.Write(OffsetMeshes);
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
                    bw.Write(UpScale);
                    bw.Write(DownScale);
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

            

            

            public string Name { get; set; }

            public Model()
            {

            }

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

            public Model(BinaryReader br)
            {
                var initPos = br.BaseStream.Position;
                var header = new Header(br);

                MdlInfo = new ModelInfo(br);

                // nodes
                Bones = new BoneSet(br);

                // bone commands
                if (br.BaseStream.Position != header.OffsetBoneCommands + initPos)
                {
                    throw new Exception("offset wrong");
                }
                RenderCommands = new RenderCommandSet(br);

                // materials
                if (br.BaseStream.Position != header.OffsetMaterial + initPos)
                {
                    //throw new Exception("offset wrong");
                }
                br.BaseStream.Position = initPos + header.OffsetMaterial;
                Materials = new MaterialSet(br);

                // shapes
                if (br.BaseStream.Position != header.OffsetMeshes + initPos)
                {
                    throw new Exception("offset wrong");
                }
                Meshes = new MeshSet(br);

                br.BaseStream.Position = initPos + header.TotalSize;
            }

            public class RenderCommandSet
            {
                public RenderCommandSet(BinaryReader br)
                {
                    RenderCommand command;
                    do
                    {
                        command = new RenderCommand(br);
                        Commands.Add(command);
                    }
                    while (command.OpCode != RenderOpCode.END);
                }

                public List<RenderCommand> Commands { get; set; } = new List<RenderCommand>();

                public void WriteTo(BinaryWriter bw)
                {
                    throw new NotImplementedException();
                }
            }

            public ModelInfo MdlInfo { get; set; }
            public BoneSet Bones { get; set; }
            public RenderCommandSet RenderCommands { get; set; }
            public MaterialSet Materials { get; set; }
            public MeshSet Meshes { get; set; }

            public class BoneSet
            {
                public class NodeData
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
                    public ushort Unknown;

                    public Vector3 Translation;
                    public Vector3[] RotateMatrix;
                    public Vector3 Scale;

                    public Matrix4x4 TRSMatrix;

                    public float A;
                    public float B;

                    static Vector3[] CalcPivotMtx(int select, int neg, float a, float b)
                    {
                        Console.WriteLine("Calculating Pivot Matrix");
                        float o = (neg & 1) == 0 ? 1 : -1;
                        float c = (neg >> 1 & 1) == 0 ? b : -b;
                        float d = (neg >> 2 & 1) == 0 ? a : -a;

                        switch (select)
                        {
                            case 0: return new Vector3[3] 
                            { 
                                new Vector3(o, 0, 0), 
                                new Vector3(0, a, b), 
                                new Vector3(0, c, d) 
                            };
                            case 1: return new Vector3[3] 
                            { 
                                new Vector3(0, o, 0), 
                                new Vector3(a, 0, b), 
                                new Vector3(c, 0, d) 
                            };
                            case 2: return new Vector3[3] 
                            { 
                                new Vector3(0, 0, o), 
                                new Vector3(a, b, 0), 
                                new Vector3(c, d, 0) 
                            };
                            case 3: return new Vector3[3] 
                            { 
                                new Vector3(0, a, b), 
                                new Vector3(o, 0, 0), 
                                new Vector3(0, c, d) 
                            };
                            case 4: return new Vector3[3] 
                            { 
                                new Vector3(a, 0, b), 
                                new Vector3(0, o, 0), 
                                new Vector3(c, 0, d) 
                            };
                            case 5: return new Vector3[3] 
                            { 
                                new Vector3(a, b, 0), 
                                new Vector3(0, 0, o), 
                                new Vector3(c, d, 0) 
                            };
                            case 6: return new Vector3[3] 
                            { 
                                new Vector3(0, a, b), 
                                new Vector3(0, c, d), 
                                new Vector3(o, 0, 0) 
                            };
                            case 7: return new Vector3[3] 
                            { 
                                new Vector3(a, 0, b), 
                                new Vector3(c, 0, d), 
                                new Vector3(0, o, 0) 
                            };
                            case 8: return new Vector3[3] 
                            { 
                                new Vector3(a, b, 0), 
                                new Vector3(c, d, 0), 
                                new Vector3(0, 0, o) 
                            };

                            default: throw new Exception($"Unexepeced select value in {nameof(CalcPivotMtx)}");
                        }
 
                    }

                    static Matrix4x4 Matrix4x4FromMatrix3x3(Vector3[] matrix3x3)
                    {
                        var result = Matrix4x4.Identity;
                        result.M11 = matrix3x3[0].X;
                        result.M12 = matrix3x3[0].Y;
                        result.M13 = matrix3x3[0].Z;
                        result.M21 = matrix3x3[1].X;
                        result.M22 = matrix3x3[1].Y;
                        result.M23 = matrix3x3[1].Z;
                        result.M31 = matrix3x3[2].X;
                        result.M32 = matrix3x3[2].Y;
                        result.M33 = matrix3x3[2].Z;
                        return result;
                    }
                    public NodeData(BinaryReader br)
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
                            hasRotate = true;
                            var a = FixedPoint.Fix_1_3_12(br.ReadUInt16());
                            var b = FixedPoint.Fix_1_3_12(br.ReadUInt16());
                            var select = f >> 4 & 0b1111;
                            var neg = f >> 8 & 0b1111;
                            RotateMatrix = CalcPivotMtx(select, neg, a, b);
                        }
                        else if (Flag.HasFlag(TransFlag.Rotate))
                        {
                            hasRotate = true;
                            RotateMatrix = new Vector3[3]
                            {
                                new Vector3(
                                    FixedPoint.Fix_1_3_12(m0),
                                    FixedPoint.Fix_1_3_12(br.ReadUInt16()),
                                    FixedPoint.Fix_1_3_12(br.ReadUInt16())
                                    ),
                                new Vector3(
                                    FixedPoint.Fix_1_3_12(br.ReadUInt16()),
                                    FixedPoint.Fix_1_3_12(br.ReadUInt16()),
                                    FixedPoint.Fix_1_3_12(br.ReadUInt16())
                                    ),
                                new Vector3(
                                    FixedPoint.Fix_1_3_12(br.ReadUInt16()),
                                    FixedPoint.Fix_1_3_12(br.ReadUInt16()),
                                    FixedPoint.Fix_1_3_12(br.ReadUInt16())
                                    )
                            };
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
                            TRSMatrix = Matrix4x4FromMatrix3x3(RotateMatrix) * TRSMatrix;
                        }
                        if (Flag.HasFlag(TransFlag.Translate))
                        {
                            TRSMatrix = Matrix4x4.CreateTranslation(Translation) * TRSMatrix;
                        }
                    }

                    public void WriteTo(BinaryWriter bw)
                    {
                        throw new NotImplementedException("Requires implementation of inverse fixed point");
                        //bw.Write(~(ushort)Flag);
                        //bw.Write(Unknown);

                        //if (Flag.HasFlag(TransFlag.Translate))
                        //{
                        //    bw.Write(Translate.X);
                        //    bw.Write(Ty);
                        //    bw.Write(Tz);
                        //}
                        //if (Flag.HasFlag(TransFlag.Rotate))
                        //{
                        //    if (Flag.HasFlag(TransFlag.Pivot))
                        //    {
                        //        bw.Write(Rot1);
                        //        bw.Write(Rot2);
                        //        bw.Write(Rot3);
                        //        bw.Write(Rot4);
                        //        bw.Write(Rot5);
                        //        bw.Write(Rot6);
                        //        bw.Write(Rot7);
                        //        bw.Write(Rot8);
                        //    }
                        //    else
                        //    {
                        //        bw.Write(A);
                        //        bw.Write(B);
                        //    }
                        //}
                        //if (Flag.HasFlag(TransFlag.Scale))
                        //{
                        //    bw.Write(Sx);
                        //    bw.Write(Sy);
                        //    bw.Write(Sz);
                        //    bw.Write(InvSx);
                        //    bw.Write(InvSy);
                        //    bw.Write(InvSz);
                        //}

                    }
                }

                public BoneSet(BinaryReader br)
                {
                    var nodeRadixDict = new RadixDict<OffsetRadixData>(br);
                    Nodes = new NodeData[nodeRadixDict.Names.Count];
                    for (int i = 0; i < nodeRadixDict.Names.Count; i++)
                    {
                        var d = new NodeData(br);
                        d.Name = nodeRadixDict.Names[i];
                        Nodes[i] = d;
                    }
                }

                public NodeData[] Nodes { get; set; }

                public void WriteTo(BinaryWriter bw)
                {
                    throw new NotImplementedException();
                }
            }

            public class MaterialSet
            {
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
                    public ushort Size;
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
                        Size = br.ReadUInt16();
                        var diffAmb = br.ReadUInt32();
                        Diffuse = Rgb15.From((ushort)(diffAmb & 0x7FFF));
                        DiffuseIsDefaultVertexColor = (diffAmb >> 15 & 1) == 1;
                        Ambient = Rgb15.From((ushort)(diffAmb >> 16 & 0x7FFF));
                        var specEmi = br.ReadUInt32();
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

                    public string Name { get; set; }
                    public string Texture { get; set; }
                    public string Palette { get; set; }
                }

                public MaterialSet(BinaryReader br)
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

                    // this padding is seen in map5
                    if ((2 * materialRadixDict.Names.Count) % 4 != 0)
                    {
                        br.Skip(4 - (2 * materialRadixDict.Names.Count) % 4);
                    }

                    Materials = new Material[materialRadixDict.Names.Count];
                    for (int i = 0; i < materialRadixDict.Names.Count; i++)
                    {
						var m = new Material(br);
						m.Texture = materialTextures[i];
						m.Palette = materialPalettes[i];
						m.Name = materialRadixDict.Names[i];
                        Materials[i] = m;
                    }
                }

                public Material[] Materials { get; set; }

                public void WriteTo(BinaryWriter bw)
                {
                    throw new NotImplementedException();
                }
            }

            public class MeshSet
            {
                private struct Mesh
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
                    public uint CommandsOffset;
                    public int CommandsLength;

                    public Mesh(BinaryReader br)
                    {
                        ItemTag = br.ReadUInt16();
                        Size = br.ReadUInt16();
                        Flag = (SHPFLAG)br.ReadUInt32();
                        CommandsOffset = br.ReadUInt32();
                        CommandsLength = br.ReadInt32();
                    }

                    public void WriteTo(BinaryWriter bw)
                    {
                        bw.Write(ItemTag);
                        bw.Write(Size);
                        bw.Write((uint)Flag);
                        bw.Write(CommandsOffset);
                        bw.Write(CommandsLength);
                    }
                }

                public MeshSet(BinaryReader br)
                {
                    var shapeRadixDict = new RadixDict<OffsetRadixData>(br);
                    Mesh[] shapes = new Mesh[shapeRadixDict.Names.Count];
                    for (int i = 0; i < shapes.Length; i++)
                    {
                        shapes[i] = new Mesh(br);
                    }

                    for (int i = 0; i < shapes.Length; i++)
                    {
                        var endPos = br.BaseStream.Position + shapes[i].CommandsLength;
                        var commandSet = new MeshCommands { Name = shapeRadixDict.Names[i] };
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
                                commandSet.Commands.Add(new MeshDisplayCommand { OpCode = id, Params = prms });
                            }
                        }
                        br.BaseStream.Position = endPos; // ensure end pos
                        MeshCommandList.Add(commandSet);
                    }
                }
                public class MeshCommands
                {
                    public string Name { get; set; }
                    public List<MeshDisplayCommand> Commands { get; set; } = new List<MeshDisplayCommand>();
                }

                public List<MeshCommands> MeshCommandList { get; set; } = new List<MeshCommands>();

                
            }
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
    }

    public class MeshDisplayCommand
    {
        public MeshDisplayOpCode OpCode { get; set; }
        public int[] Params { get; set; }
    }

}
