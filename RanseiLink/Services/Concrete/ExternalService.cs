using RanseiLink.Core.Enums;
using System.Collections.Generic;
using System.IO;

namespace RanseiLink.Services.Concrete;

internal class ExternalService : IExternalService
{
    private static readonly string _externalDirectory;
    private static readonly string _moveAnimationFile;

    static ExternalService()
    {
        _externalDirectory = Path.Combine(Directory.GetCurrentDirectory(), "External");
        _moveAnimationFile = Path.Combine(_externalDirectory, "MoveAnimations.txt");
    }

    private Dictionary<MoveAnimationId, string> _animationCache;
    public string GetMoveAnimationUri(MoveAnimationId id)
    {
        if (_animationCache == null)
        {
            _animationCache = new Dictionary<MoveAnimationId, string>();
            if (!File.Exists(_moveAnimationFile))
            {
                return "";
            }
            int count = 0;
            foreach (string line in File.ReadAllLines(_moveAnimationFile))
            {
                _animationCache[(MoveAnimationId)count] = line;
                count++;
            }
        }
        if (_animationCache.TryGetValue(id, out string uri))
        {
            return uri;
        }
        return "";
    }
}
