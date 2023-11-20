#nullable enable
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Util;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace RanseiLink.Console.GraphicsCommands;

public static class CellAnimationSerialiser
{
    public static XDocument SerialiseAnimationXml(NANR anim, NCER ncer)
    {
        var cellElem = SerialiseCells(ncer);
        var animationElem = SerialiseAnimations(anim);
        return new XDocument(new XElement("nitro_cell_animation_resource", cellElem, animationElem));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    /// <exception cref="XmlUtilException"/>
    /// <exception cref="Exception"/>
    public static (NANR nanr, NCER ncer) DeserialiseAnimationXml(XDocument doc)
    {
        var element = doc.ElementRequired("nitro_cell_animation_resource");

        var cellElement = element.ElementRequired("cell_collection");
        var (ncer, nameToFile) = DeserialiseCells(cellElement);

        var animElement = element.ElementRequired("animation_collection");
        var (nanr, imageToFrames) = DeserialiseAnimation(animElement);
        return (nanr, ncer);
    }

    private static XElement SerialiseAnimations(NANR anim)
    {
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

        return animations;
    }

    private static (NANR nanr, Dictionary<string, List<ABNK.Anim>> imageToFrames) DeserialiseAnimation(XElement element)
    {
        var nanr = new NANR();
        var imageToFrames = new Dictionary<string, List<ABNK.Anim>>();
        foreach (var animElem in element.Elements("animation"))
        {
            var anim = new ABNK.Anim();
            nanr.AnimationBanks.Banks.Add(anim);
            var name = animElem.AttributeStringNonEmpty("name");
            nanr.Labels.Names.Add(name);
            
            foreach (var frameElem in animElem.Elements("frame"))
            {
                var frame = new ABNK.Frame();
                anim.Frames.Add(frame);
                var img = frameElem.AttributeStringNonEmpty("image");
                if (!imageToFrames.TryGetValue(img, out var lst))
                {
                    lst = new();
                }
                lst.Add(anim);
                var duration = frameElem.AttributeInt("duration");
                frame.Duration = (ushort)duration;
            }
        }
        return (nanr, imageToFrames);
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

    private static (NCER ncer, Dictionary<string, string> nameToFile) DeserialiseCells(XElement element)
    {
        var ncer = new NCER();
        Dictionary<string, string> nameToFile = new();
        foreach (var groupElem in element.Elements("image"))
        {
            var name = groupElem.AttributeStringNonEmpty("name");
            var file = groupElem.AttributeStringNonEmpty("file");

            if (nameToFile.ContainsKey(name))
            {
                throw new Exception($"Duplicate 'name' attributes of images found: '{name}'");
            }
            nameToFile[name] = file;

            var bank = new CellBank();
            foreach (var cellElement in element.Elements("cell"))
            {
                var cell = DeserialiseCell(cellElement);
                bank.Add(cell);
            }
            ncer.CellBanks.Banks.Add(bank);
        }

        return (ncer, nameToFile);
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
}


