#nullable enable
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Util;

namespace RanseiLink.GuiCore.ViewModels;

public class MoveRangeMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private readonly MoveRange _model;
    private readonly int _id;
    private readonly string _name;

    public MoveRangeMiniViewModel(ICachedSpriteProvider cachedSpriteProvider,
        MoveRange model, int id, string name, ICommand selectCommand)
    {
        _cachedSpriteProvider = cachedSpriteProvider;
        _model = model;
        _id = id;
        _name = name;
        SelectCommand = selectCommand;
        UpdateImage();
    }

    private void UpdateImage()
    {
        Image = _cachedSpriteProvider.GetMoveRangePreview(_model);
    }

    public int Id => _id;

    public string Name => _name;

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
            if (name.StartsWith("Row"))
            {
                UpdateImage();
            }
        }
    }
}
