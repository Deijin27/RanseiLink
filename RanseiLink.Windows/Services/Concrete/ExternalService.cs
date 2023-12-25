using RanseiLink.Core.Enums;
using System.IO;

namespace RanseiLink.Windows.Services.Concrete;

internal class ExternalService : IExternalService
{
    private static readonly string _externalDirectory;
    private static readonly string _moveAnimationFile;
    private static readonly string _moveMovementAnimationFile;

    static ExternalService()
    {
        _externalDirectory = Path.Combine(Directory.GetCurrentDirectory(), "External");
        _moveAnimationFile = Path.Combine(_externalDirectory, "MoveAnimations.txt");
        _moveMovementAnimationFile = Path.Combine(_externalDirectory, "MoveMovementAnimations.txt");
    }

    private Dictionary<MoveAnimationId, string> _moveAnimationCache;
    public string GetMoveAnimationUri(MoveAnimationId id)
    {
        if (_moveAnimationCache == null)
        {
            _moveAnimationCache = new Dictionary<MoveAnimationId, string>();
            if (!File.Exists(_moveAnimationFile))
            {
                return "";
            }
            int count = 0;
            foreach (string line in File.ReadAllLines(_moveAnimationFile))
            {
                _moveAnimationCache[(MoveAnimationId)count] = line;
                count++;
            }
        }
        if (_moveAnimationCache.TryGetValue(id, out string uri))
        {
            return uri;
        }
        return "";
    }

    private Dictionary<MoveMovementAnimationId, string> _moveMovementAnimationCache;
    public string GetMoveMovementAnimationUri(MoveMovementAnimationId id)
    {
        if (_moveMovementAnimationCache == null)
        {
            _moveMovementAnimationCache = new Dictionary<MoveMovementAnimationId, string>();
            if (!File.Exists(_moveMovementAnimationFile))
            {
                return "";
            }
            int count = 0;
            foreach (string line in File.ReadAllLines(_moveMovementAnimationFile))
            {
                _moveMovementAnimationCache[(MoveMovementAnimationId)count] = line;
                count++;
            }
        }
        if (_moveMovementAnimationCache.TryGetValue(id, out string uri))
        {
            return uri;
        }
        return "";
    }
}
