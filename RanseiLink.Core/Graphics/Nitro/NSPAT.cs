using RanseiLink.Core.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace RanseiLink.Core.Graphics
{
    public class NSPAT
    {
        public const string RootElementName = "library_pattern_animations";
        public const string MagicNumber = "PAT0";
        public List<PatternAnimation> PatternAnimations { get; set; } = new List<PatternAnimation>();

        public NSPAT()
        {

        }

        public NSPAT(BinaryReader br)
        {
            var header = new NitroChunkHeader(br);
            if (header.MagicNumber != MagicNumber)
            {
                throw new InvalidDataException($"Unexpected magic number in chunk header '{header.MagicNumber}' (expected: {MagicNumber})");
            }
            var radix = new RadixDict<OffsetRadixData>(br);
            foreach (var name in radix.Names)
            {
                PatternAnimations.Add(new PatternAnimation(br) { Name = name });
            }
        }

        public void WriteTo(BinaryWriter bw)
        {
            var initOffset = bw.BaseStream.Position;

            // skip header and radix
            bw.Pad(NitroChunkHeader.Length + RadixDict<OffsetRadixData>.CalculateLength(PatternAnimations.Count));

            // write pattern animations
            var radix = new RadixDict<OffsetRadixData>();

            foreach (var animation in PatternAnimations)
            {
                radix.Data.Add(new OffsetRadixData { Offset = (uint)(bw.BaseStream.Position - initOffset) });
                radix.Names.Add(animation.Name);
                animation.WriteTo(bw);
            }

            // write header and radix
            var endOffset = bw.BaseStream.Position;
            bw.BaseStream.Position = initOffset;

            var header = new NitroChunkHeader
            {
                MagicNumber = MagicNumber,
                ChunkLength = (uint)(endOffset - initOffset)
            };
            header.WriteTo(bw);
            radix.WriteTo(bw);

            bw.BaseStream.Position = endOffset;
        }

        public XElement Serialize()
        {
            return new XElement(
                RootElementName,
                PatternAnimations.Select(x => x.Serialize())
                );
        }
        public static NSPAT Deserialize(XElement element)
        {
            var result = new NSPAT();
            foreach (var patternAnimEl in element.Elements("pattern_animation"))
            {
                result.PatternAnimations.Add(PatternAnimation.Deserialize(patternAnimEl));
            }
            return result;
        }

        public class PatternAnimation
        {
            public PatternAnimation()
            {

            }

            private struct Header
            {
                public const int Length = 8;

                public ushort NumFrames;
                public byte NumTextures;
                public byte NumPalettes;
                public ushort TexturesOffset;
                public ushort PalettesOffset;

                public Header(BinaryReader br)
                {
                    NumFrames = br.ReadUInt16();
                    NumTextures = br.ReadByte();
                    NumPalettes = br.ReadByte();
                    TexturesOffset = br.ReadUInt16();
                    PalettesOffset = br.ReadUInt16();
                }

                public void WriteTo(BinaryWriter bw)
                {
                    bw.Write(NumFrames);
                    bw.Write(NumTextures);
                    bw.Write(NumPalettes);
                    bw.Write(TexturesOffset);
                    bw.Write(PalettesOffset);
                }
            }

            public PatternAnimation(BinaryReader br)
            {
                var animHeader = new AnimationHeader(br);

                var header = new Header(br);
                NumFrames = header.NumFrames;

                var radix = new RadixDict<TrackRadixData>(br);

                var perTrackKeyFrameInfos = new KeyFrameInfo[radix.Names.Count][];

                for (int i = 0; i < radix.Names.Count; i++)
                {
                    var data = radix.Data[i];
                    perTrackKeyFrameInfos[i] = new KeyFrameInfo[data.NumKeyFrames];
                    for (int j = 0; j < data.NumKeyFrames; j++)
                    {
                        perTrackKeyFrameInfos[i][j] = new KeyFrameInfo(br);
                    }
                }

                var textureNames = new string[header.NumTextures];
                for (int i = 0; i < header.NumTextures; i++)
                {
                    textureNames[i] = RadixDict<TrackRadixData>.ReadName(br);
                }

                var paletteNames = new string[header.NumPalettes];
                for (int i = 0; i < header.NumPalettes; i++)
                {
                    paletteNames[i] = RadixDict<TrackRadixData>.ReadName(br);
                }

                for (int i = 0; i < radix.Names.Count; i++)
                {
                    var track = new PatternAnimationTrack { Material = radix.Names[i], Unknown = radix.Data[i].Unknown };
                    Tracks.Add(track);
                    var trackKeyFrameInfos = perTrackKeyFrameInfos[i];
                    foreach (var info in trackKeyFrameInfos)
                    {
                        track.KeyFrames.Add(new KeyFrame
                        {
                            Frame = info.Frame,
                            Texture = textureNames[info.TextureIndex],
                            Palette = paletteNames[info.PaletteIndex]
                        });
                    }

                }

            }

            public void WriteTo(BinaryWriter bw)
            {
                var initOffset = bw.BaseStream.Position;

                // skip header
                bw.Pad(Header.Length + RadixDict<TrackRadixData>.CalculateLength(Tracks.Count));

                // write tracks
                List<string> textures = new List<string>();
                List<string> palettes = new List<string>();
                var radix = new RadixDict<TrackRadixData>();
                foreach (var track in Tracks)
                {
                    radix.Names.Add(track.Material);
                    radix.Data.Add(new TrackRadixData
                    {
                        NumKeyFrames = (ushort)track.KeyFrames.Count,
                        Offset = (ushort)(bw.BaseStream.Position - initOffset),
                        Flag = 0,
                        Unknown = 1, // TODO
                    });
                    foreach (var frame in track.KeyFrames)
                    {
                        var texIndex = textures.IndexOf(frame.Texture);
                        if (texIndex == -1)
                        {
                            texIndex = textures.Count;
                            textures.Add(frame.Texture);
                        }
                        var palIndex = textures.IndexOf(frame.Palette);
                        if (palIndex == -1)
                        {
                            palIndex = palettes.Count;
                            palettes.Add(frame.Palette);
                        }
                        var info = new KeyFrameInfo
                        {
                            Frame = frame.Frame,
                            TextureIndex = (byte)texIndex,
                            PaletteIndex = (byte)palIndex
                        };
                        info.WriteTo(bw);
                    }
                }

                ushort texturesOffset = (ushort)(bw.BaseStream.Position - initOffset);
                foreach (var texture in textures)
                {
                    RadixDict<TrackRadixData>.WriteName(bw, texture);
                }

                ushort palettesOffset = (ushort)(bw.BaseStream.Position - initOffset);
                foreach (var palette in palettes)
                {
                    RadixDict<TrackRadixData>.WriteName(bw, palette);
                }


                // write header
                var endOffset = bw.BaseStream.Position;
                bw.BaseStream.Position = initOffset;
                var header = new AnimationHeader
                {
                    Category = "M",
                    Revision = 0,
                    SubCategory = "PT"
                };

                header.WriteTo(bw);
                bw.Write(NumFrames);
                bw.Write((byte)textures.Count);
                bw.Write((byte)palettes.Count);
                bw.Write(texturesOffset);
                bw.Write(palettesOffset);

                bw.BaseStream.Position = endOffset;
            }

            public string Name { get; set; }
            public ushort NumFrames { get; set; }
            public List<PatternAnimationTrack> Tracks { get; set; } = new List<PatternAnimationTrack>();

            public XElement Serialize()
            {
                var el = new XElement("pattern_animation");
                if (!string.IsNullOrEmpty(Name))
                {
                    el.Add(new XAttribute("name", Name));
                }
                if (NumFrames != 0)
                {
                    el.Add(new XAttribute("num_frames", NumFrames));
                }
                el.Add(Tracks.Select(x => x.Serialize()));
                return el;
            }

            public static PatternAnimation Deserialize(XElement element)
            {
                var result = new PatternAnimation();

                result.Name = element.Attribute("name")?.Value;
                result.NumFrames = ushort.TryParse(element.Attribute("num_frames")?.Value, out ushort numFrames) ? numFrames : (ushort)0;

                foreach (var trackEl in element.Elements("track"))
                {
                     result.Tracks.Add(PatternAnimationTrack.Deserialize(trackEl));
                }
                return result;
            }
        }

        public class PatternAnimationTrack
        {
            public string Material { get; set; }
            public float Unknown { get; set; }
            public List<KeyFrame> KeyFrames { get; set; } = new List<KeyFrame>();

            public XElement Serialize()
            {
                var el =  new XElement("track");
                if (!string.IsNullOrEmpty(Material))
                {
                    el.Add(new XAttribute("material", Material));
                }
                if (Unknown != 0)
                {
                    el.Add(new XAttribute("unknown", Unknown));
                }
                el.Add(KeyFrames.Select(x => x.Serialize()));
                
                return el;
            }

            public static PatternAnimationTrack Deserialize(XElement element)
            {
                var result = new PatternAnimationTrack
                {
                    Material = element.Attribute("material")?.Value,
                    Unknown = float.TryParse(element.Attribute("unknown")?.Value, out var unk) ? unk : 0
                };

                foreach (var keyFrameElement in element.Elements("key_frame"))
                {
                    result.KeyFrames.Add(KeyFrame.Deserialize(keyFrameElement));
                }

                return result;
            }
        }

        public class KeyFrame
        {
            public ushort Frame { get; set; }
            public string Texture { get; set; }
            public string Palette { get; set; }

            public XElement Serialize()
            {
                var el = new XElement("key_frame");
                el.Add(new XAttribute("frame", Frame));
                
                if (!string.IsNullOrEmpty(Texture))
                {
                    el.Add(new XAttribute("texture", Texture));
                }
                if (!string.IsNullOrEmpty(Palette))
                {
                    el.Add(new XAttribute("palette", Palette));
                }
                return el;
            }

            public static KeyFrame Deserialize(XElement element)
            {
                return new KeyFrame
                {
                    Frame = ushort.TryParse(element.Attribute("frame")?.Value, out ushort frame) ? frame : (ushort)0,
                    Texture = element.Attribute("texture")?.Value,
                    Palette = element.Attribute("palette")?.Value,
                };
            }
        }

        public struct KeyFrameInfo
        {
            public ushort Frame;
            public byte TextureIndex;
            public byte PaletteIndex;

            public KeyFrameInfo(BinaryReader br)
            {
                Frame = br.ReadUInt16();
                TextureIndex = br.ReadByte();
                PaletteIndex = br.ReadByte();
            }

            public void WriteTo(BinaryWriter bw)
            {
                bw.Write(Frame);
                bw.Write(TextureIndex);
                bw.Write(PaletteIndex);
            }
        }

        private class TrackRadixData : IRadixData
        {
            public ushort Length => 8;

            public ushort NumKeyFrames;
            public ushort Flag;
            public float Unknown; // fixed point
            public ushort Offset;

            public void ReadFrom(BinaryReader br)
            {
                NumKeyFrames = br.ReadUInt16();
                Flag = br.ReadUInt16();
                if (Flag != 0)
                {
                    throw new System.Exception("The flag in KeyFrameRadixData is not zero!");
                }
                Unknown = FixedPoint.Fix(br.ReadUInt16(), 0, 7, 9);
                Offset = br.ReadUInt16();
            }

            public void WriteTo(BinaryWriter bw)
            {
                bw.Write(NumKeyFrames);
                bw.Write(Flag);
                bw.Write((ushort)FixedPoint.InverseFix(Unknown, 0, 7, 9));
                bw.Write(Offset);
            }
        }
    }

    public struct AnimationHeader
    {
        public string Category;
        public byte Revision;
        public string SubCategory;

        public AnimationHeader(BinaryReader br)
        {
            Category = Encoding.UTF8.GetString(br.ReadBytes(1));
            if (Category == "M")
            {
                Revision = br.ReadByte();
                SubCategory = Encoding.UTF8.GetString(br.ReadBytes(2));
            }
            else
            {
                Revision = 0;
                SubCategory = null;
            }
        }

        public void WriteTo(BinaryWriter bw)
        {
            bw.Write(Encoding.UTF8.GetBytes(Category));
            if (Category == "M")
            {
                bw.Write(Revision);
                bw.Write(Encoding.UTF8.GetBytes(SubCategory));
            }
        }
    }

}
