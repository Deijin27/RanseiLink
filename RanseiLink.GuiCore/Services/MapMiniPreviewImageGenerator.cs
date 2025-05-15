using RanseiLink.Core.Maps;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.GuiCore.Services;
public interface IMapMiniPreviewImageGenerator
{
    Image<Rgba32> Generate(PSLM pslm);
}

internal class MapMiniPreviewImageGenerator : IMapMiniPreviewImageGenerator
{
    private readonly Dictionary<TerrainId, Rgba32> _terrainToColor;

    public MapMiniPreviewImageGenerator()
    {
        _terrainToColor = [];
        foreach (var item in TerrainToColorConverter.TerrainToHex)
        {
            _terrainToColor.Add(item.Key, Rgba32.ParseHex(item.Value));
        }
    }

    public Image<Rgba32> Generate(PSLM pslm)
    {
        var image = new Image<Rgba32>(pslm.Width, pslm.Height);
        image.ProcessPixelRows(pixelAccessor =>
        {
            for (var y = 0; y < pixelAccessor.Height; y++)
            {
                var rowSpan = pixelAccessor.GetRowSpan(y);
                var row = pslm.TerrainSection.MapMatrix[y];
                for (int x = 0; x < pixelAccessor.Width; x++)
                {
                    rowSpan[x] = _terrainToColor[row[x].Terrain];
                }
            }
        });
        return image;
    }
}
