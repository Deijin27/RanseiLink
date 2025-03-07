#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Util;

namespace RanseiLink.GuiCore.ViewModels;

public class MoveRangeMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly string _nicknameCategory;
    private readonly INicknameService _nicknameService;
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private readonly MoveRange _model;
    private readonly int _id;

    public MoveRangeMiniViewModel(string nicknameCategory, INicknameService nicknameService, ICachedSpriteProvider cachedSpriteProvider,
        MoveRange model, int id, ICommand selectCommand)
    {
        _nicknameCategory = nicknameCategory;
        _nicknameService = nicknameService;
        _cachedSpriteProvider = cachedSpriteProvider;
        _model = model;
        _id = id;
        SelectCommand = selectCommand;
        UpdateImage();
    }

    private void UpdateImage()
    {
        Image = _cachedSpriteProvider.GetMoveRangePreview(_model);
    }

    public int Id => _id;

    public string Name => _nicknameService.GetNickname(_nicknameCategory, _id);

    private object? _image;
    public object? Image
    {
        get => _image;
        set => SetProperty(ref _image, value);
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
        if (!string.IsNullOrEmpty(name))
        {
            if (name == nameof(MoveRangeViewModel.Nickname))
            {
                RaisePropertyChanged(nameof(Name));
            }
            else if (name.StartsWith("Row"))
            {
                UpdateImage();
            }
        }
    }
}
