using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RanseiLink.Core.Services.DefaultPopulaters;

public class PkmdlDefaultPopulater : IGraphicTypeDefaultPopulater
{
    private const int _pokemonSpriteWidth = 32;

    public void ProcessExportedFiles(string defaultDataFolder, IGraphicsInfo gInfo)
    {
        if (gInfo.MetaType != MetaSpriteType.PKMDL)
        {
            return;
        }

        PkmdlConstants pkmdlInfo = (PkmdlConstants)gInfo;

        string texLink = Path.Combine(defaultDataFolder, pkmdlInfo.TEXLink);
        string atxLink = Path.Combine(defaultDataFolder, pkmdlInfo.ATXLink);
        string dtxLink = Path.Combine(defaultDataFolder, pkmdlInfo.DTXLink);
        string pacLink = Path.Combine(defaultDataFolder, pkmdlInfo.PACLink);

        string texUnpacked = Path.Combine(defaultDataFolder, pkmdlInfo.TEXLinkFolder);
        string atxUnpacked = Path.Combine(defaultDataFolder, pkmdlInfo.ATXLinkFolder);
        string dtxUnpacked = Path.Combine(defaultDataFolder, pkmdlInfo.DTXLinkFolder);
        string pacUnpacked = Path.Combine(defaultDataFolder, pkmdlInfo.PACLinkFolder);

        string outFolderPath = Path.Combine(defaultDataFolder, pkmdlInfo.PngFolder);
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

            Rgba32[] palette = PaletteUtil.To32bitColors(btx0.Texture.Palettes[0].PaletteData);
            palette[0] = Color.Transparent;

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
                new SpriteImageInfo(texPixelmap, palette, _pokemonSpriteWidth, texPixelmap.Length / _pokemonSpriteWidth,
                isTiled,
                texFormat
                ));

            byte[] atxPixelmap = PixelUtil.Decompress(File.ReadAllBytes(Path.Combine(atxUnpacked, fileName)));
            var atxImg = ImageUtil.SpriteToImage(
                new SpriteImageInfo(atxPixelmap, palette, _pokemonSpriteWidth, atxPixelmap.Length / _pokemonSpriteWidth,
                isTiled,
                texFormat
                ));

            byte[] dtxPixelmap = PixelUtil.Decompress(File.ReadAllBytes(Path.Combine(dtxUnpacked, fileName)));
            var dtxImg = ImageUtil.SpriteToImage(
                new SpriteImageInfo(dtxPixelmap, palette, _pokemonSpriteWidth, dtxPixelmap.Length / _pokemonSpriteWidth,
                isTiled,
                texFormat
                ));


            string pacFile = Path.Combine(pacUnpacked, fileName);
            string pacUnpackedFolder = Path.Combine(pacUnpacked, fileName + "-Unpacked");
            PAC.Unpack(pacFile, pacUnpackedFolder, false, 4);
            byte[] pacPixelmap = PixelUtil.Decompress(File.ReadAllBytes(Path.Combine(pacUnpackedFolder, "0003")));
            var pacImg = ImageUtil.SpriteToImage(
                new SpriteImageInfo(pacPixelmap, palette, _pokemonSpriteWidth, pacPixelmap.Length / _pokemonSpriteWidth,
                isTiled,
                texFormat
                ));

            var totalWidth = texImg.Width + atxImg.Width + dtxImg.Width + pacImg.Width;
            var maxHeight = 1024;
            var combinedImage = new Image<Rgba32>(totalWidth, maxHeight);
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

            texImg.Dispose();
            atxImg.Dispose();
            dtxImg.Dispose();
            pacImg.Dispose();

            combinedImage.SaveAsPng(outFilePath);
            combinedImage.Dispose();
        });
    }
} 