#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.ViewModels;

public class PokemonMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly ICachedSpriteProvider _spriteProvider;
    private readonly Pokemon _model;
    private readonly int _id;

    public PokemonMiniViewModel(ICachedSpriteProvider spriteProvider, Pokemon model, int id, ICommand selectCommand)
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

    public TypeId Type1
    {
        get => _model.Type1;
        set => SetProperty(_model.Type1, value, v => _model.Type1 = v);
    }
    public TypeId Type2
    {
        get => _model.Type2;
        set
        {
            if (SetProperty(_model.Type2, value, v => _model.Type2 = v))
            {
                RaisePropertyChanged(nameof(HasType2));
            }
        }
    }

    public bool HasType2 => Type2 != TypeId.NoType;

    public object? Image => _spriteProvider.GetSprite(SpriteType.StlPokemonS, _id);

    public ICommand SelectCommand { get; }

    public bool MatchSearchTerm(string searchTerm)
    {
        if (Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (Enum.TryParse<TypeId>(searchTerm, ignoreCase: true, out var type))
        {
            if (Type1 == type || Type2 == type)
            {
                return true;
            }
        }

        return false;
    }

    public void NotifyPropertyChanged(string? name)
    {
        switch (name)
        {
            case nameof(Name):
            case nameof(Type1):
            case nameof(Image):
                RaisePropertyChanged(name);
                break;
            case nameof(Type2):
                RaisePropertyChanged(name);
                RaisePropertyChanged(nameof(HasType2));
                break;
        }
    }
}
