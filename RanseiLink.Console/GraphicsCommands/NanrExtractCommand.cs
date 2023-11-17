using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core;
using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using SixLabors.ImageSharp;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        var (width, height) = LoadBackground(bgLink, Path.Combine(DestinationFolder, "background.png"));
        LoadAnim(animLink, DestinationFolder, width, height);

        Directory.Delete(temp, true);

        return default;
    }

    private static (int width, int height) LoadBackground(string sourceDir, string outputFile)
    {
        var ncgrPath = Path.Combine(sourceDir, "0003.ncgr");
        if (new FileInfo(ncgrPath).Length == 0)
        {
            ncgrPath = Path.Combine(sourceDir, "0001.ncgr");
        }

        using var image = NitroImageUtil.NcgrToImage(
            NCGR.Load(ncgrPath),
            NCLR.Load(Path.Combine(sourceDir, "0004.nclr"))
            );

        image.SaveAsPng(outputFile);

        return (image.Width, image.Height);
    }

    private static void LoadAnim(string sourceDir, string outputFolder, int width, int height)
    {
        // get the images

        var ncgrPath = Path.Combine(sourceDir, "0003.ncgr");
        if (new FileInfo(ncgrPath).Length == 0)
        {
            ncgrPath = Path.Combine(sourceDir, "0001.ncgr");
        }

        var ncer = NCER.Load(Path.Combine(sourceDir, "0002.ncer"));

        var images = NitroImageUtil.NcerToMultipleImages(
            ncer,
            NCGR.Load(ncgrPath),
            NCLR.Load(Path.Combine(sourceDir, "0004.nclr")),
            width, // force them to have the same width and height as the background
            height
            );

        for (int i = 0; i < images.Count; i++)
        {
            var cellImage = images[i];
            cellImage.SaveAsPng(Path.Combine(outputFolder, NumToFileName(i)));
        }

        // get the animation

        var anim = NANR.Load(Path.Combine(sourceDir, "0000.nanr"));
        var doc = SerialiseAnimation(anim, ncer);
        doc.Save(Path.Combine(outputFolder, "animation.xml"));
    }

    private static XDocument SerialiseAnimation(NANR anim, NCER ncer)
    {
        var cells = new XElement("cell_collection");
        for (int i = 0; i < ncer.CellBanks.Banks.Count; i++)
        {
            var cellBank = ncer.CellBanks.Banks[i];
            var groupElem = new XElement("image", new XAttribute("name", i), new XAttribute("file", NumToFileName(i)));
            foreach (var cell in cellBank)
            {
                var cellElem = new XElement("cell",
                    new XAttribute("x", cell.XOffset),
                    new XAttribute("y", cell.YOffset),
                    new XAttribute("width", cell.Width),
                    new XAttribute("height", cell.Height)
                    );

                if (cell.FlipX)
                {
                    cellElem.Add(new XAttribute("flip_x", cell.FlipX));
                }
                if (cell.FlipY)
                {
                    cellElem.Add(new XAttribute("flip_y", cell.FlipY));
                }

                groupElem.Add(cellElem);    
            }
            cells.Add(groupElem);
        }

        var animations = new XElement("animation_collection");
        for (int i = 0; i < anim.AnimationBanks.Banks.Count; i++)
        {
            var bank = anim.AnimationBanks.Banks[i];
            var name = anim.Labels.Names[i];

            var trackElem = new XElement("animation", new XAttribute("name", name));
            foreach (var keyFrame in bank.Frames)
            {
                trackElem.Add(new XElement("frame",
                    new XAttribute("image", keyFrame.CellBank),
                    new XAttribute("duration", keyFrame.Duration)
                    ));
            }
            animations.Add(trackElem);
        }

        return new XDocument(new XElement("nitro_cell_animation_resource", cells, animations));
    }

    private static string NumToFileName(int num)
    {
        return $"{num.ToString().PadLeft(4, '0')}.png";
    }
}
