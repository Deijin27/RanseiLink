using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace RanseiLink.Core.Services.Concrete;

public abstract class BaseSettingsService
{
    public BaseSettingsService(string settingsFilePath)
    {
        _settingsFilePath = settingsFilePath;
    }

    private readonly string _settingsFilePath;
    protected int Get(string key, int defaultValue)
    {
        if (TryGet(key, out string value))
        {
            if (int.TryParse(value, out int parsedValue))
            {
                return parsedValue;
            }
        }
        return defaultValue;
    }

    protected void Set(string key, int value)
    {
        Set(key, value.ToString());
    }

    protected string Get(string key, string defaultValue)
    {
        return TryGet(key, out string value) ? value : defaultValue;
    }

    protected bool TryGet(string key, out string value)
    {
        if (!File.Exists(_settingsFilePath))
        {
            value = null;
            return false;
        }
        XDocument doc = XDocument.Load(_settingsFilePath);
        var element = doc.Root;
        var keyElement = element.Element(key);
        if (keyElement == null)
        {
            value = string.Empty;
            return false;
        }
        value = keyElement.Value;
        return true;
    }

    protected void Set(string key, string value)
    {
        if (!File.Exists(_settingsFilePath))
        {
            using (var file = File.Create(_settingsFilePath))
            {
                var rootElement = new XElement("Settings", new XElement(key, value));
                var newDoc = new XDocument(rootElement);
                newDoc.Save(file);
            }
            return;
        }
        XDocument doc = XDocument.Load(_settingsFilePath);
        doc.Root.SetElementValue(key, value);
        doc.Save(_settingsFilePath);

    }
}

public class SettingsService : BaseSettingsService, ISettingsService
{
    public SettingsService(string rootFolder) : base(Path.Combine(rootFolder, "RanseiLinkSettings.xml")) { }

    private static class ElementNames
    {
        public const string CurrentConsoleModSlot = "CurrentConsoleModSlot";
        public const string RecentCommitRom = "RecentCommitRom";
        public const string RecentLoadRom = "RecentLoadRom";
        public const string RecentExportModFolder = "RecentExportFolder";
        public const string Theme = "Theme";
        public const string EditorModuleOrder = "EditorModuleOrder";
    }

    public int CurrentConsoleModSlot
    {
        get => Get(ElementNames.CurrentConsoleModSlot, 0);
        set => Set(ElementNames.CurrentConsoleModSlot, value);
    }

    public string Theme
    {
        get => Get(ElementNames.Theme, "Dark");
        set => Set(ElementNames.Theme, value);
    }

    public string RecentCommitRom
    {
        get
        {
            string file = Get(ElementNames.RecentCommitRom, null);
            if (file != null && !File.Exists(file))
            {
                return null;
            }
            else
            {
                return file;
            }
        }
        set => Set(ElementNames.RecentCommitRom, value);
    }

    public string RecentLoadRom
    {
        get
        {
            string file = Get(ElementNames.RecentLoadRom, null);
            if (file != null && !File.Exists(file))
            {
                return null;
            }
            else
            {
                return file;
            }
        }
        set => Set(ElementNames.RecentLoadRom, value);
    }

    public string RecentExportModFolder
    {
        get
        {
            string file = Get(ElementNames.RecentExportModFolder, null);
            if (file != null && !Directory.Exists(file))
            {
                return null;
            }
            else
            {
                return file;
            }
        }
        set => Set(ElementNames.RecentExportModFolder, value);
    }

    public ICollection<string> EditorModuleOrder 
    {
        get
        {
            if (TryGet(ElementNames.EditorModuleOrder, out string value))
            {
                return value.Split(',');
            }
            return Array.Empty<string>();
        }
        set
        {
            Set(ElementNames.EditorModuleOrder, string.Join(',', value));
        }
    }
}
