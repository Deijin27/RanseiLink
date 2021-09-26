using Core.Enums;
using Core.Models;
using Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace Core.Services.ModelServices
{
    public interface IScenarioWarriorService : IModelDataService<ScenarioId, int, IScenarioWarrior>
    {
        IDisposableScenarioWarriorService Disposable();
    }

    public interface IDisposableScenarioWarriorService : IDisposableModelDataService<ScenarioId, int, IScenarioWarrior>
    {
    }

    public class ScenarioWarriorService : IScenarioWarriorService
    {
        private readonly string CurrentModFolder;
        private readonly ModInfo Mod;
        public ScenarioWarriorService(ModInfo mod)
        {
            Mod = mod;
            CurrentModFolder = mod.FolderPath;

        }

        public IDisposableScenarioWarriorService Disposable()
        {
            return new DisposableScenarioWarriorService(Mod);
        }

        public IScenarioWarrior Retrieve(ScenarioId scenario, int id)
        {
            if (id < 0 || id >= Constants.ScenarioWarriorCount)
            {
                throw new Exception("Invalid scenario warrior ID");
            }
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(CurrentModFolder, Constants.ScenarioWarriorPathFromId(scenario)))))
            {
                file.BaseStream.Position = id * ScenarioWarrior.DataLength;
                return new ScenarioWarrior(file.ReadBytes(ScenarioWarrior.DataLength));
            }
        }

        public void Save(ScenarioId scenario, int id, IScenarioWarrior model)
        {
            if (id < 0 || id >= Constants.ScenarioWarriorCount)
            {
                throw new Exception("Invalid scenario warrior ID");
            }
            using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(CurrentModFolder, Constants.ScenarioWarriorPathFromId(scenario)))))
            {
                file.BaseStream.Position = id * ScenarioWarrior.DataLength;
                file.Write(model.Data);
            }
        }
    }

    public class DisposableScenarioWarriorService : IDisposableScenarioWarriorService
    {
        private readonly int ItemLength;
        private readonly List<Stream> streams = new List<Stream>();

        public DisposableScenarioWarriorService(ModInfo mod)
        {
            ItemLength = ScenarioWarrior.DataLength;
            foreach (ScenarioId i in EnumUtil.GetValues<ScenarioId>())
            {
                streams.Add(File.Open(Path.Combine(mod.FolderPath, Constants.ScenarioWarriorPathFromId(i)), FileMode.Open, FileAccess.ReadWrite));
            }
        }

        public void Dispose()
        {
            foreach (var stream in streams)
            {
                stream.Close();
            }
        }

        public IScenarioWarrior Retrieve(ScenarioId scenario, int id)
        {
            if (id < 0 || id >= Constants.ScenarioWarriorCount)
            {
                throw new Exception("Invalid scenario warrior ID");
            }
            Stream stream = streams[(int)scenario];
            stream.Position = id * ItemLength;
            byte[] buffer = new byte[ItemLength];
            stream.Read(buffer, 0, ItemLength);
            return new ScenarioWarrior(buffer);
        }

        public void Save(ScenarioId scenario, int id, IScenarioWarrior model)
        {
            if (id < 0 || id >= Constants.ScenarioWarriorCount)
            {
                throw new Exception("Invalid scenario warrior ID");
            }
            Stream stream = streams[(int)scenario];
            stream.Position = id * ItemLength;
            stream.Write(model.Data, 0, ItemLength);
        }
    }
}
