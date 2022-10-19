using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("populate graphics defaults", Description = "Populate the default graphics, providing files to patch sprites, and use OverrideSpriteProvider in lua scripts.")]
public class PopulateGraphicsDefaultsCommand : ICommand
{
    private readonly IFallbackDataProvider _fallbackSpriteProvider;
    public PopulateGraphicsDefaultsCommand(IFallbackDataProvider fallbackSpriteProvider)
    {
        _fallbackSpriteProvider = fallbackSpriteProvider;
    }

    [CommandParameter(0, Description = "Path to rom file.", Name = "path", Converter = typeof(PathConverter))]
    public string Path { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine("Populating...");

        var result = _fallbackSpriteProvider.Populate(Path);

        if (result.Success)
        {
            console.Output.WriteLine("Done!");
        }
        else
        {
            console.Output.WriteLine($"Population Failed: {result.FailureReason}");
        }
        
        return default;
    }
}
