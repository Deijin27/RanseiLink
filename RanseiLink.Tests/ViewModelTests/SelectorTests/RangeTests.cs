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
    public RangeTests()
    {
        dataService = new DataService(new ModInfo() { FolderPath = TestConstants.TestModFolder });
        dialogService = new MockDialogService();
    }

    [Fact]
    public void AbilitySelector()
    {
        var selector = new AbilitySelectorViewModel(dialogService, 0, dataService.Ability);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void BaseWarriorSelector()
    {
        var selector = new BaseWarriorSelectorViewModel(dialogService, 0, dataService.BaseWarrior);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void EventSpeakerSelector()
    {
        var selector = new EventSpeakerSelectorViewModel(dialogService, 0, dataService.EventSpeaker);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void MoveRangeSelector()
    {
        var selector = new MoveRangeSelectorViewModel(dialogService, 0, dataService.MoveRange);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void MoveSelector()
    {
        var selector = new MoveSelectorViewModel(dialogService, 0, dataService.Move);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void PokemonSelector()
    {
        var selector = new PokemonSelectorViewModel(dialogService, 0, dataService.Pokemon);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void ScenarioAppearPokemonSelector()
    {
        var selector = new ScenarioAppearPokemonSelectorViewModel(dialogService, 0, dataService.ScenarioAppearPokemon);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void ScenarioKingdomSelector()
    {
        var selector = new ScenarioKingdomSelectorViewModel(dialogService, 0, dataService.ScenarioKingdom);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void WarriorMaxLinkSelector()
    {
        var selector = new MaxLinkSelectorViewModel(dialogService, 0, dataService.MaxLink);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }

    [Fact]
    public void WarriorSkillSelector()
    {
        var selector = new WarriorSkillSelectorViewModel(dialogService, 0, dataService.WarriorSkill);

        selector.Selected = selector.Items[^1];
        selector.Selected = selector.Items[2];

        Assert.Equal(0, dialogService.ShowMessageBoxCallCount);
    }
}
