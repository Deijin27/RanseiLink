using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Services;
using System.Threading.Tasks;

namespace RanseiLink.Console.GraphicsCommands;

[Command("populate graphics defaults", Description = "Populate the default graphics, providing files to patch sprites, and use OverrideSpriteProvider in lua scripts.")]
public class PopulateGraphicsDefaultsCommand : BaseCommand
{
    public PopulateGraphicsDefaultsCommand(IServiceContainer container) : base(container) { }
    public PopulateGraphicsDefaultsCommand() : base() { }

    [CommandParameter(0, Description = "Path to rom file.", Name = "path", Converter = typeof(PathConverter))]
    public string Path { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var fallbackSpriteProvider = Container.Resolve<IFallbackSpriteProvider>();

        console.Output.WriteLine("Populating...");

        fallbackSpriteProvider.Populate(Path);

        console.Output.WriteLine("Done!");
        return default;
    }
}
