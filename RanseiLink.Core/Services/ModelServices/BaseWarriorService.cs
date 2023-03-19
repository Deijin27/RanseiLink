using RanseiLink.Core.Models;
using System.IO;
using System;
using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IBaseWarriorService : IModelService<BaseWarrior>
    {
        WarriorNameTable NameTable { get; }
    }

    public class BaseWarriorService : BaseModelService<BaseWarrior>, IBaseWarriorService
    {
        public static BaseWarriorService Load(string BaseWarriorServiceDatFile) => new BaseWarriorService(BaseWarriorServiceDatFile);
        private BaseWarriorService(string BaseWarriorServiceDatFile) : base(BaseWarriorServiceDatFile, 0, 251, 252) { }

        public BaseWarriorService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.BaseBushouRomPath)) { }

        public BaseWarrior Retrieve(WarriorId id) => Retrieve((int)id);

        public override void Reload()
        {
            _cache.Clear();
            using (var br = new BinaryReader(File.OpenRead(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    _cache.Add(new BaseWarrior(br.ReadBytes(BaseWarrior.DataLength)));
                }
                br.BaseStream.Position = 0x13B0;
                NameTable = new WarriorNameTable(br.ReadBytes(WarriorNameTable.DataLength));
            }
        }

        public override void Save()
        {
            using (var bw = new BinaryWriter(File.OpenWrite(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    bw.Write(_cache[id].Data);
                }
                bw.BaseStream.Position = 0x13B0;
                bw.Write(NameTable.Data);
            }
        }

        public WarriorNameTable NameTable { get; private set; } = null!;


        public override string IdToName(int id)
        {
            if (!ValidateId(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            var warriorNameId = Retrieve(id).WarriorName;
            if (!NameTable.ValidateId(warriorNameId))
            {
                return "";
            }
            return NameTable.GetEntry(warriorNameId);
        }
    }
}