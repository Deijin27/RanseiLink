using System.Xml.Linq;

namespace RanseiLink.Core.Graphics.Conquest;

/// <summary>
/// Minimal format for Pattern animations used with pokemon models along with the other pattern animation file
/// </summary>
public static class NSPAT_RAW
{
    public const string RootElementName = "library_raw_pattern_animations";
    public static NSPAT Load(string file)
    {
        using (var br = new BinaryReader(File.OpenRead(file)))
        {
            return Load(br);
        }
    }

    public static XElement SerializeRaw(this NSPAT nspat)
    {
        return new XElement(
            RootElementName,
            nspat.PatternAnimations.Select(x => x.Serialize())
            );
    }

    private static readonly string[] _animNames = new string[]
    {
        "FN_A",
        "BN_A",
        "FN_B",
        "BN_B",
        "FN_C",
        "BN_C",
        "FN_D",
        "BN_D",
    };

    public static NSPAT Load(BinaryReader br)
    {
        var result = new NSPAT();
        ushort[] kfCounts = new ushort[8];
        for (int i = 0; i < 8; i++)
        {
            kfCounts[i] = br.ReadUInt16();
        }
        var start = br.BaseStream.Position;
        for (int i = 0; i < kfCounts.Length; i++)
        {
            var anim = new NSPAT.PatternAnimation(_animNames[i]);
            result.PatternAnimations.Add(anim);
            var track = new NSPAT.PatternAnimationTrack(string.Empty);
            anim.Tracks.Add(track);
            br.BaseStream.Seek(start + 0x40 * i, SeekOrigin.Begin);
            for (int j = 0; j < kfCounts[i] - 1; j++) // the last one is numframes
            {
                track.KeyFrames.Add(new NSPAT.KeyFrame(frame: br.ReadUInt16(), string.Empty, string.Empty));
            }
            anim.NumFrames = br.ReadUInt16(); // the last one is the number of frames
        }
        return result;
    }

    public static void WriteTo(NSPAT nspat, string file)
    {
        using (var bw = new BinaryWriter(File.Create(file)))
        {
            WriteTo(nspat, bw);
        }
    }

    public static void WriteTo(NSPAT nspat, BinaryWriter bw)
    {
        for (int i = 0; i < 8; i++)
        {
            if (i < nspat.PatternAnimations.Count && nspat.PatternAnimations[i].Tracks.Count > 0)
            {
                bw.Write((ushort)(nspat.PatternAnimations[i].Tracks[0].KeyFrames.Count + 1));
            }
            else
            {
                bw.Write((ushort)0);
            }
        }

        for (int i = 0; i < 8; i++)
        {
            if (i < nspat.PatternAnimations.Count && nspat.PatternAnimations[i].Tracks.Count > 0)
            {
                var anim = nspat.PatternAnimations[i];
                var track = anim.Tracks[0];
                bool writtenNumFrames = false;
                for (int j = 0; j < 0x20; j++)
                {
                    if (j < track.KeyFrames.Count)
                    {
                        bw.Write(track.KeyFrames[j].Frame);
                    }
                    else if (!writtenNumFrames)
                    {
                        bw.Write(anim.NumFrames);
                        writtenNumFrames = true;
                    }
                    else
                    {
                        bw.Write((ushort)0);
                    }
                }
            }
            else
            {
                bw.Pad(0x40);
            }
        }
    }
}
