using RanseiLink.Core.Models;
using RanseiLink.Core.Util;

namespace RanseiLink.GuiCore.ViewModels;

public class ItemMiniViewModel : ViewModelBase, IMiniViewModel
{
    private readonly ICachedSpriteProvider _cachedSpriteProvider;
    private readonly Item _model;
    private readonly int _id;

    public ItemMiniViewModel(ICachedSpriteProvider cachedSpriteProvider,
        Item model, int id, ICommand selectCommand)
    {
        _cachedSpriteProvider = cachedSpriteProvider;
        _model = model;
        _id = id;
        SelectCommand = selectCommand;
        UpdateImage();
    }

    private void UpdateImage()
    {
        Image = _cachedSpriteProvider.GetItemCategory(_model.Category);
    }

    public int Id => _id;

    public string Name => _model.Name;

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
            if (name == nameof(ItemViewModel.Name))
            {
                RaisePropertyChanged(nameof(Name));
            }
            else if (name == nameof(ItemViewModel.Category))
            {
                UpdateImage();
            }
        }
    }
}
