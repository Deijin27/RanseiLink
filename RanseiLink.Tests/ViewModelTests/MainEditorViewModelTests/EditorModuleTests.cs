using FluentResults;
using RanseiLink.Core.Services;
using RanseiLink.Core.Settings;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using RanseiLink.Settings;
using RanseiLink.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

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

    private readonly Mock<IServiceGetter> _mockModServiceGetter;
    private readonly Mock<IModServiceGetterFactory> _modServiceGetterFactory;
    private readonly EditorModuleOrderSetting _editorModuleOrderSetting;
    private readonly Mock<ICachedMsgBlockService> _cachedMsgBlockService;
    private readonly Mock<ISettingService> _settingService;
    private readonly Mock<IModPatchingService> _patchingService;
    private readonly Mock<IDialogService> _dialogService;
    private readonly MainEditorViewModel _mainEditorVm;
    private readonly Mock<EditorModule> _moduleA;
    private readonly Mock<EditorModule> _moduleB;
    private readonly Mock<EditorModule> _moduleC;
    private readonly Mock<EditorModule> _moduleD;

    private List<Mock<IServiceGetter>> _modServiceGetters = new List<Mock<IServiceGetter>>();

    public EditorModuleTests()
    {
        _editorModuleOrderSetting = new EditorModuleOrderSetting()
        {
            Value = new string[] { "test_module_b", "test_module_c", "test_module_a" }
        };
        _settingService = new Mock<ISettingService>();
        _settingService.Setup(i => i.Get<EditorModuleOrderSetting>()).Returns(_editorModuleOrderSetting);

        _patchingService = new Mock<IModPatchingService>();
        _dialogService = new Mock<IDialogService>();

        _cachedMsgBlockService = new Mock<ICachedMsgBlockService>();

        _mockModServiceGetter = new Mock<IServiceGetter>();
        _mockModServiceGetter.Setup(i => i.Get<ICachedMsgBlockService>()).Returns(_cachedMsgBlockService.Object);

        _modServiceGetterFactory = new Mock<IModServiceGetterFactory>();
        _modServiceGetterFactory.Setup(i => i.Create(It.IsAny<ModInfo>())).Returns(_mockModServiceGetter.Object);
        

        _moduleA = SetupTestModule("a");
        _moduleB = SetupTestModule("b");
        _moduleC = SetupTestModule("c");
        _moduleD = SetupTestModule("d");

        _mainEditorVm = new MainEditorViewModel(
            _dialogService.Object,
            _patchingService.Object,
            _settingService.Object,
            new Mock<IPluginLoader>().Object,
            _modServiceGetterFactory.Object,
            new EditorModule[] { _moduleA.Object, _moduleB.Object, _moduleC.Object, _moduleD.Object }
            );

        _mainEditorVm.SetMod(null);
    }

    [Fact]
    public void LoadModuleOrderCorrectly()
    {
        _mainEditorVm.ListItems.Should().HaveCount(4);
        _mainEditorVm.ListItems[0].ModuleId.Should().Be("test_module_b");
        _mainEditorVm.ListItems[1].ModuleId.Should().Be("test_module_c");
        _mainEditorVm.ListItems[2].ModuleId.Should().Be("test_module_a");
        _mainEditorVm.ListItems[3].ModuleId.Should().Be("test_module_d"); // not in setting are put at end of list
    }

    [Fact]
    public void ChangeModuleShouldLoadViewModel()
    {
        _mainEditorVm.CurrentModuleId = "test_module_c";
        _mainEditorVm.CurrentVm.Should().Be("module_c_view_model");
        _mainEditorVm.CurrentModuleId = "test_module_d";
        _mainEditorVm.CurrentVm.Should().Be("module_d_view_model");
    }
    
    [Fact]
    public void ChangeModuleShouldInvokePageChange()
    {
        _moduleA.Verify(i => i.OnPageOpening(), Times.Never());
        _mainEditorVm.CurrentModuleId = "test_module_a";
        _moduleA.Verify(i => i.OnPageOpening(), Times.Once());
        _moduleA.Verify(i => i.OnPageClosing(), Times.Never());
        _moduleD.Verify(i => i.OnPageOpening(), Times.Never());
        _mainEditorVm.CurrentModuleId = "test_module_d";
        _moduleA.Verify(i => i.OnPageClosing(), Times.Once());
        _moduleD.Verify(i => i.OnPageOpening(), Times.Once());
    }

    [Fact]
    public void ChangeModuleShouldInitialiseOnFirstLoad()
    {
        _moduleD.Verify(i => i.Initialise(It.IsAny<IServiceGetter>()), Times.Never(), "Initialised before loading");
        _mainEditorVm.CurrentModuleId = "test_module_d";
        _moduleD.Verify(i => i.Initialise(It.IsAny<IServiceGetter>()), Times.Once(), "Not initialised on first load");
        _mainEditorVm.CurrentModuleId = "test_module_c";
        _mainEditorVm.CurrentModuleId = "test_module_d";
        _moduleD.Verify(i => i.Initialise(It.IsAny<IServiceGetter>()), Times.Once(), "Initialised more than once");
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

    [Fact]
    public void DeactivateShouldSaveTextChanges()
    {
        _cachedMsgBlockService.Verify(i => i.SaveChangedBlocks(), Times.Never());
        _mainEditorVm.Deactivate();
        _cachedMsgBlockService.Verify(i => i.SaveChangedBlocks(), Times.Once());
    }

    [Fact]
    public void DeactivateShouldSaveModuleOrder()
    {
        // change the order to something other than it was before
        var items = _mainEditorVm.ListItems.ToList();
        _mainEditorVm.ListItems.Clear();
        foreach (var item in items.OrderBy(i => i.ModuleId))
        {
            _mainEditorVm.ListItems.Add(item);
        }

        // deactivate
        _mainEditorVm.Deactivate();

        // check saved correctly
        _editorModuleOrderSetting.Value.Should().Equal(new string[] { "test_module_a", "test_module_b", "test_module_c", "test_module_d" });
        _settingService.Verify(i => i.Save(), Times.Once());
    }

    private void PrepareForPatch()
    {
        _settingService.Setup(i => i.Get<RecentCommitRomSetting>()).Returns(new RecentCommitRomSetting());
        _settingService.Setup(i => i.Get<PatchSpritesSetting>()).Returns(new PatchSpritesSetting());
        _dialogService.Setup(i => i.ShowDialog(It.IsAny<ModCommitViewModel>())).Callback((object vm) => ((ModCommitViewModel)vm).OnClosing(true));
        _patchingService.Setup(i => i.CanPatch(It.IsAny<ModInfo>(), It.IsAny<string>(), It.IsAny<PatchOptions>())).Returns(Result.Ok());

        _dialogService.Setup(i => i.ProgressDialog(It.IsAny<Action<IProgress<ProgressInfo>>>(), It.IsAny<bool>()))
            .Callback((Action<IProgress<ProgressInfo>> action, bool opt) => action(null));
    }

    [Fact]
    public void PatchShouldSaveChangedText()
    {
        _cachedMsgBlockService.Verify(i => i.SaveChangedBlocks(), Times.Never());

        PrepareForPatch();

        _mainEditorVm.CommitRomCommand.Execute(null);

        _cachedMsgBlockService.Verify(i => i.SaveChangedBlocks(), Times.Once());
    }

    [Fact]
    public void PatchShouldInformInitialisedModules()
    {
        PrepareForPatch();

        // initialise module b and d
        _mainEditorVm.CurrentModuleId = "test_module_b";
        _moduleB.Verify(i => i.Initialise(It.IsAny<IServiceGetter>()), Times.Once());
        _mainEditorVm.CurrentModuleId = "test_module_d";
        _moduleD.Verify(i => i.Initialise(It.IsAny<IServiceGetter>()), Times.Once());

        // assert that others arent initialised
        _moduleA.Verify(i => i.Initialise(It.IsAny<IServiceGetter>()), Times.Never());
        _moduleC.Verify(i => i.Initialise(It.IsAny<IServiceGetter>()), Times.Never());

        _mainEditorVm.CommitRomCommand.Execute(null);

        _moduleA.Verify(i => i.OnPatchingRom(), Times.Never());
        _moduleC.Verify(i => i.OnPatchingRom(), Times.Never());
        _moduleB.Verify(i => i.OnPatchingRom(), Times.Once());
        _moduleD.Verify(i => i.OnPatchingRom(), Times.Once());
    }

    [Fact]
    public void RunPlugin()
    {
        _mainEditorVm.PluginPopupOpen = true;
        _mainEditorVm.PluginPopupOpen.Should().BeTrue();
        _modServiceGetterFactory.Verify(x => x.Create(It.IsAny<ModInfo>()), Times.Exactly(1));

        var plugin = new Mock<IPlugin>();
        _mainEditorVm.SelectedPlugin = new PluginInfo(plugin.Object, "Name", "Author", "Version");

        _mainEditorVm.PluginPopupOpen.Should().BeFalse();
        plugin.Verify(x => x.Run(It.IsAny<IPluginContext>()), Times.Once());
        _mockModServiceGetter.Verify(x => x.Dispose(), Times.Exactly(2));
        _modServiceGetterFactory.Verify(x => x.Create(It.IsAny<ModInfo>()), Times.Exactly(3));

    }
}
