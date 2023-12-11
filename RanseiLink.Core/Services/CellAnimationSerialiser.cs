using RanseiLink.Core.Archive;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Models;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace RanseiLink.Core.Services;

public enum AnimationTypeId
{
    Castlemap,
    KuniImage2,
    IconCastle,
    IconInst
}

public record AnimationTypeInfo(
    AnimationTypeId Id, 
    string AnimationLinkFormat, 
    SpriteType? Background = null, 
    CellAnimationSerialiser.Format? ExportFormat = null
    )
{
    public string GetRelativePath(int id)
    {
        return string.Format(AnimationLinkFormat, id);
    }
}

public static class AnimationTypeInfoResource
{
    private static readonly Dictionary<AnimationTypeId, AnimationTypeInfo> _types = new();
    private static void Register(AnimationTypeInfo type) => _types.Add(type.Id, type);

    static AnimationTypeInfoResource()
    {
        Register(new(
            Id: AnimationTypeId.Castlemap,
            AnimationLinkFormat: "graphics/strategy/castlemap_anime/03_05_parts_castlemap_{0:D2}_lo.G2DR",
            Background: SpriteType.Castlemap
            ));
        Register(new(
            Id: AnimationTypeId.KuniImage2,
            AnimationLinkFormat: "graphics/strategy/kuniimage2/03_00_parts_kuniimage_00_up_{0:D2}.G2DR",
            Background: SpriteType.KuniImage2
            ));
        Register(new(
            Id: AnimationTypeId.IconCastle,
            AnimationLinkFormat: "graphics/strategy/icon_castle/03_01_parts_castleicon_{0:D2}_lo.G2DR",
            ExportFormat: CellAnimationSerialiser.Format.OneImagePerCell
            ));
        Register(new(
            Id: AnimationTypeId.IconInst,
            AnimationLinkFormat: "graphics/strategy/icon_inst/03_05_parts_shisetsuicon_{0:D2}_lo.G2DR",
            ExportFormat: CellAnimationSerialiser.Format.OneImagePerCell
            ));
    }

    public static AnimationTypeInfo Get(AnimationTypeId id) => _types[id];
}

public class CellAnimationManager
{

    private readonly IOverrideDataProvider _overrideDataProvider;
    private readonly IFallbackDataProvider _fallbackDataProvider;

    public CellAnimationManager(IOverrideDataProvider overrideDataProvider, IFallbackDataProvider fallbackDataProvider)
    {
        _overrideDataProvider = overrideDataProvider;
        _fallbackDataProvider = fallbackDataProvider;
    }

    public DataFile GetDataFile(AnimationTypeId type, int id)
    {
        var info = AnimationTypeInfoResource.Get(type);
        return _overrideDataProvider.GetDataFile(info.GetRelativePath(id));
    }

    public void ClearOverride(AnimationTypeId type, int id)
    {
        var info = AnimationTypeInfoResource.Get(type);
        _overrideDataProvider.ClearOverride(info.GetRelativePath(id));
        if (info.Background != null)
        {
            _overrideDataProvider.ClearOverride(info.Background.Value, id);
        }
    }

    public void SetOverride(AnimationTypeId type, int id, string file, string? backgroundFile = null)
    {
        var info = AnimationTypeInfoResource.Get(type);
        _overrideDataProvider.SetOverride(info.GetRelativePath(id), file);
        if (info.Background != null && backgroundFile != null)
        {
            _overrideDataProvider.SetOverride(info.Background.Value, id, backgroundFile);
        }
    }
}

public static class CellAnimationSerialiser
{
    public enum Format
    {
        OneImagePerCell,
        OneImagePerBank
    }

    public static void ExportAnimation(PositionRelativeTo prt, string outputFolder, string animLinkFile, int width, int height, Format fmt)
    {
        var anim = G2DR.LoadAnimFromFile(animLinkFile);
        ExportAnimationXml(anim.Nanr, anim.Ncer, anim.Ncgr, anim.Nclr, outputFolder, width, height, prt, fmt);
    }

    public static void ImportAnimation(PositionRelativeTo prt, string animLinkFile, string inputFolder, int width, int height, string outputAnimLinkFile)
    {
        var tempAnim = FileUtil.GetTemporaryDirectory();
        try
        {
            LINK.Unpack(animLinkFile, tempAnim);
            var anim = G2DR.LoadCellImgFromFolder(tempAnim);
            var nanr = ImportAnimationXml(Path.Combine(inputFolder, "animation.xml"), anim.Ncer, anim.Ncgr, anim.Nclr, width, height, prt);
            G2DR.SaveAnimToFolder(tempAnim, nanr, anim.Ncer, anim.Ncgr, anim.Nclr, NcgrSlot.Infer);
            LINK.Pack(tempAnim, outputAnimLinkFile);
        }
        finally
        {
            Directory.Delete(tempAnim, true);
        }
    }

    public static void Export(PositionRelativeTo prt, Format fmt, string outputFolder, string bgLinkFile, string? animLinkFile = null)
    {
        var bg = G2DR.LoadImgFromFile(bgLinkFile);
        var (width, height) = ExportBackground(bg.Ncgr, bg.Nclr, outputFolder);

        if (animLinkFile != null)
        {
            ExportAnimation(prt, outputFolder, animLinkFile, width, height, fmt: fmt);
        }
    }


    // We will always have a background file to use, and we will always have the default of another anim to use
    // but it may be nice if we can generate without one to base it on. it's just more work.
    public static void Import(PositionRelativeTo prt, string inputFolder, string bgLinkFile, string outputBgLinkFile, string? animLinkFile = null, string? outputAnimLinkFile = null)
    {
        int width;
        int height;
        // load background
        var tempBg = FileUtil.GetTemporaryDirectory();
        try
        {
            LINK.Unpack(bgLinkFile, tempBg);
            var bg = G2DR.LoadImgFromFolder(bgLinkFile);
            (width, height) = ImportBackground(inputFolder, bg.Ncgr, bg.Nclr);
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
            ImportAnimation(prt, animLinkFile, inputFolder, width, height, outputAnimLinkFile);
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

    public static (int width, int height) ExportBackground(NCGR ncgr, NCLR nclr, string outputFolder)
    {
        using var image = NitroImageUtil.NcgrToImage(ncgr, nclr);

        image.SaveAsPng(Path.Combine(outputFolder, "background.png"));

        return (image.Width, image.Height);
    }

    public static (int width, int height) ImportBackground(string inputFolder, NCGR ncgr, NCLR nclr)
    {
        using var image = ImageUtil.LoadPngBetterError(Path.Combine(inputFolder, "background.png"));
        NitroImageUtil.NcgrFromImage(ncgr, nclr, image);
        return (image.Width, image.Height);
    }

    public static void ExportAnimationXml(NANR nanr, NCER ncer, NCGR ncgr, NCLR nclr, string outputFolder, int width, int height, PositionRelativeTo prt, Format fmt)
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
        if (fmt == Format.OneImagePerBank)
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
                    var cellData = new CellInfo()
                    {
                        File = fileName,
                        X = cell.XOffset + xShift,
                        Y = cell.YOffset + yShift,
                        FlipX = cell.FlipX,
                        FlipY = cell.FlipY,
                        Palette = cell.IndexPalette,
                        DoubleSize = cell.DoubleSize,
                        Height = cell.Height,
                        Width = cell.Width,
                    };
                    bankData.Cells.Add(cellData);
                }
            }
        }
        else if (fmt == Format.OneImagePerCell)
        {
            var distinctImages = new List<(int TileOffset, int IndexPalette, byte[] Hash, string FileName)>();
            var imageGroups = NitroImageUtil.NcerToMultipleImageGroups(ncer, ncgr, nclr);

            for (int bankId = 0; bankId < ncer.CellBanks.Banks.Count; bankId++)
            {
                // prepare image folders
                string folderName = bankId.ToString().PadLeft(4, '0');
                //Directory.CreateDirectory(Path.Combine(outputFolder, folderName));
                var group = imageGroups[bankId];

                // save bank data
                var cellBank = ncer.CellBanks.Banks[bankId];
                var bankData = new CellBankInfo(bankId.ToString());
                res.Cells.Add(bankData);

                for (int cellId = 0; cellId < cellBank.Count; cellId++)
                {
                    var cell = cellBank[cellId];

                    // save cell image
                    var cellImage = group[cellId];
                    var fileName = $"{folderName}_{cellId.ToString().PadLeft(4, '0')}.png";
                    var filePath = Path.Combine(outputFolder, fileName);
                    cellImage.SaveAsPng(filePath);

                    // only save distinct images
                    var sha = Sha256File(filePath);
                    bool found = false;
                    foreach (var tup in distinctImages)
                    {
                        // tup.TileOffset == cell.TileOffset && tup.IndexPalette == cell.IndexPalette && 
                        if (tup.Hash.SequenceEqual(sha))
                        {
                            found = true;
                            fileName = tup.FileName;
                            File.Delete(filePath);
                        }
                    }
                    if (!found)
                    {
                        distinctImages.Add((cell.TileOffset, cell.IndexPalette, sha, fileName));
                    }
                    // save cell data
                    
                    var cellData = new CellInfo() 
                    { 
                        File = fileName,
                        X = cell.XOffset + xShift,
                        Y = cell.YOffset + yShift,
                        FlipX = cell.FlipX,
                        FlipY = cell.FlipY,
                        Palette = cell.IndexPalette,
                        DoubleSize = cell.DoubleSize,
                    };
                    bankData.Cells.Add(cellData);
                }
            }
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(fmt), fmt, null);
        }

        var doc = res.Serialise();
        doc.Save(Path.Combine(outputFolder, "animation.xml"));
    }

    private static byte[] Sha256File(string file)
    {
        using (var sha = SHA256.Create())
        {
            using (var fs = File.OpenRead(file))
            {
                return sha.ComputeHash(fs);
            }
        }
    }

    /// <summary>
    /// Warning: will throw an exception on failure
    /// </summary>
    public static NANR ImportAnimationXml(string animationXmlFile, NCER ncer, NCGR ncgr, NCLR nclr, int width, int height, PositionRelativeTo prt)
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

        var fmt = res.Format;
        if (fmt == Format.OneImagePerBank)
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
                    if (cellInfo.Width < 0)
                    {
                        throw new Exception($"Missing required attribute 'width' on cell for format {fmt}");
                    }
                    if (cellInfo.Height < 0)
                    {
                        throw new Exception($"Missing required attribute 'height' on cell for format {fmt}");
                    }
                    var cell = new Cell
                    {
                        RotateOrScale = cellInfo.DoubleSize ? RotateOrScale.Scale : RotateOrScale.Rotate,
                        XOffset = cellInfo.X - xShift,
                        YOffset = cellInfo.Y - yShift,
                        FlipX = cellInfo.FlipX,
                        FlipY = cellInfo.FlipY,
                        DoubleSize = cellInfo.DoubleSize,
                        IndexPalette = (byte)cellInfo.Palette,
                        Width = cellInfo.Width,
                        Height = cellInfo.Height,
                    };
                    bank.Add(cell);
                }

                // image path is relative to the location of the xml file
                if (cellBankInfo.File == null)
                {
                    throw new Exception($"Missing required attribute 'file' on cell group for format {fmt}");
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
        else if (fmt == Format.OneImagePerCell)
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
                    var cell = new Cell
                    {
                        RotateOrScale = cellInfo.DoubleSize ? RotateOrScale.Scale : RotateOrScale.Rotate,
                        XOffset = cellInfo.X - xShift,
                        YOffset = cellInfo.Y - yShift,
                        FlipX = cellInfo.FlipX,
                        FlipY = cellInfo.FlipY,
                        DoubleSize = cellInfo.DoubleSize,
                        IndexPalette = (byte)cellInfo.Palette
                    };
                    bank.Add(cell);
                    if (cellInfo.File == null)
                    {
                        throw new Exception($"Missing required attribute 'file' on cell for format {fmt}");
                    }
                    var img = ImageUtil.LoadPngBetterError(cellInfo.File);
                    images.Add(img);
                    cell.Width = img.Width;
                    cell.Height = img.Height;
                }

                bank.EstimateMinMaxValues();
                // TODO: cellBank.ReadOnlyCellInfo = ?;
            }

            // import the image data
            NitroImageUtil.NcerFromMultipleImageGroups(ncer, ncgr, nclr, imageGroups);

            foreach (var group in imageGroups)
            {
                foreach (var image in group)
                {
                    image.Dispose();
                }
            }
        }
        else
        {
            throw new Exception($"Invalid serialisation format {res.Format}");
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
        public Format Format { get; set; }
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
            
            var cellCollection = element.ElementRequired("cell_collection");
            Format = cellCollection.AttributeEnum<Format>("format");
            Cells = cellCollection.Elements("image").Select(x => new CellBankInfo(x)).ToList();
            Animations = element.ElementRequired("animation_collection").Elements("animation").Select(x => new Anim(x)).ToList();
        }

        public XDocument Serialise()
        {
            var cellElem = new XElement("cell_collection", Cells.Select(x => x.Serialise()));
            cellElem.Add(new XAttribute("format", Format));
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
        public string? File { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Palette { get; set; }
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }
        public bool DoubleSize { get; set; }

        public CellInfo()
        {
        }

        public XElement Serialise()
        {
            var cellElem = new XElement("cell",
                        new XAttribute("x", X),
                        new XAttribute("y", Y)
                        );

            if (Width > 0)
            {
                cellElem.Add(new XAttribute("width", Width));
            }
            if (Height > 0)
            {
                cellElem.Add(new XAttribute("height", Height));
            }

            if (!string.IsNullOrEmpty(File))
            {
                cellElem.Add(new XAttribute("file", File));
            }

            if (Palette != 0)
            {
                cellElem.Add(new XAttribute("palette", Palette));
            }

            if (FlipX)
            {
                cellElem.Add(new XAttribute("flip_x", FlipX));
            }
            if (FlipY)
            {
                cellElem.Add(new XAttribute("flip_y", FlipY));
            }
            if (DoubleSize)
            {
                cellElem.Add(new XAttribute("double_size", DoubleSize));
            }

            return cellElem;
        }

        public CellInfo(XElement element)
        {
            X = element.AttributeInt("x");
            Y = element.AttributeInt("y");
            FlipX = element.AttributeBool("flip_x", false);
            FlipY = element.AttributeBool("flip_y", false);
            DoubleSize = element.AttributeBool("double_size", false);
            Palette = (byte)element.AttributeInt("palette", 0);

            Width = element.AttributeInt("width", -1);
            Height = element.AttributeInt("height", -1);
            File = element.Attribute("file")?.Value;

            if ((FlipX || FlipY) && DoubleSize)
            {
                throw new Exception("Cell cannot have both rotation (flip_x or flip_y) and scaling (double_size) at once");
            }
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


