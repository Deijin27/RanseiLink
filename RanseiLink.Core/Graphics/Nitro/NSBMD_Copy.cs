// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: C:\Users\Mia\Desktop\MKDSCMLast\Last\MKDS Course Modifier.exe
/*
using MKDS_Course_Modifier._3D_Formats;
using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.Misc;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using Tao.OpenGl;

namespace RanseiLink.Core.Graphics
{

    public class NSBMD
    {
        public const string Signature = "BMD0";
        public FileHeader Header;
        public NSBMD.ModelSet modelSet;
        public NSBTX.TexplttSet TexPlttSet;

        public NSBMD(byte[] file)
        {
            EndianBinaryReader er = new EndianBinaryReader((Stream)new MemoryStream(file), Endianness.LittleEndian);
            bool OK;
            this.Header = new FileHeader(er, "BMD0", out OK);
            if (!OK)
            {
                int num1 = (int)System.Windows.Forms.MessageBox.Show("Error 0");
            }
            else
            {
                er.BaseStream.Position = (long)this.Header[0];
                this.modelSet = new NSBMD.ModelSet(er, out OK);
                if (!OK)
                {
                    int num2 = (int)System.Windows.Forms.MessageBox.Show("Error 1");
                }
                else if (this.Header.info.dataBlocks > (ushort)1)
                {
                    er.BaseStream.Position = (long)this.Header[1];
                    er.SetMarkerOnCurrentOffset("TexplttSet");
                    this.TexPlttSet = new NSBTX.TexplttSet(er, out OK);
                    if (!OK)
                    {
                        int num3 = (int)System.Windows.Forms.MessageBox.Show("Error 2");
                    }
                }
            }
            er.ClearMarkers();
            er.Close();
        }

        public NSBMD(bool Textures)
        {
            this.Header = new FileHeader("BMD0", Textures ? (ushort)2 : (ushort)1);
            this.Header.info.version = (ushort)2;
        }

        public byte[] Write()
        {
            MemoryStream baseStream = new MemoryStream();
            EndianBinaryWriter er = new EndianBinaryWriter((Stream)baseStream, Endianness.LittleEndian);
            this.Header.info.dataBlocks = this.TexPlttSet != null ? (ushort)2 : (ushort)1;
            this.Header.Write(er);
            long position1 = er.BaseStream.Position;
            er.BaseStream.Position = 16L;
            er.Write((uint)position1);
            er.BaseStream.Position = position1;
            this.modelSet.Write(er);
            if (this.TexPlttSet != null)
            {
                long position2 = er.BaseStream.Position;
                er.BaseStream.Position = 20L;
                er.Write((uint)position2);
                er.BaseStream.Position = position2;
                this.TexPlttSet.Write(er);
            }
            er.BaseStream.Position = 8L;
            er.Write((uint)er.BaseStream.Length);
            byte[] array = baseStream.ToArray();
            er.Close();
            return array;
        }

        public class ModelSet
        {
            public const string Signature = "MDL0";
            public DataBlockHeader header;
            public Dictionary<NSBMD.ModelSet.MDL0Data> dict;
            public NSBMD.ModelSet.Model[] models;

            public ModelSet(EndianBinaryReader er, out bool OK)
            {
                er.SetMarkerOnCurrentOffset(nameof(ModelSet));
                bool OK1;
                this.header = new DataBlockHeader(er, "MDL0", out OK1);
                if (!OK1)
                {
                    OK = false;
                }
                else
                {
                    this.dict = new Dictionary<NSBMD.ModelSet.MDL0Data>(er);
                    this.models = new NSBMD.ModelSet.Model[(int)this.dict.numEntry];
                    long position = er.BaseStream.Position;
                    for (int i = 0; i < (int)this.dict.numEntry; ++i)
                    {
                        er.BaseStream.Position = (long)this.dict[i].Value.Offset + er.GetMarker(nameof(ModelSet));
                        this.models[i] = new NSBMD.ModelSet.Model(er);
                    }
                    OK = true;
                }
            }

            public ModelSet()
            {
                this.header = new DataBlockHeader("MDL0", 0U);
                this.dict = new Dictionary<NSBMD.ModelSet.MDL0Data>();
            }

            public void Write(EndianBinaryWriter er)
            {
                long position1 = er.BaseStream.Position;
                this.header.Write(er, 0);
                this.dict.Write(er);
                for (int i = 0; i < this.models.Length; ++i)
                {
                    this.dict[i].Value.Offset = (uint)(er.BaseStream.Position - position1);
                    this.models[i].Write(er);
                }
                long position2 = er.BaseStream.Position;
                er.BaseStream.Position = position1 + 4L;
                er.Write((uint)(position2 - position1));
                this.dict.Write(er);
                er.BaseStream.Position = position2;
            }

            public class MDL0Data : DictionaryData
            {
                public uint Offset;

                public override ushort GetDataSize() => 4;

                public override void Read(EndianBinaryReader er) => this.Offset = er.ReadUInt32();

                public override void Write(EndianBinaryWriter er) => er.Write(this.Offset);
            }

            public class Model
            {
                public uint size;
                public uint ofsSbc;
                public uint ofsMat;
                public uint ofsShp;
                public uint ofsEvpMtx;
                public NSBMD.ModelSet.Model.ModelInfo info;
                public NSBMD.ModelSet.Model.NodeSet nodes;
                public byte[] sbc;
                public NSBMD.ModelSet.Model.MaterialSet materials;
                public NSBMD.ModelSet.Model.ShapeSet shapes;
                public NSBMD.ModelSet.Model.EvpMatrices evpMatrices;

                public Model(EndianBinaryReader er)
                {
                    er.SetMarkerOnCurrentOffset(nameof(Model));
                    this.size = er.ReadUInt32();
                    this.ofsSbc = er.ReadUInt32();
                    this.ofsMat = er.ReadUInt32();
                    this.ofsShp = er.ReadUInt32();
                    this.ofsEvpMtx = er.ReadUInt32();
                    this.info = new NSBMD.ModelSet.Model.ModelInfo(er);
                    this.nodes = new NSBMD.ModelSet.Model.NodeSet(er);
                    long position1 = er.BaseStream.Position;
                    er.BaseStream.Position = (long)this.ofsSbc + er.GetMarker(nameof(Model));
                    this.sbc = er.ReadBytes((int)this.ofsMat - (int)this.ofsSbc);
                    er.BaseStream.Position = position1;
                    er.BaseStream.Position = (long)this.ofsMat + er.GetMarker(nameof(Model));
                    this.materials = new NSBMD.ModelSet.Model.MaterialSet(er);
                    er.BaseStream.Position = (long)this.ofsShp + er.GetMarker(nameof(Model));
                    this.shapes = new NSBMD.ModelSet.Model.ShapeSet(er);
                    if ((int)this.ofsEvpMtx != (int)this.size && this.ofsEvpMtx != 0U)
                    {
                        er.BaseStream.Position = (long)this.ofsEvpMtx + er.GetMarker(nameof(Model));
                        this.evpMatrices = new NSBMD.ModelSet.Model.EvpMatrices(er, (int)this.nodes.dict.numEntry);
                    }
                    long marker = er.GetMarker(nameof(ModelSet));
                    er.ClearMarkers();
                    long position2 = er.BaseStream.Position;
                    er.BaseStream.Position = marker;
                    er.SetMarkerOnCurrentOffset(nameof(ModelSet));
                    er.BaseStream.Position = position2;
                }

                public Model()
                {
                }

                public void Write(EndianBinaryWriter er)
                {
                    long position1 = er.BaseStream.Position;
                    er.Write(0U);
                    er.Write(0U);
                    er.Write(0U);
                    er.Write(0U);
                    er.Write(0U);
                    this.info.Write(er);
                    this.nodes.Write(er);
                    long position2 = er.BaseStream.Position;
                    er.BaseStream.Position = position1 + 4L;
                    er.Write((uint)(position2 - position1));
                    er.BaseStream.Position = position2;
                    er.Write(this.sbc, 0, this.sbc.Length);
                    long position3 = er.BaseStream.Position;
                    er.BaseStream.Position = position1 + 8L;
                    er.Write((uint)(position3 - position1));
                    er.BaseStream.Position = position3;
                    this.materials.Write(er);
                    long position4 = er.BaseStream.Position;
                    er.BaseStream.Position = position1 + 12L;
                    er.Write((uint)(position4 - position1));
                    er.BaseStream.Position = position4;
                    this.shapes.Write(er);
                    if (this.evpMatrices != null)
                    {
                        long position5 = er.BaseStream.Position;
                        er.BaseStream.Position = position1 + 16L;
                        er.Write((uint)(position5 - position1));
                        er.BaseStream.Position = position5;
                        this.evpMatrices.Write(er);
                    }
                    else
                    {
                        long position6 = er.BaseStream.Position;
                        er.BaseStream.Position = position1 + 16L;
                        er.Write((uint)(position6 - position1));
                        er.BaseStream.Position = position6;
                    }
                    long position7 = er.BaseStream.Position;
                    er.BaseStream.Position = position1;
                    er.Write((uint)(position7 - position1));
                    er.BaseStream.Position = position7;
                }

                public void ProcessSbc(
                  float X,
                  float Y,
                  float dist,
                  float elev,
                  float ang,
                  bool picking = false,
                  int texoffset = 1)
                {
                    this.ProcessSbc((NSBTX.TexplttSet)null, (NSBCA)null, 0, 0, (NSBTA)null, 0, 0, (NSBTP)null, 0, 0, (NSBMA)null, 0, 0, (NSBVA)null, 0, 0, X, Y, dist, elev, ang, picking, texoffset);
                }

                public void ProcessSbc(
                  NSBTX.TexplttSet Textures,
                  NSBCA Bca,
                  int BcaAnmNumber,
                  int BcaFrameNumber,
                  NSBTA Bta,
                  int BtaAnmNumber,
                  int BtaFrameNumber,
                  NSBTP Btp,
                  int BtpAnmNumber,
                  int BtpFrameNumber,
                  NSBMA Bma,
                  int BmaAnmNumber,
                  int BmaFrameNumber,
                  NSBVA Bva,
                  int BvaAnmNumber,
                  int BvaFrameNumber,
                  float X,
                  float Y,
                  float dist,
                  float elev,
                  float ang,
                  bool picking = false,
                  int texoffset = 1)
                {
                    int num1 = 0;
                    while (true)
                    {
                        do
                        {
                            bool flag1 = Bca != null && BcaAnmNumber >= 0;
                            bool flag2 = Bta != null && BtaAnmNumber >= 0;
                            bool flag3 = Btp != null && BtpAnmNumber >= 0;
                            bool flag4 = Bma != null && BmaAnmNumber >= 0;
                            bool flag5 = Bva != null && BvaAnmNumber >= 0;
                            Gl.glMatrixMode(5888);
                            GlNitro.Nitro3DContext Context = new GlNitro.Nitro3DContext();
                            int offset1 = 0;
                            int posScale = (int)this.info.posScale;
                            MTX44 b = new MTX44();
                            bool flag6 = true;
                            bool flag7 = true;
                            while (offset1 != this.sbc.Length)
                            {
                                byte[] sbc1 = this.sbc;
                                int index1 = offset1++;
                                byte num2;
                                byte num3;
                                byte num4;
                                switch ((int)(num2 = sbc1[index1]) & 15)
                                {
                                    case 1:
                                        goto label_104;
                                    case 2:
                                        byte[] sbc2 = this.sbc;
                                        int index2 = offset1;
                                        int num5 = index2 + 1;
                                        byte NodeID = sbc2[index2];
                                        if (!flag5)
                                        {
                                            byte[] sbc3 = this.sbc;
                                            int index3 = num5;
                                            offset1 = index3 + 1;
                                            flag6 = sbc3[index3] == (byte)1;
                                            break;
                                        }
                                        offset1 = num5 + 1;
                                        flag6 = Bva.visAnmSet.visAnm[BvaAnmNumber].GetFrame(BvaFrameNumber, (int)NodeID);
                                        break;
                                    case 3:
                                        if (flag6)
                                        {
                                            b = Context.MatrixStack[(int)this.sbc[offset1++]].Clone();
                                            break;
                                        }
                                        ++offset1;
                                        break;
                                    case 4:
                                        if (flag6 && !picking)
                                        {
                                            byte i1 = this.sbc[offset1++];
                                            flag7 = true;
                                            if (((int)this.materials.materials[(int)i1].texImageParam & (int)ushort.MaxValue) != 0)
                                            {
                                                int num6 = (int)System.Windows.MessageBox.Show("Texoffset is not 0!!!");
                                            }
                                            if (this.materials.materials[(int)i1].Fmt != Graphic.GXTexFmt.GX_TEXFMT_NONE)
                                            {
                                                if ((this.materials.materials[(int)i1].Fmt == Graphic.GXTexFmt.GX_TEXFMT_A3I5 || this.materials.materials[(int)i1].Fmt == Graphic.GXTexFmt.GX_TEXFMT_A5I3 || ((int)(this.materials.materials[(int)i1].polyAttr >> 16) & 31) != 31) && num1 != 1)
                                                    flag7 = false;
                                                if ((this.materials.materials[(int)i1].Fmt == Graphic.GXTexFmt.GX_TEXFMT_NONE || this.materials.materials[(int)i1].Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT4 || this.materials.materials[(int)i1].Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 || this.materials.materials[(int)i1].Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT256 || this.materials.materials[(int)i1].Fmt == Graphic.GXTexFmt.GX_TEXFMT_COMP4x4 || this.materials.materials[(int)i1].Fmt == Graphic.GXTexFmt.GX_TEXFMT_DIRECT) && ((int)(this.materials.materials[(int)i1].polyAttr >> 16) & 31) == 31 && num1 != 0)
                                                    flag7 = false;
                                            }
                                            else
                                                flag7 = true;
                                            if (flag7)
                                            {
                                                KeyValuePair<string, NSBMD.ModelSet.Model.MaterialSet.MaterialSetData> keyValuePair;
                                                int num7;
                                                if (flag3)
                                                {
                                                    Dictionary<NSBTP.TexPatAnmSet.TexPatAnm.DictTexPatAnmData> dict = Btp.texPatAnmSet.texPatAnm[BtpAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key = keyValuePair.Key;
                                                    if (dict.Contains(key))
                                                    {
                                                        num7 = Textures == null ? 1 : 0;
                                                        goto label_23;
                                                    }
                                                }
                                                num7 = 1;
                                            label_23:
                                                if (num7 == 0)
                                                {
                                                    Dictionary<NSBTP.TexPatAnmSet.TexPatAnm.DictTexPatAnmData> dict = Btp.texPatAnmSet.texPatAnm[BtpAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key = keyValuePair.Key;
                                                    int TexIdx;
                                                    int PlttIdx;
                                                    dict[key].GetData(out TexIdx, out PlttIdx, BtpFrameNumber);
                                                    string i2 = (string)Btp.texPatAnmSet.texPatAnm[BtpAnmNumber].texName[TexIdx];
                                                    string i3 = (string)Btp.texPatAnmSet.texPatAnm[BtpAnmNumber].plttName[PlttIdx];
                                                    GlNitro.glNitroTexImage2D(Textures.dictTex[i2].ToBitmap(Textures.dictPltt[i3]), this.materials.materials[(int)i1], (int)i1 + texoffset);
                                                }
                                                Gl.glBindTexture(3553, (int)i1 + texoffset);
                                                Gl.glMatrixMode(5890);
                                                Gl.glLoadIdentity();
                                                Gl.glScalef(1f / (float)this.materials.materials[(int)i1].origWidth, 1f / (float)this.materials.materials[(int)i1].origHeight, 1f);
                                                int num8;
                                                if (flag2)
                                                {
                                                    NSBTA.TexSRTAnmSet.TexSRTAnm texSrtAnm = Bta.texSRTAnmSet.texSRTAnm[BtaAnmNumber];
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key = keyValuePair.Key;
                                                    num8 = texSrtAnm.Contains(key) != -1 ? 1 : 0;
                                                }
                                                else
                                                    num8 = 0;
                                                if (num8 == 0)
                                                {
                                                    Gl.glMultMatrixf(this.materials.materials[(int)i1].GetMatrix());
                                                }
                                                else
                                                {
                                                    Dictionary<NSBTA.TexSRTAnmSet.TexSRTAnm.TexSRTAnmData> dict = Bta.texSRTAnmSet.texSRTAnm[BtaAnmNumber].dict;
                                                    NSBTA.TexSRTAnmSet.TexSRTAnm texSrtAnm = Bta.texSRTAnmSet.texSRTAnm[BtaAnmNumber];
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key = keyValuePair.Key;
                                                    int i4 = texSrtAnm.Contains(key);
                                                    Gl.glMultMatrixf(dict[i4].Value.GetMatrix(BtaFrameNumber, (int)this.materials.materials[(int)i1].origWidth, (int)this.materials.materials[(int)i1].origHeight));
                                                }
                                                Context.LightEnabled[0] = ((int)this.materials.materials[(int)i1].polyAttr & 1) == 1;
                                                Context.LightEnabled[1] = ((int)(this.materials.materials[(int)i1].polyAttr >> 1) & 1) == 1;
                                                Context.LightEnabled[2] = ((int)(this.materials.materials[(int)i1].polyAttr >> 2) & 1) == 1;
                                                Context.LightEnabled[3] = ((int)(this.materials.materials[(int)i1].polyAttr >> 3) & 1) == 1;
                                                int num9;
                                                if (flag4)
                                                {
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key = keyValuePair.Key;
                                                    num9 = dict.Contains(key) ? 1 : 0;
                                                }
                                                else
                                                    num9 = 0;
                                                Color color1;
                                                if (num9 == 0)
                                                {
                                                    color1 = (this.materials.materials[(int)i1].flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_DIFFUSE) != NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_DIFFUSE ? Color.Black : Graphic.ConvertABGR1555((short)((int)this.materials.materials[(int)i1].diffAmb & (int)short.MaxValue));
                                                    Color color2 = (this.materials.materials[(int)i1].flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_AMBIENT) != NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_AMBIENT ? Color.FromArgb(160, 160, 160) : Graphic.ConvertABGR1555((short)((int)(this.materials.materials[(int)i1].diffAmb >> 16) & (int)short.MaxValue));
                                                    Context.DiffuseColor = color1;
                                                    Context.AmbientColor = color2;
                                                    Color color3 = (this.materials.materials[(int)i1].flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_SPECULAR) != NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_SPECULAR ? Color.Black : Graphic.ConvertABGR1555((short)((int)this.materials.materials[(int)i1].specEmi & (int)short.MaxValue));
                                                    Color color4 = (this.materials.materials[(int)i1].flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_EMISSION) != NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_EMISSION ? Color.Black : Graphic.ConvertABGR1555((short)((int)(this.materials.materials[(int)i1].specEmi >> 16) & (int)short.MaxValue));
                                                    Context.SpecularColor = color3;
                                                    Context.EmissionColor = color4;
                                                }
                                                else
                                                {
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict1 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key1 = keyValuePair.Key;
                                                    NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData dictMatColAnmData1 = dict1[key1];
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict2 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key2 = keyValuePair.Key;
                                                    int tagDiffuse = (int)dict2[key2].tagDiffuse;
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict3 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key3 = keyValuePair.Key;
                                                    int constDiffuse = (int)dict3[key3].ConstDiffuse;
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict4 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key4 = keyValuePair.Key;
                                                    ushort[] diffuse = dict4[key4].Diffuse;
                                                    int Frame1 = BmaFrameNumber;
                                                    color1 = Graphic.ConvertABGR1555((short)dictMatColAnmData1.GetValue((uint)tagDiffuse, (ushort)constDiffuse, diffuse, Frame1));
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict5 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key5 = keyValuePair.Key;
                                                    NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData dictMatColAnmData2 = dict5[key5];
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict6 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key6 = keyValuePair.Key;
                                                    int tagAmbient = (int)dict6[key6].tagAmbient;
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict7 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key7 = keyValuePair.Key;
                                                    int constAmbient = (int)dict7[key7].ConstAmbient;
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict8 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key8 = keyValuePair.Key;
                                                    ushort[] ambient = dict8[key8].Ambient;
                                                    int Frame2 = BmaFrameNumber;
                                                    Color color5 = Graphic.ConvertABGR1555((short)dictMatColAnmData2.GetValue((uint)tagAmbient, (ushort)constAmbient, ambient, Frame2));
                                                    Context.DiffuseColor = color1;
                                                    Context.AmbientColor = color5;
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict9 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key9 = keyValuePair.Key;
                                                    NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData dictMatColAnmData3 = dict9[key9];
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict10 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key10 = keyValuePair.Key;
                                                    int tagSpecular = (int)dict10[key10].tagSpecular;
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict11 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key11 = keyValuePair.Key;
                                                    int constSpecular = (int)dict11[key11].ConstSpecular;
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict12 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key12 = keyValuePair.Key;
                                                    ushort[] specular = dict12[key12].Specular;
                                                    int Frame3 = BmaFrameNumber;
                                                    Color color6 = Graphic.ConvertABGR1555((short)dictMatColAnmData3.GetValue((uint)tagSpecular, (ushort)constSpecular, specular, Frame3));
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict13 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key13 = keyValuePair.Key;
                                                    NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData dictMatColAnmData4 = dict13[key13];
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict14 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key14 = keyValuePair.Key;
                                                    int tagEmission = (int)dict14[key14].tagEmission;
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict15 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key15 = keyValuePair.Key;
                                                    int constEmission = (int)dict15[key15].ConstEmission;
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict16 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key16 = keyValuePair.Key;
                                                    ushort[] emission = dict16[key16].Emission;
                                                    int Frame4 = BmaFrameNumber;
                                                    Color color7 = Graphic.ConvertABGR1555((short)dictMatColAnmData4.GetValue((uint)tagEmission, (ushort)constEmission, emission, Frame4));
                                                    Context.SpecularColor = color6;
                                                    Context.EmissionColor = color7;
                                                }
                                                switch (this.materials.materials[(int)i1].polyAttr >> 14 & 1U)
                                                {
                                                    case 0:
                                                        Gl.glDepthFunc(513);
                                                        break;
                                                    case 1:
                                                        int num10 = (int)System.Windows.MessageBox.Show("EQUALS!");
                                                        Gl.glDepthFunc(514);
                                                        break;
                                                }
                                                int num11 = -1;
                                                switch (this.materials.materials[(int)i1].polyAttr >> 4 & 3U)
                                                {
                                                    case 0:
                                                        num11 = 8448;
                                                        break;
                                                    case 1:
                                                        num11 = 8449;
                                                        break;
                                                    case 2:
                                                        num11 = 8448;
                                                        break;
                                                    case 3:
                                                        int num12 = (int)System.Windows.MessageBox.Show("SHADOW!");
                                                        num11 = 8448;
                                                        break;
                                                }
                                                Gl.glTexEnvi(8960, 8704, num11);
                                                int num13;
                                                if (flag4)
                                                {
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key = keyValuePair.Key;
                                                    num13 = dict.Contains(key) ? 1 : 0;
                                                }
                                                else
                                                    num13 = 0;
                                                if (num13 == 0)
                                                {
                                                    Context.Alpha = (int)(this.materials.materials[(int)i1].polyAttr >> 16) & 31;
                                                }
                                                else
                                                {
                                                    GlNitro.Nitro3DContext nitro3Dcontext = Context;
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict17 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key17 = keyValuePair.Key;
                                                    NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData dictMatColAnmData = dict17[key17];
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict18 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key18 = keyValuePair.Key;
                                                    int tagPolygonAlpha = (int)dict18[key18].tagPolygonAlpha;
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict19 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key19 = keyValuePair.Key;
                                                    int constPolygonAlpha = (int)dict19[key19].ConstPolygonAlpha;
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict20 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key20 = keyValuePair.Key;
                                                    byte[] polygonAlpha = dict20[key20].PolygonAlpha;
                                                    int Frame = BmaFrameNumber;
                                                    int num14 = (int)dictMatColAnmData.GetValue((uint)tagPolygonAlpha, (byte)constPolygonAlpha, polygonAlpha, Frame);
                                                    nitro3Dcontext.Alpha = num14;
                                                }
                                                int num15;
                                                if (flag4)
                                                {
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key = keyValuePair.Key;
                                                    num15 = dict.Contains(key) ? 1 : 0;
                                                }
                                                else
                                                    num15 = 0;
                                                if (num15 == 0)
                                                {
                                                    if (((int)(this.materials.materials[(int)i1].diffAmb >> 15) & 1) == 1 && (this.materials.materials[(int)i1].flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_VTXCOLOR) == NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_VTXCOLOR)
                                                    {
                                                        color1 = Graphic.ConvertABGR1555((short)((int)this.materials.materials[(int)i1].diffAmb & (int)short.MaxValue));
                                                        Gl.glColor4f((float)color1.R / (float)byte.MaxValue, (float)color1.G / (float)byte.MaxValue, (float)color1.B / (float)byte.MaxValue, (float)Context.Alpha / 31f);
                                                    }
                                                    else
                                                        Gl.glColor4f(0.0f, 0.0f, 0.0f, (float)Context.Alpha / 31f);
                                                }
                                                else if (((int)(this.materials.materials[(int)i1].diffAmb >> 15) & 1) == 1)
                                                {
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict21 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key21 = keyValuePair.Key;
                                                    NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData dictMatColAnmData = dict21[key21];
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict22 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key22 = keyValuePair.Key;
                                                    int tagDiffuse = (int)dict22[key22].tagDiffuse;
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict23 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key23 = keyValuePair.Key;
                                                    int constDiffuse = (int)dict23[key23].ConstDiffuse;
                                                    Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict24 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                                                    keyValuePair = this.materials.dict[(int)i1];
                                                    string key24 = keyValuePair.Key;
                                                    ushort[] diffuse = dict24[key24].Diffuse;
                                                    int Frame = BmaFrameNumber;
                                                    color1 = Graphic.ConvertABGR1555((short)dictMatColAnmData.GetValue((uint)tagDiffuse, (ushort)constDiffuse, diffuse, Frame));
                                                    Gl.glColor4f((float)color1.R / (float)byte.MaxValue, (float)color1.G / (float)byte.MaxValue, (float)color1.B / (float)byte.MaxValue, (float)Context.Alpha / 31f);
                                                }
                                                else
                                                    Gl.glColor4f(0.0f, 0.0f, 0.0f, (float)Context.Alpha / 31f);
                                                Context.UseSpecularReflectionTable = ((int)(this.materials.materials[(int)i1].specEmi >> 15) & 1) == 1;
                                                int mode = -1;
                                                switch (this.materials.materials[(int)i1].polyAttr >> 6 & 3U)
                                                {
                                                    case 0:
                                                        mode = 1032;
                                                        break;
                                                    case 1:
                                                        mode = 1028;
                                                        break;
                                                    case 2:
                                                        mode = 1029;
                                                        break;
                                                    case 3:
                                                        mode = 0;
                                                        break;
                                                }
                                                Gl.glCullFace(mode);
                                                Gl.glMatrixMode(5888);
                                                Gl.glDisable(3168);
                                                Gl.glDisable(3169);
                                                break;
                                            }
                                            break;
                                        }
                                        ++offset1;
                                        break;
                                    case 5:
                                        if (flag6 && flag7)
                                        {
                                            GlNitro.glNitroGx(this.shapes.shape[(int)this.sbc[offset1++]].DL, b.Clone(), ref Context, posScale, picking);
                                            break;
                                        }
                                        ++offset1;
                                        break;
                                    case 6:
                                        byte[] sbc4 = this.sbc;
                                        int index4 = offset1;
                                        int num16 = index4 + 1;
                                        byte index5 = sbc4[index4];
                                        byte[] sbc5 = this.sbc;
                                        int index6 = num16;
                                        int num17 = index6 + 1;
                                        byte num18 = sbc5[index6];
                                        byte[] sbc6 = this.sbc;
                                        int index7 = num17;
                                        offset1 = index7 + 1;
                                        byte num19 = sbc6[index7];
                                        bool flag8 = ((int)num19 & 1) == 1;
                                        bool flag9 = ((int)num19 >> 1 & 1) == 1;
                                        bool MayaScale = flag8;
                                        int index8 = ((int)num2 >> 5 & 1) == 1 ? (int)this.sbc[offset1++] : -1;
                                        int index9 = ((int)num2 >> 6 & 1) == 1 ? (int)this.sbc[offset1++] : -1;
                                        if (index9 != -1)
                                            b = Context.MatrixStack[index9];
                                        if (!flag1)
                                        {
                                            b = b.MultMatrix((MTX44)this.nodes.data[(int)index5].GetMatrix(MayaScale, posScale));
                                        }
                                        else
                                        {
                                            try
                                            {
                                                b = b.MultMatrix((MTX44)Bca.jntAnmSet.jntAnm[BcaAnmNumber].tagData[(int)index5].GetMatrix(BcaFrameNumber, MayaScale, posScale, this.nodes.data[(int)index5]));
                                            }
                                            catch
                                            {
                                                b = b.MultMatrix((MTX44)this.nodes.data[(int)index5].GetMatrix(MayaScale, posScale));
                                            }
                                        }
                                        if (index8 != -1)
                                        {
                                            Context.MatrixStack[index8] = b;
                                            break;
                                        }
                                        break;
                                    case 7:
                                        Gl.glCullFace(0);
                                        num3 = this.sbc[offset1++];
                                        int index10 = ((int)num2 >> 5 & 1) == 1 ? (int)this.sbc[offset1++] : -1;
                                        int index11 = ((int)num2 >> 6 & 1) == 1 ? (int)this.sbc[offset1++] : -1;
                                        if (index11 != -1)
                                            b = Context.MatrixStack[index11];
                                        float[] params1 = new float[16];
                                        Gl.glGetFloatv(2982, params1);
                                        b[0, 0] = params1[0];
                                        b[0, 1] = params1[1];
                                        b[0, 2] = params1[2];
                                        b[1, 0] = params1[4];
                                        b[1, 1] = params1[5];
                                        b[1, 2] = params1[6];
                                        b[2, 0] = params1[8];
                                        b[2, 1] = params1[9];
                                        b[2, 2] = params1[10];
                                        if (index10 != -1)
                                        {
                                            Context.MatrixStack[index10] = b;
                                            break;
                                        }
                                        break;
                                    case 8:
                                        Gl.glCullFace(0);
                                        num3 = this.sbc[offset1++];
                                        int index12 = ((int)num2 >> 5 & 1) == 1 ? (int)this.sbc[offset1++] : -1;
                                        int index13 = ((int)num2 >> 6 & 1) == 1 ? (int)this.sbc[offset1++] : -1;
                                        if (index13 != -1)
                                            b = Context.MatrixStack[index13];
                                        float[] params2 = new float[16];
                                        Gl.glGetFloatv(2982, params2);
                                        b[0, 0] = params2[0];
                                        b[0, 2] = params2[2];
                                        b[1, 0] = params2[4];
                                        b[1, 1] = params2[5];
                                        b[1, 2] = params2[6];
                                        b[2, 0] = params2[8];
                                        b[2, 2] = params2[10];
                                        if (index12 != -1)
                                        {
                                            Context.MatrixStack[index12] = b;
                                            break;
                                        }
                                        break;
                                    case 9:
                                        MTX44 mtX44_1 = new MTX44();
                                        mtX44_1.Zero();
                                        MTX44 mtX44_2 = new MTX44();
                                        mtX44_2.Zero();
                                        MTX44 mtX44_3 = new MTX44();
                                        MTX44 mtX44_4 = new MTX44();
                                        float num20 = 0.0f;
                                        byte[] sbc7 = this.sbc;
                                        int index14 = offset1;
                                        int num21 = index14 + 1;
                                        byte index15 = sbc7[index14];
                                        byte[] sbc8 = this.sbc;
                                        int index16 = num21;
                                        offset1 = index16 + 1;
                                        byte num22 = sbc8[index16];
                                        for (int index17 = 0; index17 < (int)num22; ++index17)
                                        {
                                            byte[] sbc9 = this.sbc;
                                            int index18 = offset1;
                                            int num23 = index18 + 1;
                                            byte index19 = sbc9[index18];
                                            byte[] sbc10 = this.sbc;
                                            int index20 = num23;
                                            int num24 = index20 + 1;
                                            byte index21 = sbc10[index20];
                                            byte[] sbc11 = this.sbc;
                                            int index22 = num24;
                                            offset1 = index22 + 1;
                                            float num25 = (float)sbc11[index22] / 256f;
                                            MTX44 mtX44_5 = Context.MatrixStack[(int)index19].MultMatrix(this.evpMatrices.m[(int)index21].invM);
                                            if (index17 != 0)
                                                mtX44_2 += num20 * mtX44_4;
                                            MTX44 mtX44_6 = mtX44_5;
                                            MTX44 mtX44_7 = mtX44_5.MultMatrix(this.evpMatrices.m[(int)index21].invN);
                                            num20 = num25;
                                            mtX44_1 += num20 * mtX44_6;
                                            mtX44_4 = mtX44_7;
                                        }
                                        MTX44 mtX44_8 = mtX44_2 + num20 * mtX44_4;
                                        b = mtX44_1;
                                        Context.MatrixStack[(int)index15] = b;
                                        break;
                                    case 10:
                                        int num26 = Bytes.Read4BytesAsInt32(this.sbc, offset1);
                                        int offset2 = offset1 + 4;
                                        int count = Bytes.Read4BytesAsInt32(this.sbc, offset2);
                                        offset1 = offset2 + 4;
                                        GlNitro.glNitroGx(((IEnumerable<byte>)this.sbc).ToList<byte>().GetRange(offset1 - 9 + num26, count).ToArray(), b.Clone(), ref Context, posScale);
                                        break;
                                    case 11:
                                        if ((int)num2 >> 5 != 0)
                                            break;
                                        break;
                                    case 12:
                                        byte[] sbc12 = this.sbc;
                                        int index23 = offset1;
                                        int num27 = index23 + 1;
                                        byte index24 = sbc12[index23];
                                        byte[] sbc13 = this.sbc;
                                        int index25 = num27;
                                        offset1 = index25 + 1;
                                        num4 = sbc13[index25];
                                        if (flag6 && !picking)
                                        {
                                            float[] numArray = new float[16];
                                            Gl.glGetFloatv(2982, numArray);
                                            Gl.glMatrixMode(5890);
                                            Gl.glLoadIdentity();
                                            NSBMD.ModelSet.Model.MaterialSet.Material material = this.materials.materials[(int)index24];
                                            Gl.glScalef(0.5f, -0.5f, 1f);
                                            Gl.glTranslatef(0.5f, 0.5f, 0.0f);
                                            if ((material.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_EFFECTMTX) != (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG)0)
                                                Gl.glMultMatrixf(material.effectMtx);
                                            MTX44 m = new MTX44();
                                            m.SetValues(numArray);
                                            m.MultMatrix(b);
                                            m[12] = 0.0f;
                                            m[13] = 0.0f;
                                            m[14] = 0.0f;
                                            Gl.glMultMatrixf((float[])m);
                                            Gl.glMatrixMode(5888);
                                            Gl.glTexGeni(8192, 9472, 9218);
                                            Gl.glTexGeni(8193, 9472, 9218);
                                            Gl.glEnable(3168);
                                            Gl.glEnable(3169);
                                            break;
                                        }
                                        break;
                                    case 13:
                                        byte[] sbc14 = this.sbc;
                                        int index26 = offset1;
                                        int num28 = index26 + 1;
                                        byte num29 = sbc14[index26];
                                        byte[] sbc15 = this.sbc;
                                        int index27 = num28;
                                        offset1 = index27 + 1;
                                        num4 = sbc15[index27];
                                        Gl.glTexGeni(8192, 9472, 9217);
                                        Gl.glTexGeni(8193, 9472, 9217);
                                        Gl.glEnable(3168);
                                        Gl.glEnable(3169);
                                        break;
                                }
                            }
                        label_104:
                            if (num1 == 0)
                                num1 = 1;
                            else
                                goto label_107;
                        }
                        while (!picking);
                        Gl.glDepthMask(1);
                    }
                label_107:;
                }

                public byte[] ExportBones()
                {
                    MTX44[] mtX44Array = new MTX44[31];
                    for (int index = 0; index < 31; ++index)
                        mtX44Array[index] = new MTX44();
                    int offset1 = 0;
                    int posScale = (int)this.info.posScale;
                    MTX44 Matrix = new MTX44();
                    List<MA.Node> nodeList = new List<MA.Node>();
                    bool flag1 = true;
                    while (offset1 != this.sbc.Length)
                    {
                        byte[] sbc1 = this.sbc;
                        int index1 = offset1++;
                        byte num1;
                        byte num2;
                        int num3;
                        int num4;
                        byte num5;
                        byte num6;
                        switch ((int)(num1 = sbc1[index1]) & 15)
                        {
                            case 1:
                                goto label_28;
                            case 2:
                                int index2 = offset1;
                                int num7 = index2 + 1;
                                num2 = sbc[index2];
                                int index3 = num7;
                                offset1 = index3 + 1;
                                flag1 = sbc[index3] == (byte)1;
                                break;
                            case 3:
                                if (flag1)
                                {
                                    Matrix = mtX44Array[(int)this.sbc[offset1++]].Clone();
                                    break;
                                }
                                ++offset1;
                                break;
                            case 4:
                                ++offset1;
                                break;
                            case 5:
                                ++offset1;
                                break;
                            case 6:
                                int index4 = offset1;
                                int num8 = index4 + 1;
                                byte index5 = sbc[index4];
                                int index6 = num8;
                                int num9 = index6 + 1;
                                byte index7 = sbc[index6];
                                int index8 = num9;
                                offset1 = index8 + 1;
                                byte num10 = sbc[index8];
                                bool flag2 = ((int)num10 & 1) == 1;
                                bool flag3 = ((int)num10 >> 1 & 1) == 1;
                                bool MayaScale = flag2;
                                int index9 = ((int)num1 >> 5 & 1) == 1 ? (int)this.sbc[offset1++] : -1;
                                int index10 = ((int)num1 >> 6 & 1) == 1 ? (int)this.sbc[offset1++] : -1;
                                if (index10 != -1)
                                    Matrix = mtX44Array[index10];
                                Matrix = Matrix.MultMatrix((MTX44)this.nodes.data[(int)index5].GetMatrix(MayaScale, posScale));
                                nodeList.Add(new MA.Node((float[])Matrix, this.nodes.dict.names[(int)index5], index7 == byte.MaxValue ? (string)null : this.nodes.dict.names[(int)index7]));
                                if (index9 != -1)
                                {
                                    mtX44Array[index9] = Matrix;
                                    break;
                                }
                                break;
                            case 7:
                                num2 = this.sbc[offset1++];
                                num3 = ((int)num1 >> 5 & 1) == 1 ? (int)this.sbc[offset1++] : -1;
                                num4 = ((int)num1 >> 6 & 1) == 1 ? (int)this.sbc[offset1++] : -1;
                                break;
                            case 8:
                                num2 = this.sbc[offset1++];
                                num3 = ((int)num1 >> 5 & 1) == 1 ? (int)this.sbc[offset1++] : -1;
                                num4 = ((int)num1 >> 6 & 1) == 1 ? (int)this.sbc[offset1++] : -1;
                                break;
                            case 9:
                                MTX44 mtX44_1 = new MTX44();
                                mtX44_1.Zero();
                                MTX44 mtX44_2 = new MTX44();
                                mtX44_2.Zero();
                                MTX44 mtX44_3 = new MTX44();
                                MTX44 mtX44_4 = new MTX44();
                                float num11 = 0.0f;
                                int index11 = offset1;
                                int num12 = index11 + 1;
                                byte index12 = sbc[index11];
                                int index13 = num12;
                                offset1 = index13 + 1;
                                byte num13 = sbc[index13];
                                for (int index14 = 0; index14 < (int)num13; ++index14)
                                {
                                    int index15 = offset1;
                                    int num14 = index15 + 1;
                                    byte index16 = sbc[index15];
                                    int index17 = num14;
                                    int num15 = index17 + 1;
                                    byte index18 = sbc[index17];
                                    int index19 = num15;
                                    offset1 = index19 + 1;
                                    float num16 = (float)sbc[index19] / 256f;
                                    MTX44 mtX44_5 = mtX44Array[(int)index16].MultMatrix(this.evpMatrices.m[(int)index18].invM);
                                    if (index14 != 0)
                                        mtX44_2 += num11 * mtX44_4;
                                    MTX44 mtX44_6 = mtX44_5;
                                    MTX44 mtX44_7 = mtX44_5.MultMatrix(this.evpMatrices.m[(int)index18].invN);
                                    num11 = num16;
                                    mtX44_1 += num11 * mtX44_6;
                                    mtX44_4 = mtX44_7;
                                }
                                MTX44 mtX44_8 = mtX44_2 + num11 * mtX44_4;
                                Matrix = mtX44_1;
                                mtX44Array[(int)index12] = Matrix;
                                break;
                            case 10:
                                Bytes.Read4BytesAsInt32(this.sbc, offset1);
                                int offset2 = offset1 + 4;
                                Bytes.Read4BytesAsInt32(this.sbc, offset2);
                                offset1 = offset2 + 4;
                                break;
                            case 12:
                                int index20 = offset1;
                                int num17 = index20 + 1;
                                num5 = sbc[index20];
                                int index21 = num17;
                                offset1 = index21 + 1;
                                num6 = sbc[index21];
                                break;
                            case 13:
                                int index22 = offset1;
                                int num18 = index22 + 1;
                                num5 = sbc[index22];
                                int index23 = num18;
                                offset1 = index23 + 1;
                                num6 = sbc[index23];
                                break;
                        }
                    }
                label_28:
                    return MA.WriteBones(nodeList.ToArray());
                }

                public void ExportMesh(NSBTX.TexplttSet Textures, string FileName, string ImageFormat)
                {
                    List<Group> groupList = new List<Group>();
                    List<string> materialNames = new List<string>();
                    MaterialSet.Material currentMaterial = null;
                    MTX44[] MatrixStack = new MTX44[31];
                    for (int index = 0; index < 31; ++index)
                    {
                        MatrixStack[index] = new MTX44();
                    }

                    int offset1 = 0;
                    int posScale = (int)info.posScale;
                    MTX44 currentMatrix = new MTX44();
                    bool flag1 = true;
                    int Alpha = 31;
                    while (offset1 != sbc.Length)
                    {
                        int index1 = offset1++;
                        byte num1 = sbc[index1];
                        byte num2;
                        int num3;
                        int num4;
                        byte num5;
                        byte num6;
                        switch (num1 & 15)
                        {
                            case 1:
                                goto labelBeginWritingObj;
                            case 2:
                                int index2 = offset1;
                                int num7 = index2 + 1;
                                num2 = sbc[index2];
                                int index3 = num7;
                                offset1 = index3 + 1;
                                flag1 = sbc[index3] == (byte)1;
                                break;
                            case 3:
                                if (flag1)
                                {
                                    currentMatrix = MatrixStack[(int)sbc[offset1++]].Clone();
                                    break;
                                }
                                ++offset1;
                                break;
                            case 4:
                                byte materialId = sbc[offset1++];
                                currentMaterial = materials.materials[(int)materialId];
                                materialNames.Add(materials.dict.names[(int)materialId]);
                                break;
                            case 5:
                                groupList.Add(GlNitro.glNitroGxRipper(this.shapes.shape[(int)this.sbc[offset1++]].DL, currentMatrix.Clone(), Alpha, ref MatrixStack, posScale, currentMaterial));
                                break;
                            case 6:
                                int num8 = offset1 + 1;
                                byte nodeIndex = sbc[offset1];
                                int num9 = num8 + 1;
                                byte num10 = sbc[num8];
                                offset1 = num9 + 1;
                                byte num11 = sbc[num9];
                                bool MayaScale = ((int)num11 & 1) == 1;
                                int storeIndex = ((int)num1 >> 5 & 1) == 1 ? (int)this.sbc[offset1++] : -1;
                                int loadIndex = ((int)num1 >> 6 & 1) == 1 ? (int)this.sbc[offset1++] : -1;

                                if (loadIndex != -1)
                                {
                                    currentMatrix = MatrixStack[loadIndex];
                                }

                                currentMatrix = currentMatrix.MultMatrix((MTX44)this.nodes.data[(int)nodeIndex].GetMatrix(MayaScale, posScale));

                                if (storeIndex != -1)
                                {
                                    MatrixStack[storeIndex] = currentMatrix;
                                    break;
                                }
                                break;
                            case 7:
                                Gl.glCullFace(0);
                                num2 = this.sbc[offset1++];
                                num3 = ((int)num1 >> 5 & 1) == 1 ? (int)sbc[offset1++] : -1;
                                num4 = ((int)num1 >> 6 & 1) == 1 ? (int)sbc[offset1++] : -1;
                                break;
                            case 8:
                                Gl.glCullFace(0);
                                num2 = this.sbc[offset1++];
                                num3 = ((int)num1 >> 5 & 1) == 1 ? (int)this.sbc[offset1++] : -1;
                                num4 = ((int)num1 >> 6 & 1) == 1 ? (int)this.sbc[offset1++] : -1;
                                break;
                            case 9:
                                MTX44 mtX44_2 = new MTX44();
                                mtX44_2.Zero();
                                MTX44 mtX44_3 = new MTX44();
                                mtX44_3.Zero();
                                MTX44 mtX44_4 = new MTX44();
                                MTX44 mtX44_5 = new MTX44();
                                float num12 = 0.0f;
                                int index11 = offset1;
                                int num13 = index11 + 1;
                                byte index12 = sbc[index11];
                                int index13 = num13;
                                offset1 = index13 + 1;
                                byte num14 = sbc[index13];
                                for (int index14 = 0; index14 < (int)num14; ++index14)
                                {
                                    byte[] sbc9 = this.sbc;
                                    int index15 = offset1;
                                    int num15 = index15 + 1;
                                    byte index16 = sbc9[index15];
                                    byte[] sbc10 = this.sbc;
                                    int index17 = num15;
                                    int num16 = index17 + 1;
                                    byte index18 = sbc10[index17];
                                    byte[] sbc11 = this.sbc;
                                    int index19 = num16;
                                    offset1 = index19 + 1;
                                    float num17 = (float)sbc11[index19] / 256f;
                                    MTX44 mtX44_6 = MatrixStack[(int)index16].MultMatrix(this.evpMatrices.m[(int)index18].invM);
                                    if (index14 != 0)
                                        mtX44_3 += num12 * mtX44_5;
                                    MTX44 mtX44_7 = mtX44_6;
                                    MTX44 mtX44_8 = mtX44_6.MultMatrix(this.evpMatrices.m[(int)index18].invN);
                                    num12 = num17;
                                    mtX44_2 += num12 * mtX44_7;
                                    mtX44_5 = mtX44_8;
                                }
                                MTX44 mtX44_9 = mtX44_3 + num12 * mtX44_5;
                                currentMatrix = mtX44_2;
                                MatrixStack[(int)index12] = currentMatrix;
                                break;
                            case 10:
                                int num18 = Bytes.Read4BytesAsInt32(this.sbc, offset1);
                                int offset2 = offset1 + 4;
                                int count = Bytes.Read4BytesAsInt32(this.sbc, offset2);
                                offset1 = offset2 + 4;
                                groupList.Add(GlNitro.glNitroGxRipper(((IEnumerable<byte>)this.sbc).ToList<byte>().GetRange(offset1 - 9 + num18, count).ToArray(), currentMatrix.Clone(), 31, ref MatrixStack, posScale, currentMaterial));
                                break;
                            case 11:
                                if ((int)num1 >> 5 != 0)
                                    break;
                                break;
                            case 12:
                                int num19 = offset1 + 1;
                                num5 = sbc[offset1];
                                offset1 = num19 + 1;
                                num6 = sbc[num19];
                                break;
                            case 13:
                                int num20 = offset1 + 1;
                                num5 = sbc[offset1];
                                offset1 = num20 + 1;
                                num6 = sbc[num20];
                                break;
                        }
                    }

                labelBeginWritingObj:

                    TextWriter sw = File.CreateText(FileName);
                    sw.WriteLine("# Created by MKDS Course Modifier");
                    sw.WriteLine("mtllib {0}", (object)Path.ChangeExtension(Path.GetFileName(FileName), "mtl"));
                    int vertexId = 1;
                    int index24 = 0;
                    foreach (Group group in groupList)
                    {
                        sw.WriteLine("g {0}", (object)this.shapes.dict.names[index24]);
                        sw.WriteLine("usemtl {0}", (object)materialNames[index24]);
                        foreach (Polygon polygon in group)
                        {
                            foreach (Vector3 vector3 in polygon.Vertex)
                            {
                                sw.WriteLine("v {0} {1} {2}", (vector3.X * info.posScale).ToString().Replace(",", "."), (vector3.Y * info.posScale).ToString().Replace(",", "."), (vector3.Z * info.posScale).ToString().Replace(",", "."));
                            }

                            foreach (Vector3 normal in polygon.Normals)
                            {
                                sw.WriteLine("vn {0} {1} {2}", normal.X.ToString().Replace(",", "."), normal.Y.ToString().Replace(",", "."), normal.Z.ToString().Replace(",", "."));
                            }

                            foreach (Vector2 texCoord in polygon.TexCoords)
                            {
                                sw.WriteLine("vt {0} {1}", texCoord.X.ToString().Replace(",", "."), texCoord.Y.ToString().Replace(",", "."));
                            }

                            switch (polygon.PolyType)
                            {
                                case PolygonType.Triangle:
                                    for (int j = 0; j < polygon.Vertex.Length; j += 3)
                                    {
                                        sw.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}", vertexId, vertexId + 1, vertexId + 2);
                                        vertexId += 3;
                                    }
                                    break;
                                case PolygonType.Quad:
                                    for (int j = 0; j < polygon.Vertex.Length; j += 4)
                                    {
                                        sw.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2} {3}/{3}/{3}", vertexId, vertexId + 1, vertexId + 2, vertexId + 3);
                                        vertexId += 4;
                                    }
                                    break;
                                case PolygonType.TriangleStrip:
                                    for (int j = 0; j + 2 < polygon.Vertex.Length; j += 2)
                                    {
                                        sw.WriteLine("f" + string.Format(" {0}/{0}/{0}", vertexId + j) + string.Format(" {0}/{0}/{0}", vertexId + j + 1) + string.Format(" {0}/{0}/{0}", vertexId + j + 2));
                                        if (j + 3 < polygon.Vertex.Length)
                                        {
                                            sw.WriteLine("f" + string.Format(" {0}/{0}/{0}", vertexId + j + 1) + string.Format(" {0}/{0}/{0}", vertexId + j + 3) + string.Format(" {0}/{0}/{0}", vertexId + j + 2));
                                        }
                                    }
                                    vertexId += polygon.Vertex.Length;
                                    break;
                                case PolygonType.QuadStrip:
                                    for (int j = 0; j + 2 < polygon.Vertex.Length; j += 2)
                                    {
                                        sw.WriteLine("f" + string.Format(" {0}/{0}/{0}", vertexId + j) + string.Format(" {0}/{0}/{0}", vertexId + j + 1) + string.Format(" {0}/{0}/{0}", vertexId + j + 2));
                                        if (j + 3 < polygon.Vertex.Length)
                                        {
                                            sw.WriteLine("f" + string.Format(" {0}/{0}/{0}", vertexId + j + 1) + string.Format(" {0}/{0}/{0}", vertexId + j + 3) + string.Format(" {0}/{0}/{0}", vertexId + j + 2));
                                        }
                                    }
                                    vertexId += polygon.Vertex.Length;
                                    break;
                            }
                        }
                        ++index24;
                    }
                    sw.Close();
                    File.Create(Path.ChangeExtension(FileName, "mtl")).Close();
                    TextWriter textWriter4 = (TextWriter)new StreamWriter(Path.ChangeExtension(FileName, "mtl"));
                    int index29 = 0;
                    foreach (NSBMD.ModelSet.Model.MaterialSet.Material material in this.materials.materials)
                    {
                        textWriter4.WriteLine("newmtl {0}", (object)this.materials.dict.names[index29]);
                        

                        Color color2 = Graphic.ConvertABGR1555((short)((int)(material.diffAmb >> 16) & (int)short.MaxValue));
                        TextWriter textWriter5 = textWriter4;
                        float num24 = (float)color2.R / (float)byte.MaxValue;
                        string str10 = num24.ToString().Replace(",", ".");
                        num24 = (float)color2.G / (float)byte.MaxValue;
                        string str11 = num24.ToString().Replace(",", ".");
                        num24 = (float)color2.B / (float)byte.MaxValue;
                        string str12 = num24.ToString().Replace(",", ".");
                        textWriter5.WriteLine("Ka {0} {1} {2}", (object)str10, (object)str11, (object)str12);

                        Color color1 = Graphic.ConvertABGR1555((short)((int)material.diffAmb & (int)short.MaxValue));
                        TextWriter textWriter6 = textWriter4;
                        num24 = (float)color1.R / (float)byte.MaxValue;
                        string str13 = num24.ToString().Replace(",", ".");
                        num24 = (float)color1.G / (float)byte.MaxValue;
                        string str14 = num24.ToString().Replace(",", ".");
                        num24 = (float)color1.B / (float)byte.MaxValue;
                        string str15 = num24.ToString().Replace(",", ".");
                        textWriter6.WriteLine("Kd {0} {1} {2}", (object)str13, (object)str14, (object)str15);

                        TextWriter textWriter7 = textWriter4;
                        num24 = (float)(material.polyAttr >> 16 & 31U) / 31f;
                        string str16 = num24.ToString().Replace(",", ".");
                        textWriter7.WriteLine("d {0}", (object)str16);

                        TextWriter textWriter8 = textWriter4;
                        num24 = (float)(material.polyAttr >> 16 & 31U) / 31f;
                        string str17 = num24.ToString().Replace(",", ".");
                        textWriter8.WriteLine("Tr {0}", (object)str17);

                        textWriter4.WriteLine("map_Ka {0}.{1}", (object)this.materials.dict.names[index29], (object)ImageFormat.ToLower());
                        textWriter4.WriteLine("map_Kd {0}.{1}", (object)this.materials.dict.names[index29], (object)ImageFormat.ToLower());
                        textWriter4.WriteLine("map_d {0}.{1}", (object)this.materials.dict.names[index29], (object)ImageFormat.ToLower());
                        ++index29;
                    }
                    textWriter4.Close();
                    if (Textures == null)
                        return;
                    for (int index30 = 0; index30 < this.materials.materials.Length; ++index30)
                    {
                        NSBTX.TexplttSet.DictTexData dictTexData = (NSBTX.TexplttSet.DictTexData)null;
                        for (int i1 = 0; i1 < (int)this.materials.dictTexToMatList.numEntry; ++i1)
                        {
                            if (((IEnumerable<int>)this.materials.dictTexToMatList[i1].Value.Materials).Contains<int>(index30))
                            {
                                int i2 = i1;
                                KeyValuePair<string, NSBTX.TexplttSet.DictTexData> keyValuePair;
                                for (int i3 = 0; i3 < (int)Textures.dictTex.numEntry; ++i3)
                                {
                                    keyValuePair = Textures.dictTex[i3];
                                    if (keyValuePair.Key == this.materials.dictTexToMatList[i1].Key)
                                    {
                                        i2 = i3;
                                        break;
                                    }
                                }
                                keyValuePair = Textures.dictTex[i2];
                                dictTexData = keyValuePair.Value;
                                break;
                            }
                        }
                        if (dictTexData != null)
                        {
                            NSBTX.TexplttSet.DictPlttData Palette = (NSBTX.TexplttSet.DictPlttData)null;
                            if (dictTexData.Fmt != Graphic.GXTexFmt.GX_TEXFMT_DIRECT)
                            {
                                for (int i4 = 0; i4 < (int)this.materials.dictPlttToMatList.numEntry; ++i4)
                                {
                                    if (((IEnumerable<int>)this.materials.dictPlttToMatList[i4].Value.Materials).Contains<int>(index30))
                                    {
                                        int i5 = i4;
                                        KeyValuePair<string, NSBTX.TexplttSet.DictPlttData> keyValuePair;
                                        for (int i6 = 0; i6 < (int)Textures.dictPltt.numEntry; ++i6)
                                        {
                                            keyValuePair = Textures.dictPltt[i6];
                                            if (keyValuePair.Key == this.materials.dictPlttToMatList[i4].Key)
                                            {
                                                i5 = i6;
                                                break;
                                            }
                                        }
                                        keyValuePair = Textures.dictPltt[i5];
                                        Palette = keyValuePair.Value;
                                        break;
                                    }
                                }
                            }
                            System.Drawing.Bitmap bitmap = dictTexData.ToBitmap(Palette);
                            System.Drawing.Bitmap b = new System.Drawing.Bitmap((int)((long)bitmap.Width * (long)(uint)(((int)(this.materials.materials[index30].texImageParam >> 18) & 1) + 1)), (int)((long)bitmap.Height * (long)(uint)(((int)(this.materials.materials[index30].texImageParam >> 19) & 1) + 1)));
                            using (Graphics graphics = Graphics.FromImage((Image)b))
                            {
                                graphics.DrawImage((Image)bitmap, 0, 0);
                                bool flag4 = false;
                                bool flag5 = false;
                                if (((int)(this.materials.materials[index30].texImageParam >> 16) & 1) == 1 && ((int)(this.materials.materials[index30].texImageParam >> 18) & 1) == 1)
                                {
                                    graphics.DrawImage((Image)bitmap, bitmap.Width * 2, 0, -bitmap.Width, bitmap.Height);
                                    flag4 = true;
                                }
                                if (((int)(this.materials.materials[index30].texImageParam >> 17) & 1) == 1 && ((int)(this.materials.materials[index30].texImageParam >> 19) & 1) == 1)
                                {
                                    graphics.DrawImage((Image)bitmap, 0, bitmap.Height * 2, bitmap.Width, -bitmap.Height);
                                    flag5 = true;
                                }
                                if (flag4 && flag5)
                                    graphics.DrawImage((Image)bitmap, bitmap.Width * 2, bitmap.Height * 2, -bitmap.Width, -bitmap.Height);
                            }
                            switch (ImageFormat)
                            {
                                case "PNG":
                                    b.Save(Path.GetDirectoryName(FileName) + "\\" + this.materials.dict.names[index30] + ".png", ImageFormat.Png);
                                    break;
                                case "TIFF":
                                    b.Save(Path.GetDirectoryName(FileName) + "\\" + this.materials.dict.names[index30] + ".tiff", ImageFormat.Tiff);
                                    break;
                                case "TGA":
                                    DevIl.SaveAsTGA(b, Path.GetDirectoryName(FileName) + "\\" + this.materials.dict.names[index30] + ".tga");
                                    break;
                            }
                        }
                    }
                }

                public class ModelInfo
                {
                    public byte sbcType;
                    public byte scalingRule;
                    public byte texMtxMode;
                    public byte numNode;
                    public byte numMat;
                    public byte numShp;
                    public byte firstUnusedMtxStackID;
                    public float posScale;
                    public float invPosScale;
                    public ushort numVertex;
                    public ushort numPolygon;
                    public ushort numTriangle;
                    public ushort numQuad;
                    public float boxX;
                    public float boxY;
                    public float boxZ;
                    public float boxW;
                    public float boxH;
                    public float boxD;
                    public float boxPosScale;
                    public float boxInvPosScale;

                    public ModelInfo(EndianBinaryReader er)
                    {
                        this.sbcType = er.ReadByte();
                        this.scalingRule = er.ReadByte();
                        this.texMtxMode = er.ReadByte();
                        this.numNode = er.ReadByte();
                        this.numMat = er.ReadByte();
                        this.numShp = er.ReadByte();
                        this.firstUnusedMtxStackID = er.ReadByte();
                        int num = (int)er.ReadByte();
                        this.posScale = er.ReadSingleInt32Exp12();
                        this.invPosScale = er.ReadSingleInt32Exp12();
                        this.numVertex = er.ReadUInt16();
                        this.numPolygon = er.ReadUInt16();
                        this.numTriangle = er.ReadUInt16();
                        this.numQuad = er.ReadUInt16();
                        this.boxX = er.ReadSingleInt16Exp12();
                        this.boxY = er.ReadSingleInt16Exp12();
                        this.boxZ = er.ReadSingleInt16Exp12();
                        this.boxW = er.ReadSingleInt16Exp12();
                        this.boxH = er.ReadSingleInt16Exp12();
                        this.boxD = er.ReadSingleInt16Exp12();
                        this.boxPosScale = er.ReadSingleInt32Exp12();
                        this.boxInvPosScale = er.ReadSingleInt32Exp12();
                    }

                    public ModelInfo()
                    {
                    }

                    public void Write(EndianBinaryWriter er)
                    {
                        er.Write(this.sbcType);
                        er.Write(this.scalingRule);
                        er.Write(this.texMtxMode);
                        er.Write(this.numNode);
                        er.Write(this.numMat);
                        er.Write(this.numShp);
                        er.Write(this.firstUnusedMtxStackID);
                        er.Write((byte)0);
                        er.Write((uint)((double)this.posScale * 4096.0));
                        er.Write((uint)((double)this.invPosScale * 4096.0));
                        er.Write(this.numVertex);
                        er.Write(this.numPolygon);
                        er.Write(this.numTriangle);
                        er.Write(this.numQuad);
                        er.Write((ushort)((double)this.boxX * 4096.0));
                        er.Write((ushort)((double)this.boxY * 4096.0));
                        er.Write((ushort)((double)this.boxZ * 4096.0));
                        er.Write((ushort)((double)this.boxW * 4096.0));
                        er.Write((ushort)((double)this.boxH * 4096.0));
                        er.Write((ushort)((double)this.boxD * 4096.0));
                        er.Write((uint)((double)this.boxPosScale * 4096.0));
                        er.Write((uint)((double)this.boxInvPosScale * 4096.0));
                    }
                }

                public class NodeSet
                {
                    public Dictionary<NSBMD.ModelSet.Model.NodeSet.NodeSetData> dict;
                    public NSBMD.ModelSet.Model.NodeSet.NodeData[] data;

                    public NodeSet(EndianBinaryReader er)
                    {
                        er.SetMarkerOnCurrentOffset("NodeInfo");
                        this.dict = new Dictionary<NSBMD.ModelSet.Model.NodeSet.NodeSetData>(er);
                        this.data = new NSBMD.ModelSet.Model.NodeSet.NodeData[(int)this.dict.numEntry];
                        long position = er.BaseStream.Position;
                        for (int i = 0; i < (int)this.dict.numEntry; ++i)
                        {
                            er.BaseStream.Position = er.GetMarker("NodeInfo") + (long)this.dict[i].Value.Offset;
                            this.data[i] = new NSBMD.ModelSet.Model.NodeSet.NodeData(er);
                        }
                        er.BaseStream.Position = position;
                    }

                    public void Write(EndianBinaryWriter er)
                    {
                        long position1 = er.BaseStream.Position;
                        this.dict.Write(er);
                        for (int i = 0; i < this.data.Length; ++i)
                        {
                            this.dict[i].Value.Offset = (uint)(er.BaseStream.Position - position1);
                            this.data[i].Write(er);
                        }
                        long position2 = er.BaseStream.Position;
                        er.BaseStream.Position = position1;
                        this.dict.Write(er);
                        er.BaseStream.Position = position2;
                    }

                    public NodeSet()
                    {
                    }

                    public class NodeSetData : DictionaryData
                    {
                        public uint Offset;

                        public override ushort GetDataSize() => 4;

                        public override void Read(EndianBinaryReader er) => this.Offset = er.ReadUInt32();

                        public override void Write(EndianBinaryWriter er) => er.Write(this.Offset);
                    }

                    public class NodeData
                    {
                        public const ushort NNS_G3D_SRTFLAG_TRANS_ZERO = 1;
                        public const ushort NNS_G3D_SRTFLAG_ROT_ZERO = 2;
                        public const ushort NNS_G3D_SRTFLAG_SCALE_ONE = 4;
                        public const ushort NNS_G3D_SRTFLAG_PIVOT_EXIST = 8;
                        public ushort flag;
                        public short _00;
                        public float Tx;
                        public float Ty;
                        public float Tz;
                        public float _01;
                        public float _02;
                        public float _10;
                        public float _11;
                        public float _12;
                        public float _20;
                        public float _21;
                        public float _22;
                        public float A;
                        public float B;
                        public float Sx;
                        public float Sy;
                        public float Sz;
                        public float InvSx;
                        public float InvSy;
                        public float InvSz;

                        public NodeData(EndianBinaryReader er)
                        {
                            this.flag = er.ReadUInt16();
                            this._00 = er.ReadInt16();
                            if (((int)this.flag & 1) == 0)
                            {
                                this.Tx = er.ReadSingleInt32Exp12();
                                this.Ty = er.ReadSingleInt32Exp12();
                                this.Tz = er.ReadSingleInt32Exp12();
                            }
                            if (((int)this.flag & 2) == 0 && ((int)this.flag & 8) == 0)
                            {
                                this._01 = er.ReadSingleInt16Exp12();
                                this._02 = er.ReadSingleInt16Exp12();
                                this._10 = er.ReadSingleInt16Exp12();
                                this._11 = er.ReadSingleInt16Exp12();
                                this._12 = er.ReadSingleInt16Exp12();
                                this._20 = er.ReadSingleInt16Exp12();
                                this._21 = er.ReadSingleInt16Exp12();
                                this._22 = er.ReadSingleInt16Exp12();
                            }
                            if (((int)this.flag & 2) == 0 && ((int)this.flag & 8) != 0)
                            {
                                this.A = er.ReadSingleInt16Exp12();
                                this.B = er.ReadSingleInt16Exp12();
                            }
                            if (((int)this.flag & 4) != 0)
                                return;
                            this.Sx = er.ReadSingleInt32Exp12();
                            this.Sy = er.ReadSingleInt32Exp12();
                            this.Sz = er.ReadSingleInt32Exp12();
                            this.InvSx = er.ReadSingleInt32Exp12();
                            this.InvSy = er.ReadSingleInt32Exp12();
                            this.InvSz = er.ReadSingleInt32Exp12();
                        }

                        public NodeData()
                        {
                        }

                        public void Write(EndianBinaryWriter er)
                        {
                            er.Write(this.flag);
                            er.Write(this._00);
                            if (((int)this.flag & 1) == 0)
                            {
                                er.Write((uint)((double)this.Tx * 4096.0));
                                er.Write((uint)((double)this.Ty * 4096.0));
                                er.Write((uint)((double)this.Tz * 4096.0));
                            }
                            if (((int)this.flag & 2) == 0 && ((int)this.flag & 8) == 0)
                            {
                                er.Write((ushort)((double)this._01 * 4096.0));
                                er.Write((ushort)((double)this._02 * 4096.0));
                                er.Write((ushort)((double)this._10 * 4096.0));
                                er.Write((ushort)((double)this._11 * 4096.0));
                                er.Write((ushort)((double)this._12 * 4096.0));
                                er.Write((ushort)((double)this._20 * 4096.0));
                                er.Write((ushort)((double)this._21 * 4096.0));
                                er.Write((ushort)((double)this._22 * 4096.0));
                            }
                            if (((int)this.flag & 2) == 0 && ((int)this.flag & 8) != 0)
                            {
                                er.Write((ushort)((double)this.A * 4096.0));
                                er.Write((ushort)((double)this.B * 4096.0));
                            }
                            if (((int)this.flag & 4) != 0)
                                return;
                            er.Write((uint)((double)this.Sx * 4096.0));
                            er.Write((uint)((double)this.Sy * 4096.0));
                            er.Write((uint)((double)this.Sz * 4096.0));
                            er.Write((uint)((double)this.InvSx * 4096.0));
                            er.Write((uint)((double)this.InvSy * 4096.0));
                            er.Write((uint)((double)this.InvSz * 4096.0));
                        }

                        public float[] GetMatrix(bool MayaScale, int PosScale)
                        {
                            float[] numArray1 = this.loadIdentity();
                            float[] numArray2 = this.loadIdentity();
                            float[] numArray3 = this.loadIdentity();
                            float[] numArray4 = this.loadIdentity();
                            if (((int)this.flag & 1) == 0)
                                numArray4 = this.translate(numArray4, this.Tx / (float)PosScale, this.Ty / (float)PosScale, this.Tz / (float)PosScale);
                            if (MayaScale && ((int)this.flag & 4) == 0)
                                numArray2 = this.scale(numArray2, this.InvSx / (float)PosScale, this.InvSy / (float)PosScale, this.InvSz / (float)PosScale);
                            if (((int)this.flag & 2) == 0 && ((int)this.flag & 8) == 0)
                            {
                                numArray3[0] = (float)this._00 / 4096f;
                                numArray3[1] = this._01;
                                numArray3[2] = this._02;
                                numArray3[4] = this._10;
                                numArray3[5] = this._11;
                                numArray3[6] = this._12;
                                numArray3[8] = this._20;
                                numArray3[9] = this._21;
                                numArray3[10] = this._22;
                            }
                            else if (((int)this.flag & 2) == 0 && ((int)this.flag & 8) != 0)
                                numArray3 = NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix(numArray3, GlNitro.glNitroPivot(new float[2]
                                {
              this.A,
              this.B
                                }, (int)this.flag >> 4 & 15, (int)this.flag >> 8 & 15));
                            if (((int)this.flag & 4) == 0)
                                numArray1 = this.scale(numArray1, this.Sx / (float)PosScale, this.Sy / (float)PosScale, this.Sz / (float)PosScale);
                            float[] a = NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix((float[])new MTX44(), numArray4);
                            if (MayaScale)
                                a = NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix(a, numArray2);
                            return NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix(NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix(a, numArray3), numArray1);
                        }

                        public float[] GetRotation()
                        {
                            float[] a = this.loadIdentity();
                            if (((int)this.flag & 2) == 0 && ((int)this.flag & 8) == 0)
                            {
                                a[0] = (float)this._00 / 4096f;
                                a[1] = this._01;
                                a[2] = this._02;
                                a[4] = this._10;
                                a[5] = this._11;
                                a[6] = this._12;
                                a[8] = this._20;
                                a[9] = this._21;
                                a[10] = this._22;
                            }
                            else if (((int)this.flag & 2) == 0 && ((int)this.flag & 8) != 0)
                                a = NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix(a, GlNitro.glNitroPivot(new float[2]
                                {
              this.A,
              this.B
                                }, (int)this.flag >> 4 & 15, (int)this.flag >> 8 & 15));
                            return a;
                        }

                        private float[] translate(float[] a, float x, float y, float z)
                        {
                            float[] b = this.loadIdentity();
                            b[12] = x;
                            b[13] = y;
                            b[14] = z;
                            return NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix(a, b);
                        }

                        private float[] loadIdentity()
                        {
                            float[] numArray = new float[16];
                            numArray[0] = 1f;
                            numArray[5] = 1f;
                            numArray[10] = 1f;
                            numArray[15] = 1f;
                            return numArray;
                        }

                        private static float[] multMatrix(float[] a, float[] b)
                        {
                            float[] numArray = new float[16];
                            for (int index1 = 0; index1 < 4; ++index1)
                            {
                                for (int index2 = 0; index2 < 4; ++index2)
                                {
                                    numArray[(index1 << 2) + index2] = 0.0f;
                                    for (int index3 = 0; index3 < 4; ++index3)
                                        numArray[(index1 << 2) + index2] += a[(index3 << 2) + index2] * b[(index1 << 2) + index3];
                                }
                            }
                            return numArray;
                        }

                        private float[] scale(float[] a, float x, float y, float z)
                        {
                            float[] b = this.loadIdentity();
                            b[0] = x;
                            b[5] = y;
                            b[10] = z;
                            return NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix(a, b);
                        }
                    }
                }

                public class MaterialSet
                {
                    public ushort ofsDictTexToMatList;
                    public ushort ofsDictPlttToMatList;
                    public Dictionary<NSBMD.ModelSet.Model.MaterialSet.MaterialSetData> dict;
                    public Dictionary<NSBMD.ModelSet.Model.MaterialSet.TexToMatData> dictTexToMatList;
                    public Dictionary<NSBMD.ModelSet.Model.MaterialSet.PlttToMatData> dictPlttToMatList;
                    public NSBMD.ModelSet.Model.MaterialSet.Material[] materials;

                    public MaterialSet(EndianBinaryReader er)
                    {
                        er.SetMarkerOnCurrentOffset(nameof(MaterialSet));
                        this.ofsDictTexToMatList = er.ReadUInt16();
                        this.ofsDictPlttToMatList = er.ReadUInt16();
                        this.dict = new Dictionary<NSBMD.ModelSet.Model.MaterialSet.MaterialSetData>(er);
                        long position = er.BaseStream.Position;
                        this.materials = new NSBMD.ModelSet.Model.MaterialSet.Material[(int)this.dict.numEntry];
                        for (int i = 0; i < (int)this.dict.numEntry; ++i)
                        {
                            er.BaseStream.Position = (long)this.dict[i].Value.Offset + er.GetMarker(nameof(MaterialSet));
                            this.materials[i] = new NSBMD.ModelSet.Model.MaterialSet.Material(er);
                        }
                        er.BaseStream.Position = position;
                        this.dictTexToMatList = new Dictionary<NSBMD.ModelSet.Model.MaterialSet.TexToMatData>(er);
                        this.dictPlttToMatList = new Dictionary<NSBMD.ModelSet.Model.MaterialSet.PlttToMatData>(er);
                        while (er.BaseStream.Position % 4L != 0L)
                        {
                            int num = (int)er.ReadByte();
                        }
                    }

                    public MaterialSet()
                    {
                    }

                    public void Write(EndianBinaryWriter er)
                    {
                        long position1 = er.BaseStream.Position;
                        er.Write((ushort)0);
                        er.Write((ushort)0);
                        this.dict.Write(er);
                        long position2 = er.BaseStream.Position;
                        er.BaseStream.Position = position1;
                        er.Write((ushort)(position2 - position1));
                        er.BaseStream.Position = position2;
                        this.dictTexToMatList.Write(er);
                        long position3 = er.BaseStream.Position;
                        er.BaseStream.Position = position1 + 2L;
                        er.Write((ushort)(position3 - position1));
                        er.BaseStream.Position = position3;
                        this.dictPlttToMatList.Write(er);
                        for (int i = 0; i < (int)this.dictTexToMatList.numEntry; ++i)
                        {
                            KeyValuePair<string, NSBMD.ModelSet.Model.MaterialSet.TexToMatData> dictTexToMat = this.dictTexToMatList[i];
                            dictTexToMat.Value.Offset = (ushort)(er.BaseStream.Position - position1);
                            dictTexToMat = this.dictTexToMatList[i];
                            foreach (int material in dictTexToMat.Value.Materials)
                                er.Write((byte)material);
                        }
                        for (int i = 0; i < (int)this.dictPlttToMatList.numEntry; ++i)
                        {
                            KeyValuePair<string, NSBMD.ModelSet.Model.MaterialSet.PlttToMatData> dictPlttToMat = this.dictPlttToMatList[i];
                            dictPlttToMat.Value.Offset = (ushort)(er.BaseStream.Position - position1);
                            dictPlttToMat = this.dictPlttToMatList[i];
                            foreach (int material in dictPlttToMat.Value.Materials)
                                er.Write((byte)material);
                        }
                        while (er.BaseStream.Position % 4L != 0L)
                            er.Write((byte)0);
                        for (int i = 0; i < this.materials.Length; ++i)
                        {
                            this.dict[i].Value.Offset = (uint)(er.BaseStream.Position - position1);
                            this.materials[i].Write(er);
                        }
                        long position4 = er.BaseStream.Position;
                        er.BaseStream.Position = position1 + 4L;
                        this.dict.Write(er);
                        this.dictTexToMatList.Write(er);
                        this.dictPlttToMatList.Write(er);
                        er.BaseStream.Position = position4;
                    }

                    public class MaterialSetData : DictionaryData
                    {
                        public uint Offset;

                        public override ushort GetDataSize() => 4;

                        public override void Read(EndianBinaryReader er) => this.Offset = er.ReadUInt32();

                        public override void Write(EndianBinaryWriter er) => er.Write(this.Offset);
                    }

                    public class TexToMatData : DictionaryData
                    {
                        public uint Flag;
                        public ushort Offset;
                        public byte NrMat;
                        public byte Bound;
                        public int[] Materials;

                        public override ushort GetDataSize() => 4;

                        public override void Read(EndianBinaryReader er)
                        {
                            this.Flag = er.ReadUInt32();
                            this.Offset = (ushort)(this.Flag & (uint)ushort.MaxValue);
                            this.NrMat = (byte)(this.Flag >> 16 & (uint)sbyte.MaxValue);
                            this.Bound = (byte)(this.Flag >> 24 & (uint)byte.MaxValue);
                            this.Materials = new int[(int)this.NrMat];
                            long position = er.BaseStream.Position;
                            er.BaseStream.Position = (long)this.Offset + er.GetMarker(nameof(MaterialSet));
                            for (int index = 0; index < (int)this.NrMat; ++index)
                                this.Materials[index] = (int)er.ReadByte();
                            er.BaseStream.Position = position;
                        }

                        public override void Write(EndianBinaryWriter er)
                        {
                            this.Flag = (uint)(((int)this.Bound & (int)byte.MaxValue) << 24 | ((int)this.NrMat & (int)byte.MaxValue) << 16 | (int)this.Offset & (int)ushort.MaxValue);
                            er.Write(this.Flag);
                        }
                    }

                    public class PlttToMatData : DictionaryData
                    {
                        public uint Flag;
                        public ushort Offset;
                        public byte NrMat;
                        public byte Bound;
                        public int[] Materials;

                        public override ushort GetDataSize() => 4;

                        public override void Read(EndianBinaryReader er)
                        {
                            this.Flag = er.ReadUInt32();
                            this.Offset = (ushort)(this.Flag & (uint)ushort.MaxValue);
                            this.NrMat = (byte)(this.Flag >> 16 & (uint)sbyte.MaxValue);
                            this.Bound = (byte)(this.Flag >> 24 & (uint)byte.MaxValue);
                            this.Materials = new int[(int)this.NrMat];
                            long position = er.BaseStream.Position;
                            er.BaseStream.Position = (long)this.Offset + er.GetMarker(nameof(MaterialSet));
                            for (int index = 0; index < (int)this.NrMat; ++index)
                                this.Materials[index] = (int)er.ReadByte();
                            er.BaseStream.Position = position;
                        }

                        public override void Write(EndianBinaryWriter er)
                        {
                            this.Flag = (uint)(((int)this.Bound & (int)byte.MaxValue) << 24 | ((int)this.NrMat & (int)byte.MaxValue) << 16 | (int)this.Offset & (int)ushort.MaxValue);
                            er.Write(this.Flag);
                        }
                    }

                    public class Material
                    {
                        public ushort itemTag;
                        public ushort size;
                        public uint diffAmb;
                        public uint specEmi;
                        public uint polyAttr;
                        public uint polyAttrMask;
                        public uint texImageParam;
                        public uint texImageParamMask;
                        public ushort texPlttBase;
                        public NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG flag;
                        public ushort origWidth;
                        public ushort origHeight;
                        public float magW;
                        public float magH;
                        public float scaleS;
                        public float scaleT;
                        public float rotSin;
                        public float rotCos;
                        public float transS;
                        public float transT;
                        public float[] effectMtx;
                        public Graphic.GXTexFmt Fmt;

                        public Material(EndianBinaryReader er)
                        {
                            this.itemTag = er.ReadUInt16();
                            this.size = er.ReadUInt16();
                            this.diffAmb = er.ReadUInt32();
                            this.specEmi = er.ReadUInt32();
                            this.polyAttr = er.ReadUInt32();
                            this.polyAttrMask = er.ReadUInt32();
                            this.texImageParam = er.ReadUInt32();
                            this.texImageParamMask = er.ReadUInt32();
                            this.texPlttBase = er.ReadUInt16();
                            this.flag = (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG)er.ReadUInt16();
                            this.origWidth = er.ReadUInt16();
                            this.origHeight = er.ReadUInt16();
                            this.magW = er.ReadSingleInt32Exp12();
                            this.magH = er.ReadSingleInt32Exp12();
                            if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_SCALEONE) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG)0)
                            {
                                this.scaleS = er.ReadSingleInt32Exp12();
                                this.scaleT = er.ReadSingleInt32Exp12();
                            }
                            if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_ROTZERO) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG)0)
                            {
                                this.rotSin = er.ReadSingleInt16Exp12();
                                this.rotCos = er.ReadSingleInt16Exp12();
                            }
                            if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_TRANSZERO) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG)0)
                            {
                                this.transS = er.ReadSingleInt32Exp12();
                                this.transT = er.ReadSingleInt32Exp12();
                            }
                            if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_EFFECTMTX) != NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_EFFECTMTX)
                                return;
                            this.effectMtx = er.ReadSingleInt32Exp12s(16);
                        }

                        public Material()
                        {
                        }

                        public void Write(EndianBinaryWriter er)
                        {
                            long position1 = er.BaseStream.Position;
                            er.Write(this.itemTag);
                            er.Write((ushort)0);
                            er.Write(this.diffAmb);
                            er.Write(this.specEmi);
                            er.Write(this.polyAttr);
                            er.Write(this.polyAttrMask);
                            er.Write(this.texImageParam);
                            er.Write(this.texImageParamMask);
                            er.Write(this.texPlttBase);
                            er.Write((ushort)this.flag);
                            er.Write(this.origWidth);
                            er.Write(this.origHeight);
                            er.Write((uint)((double)this.magW * 4096.0));
                            er.Write((uint)((double)this.magH * 4096.0));
                            if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_SCALEONE) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG)0)
                            {
                                er.Write((uint)((double)this.scaleS * 4096.0));
                                er.Write((uint)((double)this.scaleT * 4096.0));
                            }
                            if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_ROTZERO) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG)0)
                            {
                                er.Write((ushort)((double)this.rotSin * 4096.0));
                                er.Write((ushort)((double)this.rotCos * 4096.0));
                            }
                            if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_TRANSZERO) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG)0)
                            {
                                er.Write((uint)((double)this.transS * 4096.0));
                                er.Write((uint)((double)this.transT * 4096.0));
                            }
                            if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_EFFECTMTX) != (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG)0)
                            {
                                foreach (float num in this.effectMtx)
                                    er.Write((uint)((double)num * 4096.0));
                            }
                            long position2 = er.BaseStream.Position;
                            er.BaseStream.Position = position1 + 2L;
                            er.Write((ushort)(position2 - position1));
                            er.BaseStream.Position = position2;
                        }

                        public float[] GetMatrix()
                        {
                            MTX44 matrix = new MTX44();
                            matrix.Zero();
                            bool flag1 = (this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_SCALEONE) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG)0;
                            bool flag2 = (this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_ROTZERO) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG)0;
                            if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_TRANSZERO) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG)0)
                            {
                                matrix[0, 3] = this.transS;
                                matrix[1, 3] = this.transT;
                            }
                            float num1 = flag1 ? this.scaleS : 1f;
                            float num2 = flag1 ? this.scaleT : 1f;
                            if (flag2)
                            {
                                matrix[0, 0] = this.rotCos * num1;
                                matrix[1, 0] = -this.rotSin;
                                matrix[0, 1] = this.rotSin;
                                matrix[1, 1] = -this.rotCos * num2;
                            }
                            else
                            {
                                matrix[0, 0] = num1;
                                matrix[1, 1] = num2;
                            }
                            matrix[2, 2] = 1f;
                            matrix[3, 3] = 1f;
                            return (float[])matrix;
                        }

                        [Flags]
                        public enum NNS_G3D_MATFLAG : ushort
                        {
                            NNS_G3D_MATFLAG_TEXMTX_USE = 1,
                            NNS_G3D_MATFLAG_TEXMTX_SCALEONE = 2,
                            NNS_G3D_MATFLAG_TEXMTX_ROTZERO = 4,
                            NNS_G3D_MATFLAG_TEXMTX_TRANSZERO = 8,
                            NNS_G3D_MATFLAG_ORIGWH_SAME = 16, // 0x0010
                            NNS_G3D_MATFLAG_WIREFRAME = 32, // 0x0020
                            NNS_G3D_MATFLAG_DIFFUSE = 64, // 0x0040
                            NNS_G3D_MATFLAG_AMBIENT = 128, // 0x0080
                            NNS_G3D_MATFLAG_VTXCOLOR = 256, // 0x0100
                            NNS_G3D_MATFLAG_SPECULAR = 512, // 0x0200
                            NNS_G3D_MATFLAG_EMISSION = 1024, // 0x0400
                            NNS_G3D_MATFLAG_SHININESS = 2048, // 0x0800
                            NNS_G3D_MATFLAG_TEXPLTTBASE = 4096, // 0x1000
                            NNS_G3D_MATFLAG_EFFECTMTX = 8192, // 0x2000
                        }
                    }
                }

                public class ShapeSet
                {
                    public Dictionary<NSBMD.ModelSet.Model.ShapeSet.ShapeSetData> dict;
                    public NSBMD.ModelSet.Model.ShapeSet.Shape[] shape;

                    public ShapeSet(EndianBinaryReader er)
                    {
                        er.SetMarkerOnCurrentOffset(nameof(ShapeSet));
                        this.dict = new Dictionary<NSBMD.ModelSet.Model.ShapeSet.ShapeSetData>(er);
                        this.shape = new NSBMD.ModelSet.Model.ShapeSet.Shape[(int)this.dict.numEntry];
                        long position = er.BaseStream.Position;
                        for (int i = 0; i < (int)this.dict.numEntry; ++i)
                        {
                            er.BaseStream.Position = (long)this.dict[i].Value.Offset + er.GetMarker(nameof(ShapeSet));
                            this.shape[i] = new NSBMD.ModelSet.Model.ShapeSet.Shape(er);
                        }
                        er.BaseStream.Position = position;
                    }

                    public ShapeSet()
                    {
                    }

                    public void Write(EndianBinaryWriter er)
                    {
                        long position1 = er.BaseStream.Position;
                        this.dict.Write(er);
                        for (int i = 0; i < this.shape.Length; ++i)
                        {
                            this.dict[i].Value.Offset = (uint)(er.BaseStream.Position - position1);
                            this.shape[i].Write(er);
                        }
                        for (int i = 0; i < this.shape.Length; ++i)
                        {
                            this.shape[i].ofsDL = (uint)(er.BaseStream.Position - position1 - (long)this.dict[i].Value.Offset);
                            er.Write(this.shape[i].DL, 0, this.shape[i].DL.Length);
                        }
                        long position2 = er.BaseStream.Position;
                        er.BaseStream.Position = position1;
                        this.dict.Write(er);
                        for (int index = 0; index < this.shape.Length; ++index)
                            this.shape[index].Write(er);
                        er.BaseStream.Position = position2;
                    }

                    public class ShapeSetData : DictionaryData
                    {
                        public uint Offset;

                        public override ushort GetDataSize() => 4;

                        public override void Read(EndianBinaryReader er) => this.Offset = er.ReadUInt32();

                        public override void Write(EndianBinaryWriter er) => er.Write(this.Offset);
                    }

                    public class Shape
                    {
                        public ushort itemTag;
                        public ushort size;
                        public NSBMD.ModelSet.Model.ShapeSet.Shape.NNS_G3D_SHPFLAG flag;
                        public uint ofsDL;
                        public uint sizeDL;
                        public byte[] DL;

                        public Shape(EndianBinaryReader er)
                        {
                            long position1 = er.BaseStream.Position;
                            this.itemTag = er.ReadUInt16();
                            this.size = er.ReadUInt16();
                            this.flag = (NSBMD.ModelSet.Model.ShapeSet.Shape.NNS_G3D_SHPFLAG)er.ReadUInt32();
                            this.ofsDL = er.ReadUInt32();
                            this.sizeDL = er.ReadUInt32();
                            long position2 = er.BaseStream.Position;
                            er.BaseStream.Position = position1 + (long)this.ofsDL;
                            this.DL = er.ReadBytes((int)this.sizeDL);
                            er.BaseStream.Position = position2;
                        }

                        public Shape()
                        {
                        }

                        public void Write(EndianBinaryWriter er)
                        {
                            long position1 = er.BaseStream.Position;
                            er.Write(this.itemTag);
                            er.Write((ushort)0);
                            er.Write((uint)this.flag);
                            er.Write(this.ofsDL);
                            er.Write((uint)this.DL.Length);
                            long position2 = er.BaseStream.Position;
                            er.BaseStream.Position = position1 + 2L;
                            er.Write((ushort)(position2 - position1));
                            er.BaseStream.Position = position2;
                        }

                        [Flags]
                        public enum NNS_G3D_SHPFLAG : uint
                        {
                            NNS_G3D_SHPFLAG_USE_NORMAL = 1,
                            NNS_G3D_SHPFLAG_USE_COLOR = 2,
                            NNS_G3D_SHPFLAG_USE_TEXCOORD = 4,
                            NNS_G3D_SHPFLAG_USE_RESTOREMTX = 8,
                        }
                    }
                }

                public class EvpMatrices
                {
                    public NSBMD.ModelSet.Model.EvpMatrices.envelope[] m;

                    public EvpMatrices(EndianBinaryReader er, int NumNodes)
                    {
                        this.m = new NSBMD.ModelSet.Model.EvpMatrices.envelope[NumNodes];
                        for (int index = 0; index < NumNodes; ++index)
                            this.m[index] = new NSBMD.ModelSet.Model.EvpMatrices.envelope(er);
                    }

                    public void Write(EndianBinaryWriter er)
                    {
                        foreach (NSBMD.ModelSet.Model.EvpMatrices.envelope envelope in this.m)
                            envelope.Write(er);
                    }

                    public class envelope
                    {
                        public MTX44 invM;
                        public MTX44 invN;

                        public envelope(EndianBinaryReader er)
                        {
                            this.invM = new MTX44();
                            for (int y = 0; y < 4; ++y)
                            {
                                for (int x = 0; x < 3; ++x)
                                    this.invM[x, y] = er.ReadSingleInt32Exp12();
                            }
                            this.invN = new MTX44();
                            for (int y = 0; y < 3; ++y)
                            {
                                for (int x = 0; x < 3; ++x)
                                    this.invN[x, y] = er.ReadSingleInt32Exp12();
                            }
                        }

                        public void Write(EndianBinaryWriter er)
                        {
                            for (int y = 0; y < 4; ++y)
                            {
                                for (int x = 0; x < 3; ++x)
                                    er.Write((uint)((double)this.invM[x, y] * 4096.0));
                            }
                            for (int y = 0; y < 3; ++y)
                            {
                                for (int x = 0; x < 3; ++x)
                                    er.Write((uint)((double)this.invN[x, y] * 4096.0));
                            }
                        }
                    }
                }
            }
        }
    }
}



































































































































































































using MKDS_Course_Modifier._3D_Formats;
using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.G3D_Binary_File_Format;
using MKDS_Course_Modifier.MPDS;
using MKDS_Course_Modifier.SM64DS;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Tao.Platform.Windows;

namespace Tao.OpenGl
{
  public class GlNitro
  {
    private const float SCALE_IV = 4096f;

    public static void glNitroTexImage2D(
      System.Drawing.Bitmap b,
      NSBMD.ModelSet.Model.MaterialSet.Material m,
      int Nr)
    {
      Gl.glBindTexture(3553, Nr);
      Gl.glColor3f(1f, 1f, 1f);
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      Gl.glTexImage2D(3553, 0, 32856, b.Width, b.Height, 0, 32993, 5121, bitmapdata.Scan0);
      b.UnlockBits(bitmapdata);
      Gl.glTexParameteri(3553, 10241, 9728);
      Gl.glTexParameteri(3553, 10240, 9728);
      bool flag1 = ((int) (m.texImageParam >> 16) & 1) == 1;
      bool flag2 = ((int) (m.texImageParam >> 17) & 1) == 1;
      bool flag3 = ((int) (m.texImageParam >> 18) & 1) == 1;
      bool flag4 = ((int) (m.texImageParam >> 19) & 1) == 1;
      int num1 = !flag1 || !flag3 ? (!flag1 ? 10496 : 10497) : 33648;
      int num2 = !flag2 || !flag4 ? (!flag2 ? 10496 : 10497) : 33648;
      Gl.glTexParameterf(3553, 10242, (float) num1);
      Gl.glTexParameterf(3553, 10243, (float) num2);
    }

    public static void glNitroTexImage2D(System.Drawing.Bitmap b, int Nr, int WrapMode)
    {
      Gl.glBindTexture(3553, Nr);
      Gl.glColor3f(1f, 1f, 1f);
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      Gl.glTexImage2D(3553, 0, 32856, b.Width, b.Height, 0, 32993, 5121, bitmapdata.Scan0);
      b.UnlockBits(bitmapdata);
      Gl.glTexParameteri(3553, 10241, 9729);
      Gl.glTexParameteri(3553, 10240, 9729);
      Gl.glTexParameterf(3553, 10242, (float) WrapMode);
      Gl.glTexParameterf(3553, 10243, (float) WrapMode);
    }

    public static void glNitroTexImage2D(System.Drawing.Bitmap b, int Nr, int WrapMode, int FilterMode)
    {
      Gl.glBindTexture(3553, Nr);
      Gl.glColor3f(1f, 1f, 1f);
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      Gl.glTexImage2D(3553, 0, 32856, b.Width, b.Height, 0, 32993, 5121, bitmapdata.Scan0);
      b.UnlockBits(bitmapdata);
      Gl.glTexParameteri(3553, 10241, FilterMode);
      Gl.glTexParameteri(3553, 10240, FilterMode);
      Gl.glTexParameterf(3553, 10242, (float) WrapMode);
      Gl.glTexParameterf(3553, 10243, (float) WrapMode);
    }

    public static void glNitroTexImage2D(
      System.Drawing.Bitmap b,
      int Nr,
      int WrapModeS,
      int WrapModeT,
      int FilterModeMin,
      int FilterModeMag)
    {
      Gl.glBindTexture(3553, Nr);
      Gl.glColor3f(1f, 1f, 1f);
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      Gl.glTexImage2D(3553, 0, 32856, b.Width, b.Height, 0, 32993, 5121, bitmapdata.Scan0);
      b.UnlockBits(bitmapdata);
      Gl.glTexParameteri(3553, 10241, FilterModeMin);
      Gl.glTexParameteri(3553, 10240, FilterModeMag);
      Gl.glTexParameterf(3553, 10242, (float) WrapModeS);
      Gl.glTexParameterf(3553, 10243, (float) WrapModeT);
    }

    public static void glNitroTexImage2D(System.Drawing.Bitmap b, BMD.BMDMaterial m, int Nr)
    {
      Gl.glBindTexture(3553, Nr);
      Gl.glColor3f(1f, 1f, 1f);
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      Gl.glTexImage2D(3553, 0, 32856, b.Width, b.Height, 0, 32993, 5121, bitmapdata.Scan0);
      b.UnlockBits(bitmapdata);
      Gl.glTexParameteri(3553, 10241, 9728);
      Gl.glTexParameteri(3553, 10240, 9728);
      bool flag1 = ((int) (m.texImageParam >> 16) & 1) == 1;
      bool flag2 = ((int) (m.texImageParam >> 17) & 1) == 1;
      bool flag3 = ((int) (m.texImageParam >> 18) & 1) == 1;
      bool flag4 = ((int) (m.texImageParam >> 19) & 1) == 1;
      int num1 = !flag1 || !flag3 ? (!flag1 ? 10496 : 10497) : 33648;
      int num2 = !flag2 || !flag4 ? (!flag2 ? 10496 : 10497) : 33648;
      Gl.glTexParameterf(3553, 10242, (float) num1);
      Gl.glTexParameterf(3553, 10243, (float) num2);
    }

    public static void glNitroTexImage2D(System.Drawing.Bitmap b, HBDF.MDLFBlock.TextureBlock m, int Nr)
    {
      Gl.glBindTexture(3553, Nr);
      Gl.glColor3f(1f, 1f, 1f);
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      Gl.glTexImage2D(3553, 0, 32856, b.Width, b.Height, 0, 32993, 5121, bitmapdata.Scan0);
      b.UnlockBits(bitmapdata);
      Gl.glTexParameteri(3553, 10241, 9728);
      Gl.glTexParameteri(3553, 10240, 9728);
      bool flag1 = ((int) (m.texImageParam >> 16) & 1) == 1;
      bool flag2 = ((int) (m.texImageParam >> 17) & 1) == 1;
      bool flag3 = ((int) (m.texImageParam >> 18) & 1) == 1;
      bool flag4 = ((int) (m.texImageParam >> 19) & 1) == 1;
      int num1 = !flag1 || !flag3 ? (!flag1 ? 10496 : 10497) : 33648;
      int num2 = !flag2 || !flag4 ? (!flag2 ? 10496 : 10497) : 33648;
      Gl.glTexParameterf(3553, 10242, (float) num1);
      Gl.glTexParameterf(3553, 10243, (float) num2);
    }

    public static void glNitroBindTextures(NSBMD mod, int offset)
    {
      if (mod.TexPlttSet == null)
        return;
      for (int index1 = 0; index1 < mod.modelSet.models.Length; ++index1)
      {
        for (int index2 = 0; index2 < mod.modelSet.models[index1].materials.materials.Length; ++index2)
        {
          NSBTX.TexplttSet.DictTexData dictTexData = (NSBTX.TexplttSet.DictTexData) null;
          for (int i1 = 0; i1 < (int) mod.modelSet.models[index1].materials.dictTexToMatList.numEntry; ++i1)
          {
            if (((IEnumerable<int>) mod.modelSet.models[index1].materials.dictTexToMatList[i1].Value.Materials).Contains<int>(index2))
            {
              int i2 = i1;
              KeyValuePair<string, NSBTX.TexplttSet.DictTexData> keyValuePair;
              for (int i3 = 0; i3 < (int) mod.TexPlttSet.dictTex.numEntry; ++i3)
              {
                keyValuePair = mod.TexPlttSet.dictTex[i3];
                if (keyValuePair.Key == mod.modelSet.models[index1].materials.dictTexToMatList[i1].Key)
                {
                  i2 = i3;
                  break;
                }
              }
              keyValuePair = mod.TexPlttSet.dictTex[i2];
              dictTexData = keyValuePair.Value;
              break;
            }
          }
          if (dictTexData != null)
          {
            mod.modelSet.models[index1].materials.materials[index2].Fmt = dictTexData.Fmt;
            mod.modelSet.models[index1].materials.materials[index2].origHeight = dictTexData.T;
            mod.modelSet.models[index1].materials.materials[index2].origWidth = dictTexData.S;
            NSBTX.TexplttSet.DictPlttData Palette = (NSBTX.TexplttSet.DictPlttData) null;
            if (dictTexData.Fmt != Graphic.GXTexFmt.GX_TEXFMT_DIRECT)
            {
              for (int i4 = 0; i4 < (int) mod.modelSet.models[index1].materials.dictPlttToMatList.numEntry; ++i4)
              {
                if (((IEnumerable<int>) mod.modelSet.models[index1].materials.dictPlttToMatList[i4].Value.Materials).Contains<int>(index2))
                {
                  int i5 = i4;
                  KeyValuePair<string, NSBTX.TexplttSet.DictPlttData> keyValuePair;
                  for (int i6 = 0; i6 < (int) mod.TexPlttSet.dictPltt.numEntry; ++i6)
                  {
                    keyValuePair = mod.TexPlttSet.dictPltt[i6];
                    if (keyValuePair.Key == mod.modelSet.models[index1].materials.dictPlttToMatList[i4].Key)
                    {
                      i5 = i6;
                      break;
                    }
                  }
                  keyValuePair = mod.TexPlttSet.dictPltt[i5];
                  Palette = keyValuePair.Value;
                  break;
                }
              }
            }
            GlNitro.glNitroTexImage2D(dictTexData.ToBitmap(Palette), mod.modelSet.models[index1].materials.materials[index2], index2 + offset);
          }
        }
      }
    }

    public static void glNitroBindTextures(BMD b, int offset)
    {
      int num = 1;
      foreach (BMD.BMDMaterial material in b.Materials)
      {
        if (material.TexID == -1)
          ++num;
        else if (material.PalID != -1)
          GlNitro.glNitroTexImage2D(b.Textures[material.TexID].ToBitmap(b.Palettes[material.PalID]), material, num++);
        else
          GlNitro.glNitroTexImage2D(b.Textures[material.TexID].ToBitmap((BMD.BMDPalette) null), material, num++);
      }
    }

    public static void glNitroBindTextures(HBDF b, int offset)
    {
      if (b.TEXSBlocks.Length == 0)
        return;
      int num = 0;
      foreach (HBDF.MDLFBlock.TextureBlock texture in b.MDLFBlocks[0].Textures)
      {
        string str = b.MDLFBlocks[0].Names.Strings[(int) b.MDLFBlocks[0].NrMaterials + num * 2 + 1];
        int index1 = 0;
        for (int index2 = 0; index2 < b.TEXSBlocks[0].TEXOBlocks.Length; ++index2)
        {
          if (b.TEXSBlocks[0].TEXOBlocks[index2].TexName.Name == str)
          {
            index1 = index2;
            break;
          }
        }
        if (b.TEXSBlocks[0].TEXOBlocks[index1].PalName != null)
          GlNitro.glNitroTexImage2D(b.TEXSBlocks[0].GetIMGOByName(b.TEXSBlocks[0].TEXOBlocks[index1].TexName.Name).ToBitmap(b.TEXSBlocks[0].GetPLTOByName(b.TEXSBlocks[0].TEXOBlocks[index1].PalName.Name)), texture, num + offset);
        else
          GlNitro.glNitroTexImage2D(b.TEXSBlocks[0].GetIMGOByName(b.TEXSBlocks[0].TEXOBlocks[index1].TexName.Name).ToBitmap((HBDF.TEXSBlock.PLTOBlock) null), texture, num + offset);
        ++num;
      }
    }

    public static void glNitroBindTextures(HBDF.TEXSBlock b, int offset)
    {
      int num = 0;
      foreach (HBDF.TEXSBlock.TEXOBlock texoBlock in b.TEXOBlocks)
      {
        if (texoBlock.PalName != null)
          GlNitro.glNitroTexImage2D(b.GetIMGOByName(texoBlock.TexName.Name).ToBitmap(b.GetPLTOByName(texoBlock.PalName.Name)), num + offset, 10496, 9728);
        else
          GlNitro.glNitroTexImage2D(b.GetIMGOByName(texoBlock.TexName.Name).ToBitmap((HBDF.TEXSBlock.PLTOBlock) null), num + offset, 10496, 9728);
        ++num;
      }
    }

    public static void glNitroGx(
      byte[] polydata,
      MTX44 Curmtx,
      int Alpha,
      ref MTX44[] MatrixStack,
      int PosScale,
      bool picking = false)
    {
    }

    public static void glNitroGx(
      byte[] polydata,
      MTX44 Curmtx,
      ref GlNitro.Nitro3DContext Context,
      int PosScale,
      bool picking = false)
    {
      Gl.glMatrixMode(5888);
      int num1 = -1;
      if (polydata == null)
        return;
      int offset1 = 0;
      int length = polydata.Length;
      int[] numArray1 = new int[4];
      float[] v = new float[3];
      float[] numArray2 = new float[3];
      MTX44 m = Curmtx;
      MTX44 mtX44_1 = Curmtx;
      while (offset1 < length)
      {
        for (int index = 0; index < 4; ++index)
        {
          if (offset1 >= length)
          {
            numArray1[index] = (int) byte.MaxValue;
          }
          else
          {
            numArray1[index] = (int) polydata[offset1];
            ++offset1;
          }
        }
        for (int index1 = 0; index1 < 4 && offset1 < length; ++index1)
        {
          switch (numArray1[index1])
          {
            case 16:
              int num2 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              Context.MatrixMode = num2;
              break;
            case 18:
              offset1 += 4;
              break;
            case 19:
              offset1 += 4;
              break;
            case 20:
              int index2 = Bytes.Read4BytesAsInt32(polydata, offset1) & 31;
              offset1 += 4;
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                Context.MatrixStack[index2].CopyValuesTo(m);
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 21:
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                m.LoadIdentity();
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 22:
              for (int index3 = 0; index3 < 16; ++index3)
              {
                if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
                  m[index3] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                offset1 += 4;
              }
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 23:
              for (int y = 0; y < 4; ++y)
              {
                int x = 0;
                while (x < 3)
                {
                  if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
                    m[x, y] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                  offset1 += 4;
                  ++y;
                }
              }
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 24:
              MTX44 b1 = new MTX44();
              b1.LoadIdentity();
              for (int index4 = 0; index4 < 16; ++index4)
              {
                b1[index4] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                offset1 += 4;
              }
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                m.MultMatrix(b1).CopyValuesTo(m);
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 25:
              MTX44 b2 = new MTX44();
              b2.LoadIdentity();
              for (int y = 0; y < 4; ++y)
              {
                int x = 0;
                while (x < 3)
                {
                  b2[x, y] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                  offset1 += 4;
                  ++y;
                }
              }
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                m.MultMatrix(b2).CopyValuesTo(m);
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 26:
              MTX44 b3 = new MTX44();
              b3.LoadIdentity();
              for (int y = 0; y < 3; ++y)
              {
                int x = 0;
                while (x < 3)
                {
                  b3[x, y] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                  offset1 += 4;
                  ++y;
                }
              }
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                m.MultMatrix(b3).CopyValuesTo(m);
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 27:
              int num3 = Bytes.Read4BytesAsInt32(polydata, offset1);
              int offset2 = offset1 + 4;
              int num4 = Bytes.Read4BytesAsInt32(polydata, offset2);
              int offset3 = offset2 + 4;
              int num5 = Bytes.Read4BytesAsInt32(polydata, offset3);
              offset1 = offset3 + 4;
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                m.Scale((float) num3 / 4096f / (float) PosScale, (float) num4 / 4096f / (float) PosScale, (float) num5 / 4096f / (float) PosScale);
                break;
              }
              break;
            case 28:
              int data1 = Bytes.Read4BytesAsInt32(polydata, offset1);
              int offset4 = offset1 + 4;
              int data2 = Bytes.Read4BytesAsInt32(polydata, offset4);
              int offset5 = offset4 + 4;
              int data3 = Bytes.Read4BytesAsInt32(polydata, offset5);
              offset1 = offset5 + 4;
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                m.translate((float) GlNitro.sign(data1, 32) / 4096f / (float) PosScale, (float) GlNitro.sign(data2, 32) / 4096f / (float) PosScale, (float) GlNitro.sign(data3, 32) / 4096f / (float) PosScale);
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 32:
              long num6 = (long) Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              long num7 = num6 & 31L;
              long num8 = num6 >> 5 & 31L;
              long num9 = num6 >> 10 & 31L;
              if (!picking)
              {
                Gl.glColor4f((float) num7 / 31f, (float) num8 / 31f, (float) num9 / 31f, (float) Context.Alpha / 31f);
                break;
              }
              break;
            case 33:
              int num10 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num11 = num10 & 1023;
              if ((num11 & 512) != 0)
                num11 |= -1024;
              int num12 = num10 >> 10 & 1023;
              if ((num12 & 512) != 0)
                num12 |= -1024;
              int num13 = num10 >> 20 & 1023;
              if ((num13 & 512) != 0)
                num13 |= -1024;
              Vector3 vector3_1 = new Vector3((float) num11 / 512f, (float) num12 / 512f, (float) num13 / 512f);
              MTX44 mtX44_2 = mtX44_1.Clone();
              mtX44_2[12] = 0.0f;
              mtX44_2[13] = 0.0f;
              mtX44_2[14] = 0.0f;
              float[] numArray3 = mtX44_2.MultVector(new float[3]
              {
                vector3_1.X,
                vector3_1.Y,
                vector3_1.Z
              });
              vector3_1 = new Vector3(numArray3[0], numArray3[1], numArray3[2]);
              Vector3[] vector3Array1 = new Vector3[4];
              Vector3[] vector3Array2 = new Vector3[4];
              Vector3[] vector3Array3 = new Vector3[4];
              for (int index5 = 0; index5 < 4; ++index5)
              {
                if (Context.LightEnabled[index5])
                {
                  float num14 = Math.Max(Math.Min(Math.Max(0.0f, Vector3.Dot(-Context.LightVectors[index5], vector3_1)), 1f), 0.0f);
                  vector3Array1[index5] = Vector3.Multiply(num14 * Context.LightColors[index5].ToVector3(), Context.DiffuseColor.ToVector3());
                  vector3Array2[index5] = Vector3.Multiply(Context.LightColors[index5].ToVector3(), Context.AmbientColor.ToVector3());
                  float num15 = Math.Max(Math.Min(Math.Max(0.0f, (float) Math.Cos(2.0 * (double) Vector3.CalculateAngle(-((Context.LightVectors[index5] + new Vector3(0.0f, 0.0f, -1f)) / 2f), vector3_1))), 1f), 0.0f);
                  vector3Array3[index5] = !Context.UseSpecularReflectionTable ? Vector3.Multiply(num15 * Context.LightColors[index5].ToVector3(), Context.SpecularColor.ToVector3()) : Vector3.Multiply((float) Context.SpecularReflectionTable[(int) ((double) num15 * (double) sbyte.MaxValue)] / (float) byte.MaxValue * Context.LightColors[index5].ToVector3(), Context.SpecularColor.ToVector3());
                }
              }
              Vector3 vector3_2 = Context.EmissionColor.ToVector3();
              for (int index6 = 0; index6 < 4; ++index6)
              {
                if (Context.LightEnabled[index6])
                  vector3_2 += vector3Array1[index6] + vector3Array2[index6] + vector3Array3[index6];
              }
              vector3_2.X = Math.Min(1f, vector3_2.X);
              vector3_2.Y = Math.Min(1f, vector3_2.Y);
              vector3_2.Z = Math.Min(1f, vector3_2.Z);
              Gl.glColor4f(vector3_2.X, vector3_2.Y, vector3_2.Z, (float) Context.Alpha / 31f);
              break;
            case 34:
              int num16 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num17 = num16 & (int) ushort.MaxValue;
              if ((num17 & 32768) != 0)
                num17 |= -65536;
              int num18 = num16 >> 16 & (int) ushort.MaxValue;
              if ((num18 & 32768) != 0)
                num18 |= -65536;
              Gl.glTexCoord2f((float) num17 / 16f, (float) num18 / 16f);
              break;
            case 35:
              int num19 = Bytes.Read4BytesAsInt32(polydata, offset1);
              int offset6 = offset1 + 4;
              int num20 = GlNitro.sign(num19 & (int) ushort.MaxValue, 16);
              int num21 = GlNitro.sign(num19 >> 16 & (int) ushort.MaxValue, 16);
              int num22 = Bytes.Read4BytesAsInt32(polydata, offset6);
              offset1 = offset6 + 4;
              int num23 = GlNitro.sign(num22 & (int) ushort.MaxValue, 16);
              v[0] = (float) num20 / 4096f;
              v[1] = (float) num21 / 4096f;
              v[2] = (float) num23 / 4096f;
              Gl.glVertex3fv(m.MultVector(v));
              break;
            case 36:
              int num24 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num25 = GlNitro.sign(num24 & 1023, 10);
              int num26 = GlNitro.sign(num24 >> 10 & 1023, 10);
              int num27 = GlNitro.sign(num24 >> 20 & 1023, 10);
              v[0] = (float) num25 / 64f;
              v[1] = (float) num26 / 64f;
              v[2] = (float) num27 / 64f;
              Gl.glVertex3fv(m.MultVector(v));
              break;
            case 37:
              int num28 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num29 = GlNitro.sign(num28 & (int) ushort.MaxValue, 16);
              int num30 = GlNitro.sign(num28 >> 16 & (int) ushort.MaxValue, 16);
              v[0] = (float) num29 / 4096f;
              v[1] = (float) num30 / 4096f;
              Gl.glVertex3fv(m.MultVector(v));
              break;
            case 38:
              int num31 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num32 = GlNitro.sign(num31 & (int) ushort.MaxValue, 16);
              int num33 = GlNitro.sign(num31 >> 16 & (int) ushort.MaxValue, 16);
              v[0] = (float) num32 / 4096f;
              v[2] = (float) num33 / 4096f;
              Gl.glVertex3fv(m.MultVector(v));
              break;
            case 39:
              int num34 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num35 = GlNitro.sign(num34 & (int) ushort.MaxValue, 16);
              int num36 = GlNitro.sign(num34 >> 16 & (int) ushort.MaxValue, 16);
              v[1] = (float) num35 / 4096f;
              v[2] = (float) num36 / 4096f;
              Gl.glVertex3fv(m.MultVector(v));
              break;
            case 40:
              int num37 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num38 = GlNitro.sign(num37 & 1023, 10);
              int num39 = GlNitro.sign(num37 >> 10 & 1023, 10);
              int num40 = GlNitro.sign(num37 >> 20 & 1023, 10);
              v[0] += (float) num38 / 4096f;
              v[1] += (float) num39 / 4096f;
              v[2] += (float) num40 / 4096f;
              Gl.glVertex3fv(m.MultVector(v));
              break;
            case 41:
              Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              break;
            case 42:
              offset1 += 4;
              break;
            case 43:
              offset1 += 4;
              break;
            case 48:
              uint num41 = Bytes.Read4BytesAsUInt32(polydata, offset1);
              offset1 += 4;
              Color color1 = Graphic.ConvertABGR1555((short) ((int) num41 & (int) short.MaxValue));
              Color color2 = Graphic.ConvertABGR1555((short) ((int) (num41 >> 16) & (int) short.MaxValue));
              if (((int) num41 >> 15 & 1) == 1)
                Gl.glColor4f((float) color1.R / (float) byte.MaxValue, (float) color1.G / (float) byte.MaxValue, (float) color1.B / (float) byte.MaxValue, (float) Context.Alpha / 31f);
              Context.DiffuseColor = color1;
              Context.AmbientColor = color2;
              break;
            case 49:
              offset1 += 4;
              break;
            case 50:
              offset1 += 4;
              int num42 = (int) MessageBox.Show("0x32: Light Vector");
              break;
            case 51:
              offset1 += 4;
              break;
            case 52:
              offset1 += 128;
              break;
            case 64:
              int num43 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              num1 = num43;
              int mode;
              switch (num43)
              {
                case 0:
                  mode = 4;
                  break;
                case 1:
                  mode = 7;
                  break;
                case 2:
                  mode = 5;
                  break;
                case 3:
                  mode = 8;
                  break;
                default:
                  throw new Exception();
              }
              Gl.glBegin(mode);
              break;
            case 65:
              Gl.glEnd();
              break;
            case 80:
              offset1 += 4;
              break;
            case 96:
              offset1 += 4;
              break;
            case 112:
              offset1 += 12;
              break;
            case 113:
              offset1 += 8;
              break;
            case 114:
              offset1 += 4;
              break;
          }
        }
      }
    }

    public static Group glNitroGxRipper(
      byte[] polydata,
      MTX44 Curmtx,
      int Alpha,
      ref MTX44[] MatrixStack,
      int PosScale,
      NSBMD.ModelSet.Model.MaterialSet.Material m)
    {
      int num1 = -1;
      if (polydata == null)
        return (Group) null;
      int offset1 = 0;
      int length = polydata.Length;
      int[] numArray1 = new int[4];
      float[] v = new float[3];
      float[] numArray2 = new float[3];
      MTX44 m1 = Curmtx;
      Group group = new Group();
      Vector3 vector3 = new Vector3(float.NaN, 0.0f, 0.0f);
      Color color = Color.White;
      Vector2 vector2 = new Vector2(float.NaN, 0.0f);
      List<Vector3> vector3List1 = new List<Vector3>();
      List<Vector3> vector3List2 = new List<Vector3>();
      List<Vector2> vector2List = new List<Vector2>();
      while (offset1 < length)
      {
        for (int index = 0; index < 4; ++index)
        {
          if (offset1 >= length)
          {
            numArray1[index] = (int) byte.MaxValue;
          }
          else
          {
            numArray1[index] = (int) polydata[offset1];
            ++offset1;
          }
        }
        for (int index1 = 0; index1 < 4 && offset1 < length; ++index1)
        {
          switch (numArray1[index1])
          {
            case 16:
              int num2 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              switch (num2)
              {
                default:
                  continue;
              }
            case 18:
              offset1 += 4;
              break;
            case 19:
              offset1 += 4;
              break;
            case 20:
              int index2 = Bytes.Read4BytesAsInt32(polydata, offset1) & 31;
              offset1 += 4;
              MatrixStack[index2].CopyValuesTo(m1);
              break;
            case 21:
              m1.LoadIdentity();
              break;
            case 22:
              for (int index3 = 0; index3 < 16; ++index3)
              {
                m1[index3] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                offset1 += 4;
              }
              break;
            case 23:
              for (int y = 0; y < 4; ++y)
              {
                int x = 0;
                while (x < 3)
                {
                  m1[x, y] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                  offset1 += 4;
                  ++y;
                }
              }
              break;
            case 24:
              MTX44 b1 = new MTX44();
              b1.LoadIdentity();
              for (int index4 = 0; index4 < 16; ++index4)
              {
                b1[index4] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                offset1 += 4;
              }
              m1.MultMatrix(b1).CopyValuesTo(m1);
              break;
            case 25:
              MTX44 b2 = new MTX44();
              b2.LoadIdentity();
              for (int y = 0; y < 4; ++y)
              {
                int x = 0;
                while (x < 3)
                {
                  b2[x, y] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                  offset1 += 4;
                  ++y;
                }
              }
              m1.MultMatrix(b2).CopyValuesTo(m1);
              break;
            case 26:
              MTX44 b3 = new MTX44();
              b3.LoadIdentity();
              for (int y = 0; y < 3; ++y)
              {
                int x = 0;
                while (x < 3)
                {
                  b3[x, y] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                  offset1 += 4;
                  ++y;
                }
              }
              m1.MultMatrix(b3).CopyValuesTo(m1);
              break;
            case 27:
              int num3 = Bytes.Read4BytesAsInt32(polydata, offset1);
              int offset2 = offset1 + 4;
              int num4 = Bytes.Read4BytesAsInt32(polydata, offset2);
              int offset3 = offset2 + 4;
              int num5 = Bytes.Read4BytesAsInt32(polydata, offset3);
              offset1 = offset3 + 4;
              m1.Scale((float) num3 / 4096f / (float) PosScale, (float) num4 / 4096f / (float) PosScale, (float) num5 / 4096f / (float) PosScale);
              break;
            case 28:
              int data1 = Bytes.Read4BytesAsInt32(polydata, offset1);
              int offset4 = offset1 + 4;
              int data2 = Bytes.Read4BytesAsInt32(polydata, offset4);
              int offset5 = offset4 + 4;
              int data3 = Bytes.Read4BytesAsInt32(polydata, offset5);
              offset1 = offset5 + 4;
              m1.translate((float) GlNitro.sign(data1, 32) / 4096f / (float) PosScale, (float) GlNitro.sign(data2, 32) / 4096f / (float) PosScale, (float) GlNitro.sign(data3, 32) / 4096f / (float) PosScale);
              break;
            case 32:
              int Data = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num6 = Data & 31;
              int num7 = Data >> 5 & 31;
              int num8 = Data >> 10 & 31;
              color = Graphic.ConvertABGR1555((short) Data);
              break;
            case 33:
              int num9 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num10 = num9 & 1023;
              if ((num10 & 512) != 0)
                num10 |= -1024;
              int num11 = num9 >> 10 & 1023;
              if ((num11 & 512) != 0)
                num11 |= -1024;
              int num12 = num9 >> 20 & 1023;
              if ((num12 & 512) != 0)
                num12 |= -1024;
              vector3 = new Vector3((float) num10 / 512f, (float) num11 / 512f, (float) num12 / 512f);
              break;
            case 34:
              int num13 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num14 = num13 & (int) ushort.MaxValue;
              if ((num14 & 32768) != 0)
                num14 |= -65536;
              int num15 = num13 >> 16 & (int) ushort.MaxValue;
              if ((num15 & 32768) != 0)
                num15 |= -65536;
              vector2 = new Vector2((float) (((double) m.scaleS == 0.0 ? 1.0 : (double) m.scaleS) / (double) m.origWidth * ((double) num14 / 16.0)) / (float) (uint) (((int) (m.texImageParam >> 18) & 1) + 1), (float) (-(((double) m.scaleT == 0.0 ? 1.0 : (double) m.scaleT) / (double) m.origHeight) * ((double) num15 / 16.0)) / (float) (uint) (((int) (m.texImageParam >> 19) & 1) + 1));
              break;
            case 35:
              int num16 = Bytes.Read4BytesAsInt32(polydata, offset1);
              int offset6 = offset1 + 4;
              int num17 = GlNitro.sign(num16 & (int) ushort.MaxValue, 16);
              int num18 = GlNitro.sign(num16 >> 16 & (int) ushort.MaxValue, 16);
              int num19 = Bytes.Read4BytesAsInt32(polydata, offset6);
              offset1 = offset6 + 4;
              int num20 = GlNitro.sign(num19 & (int) ushort.MaxValue, 16);
              v[0] = (float) num17 / 4096f;
              v[1] = (float) num18 / 4096f;
              v[2] = (float) num20 / 4096f;
              float[] numArray3 = m1.MultVector(v);
              vector3List1.Add(new Vector3(numArray3[0], numArray3[1], numArray3[2]));
              vector3List2.Add(vector3);
              vector2List.Add(vector2);
              break;
            case 36:
              int num21 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num22 = GlNitro.sign(num21 & 1023, 10);
              int num23 = GlNitro.sign(num21 >> 10 & 1023, 10);
              int num24 = GlNitro.sign(num21 >> 20 & 1023, 10);
              v[0] = (float) num22 / 64f;
              v[1] = (float) num23 / 64f;
              v[2] = (float) num24 / 64f;
              float[] numArray4 = m1.MultVector(v);
              vector3List1.Add(new Vector3(numArray4[0], numArray4[1], numArray4[2]));
              vector3List2.Add(vector3);
              vector2List.Add(vector2);
              break;
            case 37:
              int num25 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num26 = GlNitro.sign(num25 & (int) ushort.MaxValue, 16);
              int num27 = GlNitro.sign(num25 >> 16 & (int) ushort.MaxValue, 16);
              v[0] = (float) num26 / 4096f;
              v[1] = (float) num27 / 4096f;
              float[] numArray5 = m1.MultVector(v);
              vector3List1.Add(new Vector3(numArray5[0], numArray5[1], numArray5[2]));
              vector3List2.Add(vector3);
              vector2List.Add(vector2);
              break;
            case 38:
              int num28 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num29 = GlNitro.sign(num28 & (int) ushort.MaxValue, 16);
              int num30 = GlNitro.sign(num28 >> 16 & (int) ushort.MaxValue, 16);
              v[0] = (float) num29 / 4096f;
              v[2] = (float) num30 / 4096f;
              float[] numArray6 = m1.MultVector(v);
              vector3List1.Add(new Vector3(numArray6[0], numArray6[1], numArray6[2]));
              vector3List2.Add(vector3);
              vector2List.Add(vector2);
              break;
            case 39:
              int num31 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num32 = GlNitro.sign(num31 & (int) ushort.MaxValue, 16);
              int num33 = GlNitro.sign(num31 >> 16 & (int) ushort.MaxValue, 16);
              v[1] = (float) num32 / 4096f;
              v[2] = (float) num33 / 4096f;
              float[] numArray7 = m1.MultVector(v);
              vector3List1.Add(new Vector3(numArray7[0], numArray7[1], numArray7[2]));
              vector3List2.Add(vector3);
              vector2List.Add(vector2);
              break;
            case 40:
              int num34 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num35 = GlNitro.sign(num34 & 1023, 10);
              int num36 = GlNitro.sign(num34 >> 10 & 1023, 10);
              int num37 = GlNitro.sign(num34 >> 20 & 1023, 10);
              v[0] += (float) num35 / 4096f;
              v[1] += (float) num36 / 4096f;
              v[2] += (float) num37 / 4096f;
              float[] numArray8 = m1.MultVector(v);
              vector3List1.Add(new Vector3(numArray8[0], numArray8[1], numArray8[2]));
              vector3List2.Add(vector3);
              vector2List.Add(vector2);
              break;
            case 41:
              Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              break;
            case 42:
              offset1 += 4;
              int num38 = (int) MessageBox.Show("");
              break;
            case 43:
              offset1 += 4;
              int num39 = (int) MessageBox.Show("");
              break;
            case 48:
              uint num40 = Bytes.Read4BytesAsUInt32(polydata, offset1);
              offset1 += 4;
              Graphic.ConvertABGR1555((short) ((int) num40 & (int) short.MaxValue));
              Graphic.ConvertABGR1555((short) ((int) (num40 >> 16) & (int) short.MaxValue));
              break;
            case 49:
              offset1 += 4;
              break;
            case 50:
              offset1 += 4;
              int num41 = (int) MessageBox.Show("0x32: Light Vector");
              break;
            case 51:
              offset1 += 4;
              int num42 = (int) MessageBox.Show("");
              break;
            case 52:
              offset1 += 128;
              int num43 = (int) MessageBox.Show("");
              break;
            case 64:
              int num44 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              num1 = num44;
              break;
            case 65:
              switch (num1)
              {
                case 0:
                  group.Add(new Polygon(PolygonType.Triangle, vector3List2.ToArray(), vector2List.ToArray(), vector3List1.ToArray()));
                  break;
                case 1:
                  group.Add(new Polygon(PolygonType.Quad, vector3List2.ToArray(), vector2List.ToArray(), vector3List1.ToArray()));
                  break;
                case 2:
                  group.Add(new Polygon(PolygonType.TriangleStrip, vector3List2.ToArray(), vector2List.ToArray(), vector3List1.ToArray()));
                  break;
                case 3:
                  group.Add(new Polygon(PolygonType.QuadStrip, vector3List2.ToArray(), vector2List.ToArray(), vector3List1.ToArray()));
                  break;
              }
              vector3List2.Clear();
              vector3List1.Clear();
              vector2List.Clear();
              break;
            case 80:
              offset1 += 4;
              int num45 = (int) MessageBox.Show("");
              break;
            case 96:
              offset1 += 4;
              int num46 = (int) MessageBox.Show("");
              break;
            case 112:
              offset1 += 12;
              int num47 = (int) MessageBox.Show("");
              break;
            case 113:
              offset1 += 8;
              int num48 = (int) MessageBox.Show("");
              break;
            case 114:
              offset1 += 4;
              int num49 = (int) MessageBox.Show("");
              break;
          }
        }
      }
      return group;
    }

    private static int sign(int data, int size)
    {
      if ((data & 1 << size - 1) != 0)
        data |= -1 << size;
      return data;
    }

    public static float[] glNitroPivot(float[] ab, int pv, int neg)
    {
      float[] numArray = new float[16];
      numArray[15] = 1f;
      float num1 = 1f;
      float num2 = ab[0];
      float num3 = ab[1];
      float num4 = num2;
      float num5 = num3;
      switch (neg)
      {
        case 1:
        case 3:
        case 5:
        case 7:
        case 9:
        case 11:
        case 13:
        case 15:
          num1 = -1f;
          break;
      }
      switch (neg - 2)
      {
        case 0:
        case 1:
        case 4:
        case 5:
        case 8:
        case 9:
        case 12:
        case 13:
          num5 = -num5;
          break;
      }
      switch (neg - 4)
      {
        case 0:
        case 1:
        case 2:
        case 3:
        case 8:
        case 9:
        case 10:
        case 11:
          num4 = -num4;
          break;
      }
      switch (pv)
      {
        case 0:
          numArray[0] = num1;
          numArray[5] = num2;
          numArray[6] = num3;
          numArray[9] = num5;
          numArray[10] = num4;
          break;
        case 1:
          numArray[1] = num1;
          numArray[4] = num2;
          numArray[6] = num3;
          numArray[8] = num5;
          numArray[10] = num4;
          break;
        case 2:
          numArray[2] = num1;
          numArray[4] = num2;
          numArray[5] = num3;
          numArray[8] = num5;
          numArray[9] = num4;
          break;
        case 3:
          numArray[4] = num1;
          numArray[1] = num2;
          numArray[2] = num3;
          numArray[9] = num5;
          numArray[10] = num4;
          break;
        case 4:
          numArray[5] = num1;
          numArray[0] = num2;
          numArray[2] = num3;
          numArray[8] = num5;
          numArray[10] = num4;
          break;
        case 5:
          numArray[6] = num1;
          numArray[0] = num2;
          numArray[1] = num3;
          numArray[8] = num5;
          numArray[9] = num4;
          break;
        case 6:
          numArray[8] = num1;
          numArray[1] = num2;
          numArray[2] = num3;
          numArray[5] = num5;
          numArray[6] = num4;
          break;
        case 7:
          numArray[9] = num1;
          numArray[0] = num2;
          numArray[2] = num3;
          numArray[4] = num5;
          numArray[6] = num4;
          break;
        case 8:
          numArray[10] = num1;
          numArray[0] = num2;
          numArray[1] = num3;
          numArray[4] = num5;
          numArray[5] = num4;
          break;
        case 9:
          numArray[0] = -num2;
          break;
      }
      return numArray;
    }

    private static void CreateBillboardMatrix(
      float[] matrix,
      Vector3 right,
      Vector3 up,
      Vector3 look,
      Vector3 pos)
    {
      matrix[0] = right.X;
      matrix[1] = right.Y;
      matrix[2] = right.Z;
      matrix[3] = 0.0f;
      matrix[4] = up.X;
      matrix[5] = up.Y;
      matrix[6] = up.Z;
      matrix[7] = 0.0f;
      matrix[8] = look.X;
      matrix[9] = look.Y;
      matrix[10] = look.Z;
      matrix[11] = 0.0f;
      matrix[12] = pos.X;
      matrix[13] = pos.Y;
      matrix[14] = pos.Z;
      matrix[15] = 1f;
    }

    public static float[] BillboardPoint(Vector3 pos, Vector3 camPos, Vector3 camUp)
    {
      Vector3 vector3 = camPos - pos;
      vector3.Normalize();
      Vector3 right = Vector3.Cross(camUp, vector3);
      Vector3 up = Vector3.Cross(vector3, right);
      float[] matrix = new float[16];
      GlNitro.CreateBillboardMatrix(matrix, right, up, vector3, pos);
      return matrix;
    }

    public static System.Drawing.Bitmap ScreenShot(SimpleOpenGlControl simpleOpenGlControl1)
    {
      System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(simpleOpenGlControl1.Width, simpleOpenGlControl1.Height);
      BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, simpleOpenGlControl1.Width, simpleOpenGlControl1.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
      Gl.glReadPixels(0, 0, simpleOpenGlControl1.Width, simpleOpenGlControl1.Height, 32993, 5121, bitmapdata.Scan0);
      bitmap.UnlockBits(bitmapdata);
      bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
      return bitmap;
    }

    public static byte[] GenerateDl(
      Vector3[] Positions,
      Vector2[] TexCoords,
      Vector3[] Normals,
      Color[] Colors)
    {
      GlNitro.DisplayListEncoder displayListEncoder = new GlNitro.DisplayListEncoder();
      displayListEncoder.Begin((byte) 0);
      for (int index = 0; index < Positions.Length; index += 3)
      {
        displayListEncoder.TexCoord(TexCoords[index]);
        if (Colors != null)
          displayListEncoder.Color(Colors[index]);
        else
          displayListEncoder.Color(Color.White);
        Vector3 position1 = Positions[index];
        displayListEncoder.BestVertex(position1);
        displayListEncoder.TexCoord(TexCoords[index + 1]);
        if (Colors != null)
          displayListEncoder.Color(Colors[index + 1]);
        else
          displayListEncoder.Color(Color.White);
        Vector3 position2 = Positions[index + 1];
        displayListEncoder.BestVertex(position2);
        displayListEncoder.TexCoord(TexCoords[index + 2]);
        if (Colors != null)
          displayListEncoder.Color(Colors[index + 2]);
        else
          displayListEncoder.Color(Color.White);
        Vector3 position3 = Positions[index + 2];
        displayListEncoder.BestVertex(position3);
      }
      displayListEncoder.End();
      return displayListEncoder.GetDisplayList();
    }

    public static byte[] RemoveNormals(byte[] DL)
    {
      GlNitro.DisplayListRecoder displayListRecoder = new GlNitro.DisplayListRecoder(DL);
      displayListRecoder.RemoveNormals();
      return displayListRecoder.GetDisplayList();
    }

    public class Nitro3DContext
    {
      public int MatrixMode = 2;
      public MTX44[] MatrixStack = new MTX44[31];
      public Vector3[] LightVectors = new Vector3[4]
      {
        new Vector3(0.0f, -1f, -1f),
        new Vector3(0.998047f, -1f, 0.0f),
        new Vector3(0.0f, -1f, 0.998047f),
        new Vector3(-1f, -1f, 0.0f)
      };
      public Color[] LightColors = new Color[4]
      {
        Color.White,
        Color.White,
        Color.White,
        Color.White
      };
      public bool[] LightEnabled = new bool[4];
      public byte[] SpecularReflectionTable = new byte[128]
      {
        (byte) 0,
        (byte) 2,
        (byte) 4,
        (byte) 6,
        (byte) 8,
        (byte) 10,
        (byte) 12,
        (byte) 14,
        (byte) 16,
        (byte) 18,
        (byte) 20,
        (byte) 22,
        (byte) 24,
        (byte) 26,
        (byte) 28,
        (byte) 30,
        (byte) 32,
        (byte) 34,
        (byte) 36,
        (byte) 38,
        (byte) 40,
        (byte) 42,
        (byte) 44,
        (byte) 46,
        (byte) 48,
        (byte) 50,
        (byte) 52,
        (byte) 54,
        (byte) 56,
        (byte) 58,
        (byte) 60,
        (byte) 62,
        (byte) 64,
        (byte) 66,
        (byte) 68,
        (byte) 70,
        (byte) 72,
        (byte) 74,
        (byte) 76,
        (byte) 78,
        (byte) 80,
        (byte) 82,
        (byte) 84,
        (byte) 86,
        (byte) 88,
        (byte) 90,
        (byte) 92,
        (byte) 94,
        (byte) 96,
        (byte) 98,
        (byte) 100,
        (byte) 102,
        (byte) 104,
        (byte) 106,
        (byte) 108,
        (byte) 110,
        (byte) 112,
        (byte) 114,
        (byte) 116,
        (byte) 118,
        (byte) 120,
        (byte) 122,
        (byte) 124,
        (byte) 126,
        (byte) 129,
        (byte) 131,
        (byte) 133,
        (byte) 135,
        (byte) 137,
        (byte) 139,
        (byte) 141,
        (byte) 143,
        (byte) 145,
        (byte) 147,
        (byte) 149,
        (byte) 151,
        (byte) 153,
        (byte) 155,
        (byte) 157,
        (byte) 159,
        (byte) 161,
        (byte) 163,
        (byte) 165,
        (byte) 167,
        (byte) 169,
        (byte) 171,
        (byte) 173,
        (byte) 175,
        (byte) 177,
        (byte) 179,
        (byte) 181,
        (byte) 183,
        (byte) 185,
        (byte) 187,
        (byte) 189,
        (byte) 191,
        (byte) 193,
        (byte) 195,
        (byte) 197,
        (byte) 199,
        (byte) 201,
        (byte) 203,
        (byte) 205,
        (byte) 207,
        (byte) 209,
        (byte) 211,
        (byte) 213,
        (byte) 215,
        (byte) 217,
        (byte) 219,
        (byte) 221,
        (byte) 223,
        (byte) 225,
        (byte) 227,
        (byte) 229,
        (byte) 231,
        (byte) 233,
        (byte) 235,
        (byte) 237,
        (byte) 239,
        (byte) 241,
        (byte) 243,
        (byte) 245,
        (byte) 247,
        (byte) 249,
        (byte) 251,
        (byte) 253,
        byte.MaxValue
      };
      public bool UseSpecularReflectionTable;
      public Color DiffuseColor;
      public Color AmbientColor;
      public Color SpecularColor;
      public Color EmissionColor;
      public int Alpha = 31;

      public Nitro3DContext()
      {
        for (int index = 0; index < 31; ++index)
          this.MatrixStack[index] = new MTX44();
      }
    }

    public class DisplayListEncoder
    {
      private List<byte> DisplayList = new List<byte>();
      private byte[] Commands = new byte[4];
      private int CommandId = 0;
      private List<byte> Args = new List<byte>();
      private Vector3 VtxState = new Vector3(float.NaN, float.NaN, float.NaN);

      public void Nop() => this.AddCommand((byte) 0);

      public void Color(Color c) => this.AddCommand((byte) 32, (uint) Graphic.encodeColor(c.ToArgb()));

      public void Normal(Vector3 Normal) => this.AddCommand((byte) 33, (uint) (((int) ((double) Normal.Z * 512.0) & 1023) << 20 | ((int) ((double) Normal.Y * 512.0) & 1023) << 10 | (int) ((double) Normal.X * 512.0) & 1023));

      public void TexCoord(Vector2 TexCoord) => this.AddCommand((byte) 34, (uint) (((int) ((double) TexCoord.Y * 16.0) & (int) ushort.MaxValue) << 16 | (int) ((double) TexCoord.X * 16.0) & (int) ushort.MaxValue));

      public void BestVertex(Vector3 Position)
      {
        if ((double) this.VtxState.X == (double) Position.X)
          this.VertexYZ(Position);
        else if ((double) this.VtxState.Y == (double) Position.Y)
          this.VertexXZ(Position);
        else if ((double) this.VtxState.Z == (double) Position.Z)
          this.VertexXY(Position);
        else if (((int) ((double) Math.Abs(Position.X) * 4096.0) & 63) == 0 && ((int) ((double) Math.Abs(Position.Y) * 4096.0) & 63) == 0 && ((int) ((double) Math.Abs(Position.Z) * 4096.0) & 63) == 0)
          this.Vertex10(Position);
        else
          this.Vertex(Position);
        this.VtxState = Position;
      }

      public void Vertex(Vector3 Position) => this.AddCommand((byte) 35, (uint) (((int) ((double) Position.Y * 4096.0) & (int) ushort.MaxValue) << 16 | (int) ((double) Position.X * 4096.0) & (int) ushort.MaxValue), (uint) (int) ((double) Position.Z * 4096.0) & (uint) ushort.MaxValue);

      public void Vertex10(Vector3 Position) => this.AddCommand((byte) 36, (uint) (((int) ((double) Position.Z * 64.0) & 1023) << 20 | ((int) ((double) Position.Y * 64.0) & 1023) << 10 | (int) ((double) Position.X * 64.0) & 1023));

      public void VertexXY(Vector3 Position) => this.AddCommand((byte) 37, (uint) (((int) ((double) Position.Y * 4096.0) & (int) ushort.MaxValue) << 16 | (int) ((double) Position.X * 4096.0) & (int) ushort.MaxValue));

      public void VertexXZ(Vector3 Position) => this.AddCommand((byte) 38, (uint) (((int) ((double) Position.Z * 4096.0) & (int) ushort.MaxValue) << 16 | (int) ((double) Position.X * 4096.0) & (int) ushort.MaxValue));

      public void VertexYZ(Vector3 Position) => this.AddCommand((byte) 39, (uint) (((int) ((double) Position.Z * 4096.0) & (int) ushort.MaxValue) << 16 | (int) ((double) Position.Y * 4096.0) & (int) ushort.MaxValue));

      public void VertexDiff(Vector3 Position) => this.AddCommand((byte) 40, (uint) (((int) ((double) Position.Z * 4096.0) & 1023) << 20 | ((int) ((double) Position.Y * 4096.0) & 1023) << 10 | (int) ((double) Position.X * 4096.0) & 1023));

      public void Begin(byte VertexType) => this.AddCommand((byte) 64, (uint) VertexType);

      public void End()
      {
        if (this.CommandId == 0)
        {
          this.AddCommand((byte) 65);
          this.Flush();
          this.Flush();
        }
        else
          this.AddCommand((byte) 65);
      }

      public void Flush()
      {
        for (int commandId = this.CommandId; commandId < 4; ++commandId)
          this.Nop();
      }

      public void AddCommand(byte Id) => this.AddCommand(Id, new uint[0]);

      public void AddCommand(byte Id, params uint[] Params)
      {
        this.Commands[this.CommandId] = Id;
        ++this.CommandId;
        foreach (uint num in Params)
          this.Args.AddRange((IEnumerable<byte>) BitConverter.GetBytes(num));
        if (this.CommandId != 4)
          return;
        this.DisplayList.AddRange((IEnumerable<byte>) this.Commands);
        this.DisplayList.AddRange((IEnumerable<byte>) this.Args);
        this.Commands = new byte[4];
        this.Args.Clear();
        this.CommandId = 0;
      }

      public byte[] GetDisplayList()
      {
        if (this.CommandId != 0)
          this.Flush();
        this.Flush();
        return this.DisplayList.ToArray();
      }
    }

    private class DisplayListRecoder
    {
      private Dictionary<byte, int> NrParams = new Dictionary<byte, int>()
      {
        {
          (byte) 0,
          0
        },
        {
          (byte) 16,
          1
        },
        {
          (byte) 17,
          0
        },
        {
          (byte) 18,
          1
        },
        {
          (byte) 19,
          1
        },
        {
          (byte) 20,
          1
        },
        {
          (byte) 21,
          0
        },
        {
          (byte) 22,
          16
        },
        {
          (byte) 23,
          12
        },
        {
          (byte) 24,
          16
        },
        {
          (byte) 25,
          12
        },
        {
          (byte) 26,
          9
        },
        {
          (byte) 27,
          3
        },
        {
          (byte) 28,
          3
        },
        {
          (byte) 32,
          1
        },
        {
          (byte) 33,
          1
        },
        {
          (byte) 34,
          1
        },
        {
          (byte) 35,
          2
        },
        {
          (byte) 36,
          1
        },
        {
          (byte) 37,
          1
        },
        {
          (byte) 38,
          1
        },
        {
          (byte) 39,
          1
        },
        {
          (byte) 40,
          1
        },
        {
          (byte) 41,
          1
        },
        {
          (byte) 42,
          1
        },
        {
          (byte) 43,
          1
        },
        {
          (byte) 48,
          1
        },
        {
          (byte) 49,
          1
        },
        {
          (byte) 50,
          1
        },
        {
          (byte) 51,
          1
        },
        {
          (byte) 52,
          32
        },
        {
          (byte) 64,
          1
        },
        {
          (byte) 65,
          0
        },
        {
          (byte) 80,
          1
        },
        {
          (byte) 96,
          1
        },
        {
          (byte) 112,
          3
        },
        {
          (byte) 113,
          2
        },
        {
          (byte) 114,
          1
        },
        {
          byte.MaxValue,
          0
        }
      };
      private byte[] DL;

      public DisplayListRecoder(byte[] DL) => this.DL = DL != null ? DL : throw new ArgumentNullException("The display list can not be null.");

      public byte[] GetDisplayList() => this.DL;

      public void RemoveNormals() => this.RemoveCommand((byte) 33);

      private void RemoveCommand(byte Nr)
      {
        GlNitro.DisplayListEncoder displayListEncoder = new GlNitro.DisplayListEncoder();
        int offset = 0;
        int length = this.DL.Length;
        int[] numArray = new int[4];
        while (offset < length)
        {
          for (int index = 0; index < 4; ++index)
          {
            if (offset >= length)
            {
              numArray[index] = (int) byte.MaxValue;
            }
            else
            {
              numArray[index] = (int) this.DL[offset];
              ++offset;
            }
          }
          for (int index = 0; index < 4 && offset < length; ++index)
          {
            if (numArray[index] != (int) Nr)
              displayListEncoder.AddCommand((byte) numArray[index], Bytes.ReadMultipleBytesAsUInt32s(this.DL, offset, this.NrParams[Nr]));
            offset += this.NrParams[(byte) numArray[index]] * 4;
          }
        }
        this.DL = displayListEncoder.GetDisplayList();
      }
    }
  }
}


*/