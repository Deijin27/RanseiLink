using CliFx.Attributes;
using CliFx.Infrastructure;
using RanseiLink.Core.Nds;
using RanseiLink.Core.Services;
using System.IO;
using System.Threading.Tasks;

namespace RanseiLink.Console.Commands;

[Command("pokemon sprite export all", Description = "Export all pokemon sprites from the given rom")]
public class PokemonSpriteExportAllCommand : BaseCommand
{
    public PokemonSpriteExportAllCommand(IServiceContainer container) : base(container) { }
    public PokemonSpriteExportAllCommand() : base() { }

    [CommandParameter(0, Description = "Path to unchanged rom file to serve as a base.", Name = "romPath", Converter = typeof(PathConverter))]
    public string RomPath { get; set; }

    [CommandOption("destinationFolder", 'd', Description = "Folder to export to. A lot of folders will be put in this, so make sure it's not desktop or something. Default is a folder called 'PokemonSprites' in the same place as the rom", Converter = typeof(PathConverter))]
    public string DestinationFolder { get; set; }

    [CommandOption("paintNetPalette", 'p', Description = "Include a palette that can be loaded into paint.net with the sprites")]
    public bool PaintNetPalette { get; set; }

    public override ValueTask ExecuteAsync(IConsole console)
    {
        var ndsFactory = Container.Resolve<NdsFactory>();
        var spriteService = Container.Resolve<ISpriteService>();

        if (DestinationFolder == null)
        {
            DestinationFolder = Path.Combine(Path.GetDirectoryName(RomPath), "PokemonSprites");
        }
        Directory.CreateDirectory(DestinationFolder);

        SpriteExportOptions options = 0;
        if (PaintNetPalette) options |= SpriteExportOptions.IncludePaintNetPalette;

        using var nds = ndsFactory(RomPath);
        spriteService.ExportAllPokemonSprites(nds, DestinationFolder, options);

        console.Output.WriteLine("Sprite export complete!");

        return default;
    }
}