using RanseiLink.Core.Archive;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Nds;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Core.Services.Concrete;

internal class SpriteService : ISpriteService
{
    private const int _pokemonSpriteWidth = 32;
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

        Link.Unpack(texLink, texUnpacked, false, 4);
        Link.Unpack(atxLink, atxUnpacked, false, 4);
        Link.Unpack(dtxLink, dtxUnpacked, false, 4);
        Link.Unpack(pacLink, pacUnpacked, false, 4);

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

            Rgba32[] palette = PLTT.To32bitColors(btx0.Texture.Palette1);
            if (options.HasFlag(SpriteExportOptions.IncludePaintNetPalette))
            {
                using var streamWriter = new StreamWriter(Path.Combine(singlePokemonDir, "PaintNet-Palette.txt"));
                streamWriter.WriteLine("; this palette is not necessary for importing a pokemon sprite back into ranseilink");
                foreach (var col in palette)
                {
                    streamWriter.WriteLine($"{col.A.ToString("X").PadLeft(2, '0')}{col.R.ToString("X").PadLeft(2, '0')}{col.G.ToString("X").PadLeft(2, '0')}{col.B.ToString("X").PadLeft(2, '0')}");
                }
            }

            string texOutFile = Path.Combine(singlePokemonDir, "TEX.png");
            ImageUtil.SaveAsPng(texOutFile, btx0.Texture.PixelMap, palette, _pokemonSpriteWidth);

            string atxOutFile = Path.Combine(singlePokemonDir, "ATX.png");
            byte[] atxPixelmap = CHAR.Decompress(File.ReadAllBytes(Path.Combine(atxUnpacked, fileName)));
            ImageUtil.SaveAsPng(atxOutFile, atxPixelmap, palette, _pokemonSpriteWidth);

            string dtxOutFile = Path.Combine(singlePokemonDir, "DTX.png");
            byte[] dtxPixelmap = CHAR.Decompress(File.ReadAllBytes(Path.Combine(dtxUnpacked, fileName)));
            ImageUtil.SaveAsPng(dtxOutFile, dtxPixelmap, palette, _pokemonSpriteWidth);

            string pacOutFile = Path.Combine(singlePokemonDir, "PAC.png");
            string pacFile = Path.Combine(pacUnpacked, fileName);
            string pacUnpackedFolder = Path.Combine(pacUnpacked, fileName + "-Unpacked");
            PAC.Unpack(pacFile, pacUnpackedFolder, false, 4);
            byte[] pacPixelmap = CHAR.Decompress(File.ReadAllBytes(Path.Combine(pacUnpackedFolder, "0003")));
            ImageUtil.SaveAsPng(pacOutFile, pacPixelmap, palette, _pokemonSpriteWidth);
        });

        Directory.Delete(tempDirectory, true);
    }
}
