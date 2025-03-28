using RanseiLink.Core.Enums;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace RanseiLink.Core.Services;

public class ModMetadata
{
    /// <summary>
    /// Name given by user to mod
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// Version given by user to mod
    /// </summary>
    public string? Version { get; set; }
    /// <summary>
    /// Author given by user to mod
    /// </summary>
    public string? Author { get; set; }

    public List<string> Tags { get; set; } = [];
}

public class ModInfo : ModMetadata
{
    private static class ElementNames
    {
        public const string ModInfo = "ModInfo";
        public const string Name = "Name";
        public const string Version = "Version";
        public const string Author = "Author";
        public const string RLModVersion = "RLModVersion";
        public const string GameCode = "GameCode";
        public const string Tags = "Tags";
        public const string Tag = "Tag";
    }
    

    /// <summary>
    /// Mod version for knowing how to load it
    /// </summary>
    public uint RLModVersion { get; set; }

    /// <summary>
    /// Do not serialize
    /// </summary>
    public string FolderPath { get; set; } = string.Empty;

    public ConquestGameCode GameCode { get; set; }

    public string RegionName => GameCode.UserFriendlyName();

    public static bool TryLoadFrom(XDocument doc, [NotNullWhen(true)] out ModInfo? modInfo)
    {
        var element = doc.Element(ElementNames.ModInfo);
        if (element == null)
        {
            modInfo = null;
            return false;
        }
        modInfo = new ModInfo
        {
            Name = element.Element(ElementNames.Name)?.Value,
            Version = element.Element(ElementNames.Version)?.Value,
            Author = element.Element(ElementNames.Author)?.Value,
            GameCode = Enum.TryParse(element.Element(ElementNames.GameCode)?.Value, out ConquestGameCode culture) ? culture : ConquestGameCode.VPYT
        };

        var tagsEl = element.Element(ElementNames.Tags);
        if (tagsEl != null)
        {
            foreach (var tagEl in tagsEl.Elements(ElementNames.Tag))
            {
                modInfo.Tags.Add(tagEl.Value);
            }
        }

        string? version = element.Attribute(ElementNames.RLModVersion)?.Value;
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
            new XElement(ElementNames.Author, Author),
            new XElement(ElementNames.GameCode, GameCode)
            );
        if (Tags.Count != 0)
        {
            var tagsEl = new XElement(ElementNames.Tags);
            foreach (var tag in Tags)
            {
                tagsEl.Add(new XElement(ElementNames.Tag, tag));
            }
            element.Add(tagsEl);
        }
        doc.Add(element);
    }

    public ModMetadata CopyMetadata()
    {
        return new()
        {
            Name = Name,
            Version = Version,
            Author = Author,
            Tags = [..Tags]
        };
    }

    public void LoadMetadata(ModMetadata metadata)
    {
        Name = metadata.Name;
        Version = metadata.Version;
        Author = metadata.Author;
        Tags = [..metadata.Tags];
    }
}