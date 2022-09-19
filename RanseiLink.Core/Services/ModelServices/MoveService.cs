using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using System.IO;
using System;

namespace RanseiLink.Core.Services.ModelServices
{
    public interface IMoveService : IModelService<Move>
    {
    }

    public class MoveService : BaseModelService<Move>, IMoveService
    {
        private MoveService(string MoveDatFile) : base(MoveDatFile, 0, 142) { }

        public MoveService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.MoveRomPath)) { }

        public Move Retrieve(MoveId id) => Retrieve((int)id);

        public override void Reload()
        {
            _cache.Clear();
            using (var br = new BinaryReader(File.OpenRead(_dataFile)))
            {
                for (int id = _minId; id <= _maxId; id++)
                {
                    _cache.Add(new Move(br.ReadBytes(Move.DataLength)));
                }
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
            }
        }

        public override string IdToName(int id)
        {
            if (!ValidateId(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            return _cache[id].Name;
        }
    } 
}