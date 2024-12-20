#nullable enable
using RanseiLink.Core.Models;
using RanseiLink.Core.Util;

namespace RanseiLink.GuiCore.ViewModels;

public class KingdomMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private readonly Kingdom _model;
    private readonly int _id;

    public KingdomMiniViewModel(ICachedSpriteProvider cachedSpriteProvider,
        Kingdom model, int id, ICommand selectCommand)
    {
        _cachedSpriteProvider = cachedSpriteProvider;
        _model = model;
        _id = id;
        SelectCommand = selectCommand;
    }

    public int Id => _id;

    public string Name => _model.Name;
    public object? Image => _cachedSpriteProvider.GetSprite(Core.Services.SpriteType.StlCastleIcon, _id);

    public ICommand SelectCommand { get; }

    public bool MatchSearchTerm(string searchTerm)
    {
        if (Name.ContainsIgnoreCaseAndAccents(searchTerm))
        {
            return true;
        }

        return false;
    }

    public void NotifyPropertyChanged(string? name)
    {
        switch (name)
        {
            case nameof(KingdomViewModel.Name):
                RaisePropertyChanged(nameof(Name));
                break;
        }
    }
}
