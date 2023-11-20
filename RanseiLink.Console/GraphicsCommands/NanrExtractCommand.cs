using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core;
using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("nanr extract", Description = "Print out informational content of Nitro Animation Resource")]
public class NanrExtractCommand : ICommand
{
    [CommandParameter(0, Description = "Path of G2DR which contains the nscr background", Name = "background")]
    public string Background { get; set; }

    [CommandParameter(1, Description = "Path of G2DR which contains the ncer parts and nanr animation", Name = "anim")]
    public string AnimatedParts { get; set; }

    [CommandOption("destinationFile", 'd', Description = "Optional destination folder; default is a dir in the same location as the file.")]
    public string DestinationFolder { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (DestinationFolder == null)
        {
            DestinationFolder = FileUtil.MakeUniquePath(Path.ChangeExtension(Background, null));
        }
        Directory.CreateDirectory(DestinationFolder);

        var temp = FileUtil.GetTemporaryDirectory();

        var bgLink = Path.Combine(temp, "background");
        LINK.Unpack(Background, bgLink, true, 4);
        var animLink = Path.Combine(temp, "anim");
        LINK.Unpack(AnimatedParts, animLink, true, 4);

        var (width, height) = LoadAndSerialiseBackground(bgLink, Path.Combine(DestinationFolder, "background.png"));
        // force the images to have the same width and height as the background
        ExportAnim(animLink, DestinationFolder, width, height);

        Directory.Delete(temp, true);

        return default;
    }

    private static (int width, int height) LoadAndSerialiseBackground(string sourceDir, string outputFile)
    {
        var ncgrPath = Path.Combine(sourceDir, "0003.ncgr");
        if (new FileInfo(ncgrPath).Length == 0)
        {
            ncgrPath = Path.Combine(sourceDir, "0001.ncgr");
        }

        var ncgr = NCGR.Load(ncgrPath);
        var nclr = NCLR.Load(Path.Combine(sourceDir, "0004.nclr"));

        using var image = NitroImageUtil.NcgrToImage(ncgr, nclr);

        image.SaveAsPng(outputFile);

        return (image.Width, image.Height);
    }


    private static void ExportAnim(string sourceDir, string outputFolder, int width, int height)
    {
        // load the source files

        var ncgrPath = Path.Combine(sourceDir, "0003.ncgr");
        if (new FileInfo(ncgrPath).Length == 0)
        {
            ncgrPath = Path.Combine(sourceDir, "0001.ncgr");
        }

        var ncer = NCER.Load(Path.Combine(sourceDir, "0002.ncer"));
        var ncgr = NCGR.Load(ncgrPath);
        var nclr = NCLR.Load(Path.Combine(sourceDir, "0004.nclr"));
        var nanr = NANR.Load(Path.Combine(sourceDir, "0000.nanr"));

        // save the images

        var images = NitroImageUtil.NcerToMultipleImages(ncer, ncgr, nclr, width, height);

        for (int i = 0; i < images.Count; i++)
        {
            var cellImage = images[i];
            cellImage.SaveAsPng(Path.Combine(outputFolder, CellAnimationSerialiser.NumToFileName(i)));
        }

        // save the animation file

        var doc = CellAnimationSerialiser.SerialiseAnimationXml(nanr, ncer);
        doc.Save(Path.Combine(outputFolder, "animation.xml"));
    }

    
}
