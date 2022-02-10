using RanseiLink.Core.Archive;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Nds;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RanseiLink.Core.Services.Concrete;



internal class SpriteService : ISpriteService
{
    private const int _pokemonSpriteWidth = 32;
    private const int _pokemonSpriteHeight = 32;
    public void ExportAllPokemonSprites(INds rom, string desinationFolder, SpriteExportOptions options = 0)
    {
        string tempDirectory = FileUtil.GetTemporaryDirectory();
        rom.ExtractCopyOfDirectory(Constants.PokemonSpriteFolderPath, tempDirectory);
        string modelPokemonDir = Path.Combine(tempDirectory, Path.GetFileName(Constants.PokemonSpriteFolderPath));

        string texLink = Path.Combine(modelPokemonDir, "POKEMON_TEX.ALL");
        string atxLink = Path.Combine(modelPokemonDir, "POKEMON_ATX.ALL");
        string dtxLink = Path.Combine(modelPokemonDir, "POKEMON_DTX.ALL");
        string pacLink = Path.Combine(modelPokemonDir, "POKEMON_PAC.ALL");

        string texUnpacked = Path.Combine(modelPokemonDir, "POKEMON_TEX");
        string atxUnpacked = Path.Combine(modelPokemonDir, "POKEMON_ATX");
        string dtxUnpacked = Path.Combine(modelPokemonDir, "POKEMON_DTX");
        string pacUnpacked = Path.Combine(modelPokemonDir, "POKEMON_PAC");

        LINK.Unpack(texLink, texUnpacked, false, 4);
        LINK.Unpack(atxLink, atxUnpacked, false, 4);
        LINK.Unpack(dtxLink, dtxUnpacked, false, 4);
        LINK.Unpack(pacLink, pacUnpacked, false, 4);

        int fileCount = Directory.GetFiles(texUnpacked).Length;

        Parallel.For(0, fileCount, i =>
        {
            string fileName = i.ToString().PadLeft(4, '0');
            string singlePokemonDir = Path.Combine(desinationFolder, $"{fileName} - {(PokemonId)i}");
            Directory.CreateDirectory(singlePokemonDir);

            BTX0 btx0;

            using (var br = new BinaryReader(File.OpenRead(Path.Combine(texUnpacked, fileName))))
            {
                btx0 = new BTX0(br);
            }

            Rgba32[] palette = RawPalette.To32bitColors(btx0.Texture.Palette1);
            if (options.HasFlag(SpriteExportOptions.IncludePaintNetPalette))
            {
                RawPalette.SaveAsPaintNetPalette(palette, Path.Combine(singlePokemonDir, "PaintNet-Palette.txt"));
            }

            string texOutFile = Path.Combine(singlePokemonDir, "TEX.png");
            ImageUtil.SaveAsPng(texOutFile, new ImageInfo(btx0.Texture.PixelMap, palette, _pokemonSpriteWidth, _pokemonSpriteHeight));

            string atxOutFile = Path.Combine(singlePokemonDir, "ATX.png");
            byte[] atxPixelmap = RawChar.Decompress(File.ReadAllBytes(Path.Combine(atxUnpacked, fileName)));
            ImageUtil.SaveAsPng(atxOutFile, new ImageInfo(atxPixelmap, palette, _pokemonSpriteWidth, _pokemonSpriteHeight));

            string dtxOutFile = Path.Combine(singlePokemonDir, "DTX.png");
            byte[] dtxPixelmap = RawChar.Decompress(File.ReadAllBytes(Path.Combine(dtxUnpacked, fileName)));
            ImageUtil.SaveAsPng(dtxOutFile, new ImageInfo(dtxPixelmap, palette, _pokemonSpriteWidth, _pokemonSpriteHeight));

            string pacOutFile = Path.Combine(singlePokemonDir, "PAC.png");
            string pacFile = Path.Combine(pacUnpacked, fileName);
            string pacUnpackedFolder = Path.Combine(pacUnpacked, fileName + "-Unpacked");
            PAC.Unpack(pacFile, pacUnpackedFolder, false, 4);
            byte[] pacPixelmap = RawChar.Decompress(File.ReadAllBytes(Path.Combine(pacUnpackedFolder, "0003")));
            ImageUtil.SaveAsPng(pacOutFile, new ImageInfo(pacPixelmap, palette, _pokemonSpriteWidth, _pokemonSpriteHeight));
        });

        Directory.Delete(tempDirectory, true);
    }

    private class StlContext
    {
        public string Temp { get; init; }
        public NCER Ncer { get; init; }
        public string StlDataFile { get; init; }
        public string StlInfoFile { get; init; }
    }

    private static StlContext BuildStlContext(INds rom, StlFile extractable)
    {
        string pathInRom = Path.Combine(Constants.GraphicsFolderPath, extractable.ToString());
        string temp = FileUtil.GetTemporaryDirectory();
        rom.ExtractCopyOfDirectory(pathInRom, temp);

        // get the stl
        string extractedDir = Path.Combine(temp, extractable.ToString());
        string stlDataFile = Directory.EnumerateFiles(extractedDir, "*.dat").FirstOrDefault(i => i.StartsWith("Stl") && i.EndsWith("Tex.dat"));
        if (stlDataFile == null)
        {
            throw new Exception("Tex.dat file not found");
        }
        string stlInfoFile = Directory.EnumerateFiles(extractedDir, "*.dat").FirstOrDefault(i => i.StartsWith("Stl") && i.EndsWith("TexInfo.dat"));
        if (stlDataFile == null)
        {
            throw new Exception("Tex.dat file not found");
        }

        // get the ncer
        string link = Path.Combine(extractedDir, $"{extractable}.G2GR");
        string linkUnpacked = Path.Combine(extractedDir, extractable.ToString());
        LINK.Unpack(link, linkUnpacked, detectExt: false, zeroPadLength: 1);
        string ncerFile = Path.Combine(linkUnpacked, "2");
        var ncer = NCER.Load(ncerFile);

        return new StlContext
        {
            Temp = temp,
            Ncer = ncer,
            StlDataFile = stlDataFile,
            StlInfoFile = stlInfoFile,
        };
    }

    public void ExtractStl(INds rom, StlFile extractable, string extractDir)
    {
        var context = BuildStlContext(rom, extractable);
        throw new NotImplementedException();
        string exportDir = FileUtil.MakeUniquePath(Path.Combine(extractDir, extractable.ToString()));
        STLCollection
            .Load(context.StlDataFile, null)
            .SaveAsPngs(exportDir, context.Ncer);

        Directory.Delete(context.Temp, true);
    }

    public void ImportStl(INds rom, StlFile extractable, string dirToImport)
    {
        var context = BuildStlContext(rom, extractable);

        STLCollection
            .LoadPngs(dirToImport, context.Ncer)
            .Save(context.StlDataFile, context.StlInfoFile);

        Directory.Delete(context.Temp, true);
    }

    public void ExtractStl(string stlDataFile, NCER ncer, string outputFolder)
    {
        throw new NotImplementedException();
    }
}
