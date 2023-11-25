#nullable enable
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RanseiLink.Console.GraphicsCommands;

public static class CellAnimationSerialiser
{
    public static void Serialise(string bgLinkFile, string animLinkFile, string outputFolder)
    {
        var bg = G2DR.LoadImgFromFile(bgLinkFile);
        var (width, height) = SerialiseBackground(bg.Ncgr, bg.Nclr, Path.Combine(outputFolder, "background.png"));

        var anim = G2DR.LoadAnimFromFile(animLinkFile);
        SerialiseAnimationXml(anim.Nanr, anim.Ncer, anim.Ncgr, anim.Nclr, outputFolder, width, height);
    }

    public static (int width, int height) SerialiseBackground(NCGR ncgr, NCLR nclr, string outputFolder)
    {
        using var image = NitroImageUtil.NcgrToImage(ncgr, nclr);

        image.SaveAsPng(Path.Combine(outputFolder, "background.png"));

        return (image.Width, image.Height);
    }

    public static void SerialiseAnimationXml(NANR nanr, NCER ncer, NCGR ncgr, NCLR nclr, string outputFolder, int width, int height)
    {
        // save the images

        var images = NitroImageUtil.NcerToMultipleImages(ncer, ncgr, nclr, width, height);

        for (int i = 0; i < images.Count; i++)
        {
            var cellImage = images[i];
            cellImage.SaveAsPng(Path.Combine(outputFolder, CellAnimationSerialiser.NumToFileName(i)));
            cellImage.Dispose();
        }

        // save the animation file

        var cells = new List<CellImage>();
        var anims = new List<Anim>();

        for (int i = 0; i < nanr.AnimationBanks.Banks.Count; i++)
        {
            var anim = nanr.AnimationBanks.Banks[i];
            var name = nanr.Labels.Names[i];
            anims.Add(new Anim(name, anim.Frames.Select(x => new AnimFrame(x.CellBank.ToString(), x.Duration)).ToList()));
        }

        for (int i = 0; i < ncer.CellBanks.Banks.Count; i++)
        {
            var cellBank = ncer.CellBanks.Banks[i];
            cells.Add(new CellImage(cellBank, i.ToString(), NumToFileName(i)));
        }

        var doc = Serialise(cells, anims);

        doc.Save(Path.Combine(outputFolder, "animation.xml"));
    }

    /// <summary>
    /// Warning: will throw an exception on failure
    /// </summary>
    public static NANR DeserialiseAnimationXml(string animationXmlFile, NCER ncer, NCGR ncgr, NCLR nclr)
    {
        // nanr is the only one where all info is recreated
        // however, i think ideally we have an intermediate "configuration" class which 
        // can either be generated from the existing files,
        // or created from scratch. Then the handling after that can be unified.
        var nanr = new NANR();

        var (cells, animations) = Deseralise(XDocument.Load(animationXmlFile));
        var dir = Path.GetDirectoryName(animationXmlFile)!;
        
        // clear it ready for adding our own cell banks
        ncer.CellBanks.Banks.Clear();
        ncer.Labels.Names.Clear();

        // load the cell info
        var nameToCellBankId = new Dictionary<string, ushort>();
        List<Image<Rgba32>> images = new();
        foreach (var cellImage in cells)
        {
            var bank = new CellBank();
            // store the mapping of name to bank to use later when loading animations
            nameToCellBankId.Add(cellImage.Name, (ushort)bank.Count);
            ncer.CellBanks.Banks.Add(bank);

            foreach (var cell in cellImage.Cells)
            {
                bank.Add(cell);
            }

            // image path is relative to the location of the xml file
            var imgPath = Path.Combine(dir, cellImage.File);
            images.Add(ImageUtil.LoadPngBetterError(imgPath));

            bank.EstimateMinMaxValues();
            // TODO: cellBank.ReadOnlyCellInfo = ?;
        }

        // import the image data
        NitroImageUtil.NcerImportFromMultipleImages(ncer, ncgr, nclr, images);

        // dispose of the images as we don't need them anymore
        foreach (var image in images)
        {
            image.Dispose();
        }

        // load the animations
        foreach (var anim in animations)
        {
            var targetAnim = new ABNK.Anim();
            // TODO: set datatype, unknowns1,2,3
            nanr.AnimationBanks.Banks.Add(targetAnim);
            nanr.Labels.Names.Add(anim.Name);
            ncer.Labels.Names.Add(anim.Name); // for some reason it stores animation names in here
            foreach (var frame in anim.Frames)
            {
                var targetFrame = new ABNK.Frame();
                targetAnim.Frames.Add(targetFrame);
                if (!nameToCellBankId.TryGetValue(frame.Image, out  var bankId))
                {
                    throw new Exception($"Animation '{anim.Name}' references cellImage of name '{frame.Image}' which doesn't exist");
                }
                targetFrame.CellBank = bankId;
                targetFrame.Duration = (ushort)frame.Duration;
            }
        }

        return nanr;
    }

    private static XDocument Serialise(List<CellImage> cells, List<Anim> animations)
    {
        var cellElem = new XElement("cell_collection", cells.Select(x => x.Serialise()));
        var animationElem = new XElement("animation_collection", animations.Select(x => x.Serialise()));

        return new XDocument(new XElement("nitro_cell_animation_resource", cellElem, animationElem));
    }

    private static (List<CellImage> Cells, List<Anim> Animations) Deseralise(XDocument doc)
    {
        var element = doc.ElementRequired("nitro_cell_animation_resource");

        var cells = element.ElementRequired("cell_collection").Elements("image").Select(x => new CellImage(x)).ToList();
        var anims = element.ElementRequired("animation_collection").Elements("animation").Select(x => new Anim(x)).ToList();

        return (cells, anims);
    }

    private static XElement SerialiseCell(Cell cell)
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

        return cellElem;
    }

    private static Cell DeserialiseCell(XElement element)
    {
        var cell = new Cell();
        cell.XOffset = element.AttributeInt("x");
        cell.YOffset = element.AttributeInt("y");
        cell.Width = element.AttributeInt("width");
        cell.Height = element.AttributeInt("height");
        cell.FlipX = element.AttributeBool("flip_x", false);
        cell.FlipY = element.AttributeBool("flip_y", false);
        return cell;
    }

    public static string NumToFileName(int num)
    {
        return $"{num.ToString().PadLeft(4, '0')}.png";
    }

    private class Anim
    {
        public string Name { get; }
        public List<AnimFrame> Frames { get; }

        public Anim(string name, List<AnimFrame> frames)
        {
            Name = name;
            Frames = frames;
        }

        public Anim(XElement animElem)
        {
            Name = animElem.AttributeStringNonEmpty("name");
            Frames = new List<AnimFrame>();
            foreach (var frameElem in animElem.Elements("frame"))
            {
                Frames.Add(new AnimFrame(frameElem));
            }
        }

        public XElement Serialise()
        {
            var trackElem = new XElement("animation", new XAttribute("name", Name));
            foreach (var keyFrame in Frames)
            {
                trackElem.Add(keyFrame.Serialise());
            }
            return trackElem;
        }
    }


    private class AnimFrame
    {
        public string Image { get; }
        public int Duration { get; }

        public AnimFrame(string image, int duration)
        {
            Image = image;
            Duration = duration;
        }

        public AnimFrame(XElement frameElem)
        {
            Image = frameElem.AttributeStringNonEmpty("image");
            Duration = frameElem.AttributeInt("duration");
        }

        public XElement Serialise()
        {
            return new XElement("frame",
                new XAttribute("image", Image),
                new XAttribute("duration", Duration)
                );
        }
    }

    private class CellImage
    {
        public CellBank Cells { get; }
        public string Name { get; }
        public string File { get; }

        public CellImage(XElement groupElem)
        {
            Name = groupElem.AttributeStringNonEmpty("name");
            File = groupElem.AttributeStringNonEmpty("file");
            Cells = new CellBank();
            foreach (var cellElement in groupElem.Elements("cell"))
            {
                var cell = DeserialiseCell(cellElement);
                Cells.Add(cell);
            }
        }

        public CellImage(CellBank cells, string name, string file)
        {
            Cells = cells;
            Name = name;
            File = file;
        }

        public XElement Serialise()
        {
            var groupElem = new XElement("image", new XAttribute("name", Name), new XAttribute("file", File));
            foreach (var cell in Cells)
            {
                groupElem.Add(SerialiseCell(cell));
            }
            return groupElem;
        }
    }
}


