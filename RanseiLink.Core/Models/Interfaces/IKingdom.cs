using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models.Interfaces;

public interface IKingdom : IDataWrapper, ICloneable<IKingdom>
{
    string Name { get; set; }

    KingdomId MapConnection0 { get; set; }
    KingdomId MapConnection1 { get; set; }
    KingdomId MapConnection2 { get; set; }
    KingdomId MapConnection3 { get; set; }
    KingdomId MapConnection4 { get; set; }
    KingdomId MapConnection5 { get; set; }
    KingdomId MapConnection6 { get; set; }
    KingdomId MapConnection7 { get; set; }
    KingdomId MapConnection8 { get; set; }
    KingdomId MapConnection9 { get; set; }
    KingdomId MapConnection10 { get; set; }
    KingdomId MapConnection11 { get; set; }
    KingdomId MapConnection12 { get; set; }
    BattleConfigId BattleConfig { get; set; }

    KingdomId[] MapConnections { get; set; }
    uint Unknown_R2_C24_L3 { get; set; }
    uint Unknown_R5_C22_L4 { get; set; }
    uint Unknown_R5_C26_L4 { get; set; }
}
