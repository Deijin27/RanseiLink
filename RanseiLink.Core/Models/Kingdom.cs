using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public partial class Kingdom : BaseDataWindow
{
    /// <summary>
    /// Kingdoms you can battle using warriors in this kingdom
    /// </summary>
    public KingdomId[] MapConnections
    {
        get => [MapConnection0, MapConnection1, MapConnection2, MapConnection3, MapConnection4, MapConnection5, MapConnection6, MapConnection7, MapConnection8, MapConnection9, MapConnection10, MapConnection11, MapConnection12];
        set
        {
            MapConnection0 = value[0];
            MapConnection1 = value[1];
            MapConnection2 = value[2];
            MapConnection3 = value[3];
            MapConnection4 = value[4];
            MapConnection5 = value[5];
            MapConnection6 = value[6];
            MapConnection7 = value[7];
            MapConnection8 = value[8];
            MapConnection9 = value[9];
            MapConnection10 = value[10];
            MapConnection11 = value[11];
            MapConnection12 = value[12];
        }
    }
}