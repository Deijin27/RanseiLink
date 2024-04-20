#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.ViewModels;

public class PokemonMiniViewModel : ViewModelBase
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
        set => Set(_model.Name, value, v => _model.Name = v);
    }

    public TypeId Type1
    {
        get => _model.Type1;
        set => Set(_model.Type1, value, v => _model.Type1 = v);
    }
    public TypeId Type2
    {
        get => _model.Type2;
        set
        {
            if (Set(_model.Type2, value, v => _model.Type2 = v))
            {
                Notify(nameof(HasType2));
            }
        }
    }

    public bool HasType2 => Type2 != TypeId.NoType;

    public object? Image => _spriteProvider.GetSprite(SpriteType.StlPokemonS, _id);

    public ICommand SelectCommand { get; }

    public void NotifyPropertyChanged(string? name)
    {
        switch (name)
        {
            case nameof(Name):
            case nameof(Type1):
            case nameof(Image):
                Notify(name);
                break;
            case nameof(Type2):
                Notify(name);
                Notify(nameof(HasType2));
                break;
        }
    }
}
