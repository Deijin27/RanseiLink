using OpenTK.Graphics.OpenGL;
using RanseiLink.Core.Graphics;
using System.Linq;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;

namespace RanseiLink.View3D;

public static class MaterialRegistry
{
    private static readonly Dictionary<NSMDL.Model.Material, int> _materialToRegisteredTex = new();

    public static int GetRegisteredTexture(NSMDL.Model.Material material)
    {
        return _materialToRegisteredTex[material];
    }

    public static void LoadMaterials(NSBMD nsbmd, NSBTX nsbtx)
    {
        var model = nsbmd.Model.Models[0];

        foreach (var material in model.Materials)
        {
            _materialToRegisteredTex[material] = LoadTexture(nsbtx.Texture, material.Texture, material.Palette);
        }
    }

    public static void UnloadMaterials()
    {
        foreach (var texId in _materialToRegisteredTex.Values)
        {
            GL.DeleteTexture(texId);
        }
        _materialToRegisteredTex.Clear();
    }

    public static int LoadTexture(NSTEX nstex, string texName, string palName)
    {
        var tex = nstex.Textures.First(x => x.Name == texName);
        var pal = nstex.Palettes.First(x => x.Name == palName);

        using var image = GenerateImage(tex, pal);
        return RegisterTexture(image);
    }

    public static Image<Rgba32> GenerateImage(NSTEX.Texture tex, NSTEX.Palette pal)
    {
        var convPal = RawPalette.To32bitColors(pal.PaletteData);
        if (tex.Color0Transparent)
        {
            convPal[0] = SixLabors.ImageSharp.Color.Transparent;
        }

        return ImageUtil.ToImage(
            imageInfo: new SpriteImageInfo(pixels: tex.TextureData, palette: convPal, width: tex.Width, height: tex.Height),
            tiled: false,
            format: tex.Format);
    }

    public static int RegisterTexture(Image<Rgba32> image)
    {
        int id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, id);

        // ImageSharp loads from the top-left pixel, whereas OpenGL loads
        // from the bottom-left, causing the texture to be flipped vertically.
        // This will correct that, making the texture display properly.
        image.Mutate(x => x.Flip(FlipMode.Vertical));

        // Use the CopyPixelDataTo function from ImageSharp to copy all
        // of the bytes from the image into an array that we can give to OpenGL.
        var pixels = new byte[4 * image.Width * image.Height];
        image.CopyPixelDataTo(pixels);

        GL.TexImage2D(
            target: TextureTarget.Texture2D,
            level: 0,
            internalformat: PixelInternalFormat.Rgba,
            width: image.Width,
            height: image.Height,
            border: 0,
            format: PixelFormat.Rgba,
            type: PixelType.UnsignedByte,
            pixels: pixels
            );

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.Repeat);
        return id;
    }
}