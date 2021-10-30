using Core.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace Core.Services.ModelServices
{
    public abstract class BaseScenarioService
    {
        private readonly Func<ScenarioId, string> RomPathFunction;
        private readonly int ItemLength;
        protected ModInfo Mod;
        private readonly int MaxIndex;
        protected BaseScenarioService(ModInfo mod, int itemLength, int maxIndex, Func<ScenarioId, string> romPathFunction)
        {
            RomPathFunction = romPathFunction;
            ItemLength = itemLength;
            Mod = mod;
            MaxIndex = maxIndex;
        }

        private void ValidateIndex(ScenarioId scenario, int id)
        {
            int scenarioInt = (int)scenario;
            if (scenarioInt > 10 || scenarioInt < 0)
            {
                throw new ArgumentOutOfRangeException($"Scenario in {GetType().Name} was {scenarioInt}; it must not be greater than 10");
            }
            if (id > MaxIndex || id < 0)
            {
                throw new ArgumentOutOfRangeException($"Index in {GetType().Name} should not be greater than {MaxIndex}");
            }
        }


        protected byte[] RetrieveData(ScenarioId scenario, int id)
        {
            ValidateIndex(scenario, id);

            using (var file = new BinaryReader(File.OpenRead(Path.Combine(Mod.FolderPath, RomPathFunction(scenario)))))
            {
                file.BaseStream.Position = id * ItemLength;
                return file.ReadBytes(ItemLength);
            }
        }

        protected void SaveData(ScenarioId scenario, int id, byte[] data)
        {
            ValidateIndex(scenario, id);
            using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(Mod.FolderPath, RomPathFunction(scenario)))))
            {
                file.BaseStream.Position = id * ItemLength;
                file.Write(data);
            }
        }
    }


    public abstract class BaseDisposableScenarioService
    {
        private readonly int ItemLength;
        private readonly int MaxIndex;
        private readonly List<Stream> streams = new List<Stream>();

        public BaseDisposableScenarioService(ModInfo mod, int itemLength, int maxIndex, Func<ScenarioId, string> romPathFunction)
        {
            MaxIndex = maxIndex;
            ItemLength = itemLength;
            foreach (ScenarioId i in EnumUtil.GetValues<ScenarioId>())
            {
                streams.Add(File.Open(Path.Combine(mod.FolderPath, romPathFunction(i)), FileMode.Open, FileAccess.ReadWrite));
            }
        }

        public void Dispose()
        {
            foreach (var stream in streams)
            {
                stream.Close();
            }
        }

        private void ValidateIndex(ScenarioId scenario, int id)
        {
            int scenarioInt = (int)scenario;
            if (scenarioInt > 10 || scenarioInt < 0)
            {
                throw new ArgumentOutOfRangeException($"Scenario in {GetType().Name} was {scenarioInt}; it must not be greater than 10");
            }
            if (id > MaxIndex || id < 0)
            {
                throw new ArgumentOutOfRangeException($"Index in {GetType().Name} should not be greater than {MaxIndex}");
            }
        }

        public byte[] RetrieveData(ScenarioId scenario, int id)
        {
            ValidateIndex(scenario, id);
            Stream stream = streams[(int)scenario];
            stream.Position = id * ItemLength;
            byte[] buffer = new byte[ItemLength];
            stream.Read(buffer, 0, ItemLength);
            return buffer;
        }

        public void SaveData(ScenarioId scenario, int id, byte[] data)
        {
            ValidateIndex(scenario, id);
            Stream stream = streams[(int)scenario];
            stream.Position = id * ItemLength;
            stream.Write(data, 0, ItemLength);
        }
    }
}