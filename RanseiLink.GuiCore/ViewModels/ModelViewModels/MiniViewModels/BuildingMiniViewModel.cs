using RanseiLink.Core.Models;
using RanseiLink.Core.Util;

namespace RanseiLink.GuiCore.ViewModels;
public class BuildingMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly ICachedSpriteProvider _spriteProvider;
    private readonly Building _model;
    private readonly int _id;

    public BuildingMiniViewModel(ICachedSpriteProvider spriteProvider, Building model, int id, ICommand selectCommand)
    {
        _spriteProvider = spriteProvider;
        _model = model;
        _id = id;
        SelectCommand = selectCommand;
    }

    public int Id => _id;

    public string Name
    {
        get => _model.Name;
        set => SetProperty(_model.Name, value, v => _model.Name = v);
    }

    public object? Sprite1Image => _spriteProvider.GetSprite(Core.Services.SpriteType.IconInstS, _model.Sprite1);

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
            case nameof(Name):
            case nameof(Sprite1Image):
                RaisePropertyChanged(name);
                break;
        }
    }
}
