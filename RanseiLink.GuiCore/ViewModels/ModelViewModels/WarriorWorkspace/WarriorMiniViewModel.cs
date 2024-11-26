#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Text.RegularExpressions;

namespace RanseiLink.GuiCore.ViewModels;

public class WarriorMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly ICachedSpriteProvider _spriteProvider;
    private readonly IBaseWarriorService _baseWarriorService;
    private readonly BaseWarrior _model;
    private readonly int _id;

    public WarriorMiniViewModel(
        ICachedSpriteProvider spriteProvider, 
        IBaseWarriorService baseWarriorService, 
        BaseWarrior model, 
        int id, 
        ICommand selectCommand)
    {
        _spriteProvider = spriteProvider;
        _baseWarriorService = baseWarriorService;
        _model = model;
        _id = id;
        SelectCommand = selectCommand;
    }

    public int Id => _id;

    public string Name
    {
        get => _baseWarriorService.IdToName(_id);
    }

    public TypeId Type1
    {
        get => _model.Speciality1;
    }
    public TypeId Type2
    {
        get => _model.Speciality2;
    }

    public bool HasType2 => Type2 != TypeId.NoType;

    public object? Image => _spriteProvider.GetSprite(SpriteType.StlBushouS, _model.Sprite);

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