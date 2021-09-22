using System.Xml.Linq;

namespace Core.Services
{
    public class ModInfo
    {
        private static class ElementNames
        {
            public const string ModInfo = "ModInfo";
            public const string Name = "Name";
            public const string Version = "Version";
            public const string Author = "Author";
            public const string RLModVersion = "RLModVersion";
        }
        /// <summary>
        /// Name given by user to mod
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Version given by user to mod
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Author given by user to mod
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Mod version for knowing how to load it
        /// </summary>
        public uint RLModVersion { get; set; }
        
        /// <summary>
        /// Do not serialize
        /// </summary>
        public string FolderPath { get; set; }

        public static bool TryLoadFrom(XDocument doc, out ModInfo modInfo)
        {
            var element = doc.Root;
            modInfo = new ModInfo
            {
                Name = element.Element(ElementNames.Name)?.Value,
                Version = element.Element(ElementNames.Version)?.Value,
                Author = element.Element(ElementNames.Author)?.Value
            };

            string version = element.Attribute(ElementNames.RLModVersion)?.Value;
            if (!uint.TryParse(version, out uint versionVal))
            {
                return false;
            }
            modInfo.RLModVersion = versionVal;
            return true;
        }

        public void SaveTo(XDocument doc)
        {
            var element = new XElement(ElementNames.ModInfo,
                new XAttribute(ElementNames.RLModVersion, RLModVersion.ToString()),
                new XElement(ElementNames.Name, Name),
                new XElement(ElementNames.Version, Version),
                new XElement(ElementNames.Author, Author)
                );
            doc.Add(element);
        }
    }
}
