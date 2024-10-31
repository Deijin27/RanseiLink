using RanseiLink.Core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RanseiLink.Core.Services;
public interface ISpriteService : IDisposable
{
    Image<Rgba32> GetMovePreview(Move move, MoveRange range);
    Image<Rgba32> GetMoveRangePreview(MoveRange range);
}