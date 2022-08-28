using System;
using System.Linq;
using System.Xml.Linq;

namespace RanseiLink.Core.Graphics.ExternalFormats
{
    public class COLLADA
    {
        private static readonly XNamespace ns = "http://www.collada.org/2008/03/COLLADASchema";

        public static XElement FromThing(NSBMD nsbmd)
        {
            return new XElement(ns + "COLLADA", new XAttribute("version", "1.5.0"),
                LibraryImages(nsbmd),
                LibraryMaterials(nsbmd)
                );
        }

        public static XElement Asset()
        {
            return new XElement(ns + "asset",
                new XElement(ns + "created", DateTime.Now),
                new XElement(ns + "modified", DateTime.Now),
                new XElement(ns + "revision", "1.0")
                );
        }

        public static XElement LibraryImages(NSBMD nsbmd)
        {
            var textures = nsbmd.Model.Models[0].Materials.Select(x => x.Texture);
            return new XElement(ns + "library_images",
                textures.Select(x => new XElement(ns + "image",
                    new XAttribute("id", x),
                    new XElement(ns + "init_from", x + ".png")
                    )));
        }

        public static XElement LibraryMaterials(NSBMD nsbmd)
        {
            var materialNames = nsbmd.Model.Models[0].Materials.Select(x => x.Name);
            return new XElement(ns + "library_materials",
                materialNames.Select((name, i) => new XElement(ns + "material",
                    new XAttribute("id", $"material{i}"),
                    new XAttribute("name", name),
                    new XElement(ns + "instance_effect", 
                        new XAttribute("url", $"#effect{i}")
                ))));
        }

    }
}
