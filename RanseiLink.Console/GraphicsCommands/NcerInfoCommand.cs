﻿using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Graphics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RanseiLink.Console.GraphicsCommands;

[Command("ncer info", Description = "Print out informational content of Nitro Cell Resource")]
public class NcerInfoCommand : ICommand
{
    [CommandParameter(0, Description = "Path of ncer file", Name = "filePath")]
    public string FilePath { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var nanr = NCER.Load(FilePath);

        var el = new XElement("AnimationBank",
            new XAttribute("BlockSize", nanr.CellBanks.BlockSize),
            new XAttribute("BankType", nanr.CellBanks.BankType),
            new XElement("Banks", nanr.CellBanks.Banks.Select(bank =>
                new XElement("Bank", bank.Select(SerialiseCell)

                ))),
            new XElement("Labels", nanr.Labels.Names.Select(name => new XElement("Name", name))),
            new XElement("Unknown", nanr.Unknown.Unknown)
            );

        console.Output.WriteLine(el.ToString());

        return default;
    }

    private static XElement SerialiseCell(Cell cell)
    {
        var e = new XElement("Cell",
            new XAttribute("cellId", cell.CellId),
            new XAttribute("height", cell.Height),
            new XAttribute("width", cell.Width),
            new XAttribute("xOffset", cell.XOffset),
            new XAttribute("yOffset", cell.YOffset),
            new XAttribute("tileOffset", cell.TileOffset)
            );

        if (cell.Depth != BitDepth.e8Bit)
        {
            e.Add(new XElement("depth", cell.Depth));
        }

        if (cell.DoubleSize)
        {
            e.Add(new XElement("doubleSize", cell.DoubleSize));
        }

        if (cell.FlipX)
        {
            e.Add(new XElement("flipX", cell.FlipX));
        }

        if (cell.FlipY)
        {
            e.Add(new XElement("flipY", cell.FlipY));
        }

        if (cell.IndexPalette != 0)
        {
            e.Add(new XElement("indexPalette", cell.IndexPalette));
        }

        if (cell.Mosaic)
        {
            e.Add(new XElement("mosaic", cell.Mosaic));
        }

        if (cell.ObjDisable)
        {
            e.Add(new XElement("objDisable", cell.ObjDisable));
        }

        if (cell.ObjMode != ObjMode.Normal)
        {
            e.Add(new XElement("objMode", cell.ObjMode));
        }

        if (cell.Priority != 0)
        {
            e.Add(new XElement("priority", cell.Priority));
        }

        if (cell.RotateOrScale != RotateOrScale.Rotate)
        {
            e.Add(new XElement("rotateOrScale", cell.RotateOrScale));
        }

        if (cell.SelectParam != 0)
        {
            e.Add(new XElement("selectParam", cell.SelectParam));
        }

        if (cell.Unused != 0)
        {
            e.Add(new XElement("unused", cell.Unused));
        }
        return e;
    }
}