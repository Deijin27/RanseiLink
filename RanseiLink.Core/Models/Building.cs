using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public partial class Building
{
    public BuildingId[] Buildings
    {
        get => [Building1, Building2, Building3, Building4, Building5, Building6, Building7, Building8];
        set
        {
            if (value.Length == 8)
            {
                Building1 = value[0];
                Building2 = value[1];
                Building3 = value[2];
                Building4 = value[3];
                Building5 = value[4];
                Building6 = value[5];
                Building7 = value[6];
                Building8 = value[7];
            }
        }
    }
}