using RanseiLink.Core;
using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Api;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpriteSheetSplitterPlugin;

[Plugin("Sprite Sheet Splitter", "Deijin", "1.0")]
public class SpriteSheetSplitterPlugin : IPlugin
{
    private IDialogService _dialogService;
    private IPluginService _pluginService;
    public void Run(IPluginContext context)
    {
        _dialogService ??= context.Services.Get<IDialogService>();
        _pluginService ??= context.Services.Get<IPluginService>();

        var result = _dialogService.ShowMessageBox(new MessageBoxArgs(
            "Split or Create?",
            "Would you like to split a sprite sheet into it's constituent sprites, or create one from a folder of sprites?",
            new MessageBoxButton[]
            {
                new MessageBoxButton("Split", MessageBoxResult.Yes),
                new MessageBoxButton("Create", MessageBoxResult.No),
                new MessageBoxButton("Cancel", MessageBoxResult.Cancel)
            }
            ));

        switch (result)
        {
            case MessageBoxResult.Yes:
                Split();
                break;
            case MessageBoxResult.No:
                Create();
                break;
            case MessageBoxResult.Cancel:
                return;
            default:
                throw new Exception($"Unexpected message box result '{result}'");
        }
    }

    private void Split()
    {
        if (!_dialogService.RequestFile("Select the sprite sheet", ".png", "PNG Image (.png)|*.png", out string file))
        {
            return;
        }

        var options = new SpriteSheetSplitterOptionForm();
        if (!_pluginService.RequestOptions(options))
        {
            return;
        }

        using var spriteSheet = Image.Load<Rgba32>(file);

        int spriteWidth = options.SpriteWidth;
        int spriteHeight = options.SpriteHeight;

        string outFolder = FileUtil.MakeUniquePath($"{file}.split");
        Directory.CreateDirectory(outFolder);

        if (spriteSheet.Width % spriteWidth != 0)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok("Invalid dimensions", $"The sprite sheet is width is not divisible by the provided sprite width"));
            return;
        }

        if (spriteSheet.Height % spriteHeight != 0)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok("Invalid dimensions", $"The sprite sheet is height is not divisible by the provided sprite height"));
            return;
        }

        List<Rectangle> rects = new();
        if (options.SplitDirection == SplitDirection.ColumnsThenRows)
        {
            for (int x = 0; x < spriteSheet.Width / spriteWidth; x++)
            {
                for (int y = 0; y < spriteSheet.Height / spriteHeight; y++)
                {
                    rects.Add(new Rectangle(spriteWidth * x, spriteHeight * y, spriteWidth, spriteHeight));
                }
            }
        }
        else
        {
            for (int y = 0; y < spriteSheet.Height / spriteHeight; y++)
            {
                for (int x = 0; x < spriteSheet.Width / spriteWidth; x++)
                {
                    rects.Add(new Rectangle(spriteWidth * x, spriteHeight * y, spriteWidth, spriteHeight));
                }
            }
        }
        
        int count = 0;
        foreach (var rect in rects)
        {
            using var sprite = spriteSheet.Clone(g =>
            {
                g.Crop(rect);
            });
            string outFile = Path.Combine(outFolder, count++.ToString().PadLeft(4, '0') + ".png");
            sprite.SaveAsPng(outFile);
        }

    }

    private void Create()
    {
        if (!_dialogService.RequestFolder("Select folder that contains sprites", out string folder))
        {
            return;
        }

        var options = new SpriteSheetCreaterOptionForm();
        if (!_pluginService.RequestOptions(options))
        {
            return;
        }

        var images = new List<Image<Rgba32>>();

        string[] files = Directory.GetFiles(folder);
        if (files.Length == 0)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok("Folder is empty", $"There are no files in the folder '{folder}'"));
            return;
        }

        Array.Sort(files);

        foreach (var file in files)
        {
            images.Add(Image.Load<Rgba32>(file));
        }

        var first = images.First();
        int spriteWidth = first.Width;
        int spriteHeight = first.Height;
        if (!images.All(i => i.Width == first.Width && i.Height == first.Height))
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok("Invalid images", $"All images must have the same dimensions"));
            return;
        }

        int spriteSheetWidth = options.SheetWidth;

        if (spriteSheetWidth % first.Width != 0)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok("Invalid images", $"The width you provided is not divisible by the widths of the sprites"));
            return;
        }

        int spriteSheetHeight = options.SheetHeight;
        if (spriteSheetHeight % first.Height != 0)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok("Invalid images", $"The height you provided is not divisible by the height of the sprites"));
            return;
        }

        if (images.Count * spriteWidth * spriteHeight > spriteSheetHeight * spriteSheetWidth)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok("Invalid images", $"The sprites do not fit in the sheet size"));
            return;
        }

        List<Point> points = new();

        if (options.SplitDirection == SplitDirection.ColumnsThenRows)
        {
            for (int x = 0; x < spriteSheetWidth / spriteWidth; x++)
            {
                for (int y = 0; y < spriteSheetHeight / spriteHeight; y++)
                {
                    points.Add(new Point(spriteWidth * x, spriteHeight * y));
                }
            }
        }
        else
        {
            for (int y = 0; y < spriteSheetHeight / spriteHeight; y++)
            {
                for (int x = 0; x < spriteSheetWidth / spriteWidth; x++)
                {
                    points.Add(new Point(spriteWidth * x, spriteHeight * y));
                }
            }
        }

        using var img = new Image<Rgba32>(spriteSheetWidth, spriteSheetHeight);

        img.Mutate(g =>
        {
            int count = 0;
            foreach (var image in images)
            {
                var point = points[count++];
                g.DrawImage(image, point, 1);
                image.Dispose();
            }
        });

        string outFile = FileUtil.MakeUniquePath($"{folder}.png");
        img.SaveAsPng(outFile);
    }
}
