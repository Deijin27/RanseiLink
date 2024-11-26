using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.ViewModels;

public class BuildingSimpleKingdomMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly KingdomId _kingdom;
    protected readonly ICachedSpriteProvider _spriteProvider;
    public BuildingSimpleKingdomMiniViewModel(ICachedSpriteProvider spriteProvider, KingdomId kingdom)
    {
        _spriteProvider = spriteProvider;
        _kingdom = kingdom;
    }

    public object? KingdomImage => _spriteProvider.GetSprite(SpriteType.StlCastleIcon, (int)_kingdom);

    public KingdomId Kingdom => _kingdom;

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
