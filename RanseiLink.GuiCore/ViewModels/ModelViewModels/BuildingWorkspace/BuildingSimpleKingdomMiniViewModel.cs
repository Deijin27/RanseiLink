using RanseiLink.Core.Enums;

namespace RanseiLink.GuiCore.ViewModels;

public class BuildingSimpleKingdomMiniViewModel : BaseSimpleKingdomMiniViewModel, IMiniViewModel
{
    public BuildingSimpleKingdomMiniViewModel(ICachedSpriteProvider spriteProvider, IIdToNameService idToNameService, KingdomId kingdom)
        : base(spriteProvider, idToNameService)
    {
        _kingdom = kingdom;
    }

    public int Id => -1;

    public ICommand SelectCommand => null!;

    public void NotifyPropertyChanged(string? name)
    {
    }

    public bool MatchSearchTerm(string searchTerm)
    {
        return true;
    }
}
