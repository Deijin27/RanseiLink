using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public partial class BuildingViewModel : ViewModelBase
{
    public delegate BuildingViewModel Factory();

    private readonly BuildingWorkspaceViewModel _parent;
    private readonly IKingdomService _kingdomService;
    private readonly ICachedSpriteProvider _cachedSpriteProvider;

    public Building Model => _model;

    public BuildingViewModel(ScenarioBuildingViewModel sbvm, BuildingWorkspaceViewModel parent, IKingdomService kingdomService, ICachedSpriteProvider cachedSpriteProvider, BuildingId id, Building model)
    {
        ScenarioBuildingVm = sbvm;   
        _parent = parent;
        _kingdomService = kingdomService;
        _cachedSpriteProvider = cachedSpriteProvider;
        _id = id;
        _model = model;
        BuildingItems = parent.BuildingItems;
        KingdomItems = parent.KingdomItems;

        this.PropertyChanged += BuildingViewModel_PropertyChanged;
    }

    private void BuildingViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Sprite1):
                RaisePropertyChanged(nameof(Sprite1Image));
                break;
            case nameof(Sprite2):
                RaisePropertyChanged(nameof(Sprite2Image));
                break;
            case nameof(Sprite3):
                RaisePropertyChanged(nameof(Sprite3Image));
                break;
            case nameof(Kingdom):
                RaisePropertyChanged(nameof(KingdomName));
                break;
        }
    }

    public ScenarioBuildingViewModel ScenarioBuildingVm { get; }

    public int Slot { get; set; }

    public string KingdomName => _kingdomService.IdToName(Kingdom);
    public object? Sprite1Image => _cachedSpriteProvider.GetSprite(Core.Services.SpriteType.IconInstS, (int)Sprite1);
    public object? Sprite2Image => _cachedSpriteProvider.GetSprite(Core.Services.SpriteType.IconInstS, (int)Sprite2);
    public object? Sprite3Image => _cachedSpriteProvider.GetSprite(Core.Services.SpriteType.IconInstS, (int)Sprite3);

    public ICommand JumpToBattleConfigCommand => _parent.JumpToBattleConfigCommand;

    public ICommand SelectCommand => _parent.ItemClickedCommand;
}
