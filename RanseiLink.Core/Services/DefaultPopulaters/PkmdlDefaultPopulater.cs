using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Core.Services.DefaultPopulaters
{
    public class PkmdlDefaultPopulater : IGraphicTypeDefaultPopulater
    {
        private const int _pokemonSpriteWidth = 32;

        public void ProcessExportedFiles(string defaultDataFolder, IGraphicsInfo gInfo)
        {
            if (!(gInfo is PkmdlConstants pkmdlInfo))
            {
                return;
            }

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

                BTX0 btx0 = new BTX0(Path.Combine(texUnpacked, fileName));

                Rgba32[] palette = RawPalette.To32bitColors(btx0.Texture.Palette1);
                palette[0] = Color.Transparent;

                var texImg = ImageUtil.ToImage(
                    new ImageInfo(btx0.Texture.PixelMap, palette, _pokemonSpriteWidth, btx0.Texture.PixelMap.Length / _pokemonSpriteWidth),
                    PointUtil.GetPoint
                    );

                byte[] atxPixelmap = RawChar.Decompress(File.ReadAllBytes(Path.Combine(atxUnpacked, fileName)));
                var atxImg = ImageUtil.ToImage(
                    new ImageInfo(atxPixelmap, palette, _pokemonSpriteWidth, atxPixelmap.Length / _pokemonSpriteWidth),
                    PointUtil.GetPoint
                    );

                byte[] dtxPixelmap = RawChar.Decompress(File.ReadAllBytes(Path.Combine(dtxUnpacked, fileName)));
                var dtxImg = ImageUtil.ToImage(
                    new ImageInfo(dtxPixelmap, palette, _pokemonSpriteWidth, dtxPixelmap.Length / _pokemonSpriteWidth),
                    PointUtil.GetPoint
                    );


                string pacFile = Path.Combine(pacUnpacked, fileName);
                string pacUnpackedFolder = Path.Combine(pacUnpacked, fileName + "-Unpacked");
                PAC.Unpack(pacFile, pacUnpackedFolder, false, 4);
                byte[] pacPixelmap = RawChar.Decompress(File.ReadAllBytes(Path.Combine(pacUnpackedFolder, "0003")));
                var pacImg = ImageUtil.ToImage(
                    new ImageInfo(pacPixelmap, palette, _pokemonSpriteWidth, pacPixelmap.Length / _pokemonSpriteWidth),
                    PointUtil.GetPoint
                    );

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
    } }