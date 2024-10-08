﻿using System.Xml.Linq;

namespace RanseiLink.Core.Settings;

public class SettingService : ISettingService
{
    private readonly XDocument _doc;
    private readonly XElement _docRoot;
    private readonly Dictionary<Type, ISetting> _settingCache = new Dictionary<Type, ISetting>();
    private readonly string _settingsFilePath;

    public SettingService(string settingsFilePath)
    {
        _settingsFilePath = settingsFilePath;
        if (File.Exists(_settingsFilePath))
        {
            try
            {
                _doc = XDocument.Load(_settingsFilePath);
                if (_doc.Root != null)
                {
                    _docRoot = _doc.Root;
                    return;
                }
            }
            catch
            {
                // corrupt setting file
                // continue to create new settings
            }
            
        }

        // create blank settings
        _docRoot = new XElement("Settings");
        _doc = new XDocument(_docRoot);
    }

    public void Save()
    {
        foreach (var setting in _settingCache.Values)
        {
            var element = _docRoot.Element(setting.UniqueElementName);
            if (setting.IsDefault)
            {
                element?.Remove();
                continue;
            }
            if (element == null)
            {
                element = new XElement(setting.UniqueElementName);
                _docRoot.Add(element);
            }
            setting.Serialize(element);
        }
        _doc.Save(_settingsFilePath);
    }

    public TSetting Get<TSetting>() where TSetting : ISetting, new()
    {
        if (!_settingCache.TryGetValue(typeof(TSetting), out var setting))
        {
            setting = new TSetting();
            _settingCache.Add(typeof(TSetting), setting);

            var element = _docRoot.Element(setting.UniqueElementName);
            if (element != null)
            {
                setting.Deserialize(element);
            }
            else
            {
                setting.IsDefault = true;
            }
        }
        return (TSetting)setting;
    }
}