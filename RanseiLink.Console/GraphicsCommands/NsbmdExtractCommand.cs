using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Graphics.ExternalFormats;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nsbmd extract", Description = "Extract obj, mtl, and texture pngs from nsbmd")]
public class NsbmdExtractCommand : ICommand
{
    [CommandParameter(0, Description = "Path of folder containing nsbmd, nsbtx etc.", Name = "sourceDir")]
    public string SourceDir { get; set; }

    [CommandOption("destinationFolder", 'd', Description = "Optional destination folder to unpack to; default is a folder in the same location as the file.")]
    public string DestinationFolder { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFolder == null)
        {
            DestinationFolder = SourceDir + " - Extracted";
        }
        Directory.CreateDirectory(DestinationFolder);

        var files = Directory.GetFiles(SourceDir);
        string nsbtxFile = null;
        string nsbmdFile = null;
        foreach (var file in files)
        {
            var fileTypeNum = PAC.ExtensionToFileTypeNumber(Path.GetExtension(file));
            if (fileTypeNum == 0)
            {
                nsbmdFile = file;
            }
            else if (fileTypeNum == 1)
            {
                nsbtxFile = file;
            }
        }

        if (nsbtxFile == null)
        {
            console.Output.WriteLine($"Could not find .nsbtx file in folder: {SourceDir}");
            return default;
        }
        if (nsbmdFile == null)
        {
            console.Output.WriteLine($"Could not find .nsbmd file in folder: {SourceDir}");
            return default;
        }

        var bmd = new NSBMD(nsbmdFile);
        var btx = new NSBTX(nsbtxFile);

        foreach (var model in bmd.Model.Models)
        {
            console.Output.WriteLine("Exporting model: {0}", model.Name);
            ExtractModel(model);
            ExtractMaterialAndTextures(model, btx, console);
        }

        return default;
    }

    private void ExtractModel(NSMDL.Model model)
    {
        var obj = ConvertModels.ModelToObj(model);
        obj.Save(Path.Combine(DestinationFolder, model.Name + ".obj"));
    }

    private void ExtractMaterialAndTextures(NSMDL.Model model, NSBTX btx, IConsole console)
    {
        var mtl = new MTL();
        for (int i = 0; i < model.Materials.Count; i++)
        {
            NSMDL.Model.Material material = model.Materials[i];
            console.Output.WriteLine("  Exporting material: {0}", material.Name);

            var m = new MTL.Material
            {
                Name = material.Name,
                AmbientColor = RawPalette.To32BitColor(material.Ambient),
                DiffuseColor = RawPalette.To32BitColor(material.Diffuse),
                SpecularColor = RawPalette.To32BitColor(material.Specular),
                Dissolve = (material.PolyAttr >> 16 & 31) / 31f,
            };
            mtl.Materials.Add(m);

            string texFileNameWithExt = material.Texture + ".png";
            m.AmbientTextureMapFile = texFileNameWithExt;
            m.DiffuseTextureMapFile = texFileNameWithExt;
            m.SpecularTextureMapFile = texFileNameWithExt;
            m.DissolveTextureMapFile = texFileNameWithExt;

            if (btx != null)
            {
                console.Output.WriteLine("    Exporting texture: {0}", material.Texture);

                var tex = btx.Texture.Textures.First(x => x.Name == material.Texture);
                var pal = btx.Texture.Palettes.First(x => x.Name == material.Palette);

                var convPal = RawPalette.To32bitColors(pal.PaletteData);
                if (tex.Color0Transparent)
                {
                    convPal[0] = SixLabors.ImageSharp.Color.Transparent;
                }
                ImageUtil.SaveAsPng(Path.Combine(DestinationFolder, texFileNameWithExt),
                    new ImageInfo(tex.TextureData, convPal, tex.Width, tex.Height),
                    tiled: false,
                    format: tex.Format);
            }

        }
        mtl.Save(Path.Combine(DestinationFolder, model.Name + ".mtl"));
    }
}
