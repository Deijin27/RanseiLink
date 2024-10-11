using RanseiLink.Core.Models;

namespace RanseiLink.Core.Services.ModelServices;

public partial interface IBaseWarriorService
{
    WarriorNameTable NameTable { get; }
}

public partial class BaseWarriorService
{
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
        var warriorNameId = Retrieve(id).Name;
        if (!NameTable.ValidateId(warriorNameId))
        {
            return "";
        }
        return NameTable.GetEntry(warriorNameId);
    }
}