using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.Core.Services;
using System.Text;
using System.Xml.Linq;

namespace RanseiLink.GuiCore.Services;

public interface INicknameService
{
    List<SelectorComboBoxItem> GetAllNicknames(string category);
    string GetNickname(string category, int id);
    void SetNickname(string category, int id, string? nickname = null);
}

public class NicknameService : INicknameService
{
    private readonly string _nicknameFolder;

    public NicknameService(ModInfo modInfo)
    {
        _nicknameFolder = Path.Combine(modInfo.FolderPath, "Nicknames");
        Directory.CreateDirectory(_nicknameFolder);

        InitialiseCategoryFromEnum<GimmickRangeId>();
        InitialiseCategoryFromEnum<MoveRangeId>();
        InitialiseCategoryFromEnum<BattleConfigId>();
        InitialiseCategoryFromEnum<GimmickObjectId>();
        InitialiseCategory(nameof(MapId), MapId.DefaultNicknames);
    }

    private void InitialiseCategoryFromEnum<T>() where T : Enum
    {
        var names = Enum.GetNames(typeof(T));//.Select(AddSpacesToPascalCase).ToArray();
        InitialiseCategory(typeof(T).Name, names);
    }

    private string AddSpacesToPascalCase(string text)
    {
        var newText = new StringBuilder();
        bool previousWasNumber = false;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            
            if (i != 0)
            {
                if (c == '_')
                {
                    continue;
                }
                if (c >= 0 && c <= 9)
                {
                    if (!previousWasNumber)
                    {
                        newText.Append(' ');
                    }
                    previousWasNumber = true;
                }
                else
                {
                    if (c >= 'A' && c <= 'Z')
                    {
                        newText.Append(' ');
                    }
                    previousWasNumber = false;
                }
            }

            newText.Append(c);
        }
        return newText.ToString();
    }

    private readonly Dictionary<string, NicknameCategory> _nicknameCategories = [];

    private class NicknameCategory
    {
        private readonly string _file;
        private readonly Dictionary<int, string> _customNames;
        private readonly Dictionary<int, string> _defaults;

        public NicknameCategory(string file, Dictionary<int, string> defaults)
        {
            _file = file;
            _defaults = defaults;
            _customNames = [];
            Load();
        }

        public List<SelectorComboBoxItem> GetAllNicknames()
        {
            return _defaults.OrderBy(x => x.Key).Select(x => new SelectorComboBoxItem(x.Key, x.Value)).ToList();
        }

        public string GetNickname(int id)
        {
            if (_customNames.TryGetValue(id, out var name))
            {
                return name;
            }
            return _defaults[id];
        }

        public void SetNickname(int id, string? name = null)
        {
            if (string.IsNullOrWhiteSpace(name) || name == _defaults[id])
            {
                _customNames.Remove(id);
            }
            else
            {
                _customNames[id] = name;
            }
            Save();
        }

        private void Save()
        {
            if (_customNames.Count == 0)
            {
                if (File.Exists(_file))
                {
                    File.Delete(_file);
                }
            }
            else
            {
                var e = new XElement("Nicknames");
                foreach (var (key, value) in _customNames)
                {
                    e.Add(new XElement("Item", new XAttribute("Id", key), new XAttribute("Value", value)));
                }
                var doc = new XDocument(e);
                doc.Save(_file);
            }
        }

        private void Load()
        {
            _customNames.Clear();
            if (!File.Exists(_file))
            {
                return;
            }
            var e = XDocument.Load(_file).Root;
            if (e == null)
            {
                return;
            }
            foreach (var itemEl in e.Elements("Item"))
            {
                var idAttr = itemEl.Attribute("Id");
                var valueAttr = itemEl.Attribute("Value");
                if (!int.TryParse(idAttr?.Value, out var id))
                {
                    continue;
                }
                if (string.IsNullOrEmpty(valueAttr?.Value))
                {
                    continue;
                }
                _customNames[id] = valueAttr.Value;
            }
        }
    }

    private void InitialiseCategory(string category, IReadOnlyList<string> defaults)
    {
        var dict = new Dictionary<int, string>();
        for (int i = 0; i < defaults.Count; i++)
        {
            dict[i] = defaults[i];
        }
        InitialiseCategory(category, dict);
    }

    private void InitialiseCategory(string category, Dictionary<int, string> defaults)
    {
        _nicknameCategories[category] = new NicknameCategory(Path.Combine(_nicknameFolder, category) + ".xml", defaults);
    }

    public List<SelectorComboBoxItem> GetAllNicknames(string category)
    {
        return _nicknameCategories[category].GetAllNicknames();
    }

    public string GetNickname(string category, int id)
    {
        return _nicknameCategories[category].GetNickname(id);
    }

    public void SetNickname(string category, int id, string? nickname = null)
    {
        _nicknameCategories[category].SetNickname(id, nickname);
    }

}
