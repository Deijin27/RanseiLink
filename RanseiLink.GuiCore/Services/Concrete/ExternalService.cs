#nullable enable
using RanseiLink.Core.Enums;
using System.Xml.Linq;


namespace RanseiLink.GuiCore.Services.Concrete;

internal class ExternalService : IExternalService
{
    private readonly string _moveAnimationFile;
    private readonly string _moveMovementAnimationFile;

    public ExternalService(AppInfo appInfo)
    {
        var externalDirectory = Path.Combine(appInfo.StartupDirectory, "External");
        _moveAnimationFile = Path.Combine(externalDirectory, "MoveAnimations.xml");
        _moveMovementAnimationFile = Path.Combine(externalDirectory, "MoveMovementAnimations.xml");
    }

    private Dictionary<int, string>? _moveAnimationCache;
    public string GetMoveAnimationUri(TrueMoveAnimationId id)
    {
        _moveAnimationCache ??= Load(_moveAnimationFile);
        if (_moveAnimationCache.TryGetValue((int)id, out string? uri))
        {
            return uri;
        }
        return _moveAnimationCache[-1];
    }

    private Dictionary<int, string>? _moveMovementAnimationCache;
    public string GetMoveMovementAnimationUri(MoveMovementAnimationId id)
    {
        _moveMovementAnimationCache ??= Load(_moveMovementAnimationFile);
        if (_moveMovementAnimationCache.TryGetValue((int)id, out string? uri))
        {
            return uri;
        }
        return _moveMovementAnimationCache[-1];
    }

    private static Dictionary<int, string> Load(string filePath)
    {
        var result = new Dictionary<int, string>();
        result[-1] = "";
        if (!File.Exists(filePath))
        {
            return result;
        }
        var doc = XDocument.Load(filePath);
        if (doc.Root == null)
        {
            return result;
        }
        var defaultAttr = doc.Root.Attribute("Default");
        if (defaultAttr != null)
        {
            result[-1] = defaultAttr.Value;
        }
        foreach (var element in doc.Root.Elements("Item"))
        {
            var keyAttr = element.Attribute("Key");
            var valueAttr = element.Attribute("Value");
            if (keyAttr == null  || valueAttr == null)
            {
                continue;
            }
            if (!int.TryParse(keyAttr.Value, out int id))
            {
                continue;
            }
            result[id] = valueAttr.Value;
        }
        return result;
    }
}
