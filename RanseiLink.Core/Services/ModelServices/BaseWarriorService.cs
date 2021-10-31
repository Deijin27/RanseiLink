using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Models;
using System.IO;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IBaseWarriorService : IModelDataService<WarriorId, IBaseWarrior>
    {
        IDisposableBaseWarriorService Disposable();
        IWarriorNameTable RetrieveNameTable();
        void SaveNameTable(IWarriorNameTable model);
    }

    public interface IDisposableBaseWarriorService : IDisposableModelDataService<WarriorId, IBaseWarrior>
    {
        IWarriorNameTable RetrieveNameTable();
        void SaveNameTable(IWarriorNameTable model);
    }

    public class BaseWarriorService : BaseModelService, IBaseWarriorService
    {
        public BaseWarriorService(ModInfo mod) : base(mod, Constants.BaseBushouRomPath, BaseWarrior.DataLength, 251) { }

        public IDisposableBaseWarriorService Disposable()
        {
            return new DisposableBaseWarriorService(Mod);
        }

        public IBaseWarrior Retrieve(WarriorId id)
        {
            return new BaseWarrior(RetrieveData((int)id));
        }

        public void Save(WarriorId id, IBaseWarrior model)
        {
            SaveData((int)id, model.Data);
        }

        public IWarriorNameTable RetrieveNameTable()
        {
            using (var file = new BinaryReader(File.OpenRead(Path.Combine(Mod.FolderPath, Constants.BaseBushouRomPath))))
            {
                file.BaseStream.Position = 0x13B0;
                return new WarriorNameTable(file.ReadBytes(WarriorNameTable.DataLength));
            }
        }

        public void SaveNameTable(IWarriorNameTable model)
        {
            using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(Mod.FolderPath, Constants.BaseBushouRomPath))))
            {
                file.BaseStream.Position = 0x13B0;
                file.Write(model.Data);
            }
        }
    }

    public class DisposableBaseWarriorService : BaseDisposableModelService, IDisposableBaseWarriorService
    {
        public DisposableBaseWarriorService(ModInfo mod) : base(mod, Constants.BaseBushouRomPath, BaseWarrior.DataLength, 251) { }

        public IBaseWarrior Retrieve(WarriorId id)
        {
            return new BaseWarrior(RetrieveData((int)id));
        }

        public void Save(WarriorId id, IBaseWarrior model)
        {
            SaveData((int)id, model.Data);
        }

        public IWarriorNameTable RetrieveNameTable()
        {
            stream.Position = 0x13B0;
            byte[] buffer = new byte[WarriorNameTable.DataLength];
            stream.Read(buffer, 0, WarriorNameTable.DataLength);
            return new WarriorNameTable(buffer);
        }

        public void SaveNameTable(IWarriorNameTable model)
        {
            stream.Position = 0x13B0;
            stream.Write(model.Data, 0, WarriorNameTable.DataLength);
        }
    }
}
