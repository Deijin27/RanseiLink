#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Util;

namespace RanseiLink.GuiCore.ViewModels;

public class WarriorSkillMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly ICachedSpriteProvider _spriteProvider;
    private readonly IBaseWarriorService _baseWarriorService;
    private readonly WarriorSkill _model;
    private readonly int _id;

    public WarriorSkillMiniViewModel(ICachedSpriteProvider spriteProvider, IBaseWarriorService baseWarriorService, WarriorSkill model, int id, ICommand selectCommand)
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
        get => _model.Name;
    }

    public object? Image
    {
        get
        {
            var idEnum = (WarriorSkillId)_id;

            foreach (var warrior in _baseWarriorService.Enumerate())
            {
                if (warrior.Skill == idEnum)
                {
                    return _spriteProvider.GetSprite(SpriteType.StlBushouS, warrior.Sprite);
                }
            }
            return null;
        }
    }

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
                RaisePropertyChanged(name);
                break;
        }
    }
}
