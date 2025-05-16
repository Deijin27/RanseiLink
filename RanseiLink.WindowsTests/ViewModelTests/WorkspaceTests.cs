using RanseiLink.Core.Services.ModelServices;
using RanseiLink.Core.Services;
using RanseiLink.GuiCore.ViewModels;
using RanseiLink.Windows.Tests;
using System.Linq;
using System.Collections.Generic;

namespace RanseiLink.WindowsTests.ViewModelTests;

public class MockBigViewModel : ViewModelBase, IBigViewModel
{
    public int Id { get; private set; }
    public object Model { get; private set; }

    public void SetModel(int id, object model)
    {
        Id = id;
        Model = model;
    }
}

public class WorkspaceTests
{
    private readonly MockBigViewModel _bigViewModel;
    private readonly WorkspaceViewModel _workspaceViewModel;
    private readonly IPokemonService _service;

    public WorkspaceTests()
    {
        _service = new PokemonService(new ModInfo() { FolderPath = TestConstants.TestModFolder });
        var clipboardService = new Mock<IClipboardService>();
        var asyncDialogService = new Mock<IAsyncDialogService>();
        var cachedSpriteProvider = new Mock<ICachedSpriteProvider>();
        var factory = new SelectorViewModelFactory(clipboardService.Object, asyncDialogService.Object);
        _bigViewModel = new MockBigViewModel();

        _workspaceViewModel = factory.CreateWorkspace(
            _bigViewModel,
            _service,
            command => _service.ValidIds().Select<int, IMiniViewModel>(id =>
                new PokemonMiniViewModel(cachedSpriteProvider.Object, _service.Retrieve(id), id, command)).ToList()
            );
    }

    [Fact]
    public void MiniViewModelsShouldHaveData()
    {
        var eevee = _workspaceViewModel.Items[0].Should().BeOfType<PokemonMiniViewModel>().Which;
        eevee.Name.Should().Be("Eevee");
        eevee.Type1.Should().Be(Core.Enums.TypeId.Normal);
        eevee.Type2.Should().Be(Core.Enums.TypeId.NoType);

        var rayquaza = _workspaceViewModel.Items[199].Should().BeOfType<PokemonMiniViewModel>().Which;
        rayquaza.Name.Should().Be("Rayquaza");
        rayquaza.Type1.Should().Be(Core.Enums.TypeId.Dragon);
        rayquaza.Type2.Should().Be(Core.Enums.TypeId.Flying);
    }

    [Fact]
    public void ClickShouldRequestNavigate()
    {
        List<int> selected = [];
        _workspaceViewModel.RequestNavigateToId += (sender, id) => { selected.Add(id); };

        var item = _workspaceViewModel.Items[15];
        item.SelectCommand.Execute(item);

        selected.Should().BeEquivalentTo([15]);
    }

    [Fact]
    public void SelectShouldChangeSelection()
    {
        _workspaceViewModel.SelectedId.Should().Be(0);
        _bigViewModel.Id.Should().Be(0);
        _bigViewModel.Model.Should().Be(_service.Retrieve(0));

        _workspaceViewModel.SelectById(15);

        _workspaceViewModel.SelectedId.Should().Be(15);
        _bigViewModel.Id.Should().Be(15);
        _bigViewModel.Model.Should().Be(_service.Retrieve(15));
    }

    [Fact]
    public void ShouldForwardChangeNotifications()
    {
        var eevee = _workspaceViewModel.Items[0];
        List<string> propertiesChanged = [];
        eevee.PropertyChanged += (sender, e) => { propertiesChanged.Add(e.PropertyName); };

        _bigViewModel.RaisePropertyChanged(nameof(PokemonViewModel.Name));
        propertiesChanged.Should().BeEquivalentTo([nameof(PokemonMiniViewModel.Name)]);
        propertiesChanged.Clear();

        _bigViewModel.RaisePropertyChanged(nameof(PokemonViewModel.Type1));
        propertiesChanged.Should().BeEquivalentTo([nameof(PokemonMiniViewModel.Type1)]);
        propertiesChanged.Clear();

        _bigViewModel.RaisePropertyChanged(nameof(PokemonViewModel.Type2));
        propertiesChanged.Should().BeEquivalentTo([nameof(PokemonMiniViewModel.Type2), nameof(PokemonMiniViewModel.HasType2)]);
        propertiesChanged.Clear();
    }

    [Fact]
    public void SearchFiltersItems()
    {
        _workspaceViewModel.SearchText.Should().BeNullOrEmpty();
        _workspaceViewModel.Items.Should().HaveCount(200);
        _workspaceViewModel.Items[0].Id.Should().Be(0); // eevee
        _workspaceViewModel.Items[199].Id.Should().Be(199); // rayquaza

        _workspaceViewModel.SearchText = "bat";
        _workspaceViewModel.Items.Should().HaveCount(3);
        _workspaceViewModel.Items[0].Id.Should().Be(22); // zubat
        _workspaceViewModel.Items[1].Id.Should().Be(23); // golbat
        _workspaceViewModel.Items[2].Id.Should().Be(24); // crobat

        _workspaceViewModel.SearchText = "";
        _workspaceViewModel.Items.Should().HaveCount(200);
        _workspaceViewModel.Items[0].Id.Should().Be(0); // eevee
        _workspaceViewModel.Items[199].Id.Should().Be(199); // rayquaza
    }

    [Fact]
    public void FilterToSingleItemShouldRequestNavigate()
    {
        List<int> selected = [];
        _workspaceViewModel.RequestNavigateToId += (sender, id) => { selected.Add(id); };

        _workspaceViewModel.SelectedId.Should().Be(0);
        _workspaceViewModel.SearchText = "Pikachu";
        _workspaceViewModel.Items.Should().HaveCount(1);
        selected.Should().BeEquivalentTo([15]);
    }
}
