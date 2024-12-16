#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Util;
using RanseiLink.GuiCore.Services.Concrete;

namespace RanseiLink.GuiCore.ViewModels;

public class MoveMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private readonly Move _model;
    private readonly int _id;

    public MoveMiniViewModel(ICachedSpriteProvider cachedSpriteProvider,
        Move model, int id, ICommand selectCommand)
    {
        _cachedSpriteProvider = cachedSpriteProvider;
        _model = model;
        _id = id;
        SelectCommand = selectCommand;
        UpdateImage();
    }

    private void UpdateImage()
    {
        Image = _cachedSpriteProvider.GetMovePreview(_model, 0);
    }

    public int Id => _id;

    public string Name => _model.Name;

    public TypeId Type => _model.Type;

    public int Power => _model.Power;

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

        if (Enum.TryParse<TypeId>(searchTerm, ignoreCase: true, out var type))
        {
            if (Type == type)
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
            case nameof(Type):
            case nameof(Power):
                RaisePropertyChanged(name);
                break;
            case nameof(MoveViewModel.PreviewImage):
                UpdateImage();
                break;
        }
    }
}
