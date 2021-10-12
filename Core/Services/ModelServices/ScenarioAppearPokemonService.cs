using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace Core.Services.ModelServices
{
    public interface IScenarioAppearPokemonService : IModelDataService<ScenarioId, IScenarioAppearPokemon>
    {
        IDisposableScenarioAppearPokemonService Disposable();
    }

    public interface IDisposableScenarioAppearPokemonService : IDisposableModelDataService<ScenarioId, IScenarioAppearPokemon>
    {
    }

    public class ScenarioAppearPokemonService : IScenarioAppearPokemonService
    {
        private readonly string CurrentModFolder;
        private readonly ModInfo Mod;
        public ScenarioAppearPokemonService(ModInfo mod)
        {
            Mod = mod;
            CurrentModFolder = mod.FolderPath;

        }

        public IDisposableScenarioAppearPokemonService Disposable()
        {
            return new DisposableScenarioAppearPokemonService(Mod);
        }

        public IScenarioAppearPokemon Retrieve(ScenarioId id)
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(CurrentModFolder, Constants.ScenarioAppearPokemonPathFromId(id)))))
            {
                return new ScenarioAppearPokemon(file.ReadBytes(ScenarioAppearPokemon.DataLength));
            }
        }

        public void Save(ScenarioId ScenarioAppear, IScenarioAppearPokemon model)
        {
            using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(CurrentModFolder, Constants.ScenarioAppearPokemonPathFromId(ScenarioAppear)))))
            {
                file.Write(model.Data);
            }
        }
    }

    public class DisposableScenarioAppearPokemonService : IDisposableScenarioAppearPokemonService
    {
        private readonly int ItemLength;
        private readonly List<Stream> streams = new List<Stream>();

        public DisposableScenarioAppearPokemonService(ModInfo mod)
        {
            ItemLength = ScenarioAppearPokemon.DataLength;
            foreach (ScenarioId i in EnumUtil.GetValues<ScenarioId>())
            {
                streams.Add(File.Open(Path.Combine(mod.FolderPath, Constants.ScenarioAppearPokemonPathFromId(i)), FileMode.Open, FileAccess.ReadWrite));
            }
        }

        public void Dispose()
        {
            foreach (var stream in streams)
            {
                stream.Close();
            }
        }

        public IScenarioAppearPokemon Retrieve(ScenarioId id)
        {
            Stream stream = streams[(int)id];
            stream.Position = 0;
            byte[] buffer = new byte[ItemLength];
            stream.Read(buffer, 0, ItemLength);
            return new ScenarioAppearPokemon(buffer);
        }

        public void Save(ScenarioId id, IScenarioAppearPokemon model)
        {
            Stream stream = streams[(int)id];
            stream.Position = 0;
            stream.Write(model.Data, 0, ItemLength);
        }
    }
}
