using FluentAssertions;
using Moq;
using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Services;
using RanseiLink.Services;
using RanseiLink.ViewModels;
using Xunit;

namespace RanseiLink.Tests.ViewModelTests;
public class MainWindowViewModelTests
{
    private readonly MainWindowViewModel _mainWindowVm;
    private readonly Mock<IModSelectionViewModel> _mockModSelectionVm;
    private readonly Mock<IMainEditorViewModel> _mockMainEditorVm;
    private readonly Mock<IThemeService> _mockThemeService;
    private readonly Mock<IFallbackSpriteManager> _mockFallbackSpriteManager;

    public MainWindowViewModelTests()
    {
        _mockModSelectionVm = new Mock<IModSelectionViewModel>();
        _mockMainEditorVm = new Mock<IMainEditorViewModel>();
        _mockThemeService = new Mock<IThemeService>();
        _mockFallbackSpriteManager = new Mock<IFallbackSpriteManager>();

        _mainWindowVm = new MainWindowViewModel(
            new Mock<IDialogService>().Object,
            new Mock<IPluginLoader>().Object,
            _mockThemeService.Object,
            _mockModSelectionVm.Object,
            _mockMainEditorVm.Object,
            _mockFallbackSpriteManager.Object
            );
    }

    [Fact]
    public void ShouldStartupOnModSelectionPage()
    {
        _mainWindowVm.CurrentVm.Should().Be(_mockModSelectionVm.Object);
        _mainWindowVm.BackButtonVisible.Should().BeFalse();
    }

    [Fact]
    public void SelectingModShouldLoadMainEditorWithMod()
    {
        var testModInfo = new ModInfo();
        _mockModSelectionVm.Raise(i => i.ModSelected += null, testModInfo);

        _mainWindowVm.CurrentVm.Should().Be(_mockMainEditorVm.Object);
        _mockMainEditorVm.Verify(i => i.SetMod(testModInfo), Times.Once());
        _mainWindowVm.BackButtonVisible.Should().BeTrue();
    }

    [Fact]
    public void NavigatingBackShouldReturnToModSelectionPage()
    {
        SelectingModShouldLoadMainEditorWithMod();

        _mainWindowVm.BackButtonCommand?.Execute(null);

        _mockMainEditorVm.Verify(i => i.Deactivate(), Times.Once());

        _mainWindowVm.CurrentVm.Should().Be(_mockModSelectionVm.Object);
        _mainWindowVm.BackButtonVisible.Should().BeFalse();

    }

    [Fact]
    public void ShutdownShouldNotDeactivateMainEditorIfNotSelected()
    {
        _mainWindowVm.OnShutdown();
        _mockMainEditorVm.Verify(i => i.Deactivate(), Times.Never());
    }

    [Fact]
    public void ShutdownShouldDeactivateMainEditorIfSelected()
    {
        SelectingModShouldLoadMainEditorWithMod();
        _mainWindowVm.OnShutdown();
        _mockMainEditorVm.Verify(i => i.Deactivate(), Times.Once());

    }

    [Fact]
    public void CanToggleTheme()
    {
        _mockThemeService.Setup(i => i.CurrentTheme).Returns(Theme.Light);
        _mainWindowVm.ToggleThemeCommand?.Execute(null);
        _mockThemeService.Verify(i => i.SetTheme(Theme.Dark), Times.Once());

        _mockThemeService.Setup(i => i.CurrentTheme).Returns(Theme.Dark);
        _mainWindowVm.ToggleThemeCommand?.Execute(null);
        _mockThemeService.Verify(i => i.SetTheme(Theme.Light), Times.Once());
    }
}
