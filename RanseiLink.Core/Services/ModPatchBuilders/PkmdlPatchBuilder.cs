using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services.ModelServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RanseiLink.Core.Services.ModPatchBuilders
{
    public class PkmdlPatchBuilder : IGraphicTypePatchBuilder
    {
        private const int _numPokemon = 200;
        private const int _pokemonSpriteWidth = 32;
        private const int _pokemonSpriteHeight = 32;
        private const int _texSpriteCount = 24;

        private readonly IOverrideDataProvider _overrideSpriteProvider;
        private readonly IPokemonService _pokemonService; // this will be used to get the sprite asymmetry, whether the animation is double duration, and the pokemon's national dex number
        private readonly string _graphicsProviderFolder;

        public PkmdlPatchBuilder(IOverrideDataProvider overrideSpriteProvider, ModInfo mod, IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
            _overrideSpriteProvider = overrideSpriteProvider;
            _graphicsProviderFolder = Constants.DefaultDataFolder(mod.GameCode);
        }

        public void GetFilesToPatch(ConcurrentBag<FileToPatch> filesToPatch, IGraphicsInfo gInfo)
        {
            if (!(gInfo is PkmdlConstants pkmdlInfo))
            {
                return;
            }

            var spriteFiles = _overrideSpriteProvider.GetAllSpriteFiles(pkmdlInfo.Type);
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
            string texUnpacked = Path.Combine(_graphicsProviderFolder, pkmdlInfo.TEXLinkFolder);
            string atxUnpacked = Path.Combine(_graphicsProviderFolder, pkmdlInfo.ATXLinkFolder);
            string dtxUnpacked = Path.Combine(_graphicsProviderFolder, pkmdlInfo.DTXLinkFolder);
            string pacUnpacked = Path.Combine(_graphicsProviderFolder, pkmdlInfo.PACLinkFolder);

            var spriteFileDict = spriteFiles.ToDictionary(i => i.Id);

            var texLinkFiles = new string[_numPokemon];
            var atxLinkFiles = new string[_numPokemon];
            var dtxLinkFiles = new string[_numPokemon];
            var pacLinkFiles = new string[_numPokemon];

            Parallel.For(0, _numPokemon, i =>
            {
                string fileName = i.ToString().PadLeft(4, '0');
                var spriteFile = spriteFileDict[i];
                if (spriteFile.IsOverride)
                {
                    // determine heights of columns within sprite sheet based on pokemon data
                    var pokemon = _pokemonService.Retrieve(i);
                    int atxHeight;
                    int dtxHeight;
                    int pacHeight;
                    if (pokemon.AsymmetricBattleSprite)
                    {
                        atxHeight = _pokemonSpriteHeight * 16;
                        dtxHeight = _pokemonSpriteHeight * 12;
                        pacHeight = _pokemonSpriteHeight * 24;
                    }
                    else
                    {
                        atxHeight = _pokemonSpriteHeight * 8;
                        dtxHeight = _pokemonSpriteHeight * 8;
                        pacHeight = _pokemonSpriteHeight * 16;
                    }

                    if (pokemon.LongAttackAnimation)
                    {
                        atxHeight *= 2;
                    }

                    // load columns from sprite sheet
                    using (var combinedImage = Image.Load<Rgba32>(spriteFile.File))
                    {

                        // populate palette ------------------------------------------------------------------------------------------------------

                        var palette = new List<Rgba32> { Color.Transparent };

                        // TEX ------------------------------------------------------------------------------------------------------

                        string texTemp = Path.GetTempFileName();
                        NSBTX btx0 = new NSBTX(new NSTEX());
                        int texHeight = _pokemonSpriteHeight * _texSpriteCount;
                        using (var texImg = combinedImage.Clone(g =>
                        {
                            g.Crop(new Rectangle(0, 0, _pokemonSpriteWidth, texHeight));
                        }))
                        {
                            byte[] mergedPixels = ImageUtil.FromImage(texImg, palette, PointUtil.GetIndex, color0ToTransparent: true);
                            for (int texNumber = 0; texNumber < _texSpriteCount; texNumber++)
                            {
                                var subArray = new byte[_pokemonSpriteWidth * _pokemonSpriteHeight];
                                Array.Copy(
                                    sourceArray: mergedPixels,
                                    sourceIndex: _pokemonSpriteHeight * _pokemonSpriteWidth * texNumber,
                                    destinationArray: subArray,
                                    destinationIndex: 0,
                                    length: _pokemonSpriteHeight * _pokemonSpriteWidth
                                    );
                                btx0.Texture.Textures.Add(new NSTEX.Texture(
                                    name: "base_fix_" + texNumber.ToString().PadLeft(2, '0'),
                                    textureData: subArray
                                    )
                                {
                                    Height = _pokemonSpriteHeight,
                                    Width = _pokemonSpriteWidth,
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
                            g.Crop(new Rectangle(_pokemonSpriteWidth, 0, _pokemonSpriteWidth, atxHeight));

                        }))
                        {
                            string atxTemp = Path.GetTempFileName();
                            File.WriteAllBytes(atxTemp, RawChar.Compress(ImageUtil.FromImage(atxImg, palette, PointUtil.GetIndex)));
                            atxLinkFiles[i] = atxTemp;
                        }


                        // DTX ------------------------------------------------------------------------------------------------------

                        var dtxDecompressedLen = new FileInfo(Path.Combine(atxUnpacked, fileName)).Length * 2;
                        using (var dtxImg = combinedImage.Clone(g =>
                        {
                            g.Crop(new Rectangle(_pokemonSpriteWidth * 2, 0, _pokemonSpriteWidth, dtxHeight));
                        }))
                        {
                            string dtxTemp = Path.GetTempFileName();
                            File.WriteAllBytes(dtxTemp, RawChar.Compress(ImageUtil.FromImage(dtxImg, palette, PointUtil.GetIndex)));
                            dtxLinkFiles[i] = dtxTemp;
                        }


                        // PAC ------------------------------------------------------------------------------------------------------

                        string pacUnpackedFolder = Path.Combine(pkmdlInfo.PACLinkFolder, fileName + "-Unpacked");

                        // convert the png
                        using (var pacImg = combinedImage.Clone(g =>
                        {
                            g.Crop(new Rectangle(_pokemonSpriteWidth * 3, 0, _pokemonSpriteWidth, pacHeight));
                        }))
                        {
                            string pacCharTemp = Path.GetTempFileName();
                            File.WriteAllBytes(pacCharTemp, RawChar.Compress(ImageUtil.FromImage(pacImg, palette, PointUtil.GetIndex)));
                            string pacTemp = Path.GetTempFileName();
                            string[] pacFiles = new string[]
                            {
                                Path.Combine(_graphicsProviderFolder, pacUnpackedFolder, "0000"),
                                _overrideSpriteProvider.GetDataFile(Path.Combine(pacUnpackedFolder, "0001")).File,
                                _overrideSpriteProvider.GetDataFile(Path.Combine(pacUnpackedFolder, "0002")).File,
                                pacCharTemp
                            };
                            PAC.Pack(pacFiles, pacTemp, new[] { PAC.FileTypeNumber.NSBMD, PAC.FileTypeNumber.NSBTP, PAC.FileTypeNumber.PAT, PAC.FileTypeNumber.CHAR }, 1);
                            File.Delete(pacCharTemp);
                            pacLinkFiles[i] = pacTemp;
                        }


                        // TEX Palette -------------------------------------------------------------------------------------------
                        if (palette.Count > 16)
                        {
                            throw new System.Exception($"More than 16 colors in image when building tex file in {nameof(PkmdlPatchBuilder)}. This should have been filtered out by palette simplifier");
                        }

                        var resizedPalette = new Rgba32[16];
                        for (int paletteIndex = 0; paletteIndex < palette.Count; paletteIndex++)
                        {
                            resizedPalette[paletteIndex] = palette[paletteIndex];
                        }
                        resizedPalette[0] = Color.FromRgb(120, 120, 120);

                        var convertedPalette = RawPalette.From32bitColors(resizedPalette);
                        btx0.Texture.Palettes.Add(new NSTEX.Palette (name: "base_fix_f_pl", paletteData: convertedPalette));
                        btx0.Texture.Palettes.Add(new NSTEX.Palette (name: "base_fix_b_pl", paletteData: convertedPalette));
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

            filesToPatch.Add(new FileToPatch(pkmdlInfo.TEXLink, texLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
            filesToPatch.Add(new FileToPatch(pkmdlInfo.ATXLink, atxLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
            filesToPatch.Add(new FileToPatch(pkmdlInfo.DTXLink, dtxLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
            filesToPatch.Add(new FileToPatch(pkmdlInfo.PACLink, pacLink, FilePatchOptions.DeleteSourceWhenDone | FilePatchOptions.VariableLength));
        }
    } 
}