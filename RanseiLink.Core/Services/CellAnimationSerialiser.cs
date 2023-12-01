using RanseiLink.Core.Archive;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RanseiLink.Core.Services;

public static class CellAnimationSerialiser
{
    public static void SerialiseAnimation(PositionRelativeTo prt, string outputFolder, string animLinkFile, int width, int height, bool mergeCells)
    {
        var anim = G2DR.LoadAnimFromFile(animLinkFile);
        SerialiseAnimationXml(anim.Nanr, anim.Ncer, anim.Ncgr, anim.Nclr, outputFolder, width, height, prt, mergeCells);
    }

    public static void Serialise(PositionRelativeTo prt, string outputFolder, string bgLinkFile, string? animLinkFile = null)
    {
        var bg = G2DR.LoadImgFromFile(bgLinkFile);
        var (width, height) = SerialiseBackground(bg.Ncgr, bg.Nclr, outputFolder);

        if (animLinkFile != null)
        {
            SerialiseAnimation(prt, outputFolder, animLinkFile, width, height, mergeCells: true);
        }
    }


    // We will always have a background file to use, and we will always have the default of another anim to use
    // but it may be nice if we can generate without one to base it on. it's just more work.
    public static void Deserialise(PositionRelativeTo prt, string inputFolder, string bgLinkFile, string outputBgLinkFile, string? animLinkFile = null, string? outputAnimLinkFile = null)
    {
        int width;
        int height;
        // load background
        var tempBg = FileUtil.GetTemporaryDirectory();
        try
        {
            LINK.Unpack(bgLinkFile, tempBg);
            var bg = G2DR.LoadImgFromFolder(bgLinkFile);
            (width, height) = DeserialiseBackground(inputFolder, bg.Ncgr, bg.Nclr);
            G2DR.SaveImgToFolder(tempBg, bg.Ncgr, bg.Nclr, NcgrSlot.Infer);
            LINK.Pack(tempBg, outputBgLinkFile);
        }
        finally
        {
            Directory.Delete(tempBg, true);
        }
        
        // load animation
        if (animLinkFile != null && outputAnimLinkFile != null)
        {
            var tempAnim = FileUtil.GetTemporaryDirectory();
            try
            {
                LINK.Unpack(animLinkFile, tempAnim);
                var anim = G2DR.LoadCellImgFromFolder(tempAnim);
                var nanr = DeserialiseAnimationXml(Path.Combine(inputFolder, "animation.xml"), anim.Ncer, anim.Ncgr, anim.Nclr, width, height, prt);
                G2DR.SaveAnimToFolder(tempAnim, nanr, anim.Ncer, anim.Ncgr, anim.Nclr, NcgrSlot.Infer);
                LINK.Pack(tempAnim, outputAnimLinkFile);
            }
            finally
            {
                Directory.Delete(tempAnim, true);
            }
        }
    }
    /*
    public static void DeserialiseFromScratch(string inputFolder, string outputBgLinkFile, string? outputAnimLinkFile = null)
    {
        // technically we will always have the background file default to work off of
        var tempBg = FileUtil.GetTemporaryDirectory();
        try
        {
            var ncgr = new NCGR();
            var nclr = new NCLR();
            
            // TODO: set up the ncgr and nclr properly
            var (width, height) = DeserialiseBackground(inputFolder, ncgr, nclr);
            // G2DR.SaveScreenToFile(tempBg, nscr, ncgr, nclr, NcgrSlot.Slot1);
        }
        finally
        {
            Directory.Delete(tempBg, true);
        }

        // load animation
        if (outputAnimLinkFile != null)
        {
            var tempAnim = FileUtil.GetTemporaryDirectory();
            try
            {
                var ncer = new NCER();
                var ncgr = new NCGR();
                var nclr = new NCLR();
                // TODO: set up the nanr, ncgr and nclr properly
                var nanr = DeserialiseAnimationXml(Path.Combine(inputFolder, "animation.xml"), ncer, ncgr, nclr);
                // G2DR.SaveAnimToFile(tempAnim, nanr, ncer, ncgr, nclr, NcgrSlot.Slot1);
            }
            finally
            {
                Directory.Delete(tempAnim, true);
            }
        }
    }
    */

    public static (int width, int height) SerialiseBackground(NCGR ncgr, NCLR nclr, string outputFolder)
    {
        using var image = NitroImageUtil.NcgrToImage(ncgr, nclr);

        image.SaveAsPng(Path.Combine(outputFolder, "background.png"));

        return (image.Width, image.Height);
    }

    public static (int width, int height) DeserialiseBackground(string inputFolder, NCGR ncgr, NCLR nclr)
    {
        using var image = ImageUtil.LoadPngBetterError(Path.Combine(inputFolder, "background.png"));
        NitroImageUtil.NcgrFromImage(ncgr, nclr, image);
        return (image.Width, image.Height);
    }

    public static void SerialiseAnimationXml(NANR nanr, NCER ncer, NCGR ncgr, NCLR nclr, string outputFolder, int width, int height, PositionRelativeTo prt, bool mergeCells)
    {
        int xShift;
        int yShift;
        if (prt == PositionRelativeTo.Centre)
        {
            xShift = width / 2;
            yShift = height / 2;
        }
        else
        {
            xShift = 0;
            yShift = 0;
        }

        var res = new Resource();

        // save animations
        for (int i = 0; i < nanr.AnimationBanks.Banks.Count; i++)
        {
            var anim = nanr.AnimationBanks.Banks[i];
            var name = nanr.Labels.Names[i];
            res.Animations.Add(new Anim(name, anim.Frames.Select(x => new AnimFrame(x.CellBank.ToString(), x.Duration)).ToList()));
        }

        // save cells
        if (mergeCells)
        {
            var images = NitroImageUtil.NcerToMultipleImages(ncer, ncgr, nclr, width, height, prt);

            for (int bankId = 0; bankId < ncer.CellBanks.Banks.Count; bankId++)
            {
                // save bank image
                var bankImage = images[bankId];
                var fileName = $"{bankId.ToString().PadLeft(4, '0')}.png";
                bankImage.SaveAsPng(Path.Combine(outputFolder, fileName));
                bankImage.Dispose();

                // save bank data
                var cellBank = ncer.CellBanks.Banks[bankId];
                var bankData = new CellBankInfo(bankId.ToString()) { File = fileName };
                res.Cells.Add(bankData);

                foreach (var cell in cellBank)
                {
                    // save cell data
                    var cellData = new CellInfo(cell);
                    cellData.Cell.XOffset += xShift;
                    cellData.Cell.YOffset += yShift;
                }
            }
        }
        else
        {
            var imageGroups = NitroImageUtil.NcerToMultipleImageGroups(ncer, ncgr, nclr);

            for (int bankId = 0; bankId < ncer.CellBanks.Banks.Count; bankId++)
            {
                // prepare image folders
                string folderName = bankId.ToString().PadLeft(4, '0');
                Directory.CreateDirectory(Path.Combine(outputFolder, folderName));
                var group = imageGroups[bankId];

                // save bank data
                var cellBank = ncer.CellBanks.Banks[bankId];
                var bankData = new CellBankInfo(bankId.ToString());
                res.Cells.Add(bankData);

                for (int cellId = 0; cellId < cellBank.Count; cellId++)
                {
                    // save cell image
                    var cellImage = group[cellId];
                    var fileName = $"{folderName}/{cellId.ToString().PadLeft(4, '0')}.png";
                    cellImage.SaveAsPng(Path.Combine(outputFolder, fileName));

                    // save cell data
                    var cell = cellBank[cellId];
                    var cellData = new CellInfo(cell);
                    cellData.Cell.XOffset += xShift;
                    cellData.Cell.YOffset += yShift;
                }
            }
        }

        var doc = res.Serialise();
        doc.Save(Path.Combine(outputFolder, "animation.xml"));
    }

    /// <summary>
    /// Warning: will throw an exception on failure
    /// </summary>
    public static NANR DeserialiseAnimationXml(string animationXmlFile, NCER ncer, NCGR ncgr, NCLR nclr, int width, int height, PositionRelativeTo prt, bool mergeCells)
    {
        int xShift;
        int yShift;
        if (prt == PositionRelativeTo.Centre)
        {
            xShift = width / 2;
            yShift = height / 2;
        }
        else
        {
            xShift = 0;
            yShift = 0;
        }

        // nanr is the only one where all info is recreated
        // however, i think ideally we have an intermediate "configuration" class which 
        // can either be generated from the existing files,
        // or created from scratch. Then the handling after that can be unified.
        var nanr = new NANR();

        var res = new Resource(XDocument.Load(animationXmlFile));
        var dir = Path.GetDirectoryName(animationXmlFile)!;
        
        // clear it ready for adding our own cell banks
        ncer.CellBanks.Banks.Clear();
        ncer.Labels.Names.Clear();

        // load the cell info
        var nameToCellBankId = new Dictionary<string, ushort>();

        if (mergeCells)
        {
            List<Image<Rgba32>> images = new();
            foreach (var cellBankInfo in res.Cells)
            {
                var bank = new CellBank();
                // store the mapping of name to bank to use later when loading animations
                nameToCellBankId.Add(cellBankInfo.Name, (ushort)bank.Count);
                ncer.CellBanks.Banks.Add(bank);

                foreach (var cellInfo in cellBankInfo.Cells)
                {
                    var cell = cellInfo.Cell;
                    cell.XOffset -= xShift;
                    cell.YOffset -= yShift;
                    bank.Add(cell);
                }

                // image path is relative to the location of the xml file
                if (cellBankInfo.File == null)
                {
                    throw new Exception("Missing required attribute 'file' on cell group");
                }
                var imgPath = Path.Combine(dir, cellBankInfo.File);
                images.Add(ImageUtil.LoadPngBetterError(imgPath));

                bank.EstimateMinMaxValues();
                // TODO: cellBank.ReadOnlyCellInfo = ?;
            }

            // import the image data
            NitroImageUtil.NcerFromMultipleImages(ncer, ncgr, nclr, images, prt);

            // dispose of the images as we don't need them anymore
            foreach (var image in images)
            {
                image.Dispose();
            }
        }
        else
        {
            var imageGroups = new List<IReadOnlyList<Image<Rgba32>>>();
            foreach (var cellBankInfo in res.Cells)
            {
                List<Image<Rgba32>> images = new();
                imageGroups.Add(images);
                var bank = new CellBank();
                // store the mapping of name to bank to use later when loading animations
                nameToCellBankId.Add(cellBankInfo.Name, (ushort)bank.Count);
                ncer.CellBanks.Banks.Add(bank);

                foreach (var cellInfo in cellBankInfo.Cells)
                {
                    var cell = cellInfo.Cell;
                    cell.XOffset -= xShift;
                    cell.YOffset -= yShift;
                    bank.Add(cell);
                    if (cellInfo.File == null)
                    {
                        throw new Exception("Missing required attribute 'file' on cell");
                    }
                    images.Add(ImageUtil.LoadPngBetterError(cellInfo.File));
                }

                bank.EstimateMinMaxValues();
                // TODO: cellBank.ReadOnlyCellInfo = ?;
            }

            // import the image data
            NitroImageUtil.NcerFromMultipleImageGroups(ncer, ncgr, nclr, imageGroups);
        }
        
        // load the animations
        foreach (var anim in res.Animations)
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
                if (!nameToCellBankId.TryGetValue(frame.Image, out var bankId))
                {
                    throw new Exception($"Animation '{anim.Name}' references cellImage of name '{frame.Image}' which doesn't exist");
                }
                targetFrame.CellBank = bankId;
                targetFrame.Duration = (ushort)frame.Duration;
            }
        }

        return nanr;
    }

    

    public class Resource
    {
        public List<CellBankInfo> Cells { get; }
        public List<Anim> Animations { get; }

        public Resource()
        {
            Cells = new();
            Animations = new();
        }

        public Resource(XDocument doc)
        {
            var element = doc.ElementRequired("nitro_cell_animation_resource");

            Cells = element.ElementRequired("cell_collection").Elements("image").Select(x => new CellBankInfo(x)).ToList();
            Animations = element.ElementRequired("animation_collection").Elements("animation").Select(x => new Anim(x)).ToList();
        }

        public XDocument Serialise()
        {
            var cellElem = new XElement("cell_collection", Cells.Select(x => x.Serialise()));
            var animationElem = new XElement("animation_collection", Animations.Select(x => x.Serialise()));

            return new XDocument(new XElement("nitro_cell_animation_resource", cellElem, animationElem));
        }
    }

    public class Anim
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

    public class AnimFrame
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

    public class CellInfo
    {
        public Cell Cell { get; set; }
        public string? File { get; set; }

        public CellInfo(Cell cell)
        {
            Cell = cell;
        }

        public XElement Serialise()
        {
            var cellElem = new XElement("cell",
                        new XAttribute("x", Cell.XOffset),
                        new XAttribute("y", Cell.YOffset),
                        new XAttribute("width", Cell.Width),
                        new XAttribute("height", Cell.Height)
                        );

            if (Cell.FlipX)
            {
                cellElem.Add(new XAttribute("flip_x", Cell.FlipX));
            }
            if (Cell.FlipY)
            {
                cellElem.Add(new XAttribute("flip_y", Cell.FlipY));
            }

            if (!string.IsNullOrEmpty(File))
            {
                cellElem.Add(new XAttribute("file", File));
            }

            return cellElem;
        }

        public CellInfo(XElement element)
        {
            Cell = new Cell
            {
                XOffset = element.AttributeInt("x"),
                YOffset = element.AttributeInt("y"),
                Width = element.AttributeInt("width"),
                Height = element.AttributeInt("height"),
                FlipX = element.AttributeBool("flip_x", false),
                FlipY = element.AttributeBool("flip_y", false)
            };

            File = element.Attribute("file")?.Value;
        }
    }

    public class CellBankInfo
    {
        public List<CellInfo> Cells { get; }
        public string Name { get; }
        public string? File { get; set; }

        public CellBankInfo(XElement groupElem)
        {
            Name = groupElem.AttributeStringNonEmpty("name");
            File = groupElem.Attribute("file")?.Value;
            Cells = new();
            foreach (var cellElement in groupElem.Elements("cell"))
            {
                var cell = new CellInfo(cellElement);
                Cells.Add(cell);
            }
        }

        public CellBankInfo(string name)
        {
            Cells = new();
            Name = name;
        }

        public XElement Serialise()
        {
            var groupElem = new XElement("image", new XAttribute("name", Name));
            if (!string.IsNullOrEmpty(File))
            {
                groupElem.Add(new XAttribute("file", File));
            }
            foreach (var cell in Cells)
            {
                groupElem.Add(cell.Serialise());
            }
            return groupElem;
        }
    }
}


