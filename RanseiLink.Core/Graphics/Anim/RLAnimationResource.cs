using RanseiLink.Core.Util;
using SixLabors.ImageSharp;
using System.Xml.Linq;

namespace RanseiLink.Core.Graphics;

public class RLAnimationResource
{
    public const int Version = 1;
    public string? Background { get; set; }
    public RLAnimationFormat Format { get; set; }
    public List<ClusterInfo> Clusters { get; }
    public List<Anim> Animations { get; }

    public RLAnimationResource(RLAnimationFormat format, string? background = null)
    {
        Format = format;
        Background = background;
        Clusters = [];
        Animations = [];
    }

    public RLAnimationResource(XDocument doc)
    {
        var root = doc.ElementRequired("nitro_cell_animation_resource");
        var version = root.AttributeInt("version");
        if (version != Version)
        {
            throw new Exception($"Unknown animation format version '{version}'");
        }

        Background = root.Attribute("background")?.Value;
        var cellCollection = root.ElementRequired("cell_collection");
        Format = cellCollection.AttributeEnum<RLAnimationFormat>("format");
        Clusters = cellCollection.Elements("cluster").Select(x => new ClusterInfo(x)).ToList();
        Animations = root.ElementRequired("animation_collection").Elements("animation").Select(x => new Anim(x)).ToList();
    }

    public XDocument Serialise()
    {
        var cellElem = new XElement("cell_collection", Clusters.Select(x => x.Serialise()));
        cellElem.Add(new XAttribute("format", Format));
        var animationElem = new XElement("animation_collection", Animations.Select(x => x.Serialise()));

        var versionAttr = new XAttribute("version", Version);

        var root = new XElement("nitro_cell_animation_resource", versionAttr, cellElem, animationElem);
        if (Background != null)
        {
            root.Add(new XAttribute("background", Background));
        }
        return new XDocument(root);
    }

    public class Anim
    {
        public string Name { get; set; }
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
        public string Cluster { get; }
        public int Duration { get; }

        public AnimFrame(string cluster, int duration)
        {
            Cluster = cluster;
            Duration = duration;
        }

        public AnimFrame(XElement frameElem)
        {
            Cluster = frameElem.AttributeStringNonEmpty("cluster");
            Duration = frameElem.AttributeInt("duration");
        }

        public XElement Serialise()
        {
            return new XElement("frame",
                new XAttribute("cluster", Cluster),
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

    public class ClusterInfo
    {
        public List<CellInfo> Cells { get; }
        public string Name { get; }
        public string? File { get; set; }

        public ClusterInfo(XElement groupElem)
        {
            Name = groupElem.AttributeStringNonEmpty("name");
            File = groupElem.Attribute("file")?.Value;
            Cells = [];
            foreach (var cellElement in groupElem.Elements("cell"))
            {
                var cell = new CellInfo(cellElement);
                Cells.Add(cell);
            }
        }

        public ClusterInfo(string name)
        {
            Cells = [];
            Name = name;
        }

        public XElement Serialise()
        {
            var clusterElem = new XElement("cluster", new XAttribute("name", Name));
            if (!string.IsNullOrEmpty(File))
            {
                clusterElem.Add(new XAttribute("file", File));
            }
            foreach (var cell in Cells)
            {
                clusterElem.Add(cell.Serialise());
            }
            return clusterElem;
        }
    }
}
