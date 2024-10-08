﻿using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace RanseiLink.Core.Services.ModelServices;

public interface IChildScenarioPokemonService : IModelService<ScenarioPokemon>
{
}

public interface IScenarioPokemonService : IModelService<IChildScenarioPokemonService>
{

}

public class ScenarioPokemonService : BaseModelService<IChildScenarioPokemonService>, IScenarioPokemonService
{
    public ScenarioPokemonService(ModInfo mod, IPokemonService pokemonService) : base(string.Empty, 0, 10)
    {
        for (int i = _minId; i <= _maxId; i++)
        {
            _cache.Add(new ChildScenarioPokemonService(Path.Combine(mod.FolderPath, Constants.ScenarioPokemonPathFromId(i)), pokemonService));
        }
    }

    public IChildScenarioPokemonService Retrieve(ScenarioId id) => Retrieve((int)id);

    public override string IdToName(int id)
    {
        if (!ValidateId(id))
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }
        return ((ScenarioId)id).ToString();
    }

    public override void Reload()
    {
        foreach (var childService in Enumerate())
        {
            childService.Reload();
        }
    }

    public override void Save()
    {
        foreach (var childService in Enumerate())
        {
            childService.Save();
        }
    }
}

public class ChildScenarioPokemonService : BaseNewableDataModelService<ScenarioPokemon>, IChildScenarioPokemonService
{
    private readonly IPokemonService _pokemonService;
    public ChildScenarioPokemonService(string scenarioPokemonDatFile, IPokemonService pokemonService) : base(scenarioPokemonDatFile, 0, 199)
    {
        _pokemonService = pokemonService;
    }

    public override string IdToName(int id)
    {
        var pokemon = (int)Retrieve(id).Pokemon;
        if (!_pokemonService.ValidateId(pokemon))
        {
            return "Default";
        }
        return _pokemonService.IdToName(pokemon);
    }
} 