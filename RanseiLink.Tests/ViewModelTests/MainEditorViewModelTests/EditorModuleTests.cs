using Moq;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.PluginModule.Services;
using RanseiLink.Settings;
using RanseiLink.ViewModels;
using Xunit;

namespace RanseiLink.Tests.ViewModelTests.MainEditorViewModelTests;

public class EditorModuleTests
{
    private static Mock<EditorModule> SetupTestModule(string name)
    {
        var mock = new Mock<EditorModule>();
        mock.Setup(i => i.UniqueId).Returns($"test_module_{name}");
        mock.Setup(i => i.ListName).Returns($"{name} Module");
        mock.Setup(i => i.ViewModel).Returns($"module_{name}_view_model");
        return mock;
    }

    private readonly MainEditorViewModel _mainEditorVm;
    private readonly Mock<EditorModule> _moduleA;
    private readonly Mock<EditorModule> _moduleB;
    private readonly Mock<EditorModule> _moduleC;
    private readonly Mock<EditorModule> _moduleD;
    public EditorModuleTests()
    {
        var editorModuleOrderSetting = new EditorModuleOrderSetting()
        {
            Value = new string[] { "test_module_b", "test_module_c", "test_module_a" }
        };
        var settingService = new Mock<ISettingService>();
        settingService.Setup(i => i.Get<EditorModuleOrderSetting>()).Returns(editorModuleOrderSetting);

        _moduleA = SetupTestModule("a");
        _moduleB = SetupTestModule("b");
        _moduleC = SetupTestModule("c");
        _moduleD = SetupTestModule("d");

        _mainEditorVm = new MainEditorViewModel(
            new Mock<IDialogService>().Object,
            new Mock<IModManager>().Object,
            settingService.Object,
            new Mock<IFallbackSpriteProvider>().Object,
            new Mock<IPluginLoader>().Object,
            new Mock<IModServiceGetterFactory>().Object,
            new EditorModule[] { _moduleA.Object, _moduleB.Object, _moduleC.Object, _moduleD.Object }
            );
    }

    [Fact]
    public void LoadModuleOrderCorrectly()
    {
        Assert.Equal(4, _mainEditorVm.ListItems.Count);
        Assert.Equal("test_module_b", _mainEditorVm.ListItems[0].ModuleId);
        Assert.Equal("test_module_c", _mainEditorVm.ListItems[1].ModuleId);
        Assert.Equal("test_module_a", _mainEditorVm.ListItems[2].ModuleId);
        Assert.Equal("test_module_d", _mainEditorVm.ListItems[3].ModuleId); // not in setting are put at end of list
    }

    [Fact]
    public void ChangeModuleShouldLoadViewModel()
    {
        _mainEditorVm.CurrentModuleId = "test_module_c";
        Assert.Equal("module_c_view_model", _mainEditorVm.CurrentVm);
        _mainEditorVm.CurrentModuleId = "test_module_d";
        Assert.Equal("module_d_view_model", _mainEditorVm.CurrentVm);
    }
    
    [Fact]
    public void ChangeModuleShouldInvokePageChange()
    {
        _moduleA.Verify(i => i.OnPageOpening(), Times.Never());
        _mainEditorVm.CurrentModuleId = "test_module_a";
        _moduleA.Verify(i => i.OnPageOpening(), Times.Once());
        _moduleB.Verify(i => i.OnPageOpening(), Times.Never());
        _mainEditorVm.CurrentModuleId = "test_module_b";
        _moduleA.Verify(i => i.OnPageClosing(), Times.Once());
        _moduleA.Verify(i => i.OnPageOpening(), Times.Once());
    }

    [Fact]
    public void ChangeModuleShouldInitialiseOnFirstLoad()
    {
        _moduleB.Verify(i => i.Initialise(It.IsAny<IServiceGetter>()), Times.Never(), "Initialised before loading");
        _mainEditorVm.CurrentModuleId = "test_module_b";
        _moduleB.Verify(i => i.Initialise(It.IsAny<IServiceGetter>()), Times.Once(), "Not initialised on first load");
        _mainEditorVm.CurrentModuleId = "test_module_c";
        _mainEditorVm.CurrentModuleId = "test_module_b";
        _moduleB.Verify(i => i.Initialise(It.IsAny<IServiceGetter>()), Times.Once(), "Initialised more than once");
    }

    [Fact]
    public void DeactivateShouldDeactivateInitialisedModules()
    {
        // initialise module b and d
        _mainEditorVm.CurrentModuleId = "test_module_b";
        _moduleB.Verify(i => i.Initialise(It.IsAny<IServiceGetter>()), Times.Once());
        _mainEditorVm.CurrentModuleId = "test_module_d";
        _moduleD.Verify(i => i.Initialise(It.IsAny<IServiceGetter>()), Times.Once());

        // assert that others arent initialised
        _moduleA.Verify(i => i.Initialise(It.IsAny<IServiceGetter>()), Times.Never());
        _moduleC.Verify(i => i.Initialise(It.IsAny<IServiceGetter>()), Times.Never());

        // deactivate
        _mainEditorVm.Deactivate();

        _moduleA.Verify(i => i.Deactivate(), Times.Never());
        _moduleC.Verify(i => i.Deactivate(), Times.Never());
        _moduleB.Verify(i => i.Deactivate(), Times.Once());
        _moduleD.Verify(i => i.Deactivate(), Times.Once());

    }
}
