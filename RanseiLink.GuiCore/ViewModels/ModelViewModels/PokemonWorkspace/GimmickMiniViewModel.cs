#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.ViewModels;

public class GimmickMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly ICachedSpriteProvider _spriteProvider;
    private readonly Gimmick _model;
    private readonly int _id;

    public GimmickMiniViewModel(ICachedSpriteProvider spriteProvider, Gimmick model, int id, ICommand selectCommand)
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
        get => _model.AttackType;
        set => SetProperty(_model.AttackType, value, v => _model.AttackType = v);
    }
    public TypeId Type2
    {
        get => _model.DestroyType;
        set
        {
            if (SetProperty(_model.DestroyType, value, v => _model.DestroyType = v))
            {
                RaisePropertyChanged(nameof(HasType2));
            }
        }
    }

    public bool HasType2 => Type2 != TypeId.NoType;

    public object? Image => _spriteProvider.GetSprite(SpriteType.StlStageObje, _model.Image1);

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
            case nameof(_model.Name):
                RaisePropertyChanged(nameof(Name));
                break;
            case nameof(_model.Image1):
                RaisePropertyChanged(nameof(Image));
                break;
            case nameof(_model.AttackType):
                RaisePropertyChanged(nameof(Type1));
                break;
            case nameof(_model.DestroyType):
                RaisePropertyChanged(nameof(Type2));
                RaisePropertyChanged(nameof(HasType2));
                break;
        }
    }
}
