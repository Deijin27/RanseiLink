using FluentResults;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Util;
using System.Xml.Linq;

namespace RanseiLink.Core.Services;

public class PaletteTextureMap
{
    private readonly List<PaletteTexturePair> _pairs = [];

    public void Add(string texture, string palette)
    {
        foreach (var item in _pairs)
        {
            if (texture == item.Texture && palette == item.Palette)
            {
                return;
            }
        }

        _pairs.Add(new PaletteTexturePair(GetOutputImage(texture, palette), texture, palette));
    }

    public string GetOutputImage(string texture, string palette)
    {
        const string baseFix = "base_fix_";
        if (texture.StartsWith(baseFix) && palette.StartsWith(baseFix))
        {
            // nicety, make the file name a bit shorter
            palette = palette[baseFix.Length..];
        }
        return $"{texture}_{palette}.png";
    }

    public static Result<PaletteTextureMap> Load(string file)
    {
        XDocument doc;
        try
        {
            doc = XDocument.Load(file);

            var root = doc.ElementRequired("palette_texture_map");

            var map = new PaletteTextureMap();

            foreach (var item in root.Elements("item"))
            {
                var image = item.AttributeStringNonEmpty("image");
                var texture = item.AttributeStringNonEmpty("texture");
                var palette = item.AttributeStringNonEmpty("palette");
                // ensure not duplicate, we don't wanna ignore potential user mistakes that need correcting
                foreach (var existing in map._pairs)
                {
                    if (string.Equals(existing.Image, image, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception($"Duplicate item image='{image}'");
                    }
                    if (existing.Texture == texture && existing.Palette == palette)
                    {
                        throw new Exception($"Duplicate item texture='{texture}' palette='{palette}'");
                    }
                }
                // add it to the collection
                map._pairs.Add(new PaletteTexturePair(image, texture, palette));
            }

            return Result.Ok(map);
        }
        catch (Exception e)
        {
            return Result.Fail<PaletteTextureMap>($"Failed to load palette texture map xml document '{file}'. Reason: {e}");
        }
    }


    public void Save(string path)
    {
        var el = new XElement("palette_texture_map");
        foreach (var pair in _pairs)
        {
            el.Add(new XElement("item",
                new XAttribute("image", pair.Image),
                new XAttribute("texture", pair.Texture),
                new XAttribute("palette", pair.Palette)
                ));
        }
        var doc = new XDocument(el);
        doc.Save(path);
    }

    public void SaveTextures(NSTEX nstex, string outputFolder)
    {
        foreach (var pair in _pairs)
        {
            ExtractTexture(nstex, pair, outputFolder);
        }
    }

    private static void ExtractTexture(NSTEX nstex, PaletteTexturePair pair, string destinationFolder)
    {
        var tex = nstex.Textures.First(x => x.Name == pair.Texture);
        var pal = nstex.Palettes.First(x => x.Name == pair.Palette);

        var convPal = new Palette(pal.PaletteData, true);
        ImageUtil.SpriteToPng(Path.Combine(destinationFolder, pair.Image),
            new SpriteImageInfo(tex.TextureData, convPal, tex.Width, tex.Height,
              IsTiled: false,
              Format: tex.Format));
    }

    private record PaletteTexturePair(string Image, string Texture, string Palette);
}
