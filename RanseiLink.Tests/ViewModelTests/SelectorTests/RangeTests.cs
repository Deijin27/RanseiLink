using RanseiLink.Core;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;
using RanseiLink.Core.Services.Registration;
using RanseiLink.PluginModule.Services.Registration;
using RanseiLink.Services;
using RanseiLink.Tests.Mocks;
using RanseiLink.ViewModels;
using Xunit;

namespace RanseiLink.Tests.ViewModelTests.SelectorTests;

/// <summary>
/// If you add a default ID to an enum, you have to make sure to use <see cref="EnumUtil.GetValuesExceptDefaults{T}"/> rather than <see cref="EnumUtil.GetValues{T}"/>
/// when reading data from files where the defaults aren't assigned a section of data. These tests help spot when you forget to do this.
/// If I add more rigorous testing for selectors, these will probably not be necessary anymore as they will be covered there.
/// </summary>
public class RangeTests
{
    private readonly MockDialogService dialogService;
    private readonly IDataService dataService;
    private readonly IServiceContainer _container;
    public RangeTests()
    {
        dataService = new DataService(new ModInfo() { FolderPath = TestConstants.TestModFolder });
        dialogService = new MockDialogService();
        _container = new ServiceContainer();
        _container.RegisterCoreServices();
        _container.RegisterPluginServices();
        _container.RegisterWpfServices();
        _container.RegisterSingleton<IDialogService>(dialogService);
    }

    [Fact]
    public void AbilitySelector()
    {
        var selector = new AbilitySelectorViewModel(_container, dataService.Ability);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void BaseWarriorSelector()
    {
        var selector = new BaseWarriorSelectorViewModel(_container, dataService.BaseWarrior);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void EventSpeakerSelector()
    {
        var selector = new EventSpeakerSelectorViewModel(_container, dataService.EventSpeaker);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void MoveRangeSelector()
    {
        var selector = new MoveRangeSelectorViewModel(_container, dataService.MoveRange);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void MoveSelector()
    {
        var selector = new MoveSelectorViewModel(_container, dataService.Move);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void PokemonSelector()
    {
        var selector = new PokemonSelectorViewModel(_container, dataService.Pokemon);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void ScenarioAppearPokemonSelector()
    {
        var selector = new ScenarioAppearPokemonSelectorViewModel(_container, dataService.ScenarioAppearPokemon);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void ScenarioKingdomSelector()
    {
        var selector = new ScenarioKingdomSelectorViewModel(_container, dataService.ScenarioKingdom);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void WarriorMaxLinkSelector()
    {
        var selector = new MaxLinkSelectorViewModel(_container, dataService.MaxLink);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void WarriorSkillSelector()
    {
        var selector = new WarriorSkillSelectorViewModel(_container, dataService.WarriorSkill);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void ItemSelector()
    {
        var selector = new ItemSelectorViewModel(_container, dataService.Item);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void BuildingSelector()
    {
        var selector = new BuildingSelectorViewModel(_container, dataService.Building);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }
}
