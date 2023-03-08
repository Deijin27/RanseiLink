using RanseiLink.Core.Enums;
using FluentResults;

namespace RanseiLink.Core.Services
{
    public interface IPokemonAnimationService
    {
        Result ExportAnimations(PokemonId id, string dest);
        Result ImportAnimation(PokemonId id, string file);
        bool IsAnimationOverwritten(PokemonId id);
        void RevertAnimation(PokemonId id);
    }
}
