using RanseiLink.Core.Models;
using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Services.ModelServices;

public interface IBaseWarriorService : IModelService<BaseWarrior>
{
    WarriorNameTable NameTable { get; }
}

public class BaseWarriorService : BaseNewableDataModelService<BaseWarrior>, IBaseWarriorService
{
    public static BaseWarriorService Load(string BaseWarriorServiceDatFile) => new BaseWarriorService(BaseWarriorServiceDatFile);
    private BaseWarriorService(string BaseWarriorServiceDatFile) : base(BaseWarriorServiceDatFile, 0, 251, 252) { }

    public BaseWarriorService(ModInfo mod) : this(Path.Combine(mod.FolderPath, Constants.BaseBushouRomPath)) { }

    public BaseWarrior Retrieve(WarriorId id) => Retrieve((int)id);

    public override void PostLoad(Stream stream)
    {
        stream.Position = 0x13B0;
        NameTable = new WarriorNameTable();
        NameTable.Read(stream);
    }

    public override void PostSave(Stream stream)
    {
        stream.Position = 0x13B0;
        NameTable.Write(stream);
    }

    public WarriorNameTable NameTable { get; private set; } = null!;


    public override string IdToName(int id)
    {
        var warriorNameId = Retrieve(id).WarriorName;
        if (!NameTable.ValidateId(warriorNameId))
        {
            return "";
        }
        return NameTable.GetEntry(warriorNameId);
    }
}