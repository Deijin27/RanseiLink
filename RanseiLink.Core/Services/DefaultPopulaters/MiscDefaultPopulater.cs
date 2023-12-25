using RanseiLink.Core.Resources;

namespace RanseiLink.Core.Services.DefaultPopulaters;

[DefaultPopulater]
public class MiscDefaultPopulater : IGraphicTypeDefaultPopulater
{
    public MetaSpriteType Id => MetaSpriteType.Misc;

    private readonly Dictionary<MetaMiscItemId, IMiscItemDefaultPopulater> _populaters;
    public MiscDefaultPopulater(IMiscItemDefaultPopulater[] populaters)
    {
        _populaters = new();
        foreach (var populater in populaters)
        {
            _populaters.Add(populater.Id, populater);
        }
    }

    public void ProcessExportedFiles(string defaultDataFolder, IGraphicsInfo gInfo)
    {
        var miscInfo = (MiscConstants)gInfo;

        foreach (var item in miscInfo.Items)
        {
            _populaters[item.MetaId].ProcessExportedFiles(defaultDataFolder, miscInfo, item);
        }
    }
}
