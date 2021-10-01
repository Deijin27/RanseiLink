using System;
using System.IO;

namespace Core.Services.ModelServices
{
    public abstract class BaseModelService
    {
        protected readonly string CurrentModFolder;
        private readonly string RomPath;
        private readonly int ItemLength;
        protected ModInfo Mod;

        public BaseModelService(ModInfo mod, string romPath, int itemLength)
        {
            Mod = mod;
            RomPath = romPath;
            ItemLength = itemLength;
            CurrentModFolder = mod.FolderPath;
        }

        protected byte[] RetrieveData(int id)
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(CurrentModFolder, RomPath))))
            {
                file.BaseStream.Position = id * ItemLength;
                return file.ReadBytes(ItemLength);
            }
        }

        protected void SaveData(int id, byte[] data)
        {
            using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(CurrentModFolder, RomPath))))
            {
                file.BaseStream.Position = id * ItemLength;
                file.Write(data);
            }
        }
    }

    public abstract class BaseDisposableModelService : IDisposable
    {
        private readonly int ItemLength;

        protected readonly Stream stream;

        public BaseDisposableModelService(ModInfo mod, string romPath, int itemLength)
        {
            ItemLength = itemLength;
            stream = File.Open(Path.Combine(mod.FolderPath, romPath), FileMode.Open, FileAccess.ReadWrite);
        }

        public void Dispose()
        {
            stream.Close();
        }

        protected byte[] RetrieveData(int id)
        {
            stream.Position = id * ItemLength;
            byte[] buffer = new byte[ItemLength];
            stream.Read(buffer, 0, ItemLength);
            return buffer;
        }

        protected void SaveData(int id, byte[] data)
        {
            stream.Position = id * ItemLength;
            stream.Write(data, 0, ItemLength);
        }
    }
}