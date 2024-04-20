#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public class WarriorMiniViewModel : ViewModelBase
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