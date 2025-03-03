using RanseiLink.Core.Enums;

namespace RanseiLink.Core.Models;

public partial class Pokemon
{
    public List<PokemonId> Evolutions { get; set; } = [];

    public bool GetEncounterable(KingdomId kingdom, bool requiresLevel2)
    {
        return GetEncounterable((int)kingdom, requiresLevel2);
    }

    public void SetEncounterable(KingdomId kingdom, bool requiresLevel2, bool value)
    {
        SetEncounterable((int)kingdom, requiresLevel2, value);
    }

    public bool GetEncounterable(int kingdom, bool requiresLevel2)
    {
        var shift = kingdom * 3 + (requiresLevel2 ? 1 : 0);
        return (BitConverter.ToUInt64(Data, 9 * 4) >> shift & 1) == 1;
    }

    public void SetEncounterable(int kingdom, bool requiresLevel2, bool value)
    {
        var shift = kingdom * 3 + (requiresLevel2 ? 1 : 0);
        var num = BitConverter.ToUInt64(Data, 9 * 4) & ~(1uL << shift);
        if (value)
        {
            num |= 1UL << shift;
        }
        BitConverter.GetBytes(num).CopyTo(Data, 9 * 4);
    }

    public bool HasAbility(AbilityId id)
    {
        return Ability1 == id || Ability2 == id || Ability3 == id;
    }
}