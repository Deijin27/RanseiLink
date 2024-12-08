using RanseiLink.Core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.Core.Services;
public interface ISpriteService : IDisposable
{
    Image<Rgba32> GetMovePreview(Move move, MoveRange range, MovePreviewOptions options = MovePreviewOptions.All);
    Image<Rgba32> GetMoveRangePreview(MoveRange range);
}

[Flags]
public enum MovePreviewOptions
{
    DrawPowerStars = 1,
    DrawType = 2,
    All = DrawPowerStars | DrawType
}