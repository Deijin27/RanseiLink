using RanseiLink.Core;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.Concrete;
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
        _container.RegisterSingleton<IDialogService>(dialogService);
    }

    [Fact]
    public void AbilitySelector()
    {
        var selector = new AbilitySelectorViewModel(_container, 0, dataService.Ability);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void BaseWarriorSelector()
    {
        var selector = new BaseWarriorSelectorViewModel(_container, 0, dataService.BaseWarrior);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void EventSpeakerSelector()
    {
        var selector = new EventSpeakerSelectorViewModel(_container, 0, dataService.EventSpeaker);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void MoveRangeSelector()
    {
        var selector = new MoveRangeSelectorViewModel(_container, 0, dataService.MoveRange);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void MoveSelector()
    {
        var selector = new MoveSelectorViewModel(_container, 0, dataService.Move);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void PokemonSelector()
    {
        var selector = new PokemonSelectorViewModel(_container, 0, dataService.Pokemon);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void ScenarioAppearPokemonSelector()
    {
        var selector = new ScenarioAppearPokemonSelectorViewModel(_container, 0, dataService.ScenarioAppearPokemon);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void ScenarioKingdomSelector()
    {
        var selector = new ScenarioKingdomSelectorViewModel(_container, 0, dataService.ScenarioKingdom);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void WarriorMaxLinkSelector()
    {
        var selector = new MaxLinkSelectorViewModel(_container, 0, dataService.MaxLink);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void WarriorSkillSelector()
    {
        var selector = new WarriorSkillSelectorViewModel(_container, 0, dataService.WarriorSkill);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }
}
