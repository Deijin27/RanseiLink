using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics.Conquest;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;
using System;
using System.Xml.Linq;
using System.IO;
using FluentResults;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Core.Services.Concrete;

public class PokemonAnimationService : IPokemonAnimationService
{
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly IPokemonService _pokemonService;

    public PokemonAnimationService(IOverrideDataProvider spriteProvider, IPokemonService pokemonService)
    {
        _spriteProvider = spriteProvider;
        _pokemonService = pokemonService;
    }

    public Result ImportAnimation(PokemonId id, string file)
    {
        var result = file;

        var temp1 = Path.GetTempFileName();
        var temp2 = Path.GetTempFileName();
        try
        {
            NSPAT nspat;
            NSPAT nspatRaw;

            var doc = XDocument.Load(result);

            var root = doc.Element(PatternGroupElementName);
            if (root == null)
            {
                return Result.Fail($"Failed to load the document because it doesn't match what is expected for a pattern animation (expected root element: {PatternGroupElementName})");
            }
            var nonRawEl = root.Element(NSPAT.RootElementName);
            if (nonRawEl == null)
            {
                return Result.Fail($"Failed to load the document because it doesn't match what is expected for a pattern animation (element not found: {NSPAT.RootElementName})");
            }
            var rawEl = root.Element(NSPAT_RAW.RootElementName);
            if (rawEl == null)
            {
                return Result.Fail($"Failed to load the document because it doesn't match what is expected for a pattern animation (element not found: {NSPAT_RAW.RootElementName})");
            }
            nspat = NSPAT.Deserialize(nonRawEl);
            if (nspat == null)
            {
                return Result.Fail($"Failed to load the document because it doesn't match what is expected for a pattern animation (failed to deserialize element: {NSPAT.RootElementName}");
            }
            nspatRaw = NSPAT.Deserialize(rawEl);
            if (nspatRaw == null)
            {
                return Result.Fail($"Failed to load the document because it doesn't match what is expected for a pattern animation (failed to deserialize element: {NSPAT_RAW.RootElementName}");
            }

            // raw
            NSPAT_RAW.WriteTo(nspat, temp1);
            var nspatRawFile = ResolveRelativeAnimPath(id, true);
            _spriteProvider.SetOverride(nspatRawFile, temp1);

            var pokemon = _pokemonService.Retrieve((int)id);
            // non raw
            // need to make sure the natdex number is correct
            foreach (var anim in nspat.PatternAnimations)
            {
                var name = anim.Name;
                var end = name.Substring(name.Length - 5);
                var num = pokemon.NationalPokedexNumber.ToString().PadLeft(3, '0');
                anim.Name = $"POKEMON{num}{end}";
            }
            new NSBTP(nspat).WriteTo(temp2);
            var nsbtpFile = ResolveRelativeAnimPath(id, false);
            _spriteProvider.SetOverride(nsbtpFile, temp2);

            pokemon.AsymmetricBattleSprite = bool.TryParse(root.Attribute(AsymmetricalAttributeName)?.Value, out var asv) && asv;
            pokemon.LongAttackAnimation = bool.TryParse(root.Attribute(LongAttackAttributeName)?.Value, out var lav) && lav;
        }
        catch (Exception ex)
        {
            return Result.Fail("Unable to import file due to error:\n\n" + ex.ToString());
        }
        finally
        {
            File.Delete(temp1);
            File.Delete(temp2);
        }

        return Result.Ok();
    }

    public bool IsAnimationOverwritten(PokemonId id)
    {
        return _spriteProvider.GetDataFile(ResolveRelativeAnimPath(id, false)).IsOverride
            || _spriteProvider.GetDataFile(ResolveRelativeAnimPath(id, true)).IsOverride;
    }

    public void RevertAnimation(PokemonId id)
    {
        string nonRawFile = ResolveRelativeAnimPath(id, false);
        _spriteProvider.ClearOverride(nonRawFile);

        var rawFile = ResolveRelativeAnimPath(id, true);
        _spriteProvider.ClearOverride(rawFile);
    }

    private string ResolveRelativeAnimPath(PokemonId id, bool raw)
    {
        var info = (PkmdlConstants)GraphicsInfoResource.Get(SpriteType.ModelPokemon);
        var pacLinkRelative = info.PACLinkFolder;
        string fileName = ((int)id).ToString().PadLeft(4, '0');
        string pacUnpackedFolder = Path.Combine(pacLinkRelative, fileName + "-Unpacked");
        return Path.Combine(pacUnpackedFolder, raw ? "0002" : "0001");
    }

    public Result ExportAnimations(PokemonId id, string dest)
    {
        if (!_spriteProvider.IsDefaultsPopulated())
        {
            return Result.Fail("Cannot export animation since graphics defaults are not populated");
        }

        dest = FileUtil.MakeUniquePath(dest);
        var file = _spriteProvider.GetDataFile(ResolveRelativeAnimPath(id, false));
        var rawfile = _spriteProvider.GetDataFile(ResolveRelativeAnimPath(id, true));

        var pokemon = _pokemonService.Retrieve((int)id);
        var nsbtp = new NSBTP(file.File);
        var nspat = NSPAT_RAW.Load(rawfile.File);
        var doc = new XDocument(new XElement(PatternGroupElementName,
            new XAttribute(LongAttackAttributeName, pokemon.LongAttackAnimation),
            new XAttribute(AsymmetricalAttributeName, pokemon.AsymmetricBattleSprite),
            nsbtp.PatternAnimations.Serialize(),
            nspat.SerializeRaw()
            ));
        doc.Save(dest);

        return Result.Ok();
    }

    private const string LongAttackAttributeName = "long_attack";
    private const string AsymmetricalAttributeName = "asymmetrical";
    private const string PatternGroupElementName = "library_collection";
}
