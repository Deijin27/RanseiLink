using FluentResults;
using RanseiLink.Core.Graphics;
using System.Collections.Generic;
using System.Xml.Linq;

namespace RanseiLink.Console.GraphicsCommands;

public static class CellAnimationSerialiser
{
    public static XDocument SerialiseAnimationXml(NANR anim, NCER ncer)
    {
        var cellElem = SerialiseCells(ncer);
        var animationElem = SerialiseAnimations(anim);
        return new XDocument(new XElement(nitro_cell_animation_resource, cellElem, animationElem));
    }

    private const string nitro_cell_animation_resource = nameof(nitro_cell_animation_resource);
    private const string cell_collection = nameof(cell_collection);
    private const string animation_collection = nameof(animation_collection);

    private static Result<(NANR nanr, NCER ncer)> DeserialiseAnimationXml(XDocument doc)
    {
        var element = doc.Element(nitro_cell_animation_resource);
        if (element == null)
        {
            return Result.Fail($"Missing root element '{nitro_cell_animation_resource}' in animation xml document");
        }

        var cellElement = element.Element(cell_collection);
        if (cellElement == null)
        {
            return Result.Fail($"Missing '{cell_collection}' element in '{nitro_cell_animation_resource}'");
        }
        var (ncer, nameToFile) = DeserialiseCells(cellElement);

        var animElement = element.Element(animation_collection);
        if (animElement == null)
        {
            return Result.Fail($"Missing '{animation_collection}' element in '{nitro_cell_animation_resource}'");
        }
    }

    private static XElement SerialiseAnimations(NANR anim)
    {
        var animations = new XElement(animation_collection);
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

        return animations;
    }

    private static XElement SerialiseCells(NCER ncer)
    {
        var cells = new XElement("cell_collection");
        for (int i = 0; i < ncer.CellBanks.Banks.Count; i++)
        {
            var cellBank = ncer.CellBanks.Banks[i];
            var groupElem = new XElement("image", new XAttribute("name", i), new XAttribute("file", NumToFileName(i)));
            foreach (var cell in cellBank)
            {
                groupElem.Add(SerialiseCell(cell));
            }
            cells.Add(groupElem);
        }

        return cells;
    }

    private static Result<(NCER ncer, Dictionary<string, string> nameToFile)> DeserialiseCells(XElement element)
    {
        var ncer = new NCER();
        Dictionary<string, string> nameToFile = new();
        foreach (var groupElem in element.Elements("image"))
        {
            var nameAttr = groupElem.Attribute("name");
            if (nameAttr == null)
            {
                return Result.Fail("image element missing 'name' attribute");
            }
            if (string.IsNullOrEmpty(nameAttr.Value))
            {
                return Result.Fail("image element 'name' attribute was empty");
            }
            var name = nameAttr.Value;

            var fileAttr = groupElem.Attribute("file");
            if (fileAttr == null)
            {
                return Result.Fail("image element missing 'file' attribute");
            }
            if (string.IsNullOrEmpty(fileAttr.Value))
            {
                return Result.Fail("image element 'file' attribute was empty");
            }

            if (nameToFile.ContainsKey(name))
            {
                return Result.Fail($"Duplicate 'name' attributes of images found: '{name}'");
            }
            nameToFile[name] = fileAttr.Value;

            var bank = new CellBank();
            foreach (var cellElement in element.Elements("cell"))
            {
                var cellResult = DeserialiseCell(cellElement);
                if (cellResult.IsFailed)
                {
                    return cellResult.ToResult();
                }
                bank.Add(cellResult.Value);
            }
            ncer.CellBanks.Banks.Add(bank);
        }

        return Result.Ok((ncer, nameToFile));
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

    private static Result<Cell> DeserialiseCell(XElement element)
    {
        var cell = new Cell();


        var xAttr = element.Attribute("x");
        if (xAttr == null)
        {
            return Result.Fail("Cell element missing 'x' attribute");
        }
        if (!int.TryParse(xAttr.Value, out var x))
        {
            return Result.Fail("Cell 'x' attribute did not have integer number value");
        }
        cell.XOffset = x;


        var yAttr = element.Attribute("y");
        if (yAttr == null)
        {
            return Result.Fail("Cell element missing 'y' attribute");
        }
        if (!int.TryParse(yAttr.Value, out var y))
        {
            return Result.Fail("Cell 'y' attribute did not have integer number value");
        }
        cell.YOffset = y;


        var wAttr = element.Attribute("width");
        if (wAttr == null)
        {
            return Result.Fail("Cell element missing 'width' attribute");
        }
        if (!int.TryParse(wAttr.Value, out var width) || width < 1)
        {
            return Result.Fail("Cell 'width' attribute did not have positive integer number value");
        }
        cell.Width = width;


        var hAttr = element.Attribute("height");
        if (hAttr == null)
        {
            return Result.Fail("Cell element missing 'height' attribute");
        }
        if (!int.TryParse(hAttr.Value, out var height) || height < 1)
        {
            return Result.Fail("Cell 'width' attribute did not have positive integer number value");
        }


        var flipXAttr = element.Attribute("flip_x");
        if (flipXAttr != null)
        {
            if (!bool.TryParse(flipXAttr.Value, out var flipX))
            {
                return Result.Fail("Cell 'flip_x' did not have bool value");
            }
            cell.FlipX = flipX;
        }


        var flipYAttr = element.Attribute("flip_y");
        if (flipYAttr != null)
        {
            if (!bool.TryParse(flipXAttr.Value, out var flipY))
            {
                return Result.Fail("Cell 'flip_y' did not have bool value");
            }
            cell.FlipY = flipY;
        }


        return cell;
    }

    public static string NumToFileName(int num)
    {
        return $"{num.ToString().PadLeft(4, '0')}.png";
    }
}
