
using RanseiLink.Core;
using RanseiLink.Core.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace RanseiLink.Console.GraphicsCommands;

public class MinimapParts
{
    public static MinimapParts Instance { get; } = new MinimapParts();

    private MinimapParts()
    {
        var folder = MinimapGenerateCommand.Folder;
        var innerFile = Path.Combine(folder, "inner.png");
        var upperLeftFile = Path.Combine(folder, "upper_left.png");
        var bottomLeftFile = Path.Combine(folder, "bottom_left.png");
        var bottomMiddleFile = Path.Combine(folder, "bottom_middle.png");
        var leftMiddleFile = Path.Combine(folder, "left_middle.png");
        var upperMiddleFile = Path.Combine(folder, "upper_middle.png");

        Inner = Image.Load<Rgba32>(innerFile);
        UpperLeft = Image.Load<Rgba32>(upperLeftFile);
        UpperRight = UpperLeft.Clone(g => { g.Flip(FlipMode.Horizontal); });
        BottomLeft = Image.Load<Rgba32>(bottomLeftFile);
        BottomRight = BottomLeft.Clone(g => { g.Flip(FlipMode.Horizontal); });
        BottomMiddle = Image.Load<Rgba32>(bottomMiddleFile);
        LeftMiddle = Image.Load<Rgba32>(leftMiddleFile);
        RightMiddle = LeftMiddle.Clone(g => { g.Flip(FlipMode.Horizontal); });
        UpperMiddle = Image.Load<Rgba32>(upperMiddleFile);
    }

    public Image<Rgba32> Inner { get; }
    public Image<Rgba32> UpperLeft { get; }
    public Image<Rgba32> UpperRight { get; }
    public Image<Rgba32> BottomLeft { get; }
    public Image<Rgba32> BottomRight { get; }
    public Image<Rgba32> BottomMiddle { get; }
    public Image<Rgba32> LeftMiddle { get; }
    public Image<Rgba32> RightMiddle { get; }
    public Image<Rgba32> UpperMiddle { get; }
}

[Command("minimap generate", Description = "Generate a minimap")]
public class MinimapGenerateCommand : ICommand
{
    public static readonly string Folder = Path.Combine(FileUtil.DesktopDirectory, "minimap_generator");

    public ValueTask ExecuteAsync(IConsole console)
    {
        var parts = MinimapParts.Instance;

        var gridSize = new Size(16, 16);
        var rotatedSize = Rotate(gridSize);

        using var big = GenerateMinimap(parts, gridSize);
        using var bigRotated = GenerateMinimap(parts, rotatedSize);

        var orientations = new List<Image<Rgba32>>
        {
            big,
            bigRotated,
            big,
            bigRotated
        };

        using var combined = ImageUtil.CombineImagesVertically(orientations);
        combined.SaveAsPng(Path.Combine(Folder, "output.png"));

        return default;
    }

    private Image<Rgba32> GenerateMinimap(MinimapParts parts, Size gridSize)
    {
        var big = NewBase();

        int gridWidth = gridSize.Width;
        int gridHeight = gridSize.Height;
        int gridWidthMinus1 = gridWidth - 1;
        int gridHeightMinus1 = gridHeight - 1;

        var middle = Middle(gridSize);
        var origin = new Point(big.Width / 2 - middle.Width - 5, big.Height / 2 - middle.Height - 12);

        List<(Action<IImageProcessingContext> Draw, int Priority)> draws = [];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                var offset = ConvertPoint(x, y);
                var point = origin + offset;
                var pointDownByOne = new Point(point.X, point.Y + 1);

                if (x == 0 && y == 0)
                {
                    // top middle corner
                    draws.Add((g => g.DrawImage(parts.UpperMiddle, pointDownByOne, 1), 10));
                }
                else if (x == gridWidthMinus1 && y == gridHeightMinus1)
                {
                    // bottom middle corner
                    draws.Add((g => g.DrawImage(parts.BottomMiddle, pointDownByOne, 1), 10));
                }
                else if (x == 0 && y == gridHeightMinus1)
                {
                    // left middle corner
                    draws.Add((g => g.DrawImage(parts.LeftMiddle, point, 1), 10));
                }
                else if (x == gridWidthMinus1 && y == 0)
                {
                    // right middle corner
                    draws.Add((g => g.DrawImage(parts.RightMiddle, point, 1), 10));
                }
                else if (x == 0)
                {
                    // top left edge
                    draws.Add((g => g.DrawImage(parts.UpperLeft, point, 1), 5));
                }
                else if (y == 0)
                {
                    // top right edge
                    draws.Add((g => g.DrawImage(parts.UpperRight, point, 1), 5));
                }
                else if (x == gridWidthMinus1)
                {
                    // bottom right edge
                    draws.Add((g => g.DrawImage(parts.BottomRight, pointDownByOne, 1), 5));
                }
                else if (y == gridHeightMinus1)
                {
                    // bottom left edge
                    draws.Add((g => g.DrawImage(parts.BottomLeft, pointDownByOne, 1), 5));
                }
                else
                {
                    // inner
                    draws.Add((g => g.DrawImage(parts.Inner, pointDownByOne, 1), 0));
                }
            }
        }

        big.Mutate(g =>
        {
            foreach (var priorityGroup in draws.GroupBy(x => x.Priority).OrderBy(x => x.Key))
            {
                foreach (var (Draw, Priority) in priorityGroup)
                {
                    Draw(g);
                }
            }
        });

        return big;
    }

    private Image<Rgba32> NewBase()
    {
        return new Image<Rgba32>(128, 96);
    }

    private Size Rotate(Size size)
    {
        return new Size(size.Height, size.Width);
    }

    private Size Middle(Size gridSize)
    {
        var middle = ConvertPoint(gridSize.Width / 2, gridSize.Height / 2);
        if (gridSize.Width % 2 != 0)
        {
            middle.Width += 2;
        }
        if (gridSize.Height % 2 != 0)
        {
            middle.Height += 2;
        }
        return middle;
    }

    private Size ConvertPoint(int x, int y)
    {
        var newX = x * 4 - y * 4;
        var newY = y * 2 + x * 2;
        return new Size(newX, newY);
    }
}
