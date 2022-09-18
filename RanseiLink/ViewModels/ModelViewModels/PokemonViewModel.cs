using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Graphics.Conquest;
using RanseiLink.Core.Models;
using RanseiLink.Core.Resources;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Xml.Linq;

namespace RanseiLink.ViewModels;

public class PokemonViewModel : ViewModelBase
{
    private Pokemon _model;
    private readonly List<SelectorComboBoxItem> _evolutionEntryOptions;
    private readonly IIdToNameService _idToNameService;
    private readonly IKingdomService _kingdomService;
    private readonly IItemService _itemService;
    private readonly IOverrideDataProvider _spriteProvider;
    private readonly IDialogService _dialogService;
    private PokemonId _id;
    private readonly SpriteItemViewModel.Factory _spriteItemVmFactory;
    public PokemonViewModel(IJumpService jumpService, IIdToNameService idToNameService, IKingdomService kingdomService, IItemService itemService, 
        IOverrideDataProvider spriteProvider, SpriteItemViewModel.Factory spriteItemVmFactory, IDialogService dialogService)
    {
        _spriteItemVmFactory = spriteItemVmFactory;
        _idToNameService = idToNameService;
        _kingdomService = kingdomService;
        _itemService = itemService;
        _spriteProvider = spriteProvider;
        _dialogService = dialogService;
        _model = new Pokemon();

        MoveItems = _idToNameService.GetComboBoxItemsExceptDefault<IMoveService>();
        AbilityItems = _idToNameService.GetComboBoxItemsPlusDefault<IAbilityService>();

        _evolutionEntryOptions = _idToNameService.GetComboBoxItemsExceptDefault<IPokemonService>();

        JumpToMoveCommand = new RelayCommand<int>(id => jumpService.JumpTo(MoveSelectorEditorModule.Id, id));
        JumpToAbilityCommand = new RelayCommand<int>(id => jumpService.JumpTo(AbilitySelectorEditorModule.Id, id));
        AddEvolutionCommand = new RelayCommand(AddEvolution);
        RemoveEvolutionCommand = new RelayCommand(RemoveEvolution);
        ViewSpritesCommand = new RelayCommand(ViewSprites);
        ImportAnimationCommand = new RelayCommand(ImportAnimation);
        ExportAnimationsCommand = new RelayCommand(ExportAnimations);
        RevertAnimationCommand = new RelayCommand(RevertAnimation);
    }

    public void SetModel(PokemonId id, Pokemon model)
    {
        _model = model;
        _id = id;
        Evolutions.Clear();
        for (int i = 0; i < _model.Evolutions.Count; i++)
        {
            var newItem = new EvolutionComboBoxItem(_model.Evolutions, i, _evolutionEntryOptions);
            Evolutions.Add(newItem);
        }
        HabitatItems.Clear();
        foreach (KingdomId kingdom in EnumUtil.GetValuesExceptDefaults<KingdomId>())
        {
            HabitatItems.Add(new HabitatItem(model, kingdom, _idToNameService.IdToName<IKingdomService>((int)kingdom)));
        }
        RaiseAllPropertiesChanged();
    }

    public ICommand ImportAnimationCommand { get; }
    public ICommand ImportRawAnimationCommand { get; }
    public ICommand RevertRawAnimationCommand { get; }
    public ICommand RevertAnimationCommand { get; }
    public ICommand ExportAnimationsCommand { get; }
    public ICommand JumpToMoveCommand { get; }
    public ICommand JumpToAbilityCommand { get; }

    public List<SelectorComboBoxItem> MoveItems { get; }
    public List<SelectorComboBoxItem> AbilityItems { get; }

    public string Name
    {
        get => _model.Name;
        set => RaiseAndSetIfChanged(_model.Name, value, v => _model.Name = v);
    }

    public TypeId Type1
    {
        get => _model.Type1;
        set => RaiseAndSetIfChanged(_model.Type1, value, v => _model.Type1 = v);
    }
    public TypeId Type2
    {
        get => _model.Type2;
        set => RaiseAndSetIfChanged(_model.Type2, value, v => _model.Type2 = v);
    }

    public int Move
    {
        get => (int)_model.Move;
        set => RaiseAndSetIfChanged(_model.Move, (MoveId)value, v => _model.Move = v);
    }

    public int Ability1
    {
        get => (int)_model.Ability1;
        set => RaiseAndSetIfChanged(_model.Ability1, (AbilityId)value, v => _model.Ability1 = v);
    }
    public int Ability2
    {
        get => (int)_model.Ability2;
        set => RaiseAndSetIfChanged(_model.Ability2, (AbilityId)value, v => _model.Ability2 = v);
    }
    public int Ability3
    {
        get => (int)_model.Ability3;
        set => RaiseAndSetIfChanged(_model.Ability3, (AbilityId)value, v => _model.Ability3 = v);
    }

    public int Hp
    {
        get => _model.Hp;
        set => RaiseAndSetIfChanged(_model.Hp, value, v => _model.Hp = v);
    }

    public int Atk
    {
        get => _model.Atk;
        set => RaiseAndSetIfChanged(_model.Atk, value, v => _model.Atk = v);
    }

    public int Def
    {
        get => _model.Def;
        set => RaiseAndSetIfChanged(_model.Def, value, v => _model.Def = v);
    }

    public int Spe
    {
        get => _model.Spe;
        set => RaiseAndSetIfChanged(_model.Spe, value, v => _model.Spe = v);
    }

    public bool IsLegendary
    {
        get => _model.IsLegendary;
        set => RaiseAndSetIfChanged(_model.IsLegendary, value, v => _model.IsLegendary = v);
    }

    public IdleMotionId IdleMotion
    {
        get => _model.IdleMotion;
        set => RaiseAndSetIfChanged(_model.IdleMotion, value, v => _model.IdleMotion = v);
    }

    public int NameOrderIndex
    {
        get => _model.NameOrderIndex;
        set => RaiseAndSetIfChanged(_model.NameOrderIndex, value, v => _model.NameOrderIndex = v);
    }

    public int NationalPokedexNumber
    {
        get => _model.NationalPokedexNumber;
        set => RaiseAndSetIfChanged(_model.NationalPokedexNumber, value, v => _model.NationalPokedexNumber = v);
    }

    public int MovementRange
    {
        get => _model.MovementRange;
        set => RaiseAndSetIfChanged(_model.MovementRange, value, v => _model.MovementRange = value);
    }

    public int CatchRate
    {
        get => _model.CatchRate;
        set => RaiseAndSetIfChanged(_model.CatchRate, value, v => _model.CatchRate = v);
    }

    public int UnknownAnimationValue
    {
        get => _model.UnknownAnimationValue;
        set => RaiseAndSetIfChanged(_model.UnknownAnimationValue, value, v => _model.UnknownAnimationValue = v);
    }

    public int UnknownValue2
    {
        get => _model.UnknownValue2;
        set => RaiseAndSetIfChanged(_model.UnknownValue2, value, v => _model.UnknownValue2 = v);
    }

    public bool AsymmetricBattleSprite
    {
        get => _model.AsymmetricBattleSprite;
        set => RaiseAndSetIfChanged(_model.AsymmetricBattleSprite, value, v => _model.AsymmetricBattleSprite = v);
    }

    public bool LongAttackAnimation
    {
        get => _model.LongAttackAnimation;
        set => RaiseAndSetIfChanged(_model.LongAttackAnimation, value, v => _model.LongAttackAnimation = v);
    }

    public EvolutionConditionId EvolutionCondition1
    {
        get => _model.EvolutionCondition1;
        set
        {
            if (RaiseAndSetIfChanged(_model.EvolutionCondition1, value, v => _model.EvolutionCondition1 = value))
            {
                RaisePropertyChanged(nameof(QuantityForEvolutionCondition1Name));
            }
        }
    }

    public int QuantityForEvolutionCondition1
    {
        get => _model.QuantityForEvolutionCondition1;
        set 
        {
            if (RaiseAndSetIfChanged(_model.QuantityForEvolutionCondition1, value, v => _model.QuantityForEvolutionCondition1 = value))
            {
                RaisePropertyChanged(nameof(QuantityForEvolutionCondition1Name));
            }
        }
    }

    public string QuantityForEvolutionCondition1Name => GetNameOfQuantityForEvolutionCondition(EvolutionCondition1, _model.QuantityForEvolutionCondition1);

    private string GetNameOfQuantityForEvolutionCondition(EvolutionConditionId id, int quantity)
    {
        switch (id)
        {
            case EvolutionConditionId.Kingdom:
                if (_kingdomService.ValidateId(quantity))
                {
                    return _kingdomService.IdToName(quantity);
                }
                return "Invalid";

            case EvolutionConditionId.WarriorGender:
                return $"{(GenderId)quantity}";

            case EvolutionConditionId.Item:
                if (_itemService.ValidateId(quantity))
                {
                    return _itemService.IdToName(quantity);
                }
                return "Invalid";
            default:
                return "";
        }
    }

    public EvolutionConditionId EvolutionCondition2
    {
        get => _model.EvolutionCondition2;
        set
        {
            if (RaiseAndSetIfChanged(_model.EvolutionCondition2, value, v => _model.EvolutionCondition2 = value))
            {
                RaisePropertyChanged(nameof(QuantityForEvolutionCondition2Name));
            }
        }
    }

    public int QuantityForEvolutionCondition2
    {
        get => _model.QuantityForEvolutionCondition2;
        set
        {
            if (RaiseAndSetIfChanged(_model.QuantityForEvolutionCondition2, value, v => _model.QuantityForEvolutionCondition2 = value))
            {
                RaisePropertyChanged(nameof(QuantityForEvolutionCondition2Name));
            }
        }
    }

    public string QuantityForEvolutionCondition2Name => GetNameOfQuantityForEvolutionCondition(EvolutionCondition2, _model.QuantityForEvolutionCondition2);

    public ObservableCollection<HabitatItem> HabitatItems { get; } = new();

    public class HabitatItem : ViewModelBase
    {
        private readonly KingdomId _kingdom;
        private readonly Pokemon _model;
        public string KingdomName { get; }

        public HabitatItem(Pokemon pokemon, KingdomId kingdom, string kingdomName)
        {
            _kingdom = kingdom;
            _model = pokemon;
            KingdomName = kingdomName;
        }

        public bool EncounterableAtDefaultArea
        {
            get => _model.GetEncounterable(_kingdom, false);
            set => RaiseAndSetIfChanged(EncounterableAtDefaultArea, value, v => _model.SetEncounterable(_kingdom, false, v));
        }

        public bool EncounterableWithLevel2Area
        {
            get => _model.GetEncounterable(_kingdom, true);
            set => RaiseAndSetIfChanged(EncounterableWithLevel2Area, value, v => _model.SetEncounterable(_kingdom, true, v));
        }
    }

    public class EvolutionComboBoxItem : ViewModelBase
    {
        private readonly List<PokemonId> _evolutionTable;
        private readonly int _id;
        public EvolutionComboBoxItem(List<PokemonId> evolutionTable, int id, List<SelectorComboBoxItem> options)
        {
            _id = id;
            _evolutionTable = evolutionTable;
            Options = options;
        }
        public int Id
        {
            get => (int)_evolutionTable[_id];
            set => RaiseAndSetIfChanged(Id, value, v => _evolutionTable[_id] = (PokemonId)v);
        }
        public List<SelectorComboBoxItem> Options { get; }
    }

    public ObservableCollection<EvolutionComboBoxItem> Evolutions { get; } = new();

    public ICommand AddEvolutionCommand { get; }
    public ICommand RemoveEvolutionCommand { get; }

    private void AddEvolution()
    {
        _model.Evolutions.Add(PokemonId.Eevee);
        var newItem = new EvolutionComboBoxItem(_model.Evolutions, _model.Evolutions.Count - 1, _evolutionEntryOptions);
        Evolutions.Add(newItem);
    }

    private void RemoveEvolution()
    {
        _model.Evolutions.RemoveAt(_model.Evolutions.Count - 1);
        Evolutions.RemoveAt(Evolutions.Count - 1);
    }

    public string SmallSpritePath => _spriteProvider.GetSpriteFile(SpriteType.StlPokemonM, (int)_id).File;

    public ICommand ViewSpritesCommand { get; }
    private void ViewSprites()
    {
        List<SpriteFile> sprites = new();
        int id = (int)_id;
        sprites.Add(_spriteProvider.GetSpriteFile(SpriteType.StlPokemonB, id));
        sprites.Add(_spriteProvider.GetSpriteFile(SpriteType.StlPokemonCI, id));
        sprites.Add(_spriteProvider.GetSpriteFile(SpriteType.StlPokemonL, id));
        sprites.Add(_spriteProvider.GetSpriteFile(SpriteType.StlPokemonM, id));
        sprites.Add(_spriteProvider.GetSpriteFile(SpriteType.StlPokemonS, id));
        sprites.Add(_spriteProvider.GetSpriteFile(SpriteType.StlPokemonSR, id));
        //sprites.Add(_spriteProvider.GetSpriteFile(SpriteType.StlPokemonWu, id));
        sprites.Add(_spriteProvider.GetSpriteFile(SpriteType.ModelPokemon, id));

        var dialog = new Dialogs.ImageListDialog(sprites, _spriteItemVmFactory) { Owner = System.Windows.Application.Current.MainWindow };
        dialog.ShowDialog();

        RaisePropertyChanged(nameof(SmallSpritePath));

    }

    private void ImportAnimation()
    {
        var proceed = _dialogService.RequestFile(
            "Select the raw pattern animation library file",
            ".xml",
            "Pattern Animation XML (.xml)|*.xml",
            out string result
            );

        if (!proceed)
        {
            return;
        }

        
        var temp1 = Path.GetTempFileName();
        var temp2 = Path.GetTempFileName();
        try
        {
            NSPAT nspat;
            NSPAT nspatRaw;

            var doc = XDocument.Load(result);
            if (doc.Root.Name != PatternGroupElementName)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    "File invalid",
                    $"Failed to load the document because it doesn't match what is expected for a pattern animation (found: {doc.Root.Name}, expected: {PatternGroupElementName})",
                    MessageBoxType.Warning
                    ));
                return;
            }
            var nonRawEl = doc.Root.Element(NSPAT.RootElementName);
            if (nonRawEl == null)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    "File invalid",
                    $"Failed to load the document because it doesn't match what is expected for a pattern animation (element not found: {NSPAT.RootElementName})",
                    MessageBoxType.Warning
                    ));
                return;
            }
            var rawEl = doc.Root.Element(NSPAT_RAW.RootElementName);
            if (nonRawEl == null)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    "File invalid",
                    $"Failed to load the document because it doesn't match what is expected for a pattern animation (element not found: {NSPAT_RAW.RootElementName})",
                    MessageBoxType.Warning
                    ));
                return;
            }
            nspat = NSPAT.Deserialize(nonRawEl);
            if (nspat == null)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    "File invalid",
                    $"Failed to load the document because it doesn't match what is expected for a pattern animation (failed to deserialize element: {NSPAT.RootElementName}",
                    MessageBoxType.Warning
                    ));
                return;
            }
            nspatRaw = NSPAT.Deserialize(rawEl);
            if (nspatRaw == null)
            {
                _dialogService.ShowMessageBox(MessageBoxArgs.Ok(
                    "File invalid",
                    $"Failed to load the document because it doesn't match what is expected for a pattern animation (failed to deserialize element: {NSPAT_RAW.RootElementName}",
                    MessageBoxType.Warning
                    ));
                return;
            }

            // raw
            NSPAT_RAW.WriteTo(nspat, temp1);
            var nspatRawFile = ResolveRelativeAnimPath(true);
            _spriteProvider.SetOverride(nspatRawFile, temp1);

            // non raw
            // need to make sure the natdex number is correct
            foreach (var anim in nspat.PatternAnimations)
            {
                var name = anim.Name;
                var end = name.Substring(name.Length - 5);
                var num = NationalPokedexNumber.ToString().PadLeft(3, '0');
                anim.Name = $"POKEMON{num}{end}";
            }
            new NSBTP { PatternAnimations = nspat }.WriteTo(temp2);
            var nsbtpFile = ResolveRelativeAnimPath(false);
            _spriteProvider.SetOverride(nsbtpFile, temp2);

            AsymmetricBattleSprite = bool.TryParse(doc.Root.Attribute(AsymmetricalAttributeName)?.Value, out var asv) && asv;
            LongAttackAnimation = bool.TryParse(doc.Root.Attribute(LongAttackAttributeName)?.Value, out var lav) && lav;
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessageBox(MessageBoxArgs.Ok("Unable to import file due to error", ex.ToString(), MessageBoxType.Warning));
        }
        finally
        {
            File.Delete(temp1);
            File.Delete(temp2);
            // update ui
            RaisePropertyChanged(nameof(IsAnimationOverwritten));
        }

        return;

    }

    public bool IsAnimationOverwritten => _spriteProvider.GetDataFile(ResolveRelativeAnimPath(false)).IsOverride || _spriteProvider.GetDataFile(ResolveRelativeAnimPath(true)).IsOverride;
 
    private void RevertAnimation()
    {
        string nonRawFile = ResolveRelativeAnimPath(false);
        _spriteProvider.ClearOverride(nonRawFile);

        var rawFile = ResolveRelativeAnimPath(true);
        _spriteProvider.ClearOverride(rawFile);

        RaisePropertyChanged(nameof(IsAnimationOverwritten));
    }

    private string ResolveRelativeAnimPath(bool raw)
    {
        var info = (PkmdlConstants)GraphicsInfoResource.Get(SpriteType.ModelPokemon);
        var pacLinkRelative = info.PACLinkFolder;
        string fileName = ((int)_id).ToString().PadLeft(4, '0');
        string pacUnpackedFolder = Path.Combine(pacLinkRelative, fileName + "-Unpacked");
        return Path.Combine(pacUnpackedFolder, raw ? "0002" : "0001");
    }

    private void ExportAnimations()
    {
        if (!_spriteProvider.IsDefaultsPopulated())
        {
            return;
        }

        var proceed = _dialogService.RequestFolder(
            "Select folder to export animation file to",
            out string destFolder
            );

        if (!proceed)
        {
            return;
        }

        var dest = FileUtil.MakeUniquePath(Path.Combine(destFolder, $"{(int)_id}_{Name}_NSPAT.xml"));
        var file = _spriteProvider.GetDataFile(ResolveRelativeAnimPath(false));
        var rawfile = _spriteProvider.GetDataFile(ResolveRelativeAnimPath(true));

        var nsbtp = new NSBTP(file.File);
        var nspat = NSPAT_RAW.Load(rawfile.File);
        var doc = new XDocument(new XElement(PatternGroupElementName,
            new XAttribute(LongAttackAttributeName, LongAttackAnimation),
            new XAttribute(AsymmetricalAttributeName, AsymmetricBattleSprite),
            nsbtp.PatternAnimations.Serialize(),
            nspat.SerializeRaw()
            ));
        doc.Save(dest);
    }

    private const string LongAttackAttributeName = "long_attack";
    private const string AsymmetricalAttributeName = "asymmetrical";
    private const string PatternGroupElementName = "library_collection";
}

