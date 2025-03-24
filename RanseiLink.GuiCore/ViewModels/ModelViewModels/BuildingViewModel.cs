using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.GuiCore.ViewModels;

public partial class BuildingViewModel : ViewModelBase, IBigViewModel
{
    private readonly IBuildingService _buildingService;
    private readonly IKingdomService _kingdomService;
    private readonly ICachedSpriteProvider _cachedSpriteProvider;

    public Building Model => _model;

    public BuildingViewModel(INicknameService nicknameService, IBuildingService buildingService, IJumpService jumpService, IIdToNameService idToNameService, IScenarioBuildingService scenarioBuildingService, IKingdomService kingdomService, ICachedSpriteProvider cachedSpriteProvider)
    {
        ScenarioBuildingVm = new ScenarioBuildingViewModel(scenarioBuildingService);
        _buildingService = buildingService;
        _kingdomService = kingdomService;
        _cachedSpriteProvider = cachedSpriteProvider;
        BuildingItems = idToNameService.GetComboBoxItemsPlusDefault<IBuildingService>();
        KingdomItems = idToNameService.GetComboBoxItemsPlusDefault<IKingdomService>();
        BattleConfigItems = nicknameService.GetAllNicknames(nameof(BattleConfigId));
        JumpToBattleConfigCommand = new RelayCommand<int>(id => jumpService.JumpTo(BattleConfigSelectorEditorModule.Id, id));

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

    public void SetModel(int id, object model)
    {
        _id = (BuildingId)id;
        _model = (Building)model;

        // Find slot by looking through ones with matching kingdom in order
        int slot = 0;
        foreach (var item in _buildingService.Enumerate())
        {
            if (item == _model)
            {
                break;
            }
            else if (item.Kingdom == _model.Kingdom)
            {
                slot++;
            }
        }

        ScenarioBuildingVm.SetSelected(_model.Kingdom, slot);

        RaiseAllPropertiesChanged();
    }

    public ScenarioBuildingViewModel ScenarioBuildingVm { get; }

    public string KingdomName => _kingdomService.IdToName(Kingdom);
    public object? Sprite1Image => _cachedSpriteProvider.GetSprite(Core.Services.SpriteType.IconInstS, Sprite1);
    public object? Sprite2Image => _cachedSpriteProvider.GetSprite(Core.Services.SpriteType.IconInstS, Sprite2);
    public object? Sprite3Image => _cachedSpriteProvider.GetSprite(Core.Services.SpriteType.IconInstS, Sprite3);

    public ICommand JumpToBattleConfigCommand { get; }

    #region Animation

    public AnimationViewModel? IconAnimVm { get; private set; }


    private int _selectedAnimation;
    public int SelectedAnimation
    {
        get => _selectedAnimation;
        set
        {
            if (SetProperty(_selectedAnimation, value, v => _selectedAnimation = v))
            {
                RaisePropertyChanged(nameof(SelectedAnimationImage));
                IconAnimVm?.OnIdChanged();
            }
        }
    }

    public object? SelectedAnimationImage => _cachedSpriteProvider.GetSprite(SpriteType.IconInstS, SelectedAnimation);

    #endregion
}
