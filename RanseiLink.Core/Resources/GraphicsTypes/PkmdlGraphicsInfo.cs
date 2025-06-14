using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Xml.Linq;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Services;

namespace RanseiLink.Core.Resources;

public class PkmdlGraphicsInfo : GroupedGraphicsInfo
{
    public string TEXLink { get; }
    public string TEXLinkFolder { get; }
    public string ATXLink { get; }
    public string ATXLinkFolder { get; }
    public string DTXLink { get; }
    public string DTXLinkFolder { get; }
    public string PACLink { get; }
    public string PACLinkFolder { get; }
    public override string PngFolder { get; }
    public override int PaletteCapacity => 16;

    private const int __pokemonSpriteWidth = 32;
    private const int __numPokemon = 200;
    private const int __pokemonSpriteHeight = 32;
    private const int __texSpriteCount = 24;

    public PkmdlGraphicsInfo(MetaSpriteType metaType, XElement element) : base(metaType, element)
    {
        TEXLink = FileUtil.NormalizePath(element.Element("TEXLink")!.Value);
        ATXLink = FileUtil.NormalizePath(element.Element("ATXLink")!.Value);
        DTXLink = FileUtil.NormalizePath(element.Element("DTXLink")!.Value);
        PACLink = FileUtil.NormalizePath(element.Element("PACLink")!.Value);

        TEXLinkFolder = Path.Combine(Path.GetDirectoryName(TEXLink)!, "TEXLink-Unpacked");
        ATXLinkFolder = Path.Combine(Path.GetDirectoryName(ATXLink)!, "ATXLink-Unpacked");
        DTXLinkFolder = Path.Combine(Path.GetDirectoryName(DTXLink)!, "DTXLink-Unpacked");
        PACLinkFolder = Path.Combine(Path.GetDirectoryName(PACLink)!, "PACLink-Unpacked");

        PngFolder = Path.Combine(Path.GetDirectoryName(TEXLink)!, "Pngs");
    }

    public override void ProcessExportedFiles(PopulateDefaultsContext context)
    {
        string texLink = Path.Combine(context.DefaultDataFolder, TEXLink);
        string atxLink = Path.Combine(context.DefaultDataFolder, ATXLink);
        string dtxLink = Path.Combine(context.DefaultDataFolder, DTXLink);
        string pacLink = Path.Combine(context.DefaultDataFolder, PACLink);

        string texUnpacked = Path.Combine(context.DefaultDataFolder, TEXLinkFolder);
        string atxUnpacked = Path.Combine(context.DefaultDataFolder, ATXLinkFolder);
        string dtxUnpacked = Path.Combine(context.DefaultDataFolder, DTXLinkFolder);
        string pacUnpacked = Path.Combine(context.DefaultDataFolder, PACLinkFolder);

        string outFolderPath = Path.Combine(context.DefaultDataFolder, PngFolder);
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

            NSBTX btx0 = new NSBTX(Path.Combine(texUnpacked, fileName));

            var palette = new Palette(btx0.Texture.Palettes[0].PaletteData, true);

            // merge individual TEX textures into one for speed
            var texPixelmap = new byte[btx0.Texture.Textures.Sum(x => x.TextureData.Length)];
            int pos = 0;
            foreach (var pixelmap in btx0.Texture.Textures)
            {
                pixelmap.TextureData.CopyTo(texPixelmap, pos);
                pos += pixelmap.TextureData.Length;
            }

            const bool isTiled = false;
            const TexFormat texFormat = TexFormat.Pltt16;

            // generate images
            var texImg = ImageUtil.SpriteToImage(
                new SpriteImageInfo(texPixelmap, palette, __pokemonSpriteWidth, texPixelmap.Length / __pokemonSpriteWidth,
                isTiled,
                texFormat
                ));

            byte[] atxPixelmap = PixelUtil.Decompress(File.ReadAllBytes(Path.Combine(atxUnpacked, fileName)));
            var atxImg = ImageUtil.SpriteToImage(
                new SpriteImageInfo(atxPixelmap, palette, __pokemonSpriteWidth, atxPixelmap.Length / __pokemonSpriteWidth,
                isTiled,
                texFormat
                ));

            byte[] dtxPixelmap = PixelUtil.Decompress(File.ReadAllBytes(Path.Combine(dtxUnpacked, fileName)));
            var dtxImg = ImageUtil.SpriteToImage(
                new SpriteImageInfo(dtxPixelmap, palette, __pokemonSpriteWidth, dtxPixelmap.Length / __pokemonSpriteWidth,
                isTiled,
                texFormat
                ));


            string pacFile = Path.Combine(pacUnpacked, fileName);
            string pacUnpackedFolder = Path.Combine(pacUnpacked, fileName + "-Unpacked");
            PAC.Unpack(pacFile, pacUnpackedFolder, false, 4);
            byte[] pacPixelmap = PixelUtil.Decompress(File.ReadAllBytes(Path.Combine(pacUnpackedFolder, "0003")));
            var pacImg = ImageUtil.SpriteToImage(
                new SpriteImageInfo(pacPixelmap, palette, __pokemonSpriteWidth, pacPixelmap.Length / __pokemonSpriteWidth,
                isTiled,
                texFormat
                ));

            var totalWidth = texImg.Width + atxImg.Width + dtxImg.Width + pacImg.Width;
            var maxHeight = 1024;
            var combinedImage = new Image<Rgba32>(totalWidth, maxHeight);
            combinedImage.Mutate(g =>
            {
                g.DrawImage(
                    texImg,
                    new Point(0, 0),
                    1
                    );
                g.DrawImage(
                    atxImg,
                    new Point(texImg.Width, 0),
                    1
                    );
                g.DrawImage(
                    dtxImg,
                    new Point(texImg.Width + atxImg.Width, 0),
                    1
                    );
                g.DrawImage(
                    pacImg,
                    new Point(texImg.Width + atxImg.Width + dtxImg.Width, 0),
                    1
                    );
            });

            texImg.Dispose();
            atxImg.Dispose();
            dtxImg.Dispose();
            pacImg.Dispose();

            combinedImage.SaveAsPng(outFilePath);
            combinedImage.Dispose();
        });
    }

    public override void GetFilesToPatch(GraphicsPatchContext context)
    {
        var spriteFiles = context.OverrideDataProvider.GetAllSpriteFiles(Type);
        if (!spriteFiles.Any(i => i.IsOverride))
        {
            return;
        }

        var pokemonService = context.ModServiceGetter.Get<IPokemonService>();

        // temporary link files
        string texLink = Path.GetTempFileName();
        string atxLink = Path.GetTempFileName();
        string dtxLink = Path.GetTempFileName();
        string pacLink = Path.GetTempFileName();

        // link files unpacked previously
        string texUnpacked = Path.Combine(context.DefaultDataFolder, TEXLinkFolder);
        string atxUnpacked = Path.Combine(context.DefaultDataFolder, ATXLinkFolder);
        string dtxUnpacked = Path.Combine(context.DefaultDataFolder, DTXLinkFolder);
        string pacUnpacked = Path.Combine(context.DefaultDataFolder, PACLinkFolder);

        var spriteFileDict = spriteFiles.ToDictionary(i => i.Id);

        var texLinkFiles = new string[__numPokemon];
        var atxLinkFiles = new string[__numPokemon];
        var dtxLinkFiles = new string[__numPokemon];
        var pacLinkFiles = new string[__numPokemon];

        Parallel.For(0, __numPokemon, i =>
        {
            string fileName = i.ToString().PadLeft(4, '0');
            var spriteFile = spriteFileDict[i];
            if (spriteFile.IsOverride)
            {
                // determine heights of columns within sprite sheet based on pokemon data
                var pokemon = pokemonService.Retrieve(i);
                int atxHeight;
                int dtxHeight;
                int pacHeight;
                if (pokemon.AsymmetricBattleSprite)
                {
                    atxHeight = __pokemonSpriteHeight * 16;
                    dtxHeight = __pokemonSpriteHeight * 12;
                    pacHeight = __pokemonSpriteHeight * 24;
                }
                else
                {
                    atxHeight = __pokemonSpriteHeight * 8;
                    dtxHeight = __pokemonSpriteHeight * 8;
                    pacHeight = __pokemonSpriteHeight * 16;
                }

                if (pokemon.LongAttackAnimation)
                {
                    atxHeight *= 2;
                }

                // load columns from sprite sheet
                using (var combinedImage = Image.Load<Rgba32>(spriteFile.File))
                {
                    // TEX ------------------------------------------------------------------------------------------------------

                    const bool isTiled = false;
                    const TexFormat texFormat = TexFormat.Pltt16;
                    const bool color0Transparent = true;
                    var palette = new Palette(texFormat, color0Transparent);
                    string texTemp = Path.GetTempFileName();
                    NSBTX btx0 = new NSBTX(new NSTEX());
                    int texHeight = __pokemonSpriteHeight * __texSpriteCount;
                    using (var texImg = combinedImage.Clone(g =>
                    {
                        g.Crop(new Rectangle(0, 0, __pokemonSpriteWidth, texHeight));
                    }))
                    {
                        byte[] mergedPixels = ImageUtil.SharedPalettePixelsFromImage(texImg, palette, isTiled, texFormat, color0Transparent);
                        for (int texNumber = 0; texNumber < __texSpriteCount; texNumber++)
                        {
                            var subArray = new byte[__pokemonSpriteWidth * __pokemonSpriteHeight];
                            Array.Copy(
                                sourceArray: mergedPixels,
                                sourceIndex: __pokemonSpriteHeight * __pokemonSpriteWidth * texNumber,
                                destinationArray: subArray,
                                destinationIndex: 0,
                                length: __pokemonSpriteHeight * __pokemonSpriteWidth
                                );
                            btx0.Texture.Textures.Add(new NSTEX.Texture(
                                name: "base_fix_" + texNumber.ToString().PadLeft(2, '0'),
                                textureData: subArray
                                )
                            {
                                Height = __pokemonSpriteHeight,
                                Width = __pokemonSpriteWidth,
                                Format = TexFormat.Pltt16,
                                FlipX = false,
                                FlipY = false,
                                RepeatX = false,
                                RepeatY = false,
                                Color0Transparent = true,

                            });
                        }
                    }

                    // ATX ------------------------------------------------------------------------------------------------------

                    var atxDecompressedLen = new FileInfo(Path.Combine(atxUnpacked, fileName)).Length * 2;
                    using (var atxImg = combinedImage.Clone(g =>
                    {
                        g.Crop(new Rectangle(__pokemonSpriteWidth, 0, __pokemonSpriteWidth, atxHeight));

                    }))
                    {
                        string atxTemp = Path.GetTempFileName();
                        File.WriteAllBytes(atxTemp, PixelUtil.Compress(ImageUtil.SharedPalettePixelsFromImage(atxImg, palette, isTiled, texFormat, color0Transparent)));
                        atxLinkFiles[i] = atxTemp;
                    }


                    // DTX ------------------------------------------------------------------------------------------------------

                    var dtxDecompressedLen = new FileInfo(Path.Combine(atxUnpacked, fileName)).Length * 2;
                    using (var dtxImg = combinedImage.Clone(g =>
                    {
                        g.Crop(new Rectangle(__pokemonSpriteWidth * 2, 0, __pokemonSpriteWidth, dtxHeight));
                    }))
                    {
                        string dtxTemp = Path.GetTempFileName();
                        File.WriteAllBytes(dtxTemp, PixelUtil.Compress(ImageUtil.SharedPalettePixelsFromImage(dtxImg, palette, isTiled, texFormat, color0Transparent)));
                        dtxLinkFiles[i] = dtxTemp;
                    }


                    // PAC ------------------------------------------------------------------------------------------------------

                    string pacUnpackedFolder = Path.Combine(PACLinkFolder, fileName + "-Unpacked");

                    // convert the png
                    using (var pacImg = combinedImage.Clone(g =>
                    {
                        g.Crop(new Rectangle(__pokemonSpriteWidth * 3, 0, __pokemonSpriteWidth, pacHeight));
                    }))
                    {
                        string pacCharTemp = Path.GetTempFileName();
                        File.WriteAllBytes(pacCharTemp, PixelUtil.Compress(ImageUtil.SharedPalettePixelsFromImage(pacImg, palette, isTiled, texFormat, color0Transparent)));
                        string pacTemp = Path.GetTempFileName();
                        string[] pacFiles = new string[]
                        {
                            Path.Combine(context.DefaultDataFolder, pacUnpackedFolder, "0000"),
                            context.OverrideDataProvider.GetDataFile(Path.Combine(pacUnpackedFolder, "0001")).File,
                            context.OverrideDataProvider.GetDataFile(Path.Combine(pacUnpackedFolder, "0002")).File,
                            pacCharTemp
                        };
                        PAC.Pack(pacFiles, pacTemp, new[] { PAC.FileTypeNumber.NSBMD, PAC.FileTypeNumber.NSBTP, PAC.FileTypeNumber.PAT, PAC.FileTypeNumber.CHAR }, 1);
                        File.Delete(pacCharTemp);
                        pacLinkFiles[i] = pacTemp;
                    }


                    // TEX Palette -------------------------------------------------------------------------------------------
                    if (palette.Count > 16)
                    {
                        throw new System.Exception($"More than 16 colors in image when building tex file in {nameof(PkmdlGraphicsInfo)}. This should have been filtered out by palette simplifier");
                    }

                    var resizedPalette = new Rgba32[16];
                    for (int paletteIndex = 0; paletteIndex < palette.Count; paletteIndex++)
                    {
                        resizedPalette[paletteIndex] = palette[paletteIndex];
                    }
                    resizedPalette[0] = Color.FromRgb(120, 120, 120);

                    var convertedPalette = PaletteUtil.From32bitColors(resizedPalette);
                    btx0.Texture.Palettes.Add(new NSTEX.Palette(name: "base_fix_f_pl", paletteData: convertedPalette));
                    btx0.Texture.Palettes.Add(new NSTEX.Palette(name: "base_fix_b_pl", paletteData: convertedPalette));
                    btx0.WriteTo(texTemp);
                    texLinkFiles[i] = texTemp;
                }
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

        context.FilesToPatch.Add(new FileToPatch(TEXLink, texLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        context.FilesToPatch.Add(new FileToPatch(ATXLink, atxLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        context.FilesToPatch.Add(new FileToPatch(DTXLink, dtxLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        context.FilesToPatch.Add(new FileToPatch(PACLink, pacLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
    }
}