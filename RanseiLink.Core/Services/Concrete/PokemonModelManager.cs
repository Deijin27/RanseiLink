using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RanseiLink.Core.Services.Concrete;

internal static class PokemonModelManager
{
    private const int _pokemonSpriteWidth = 32;

    /// <summary>
    /// Unpack models. Only for use in <see cref="FallbackSpriteProvider"/>
    /// </summary>
    public static void UnpackModels(PkmdlConstants pkmdlInfo, string graphicsProviderFolder)
    {
        string texLink = Path.Combine(graphicsProviderFolder, pkmdlInfo.TEXLink);
        string atxLink = Path.Combine(graphicsProviderFolder, pkmdlInfo.ATXLink);
        string dtxLink = Path.Combine(graphicsProviderFolder, pkmdlInfo.DTXLink);
        string pacLink = Path.Combine(graphicsProviderFolder, pkmdlInfo.PACLink);

        string texUnpacked = Path.Combine(graphicsProviderFolder, pkmdlInfo.TEXLinkFolder);
        string atxUnpacked = Path.Combine(graphicsProviderFolder, pkmdlInfo.ATXLinkFolder);
        string dtxUnpacked = Path.Combine(graphicsProviderFolder, pkmdlInfo.DTXLinkFolder);
        string pacUnpacked = Path.Combine(graphicsProviderFolder, pkmdlInfo.PACLinkFolder);

        string outFolderPath = Path.Combine(graphicsProviderFolder, pkmdlInfo.PngFolder);
        Directory.CreateDirectory(outFolderPath);

        LINK.Unpack(texLink, texUnpacked, false, 4);
        LINK.Unpack(atxLink, atxUnpacked, false, 4);
        LINK.Unpack(dtxLink, dtxUnpacked, false, 4);
        LINK.Unpack(pacLink, pacUnpacked, false, 4);

        int fileCount = Directory.GetFiles(texUnpacked).Length;

        Parallel.For(0, fileCount, i =>
        {
            string fileName = i.ToString().PadLeft(4, '0');
            string outFilePath = Path.Combine(outFolderPath, fileName + ".png");

            BTX0 btx0 = new BTX0(Path.Combine(texUnpacked, fileName));

            Rgba32[] palette = RawPalette.To32bitColors(btx0.Texture.Palette1);
            palette[0] = Color.Transparent;

            using var texImg = ImageUtil.ToImage(
                new ImageInfo(btx0.Texture.PixelMap, palette, _pokemonSpriteWidth, btx0.Texture.PixelMap.Length / _pokemonSpriteWidth), 
                PointUtil.GetPoint
                );

            byte[] atxPixelmap = RawChar.Decompress(File.ReadAllBytes(Path.Combine(atxUnpacked, fileName)));
            using var atxImg = ImageUtil.ToImage(
                new ImageInfo(atxPixelmap, palette, _pokemonSpriteWidth, atxPixelmap.Length / _pokemonSpriteWidth), 
                PointUtil.GetPoint
                );

            byte[] dtxPixelmap = RawChar.Decompress(File.ReadAllBytes(Path.Combine(dtxUnpacked, fileName)));
            using var dtxImg = ImageUtil.ToImage(
                new ImageInfo(dtxPixelmap, palette, _pokemonSpriteWidth, dtxPixelmap.Length / _pokemonSpriteWidth),
                PointUtil.GetPoint
                );

            
            string pacFile = Path.Combine(pacUnpacked, fileName);
            string pacUnpackedFolder = Path.Combine(pacUnpacked, fileName + "-Unpacked");
            PAC.Unpack(pacFile, pacUnpackedFolder, false, 4);
            byte[] pacPixelmap = RawChar.Decompress(File.ReadAllBytes(Path.Combine(pacUnpackedFolder, "0003")));
            using var pacImg = ImageUtil.ToImage(
                new ImageInfo(pacPixelmap, palette, _pokemonSpriteWidth, pacPixelmap.Length / _pokemonSpriteWidth),
                PointUtil.GetPoint
                );

            var totalWidth = texImg.Width + atxImg.Width + dtxImg.Width + pacImg.Width;
            var maxHeight = 1024;
            using var combinedImage = new Image<Rgba32>(totalWidth, maxHeight);
            combinedImage.Mutate(g =>
            {
                g.DrawImage(
                    image: texImg, 
                    location: new Point(0, 0), 
                    opacity: 1
                    );
                g.DrawImage(
                    image: atxImg, 
                    location: new Point(texImg.Width, 0), 
                    opacity: 1
                    );
                g.DrawImage(
                    image: dtxImg,
                    location: new Point(texImg.Width + atxImg.Width, 0),
                    opacity: 1
                    );
                g.DrawImage(
                    image: pacImg,
                    location: new Point(texImg.Width + atxImg.Width + dtxImg.Width, 0),
                    opacity: 1
                    );
            });

            combinedImage.SaveAsPng(outFilePath);
        });
    }

    /// <summary>
    /// Pack models. Only for use in <see cref="ModService"/>
    /// </summary>
    public static void PackModels(PkmdlConstants pkmdlInfo, ConcurrentBag<FileToPatch> filesToPatch, IOverrideSpriteProvider overrideSpriteProvider, string graphicsProviderFolder)
    {
        var spriteFiles = overrideSpriteProvider.GetAllSpriteFiles(pkmdlInfo.Type);
        if (!spriteFiles.Any(i => i.IsOverride))
        {
            return;
        }

        // temporary link files
        string texLink = Path.GetTempFileName();
        string atxLink = Path.GetTempFileName();
        string dtxLink = Path.GetTempFileName();
        string pacLink = Path.GetTempFileName();

        // link files unpacked previously
        string texUnpacked = Path.Combine(graphicsProviderFolder, pkmdlInfo.TEXLinkFolder);
        string atxUnpacked = Path.Combine(graphicsProviderFolder, pkmdlInfo.ATXLinkFolder);
        string dtxUnpacked = Path.Combine(graphicsProviderFolder, pkmdlInfo.DTXLinkFolder);
        string pacUnpacked = Path.Combine(graphicsProviderFolder, pkmdlInfo.PACLinkFolder);

        var spriteFileDict = spriteFiles.ToDictionary(i => i.Id);

        var texLinkFiles = new string[200];
        var atxLinkFiles = new string[200];
        var dtxLinkFiles = new string[200];
        var pacLinkFiles = new string[200];

        Parallel.For(0, 200, i =>
        {
            string fileName = i.ToString().PadLeft(4, '0');
            var spriteFile = spriteFileDict[(uint)i];
            if (spriteFile.IsOverride)
            {
                using var combinedImage = Image.Load<Rgba32>(spriteFile.File);

                // populate palette ------------------------------------------------------------------------------------------------------

                var palette = new List<Rgba32> { Color.Transparent };

                // TEX ------------------------------------------------------------------------------------------------------

                string texSource = Path.Combine(texUnpacked, fileName);
                string texTemp = Path.GetTempFileName();
                File.Copy(texSource, texTemp, true);
                BTX0 btx0 = new BTX0(texSource);
                int height = btx0.Texture.PixelMap.Length / _pokemonSpriteWidth;
                using var texImg = combinedImage.Clone(g =>
                {
                    g.Crop(new Rectangle(0, 0, _pokemonSpriteWidth, height));
                });
                btx0.Texture.PixelMap = ImageUtil.FromImage(texImg, palette, PointUtil.GetIndex);

                // ATX ------------------------------------------------------------------------------------------------------

                var atxDecompressedLen = new FileInfo(Path.Combine(atxUnpacked, fileName)).Length * 2;
                using var atxImg = combinedImage.Clone(g =>
                {
                    g.Crop(new Rectangle(_pokemonSpriteWidth, 0, _pokemonSpriteWidth, (int)(atxDecompressedLen / _pokemonSpriteWidth)));

                });
                string atxTemp = Path.GetTempFileName();
                File.WriteAllBytes(atxTemp, RawChar.Compress(ImageUtil.FromImage(atxImg, palette, PointUtil.GetIndex)));
                atxLinkFiles[i] = atxTemp;

                // DTX ------------------------------------------------------------------------------------------------------

                var dtxDecompressedLen = new FileInfo(Path.Combine(atxUnpacked, fileName)).Length * 2;
                using var dtxImg = combinedImage.Clone(g =>
                {
                    g.Crop(new Rectangle(_pokemonSpriteWidth*2, 0, _pokemonSpriteWidth, (int)(dtxDecompressedLen / _pokemonSpriteWidth)));
                });
                string dtxTemp = Path.GetTempFileName();
                File.WriteAllBytes(dtxTemp, RawChar.Compress(ImageUtil.FromImage(dtxImg, palette, PointUtil.GetIndex)));
                dtxLinkFiles[i] = dtxTemp;

                // PAC ------------------------------------------------------------------------------------------------------

                string pacUnpackedFolder = Path.Combine(pacUnpacked, fileName + "-Unpacked");
                var pacDecompressedLen = new FileInfo(Path.Combine(pacUnpackedFolder, "0003")).Length * 2;
                using var pacImg = combinedImage.Clone(g =>
                {
                    g.Crop(new Rectangle(_pokemonSpriteWidth*3, 0, _pokemonSpriteWidth, (int)(pacDecompressedLen / _pokemonSpriteWidth)));
                });
                string pacCharTemp = Path.GetTempFileName();
                File.WriteAllBytes(pacCharTemp, RawChar.Compress(ImageUtil.FromImage(pacImg, palette, PointUtil.GetIndex)));
                string pacTemp = Path.GetTempFileName();
                string[] pacFiles = new string[]
                {
                    Path.Combine(pacUnpackedFolder, "0000"),
                    Path.Combine(pacUnpackedFolder, "0001"),
                    Path.Combine(pacUnpackedFolder, "0002"),
                    pacCharTemp
                };
                PAC.Pack(pacFiles, new[] { 0, 2, 5, 6 }, pacTemp, 1);
                File.Delete(pacCharTemp);
                pacLinkFiles[i] = pacTemp;

                // TEX Palette -------------------------------------------------------------------------------------------
                if (palette.Count > 16)
                {
                    throw new System.Exception($"More than 16 colors when building tex file in {nameof(PokemonModelManager)}");
                }

                var resizedPalette = new Rgba32[16];
                for (int paletteIndex = 0; paletteIndex < palette.Count; paletteIndex++)
                {
                    resizedPalette[paletteIndex] = palette[paletteIndex];
                }
                resizedPalette[0] = Color.FromRgb(120, 120, 120);

                var convertedPalette = RawPalette.From32bitColors(resizedPalette);
                btx0.Texture.Palette1 = convertedPalette;
                btx0.Texture.Palette2 = convertedPalette;
                btx0.WriteTo(texTemp);
                texLinkFiles[i] = texTemp;
            }
            else
            {
                texLinkFiles[i] = Path.Combine(texUnpacked, fileName);
                atxLinkFiles[i] = Path.Combine(atxUnpacked, fileName);
                dtxLinkFiles[i] = Path.Combine(dtxUnpacked, fileName);
                pacLinkFiles[i] = Path.Combine(pacUnpacked, fileName);
            }
        });

        LINK.Pack(texLinkFiles, texLink);
        LINK.Pack(atxLinkFiles, atxLink);
        LINK.Pack(dtxLinkFiles, dtxLink);
        LINK.Pack(pacLinkFiles, pacLink);

        filesToPatch.Add(new FileToPatch(pkmdlInfo.TEXLink, texLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        filesToPatch.Add(new FileToPatch(pkmdlInfo.ATXLink, atxLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        filesToPatch.Add(new FileToPatch(pkmdlInfo.DTXLink, dtxLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        filesToPatch.Add(new FileToPatch(pkmdlInfo.PACLink, pacLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
    }
}
