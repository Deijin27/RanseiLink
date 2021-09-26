using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace Core.Services.ModelServices
{
    public interface IScenarioPokemonService : IModelDataService<ScenarioId, int, IScenarioPokemon>
    {
        IDisposableScenarioPokemonService Disposable();
    }

    public interface IDisposableScenarioPokemonService : IDisposableModelDataService<ScenarioId, int, IScenarioPokemon>
    {
    }

    public class ScenarioPokemonService : IScenarioPokemonService
    {
        private readonly string CurrentModFolder;
        private readonly ModInfo Mod;
        public ScenarioPokemonService(ModInfo mod)
        {
            Mod = mod;
            CurrentModFolder = mod.FolderPath;

        }

        public IDisposableScenarioPokemonService Disposable()
        {
            return new DisposableScenarioPokemonService(Mod);
        }

        public IScenarioPokemon Retrieve(ScenarioId scenario, int id)
        {
            if (id < 0 || id >= Constants.ScenarioPokemonCount)
            {
                throw new Exception("Invalid scenario pokemon ID");
            }
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(CurrentModFolder, Constants.ScenarioPokemonPathFromId(scenario)))))
            {
                file.BaseStream.Position = id * ScenarioPokemon.DataLength;
                return new ScenarioPokemon(file.ReadBytes(ScenarioPokemon.DataLength));
            }
        }

        public void Save(ScenarioId scenario, int id, IScenarioPokemon model)
        {
            if (id < 0 || id >= Constants.ScenarioPokemonCount)
            {
                throw new Exception("Invalid scenario pokemon ID");
            }
            using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(CurrentModFolder, Constants.ScenarioPokemonPathFromId(scenario)))))
            {
                file.BaseStream.Position = id * ScenarioPokemon.DataLength;
                file.Write(model.Data);
            }
        }
    }

    public class DisposableScenarioPokemonService : IDisposableScenarioPokemonService
    {
        private readonly int ItemLength;
        private readonly List<Stream> streams = new List<Stream>();

        public DisposableScenarioPokemonService(ModInfo mod)
        {
            ItemLength = ScenarioPokemon.DataLength;
            foreach (ScenarioId i in EnumUtil.GetValues<ScenarioId>())
            {
                streams.Add(File.Open(Path.Combine(mod.FolderPath, Constants.ScenarioPokemonPathFromId(i)), FileMode.Open, FileAccess.ReadWrite));
            }
        }

        public void Dispose()
        {
            foreach (var stream in streams)
            {
                stream.Close();
            }
        }

        public IScenarioPokemon Retrieve(ScenarioId scenario, int id)
        {
            if (id < 0 || id >= Constants.ScenarioPokemonCount)
            {
                throw new Exception("Invalid scenario pokemon ID");
            }
            Stream stream = streams[(int)scenario];
            stream.Position = id * ItemLength;
            byte[] buffer = new byte[ItemLength];
            stream.Read(buffer, 0, ItemLength);
            return new ScenarioPokemon(buffer);
        }

        public void Save(ScenarioId scenario, int id, IScenarioPokemon model)
        {
            if (id < 0 || id >= Constants.ScenarioPokemonCount)
            {
                throw new Exception("Invalid scenario pokemon ID");
            }
            Stream stream = streams[(int)scenario];
            stream.Position = id * ItemLength;
            stream.Write(model.Data, 0, ItemLength);
        }
    }
}
