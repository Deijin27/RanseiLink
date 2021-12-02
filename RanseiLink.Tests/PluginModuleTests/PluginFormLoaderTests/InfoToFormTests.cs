using RanseiLink.Core.Services;
using RanseiLink.PluginModule.Services;
using RanseiLink.PluginModule.Services.Registration;
using System.Linq;
using Xunit;

namespace RanseiLink.Tests.PluginModuleTests.PluginFormLoaderTests;

public class InfoToFormTests
{
    private readonly PluginFormInfo _loadedInfo;
    private readonly TestPluginForm _form;
    private readonly IPluginFormLoader _formLoader;
    public InfoToFormTests()
    {
        IServiceContainer container = new ServiceContainer();
        container.RegisterPluginServices();
        _formLoader = container.Resolve<IPluginFormLoader>();
        _form = new TestPluginForm();
        _loadedInfo = _formLoader.FormToInfo(_form);
    }

    [Fact]
    public void BoolOptionsLoadCorrectly()
    {
        _loadedInfo.UngroupedItems.OfType<BoolPluginFormItem>().Single().Value = false;
        _formLoader.InfoToForm(_loadedInfo);
        Assert.False(_form.TestBoolOption1);
    }

    [Fact]
    public void UIntOptionsLoadCorrectly()
    {
        _loadedInfo.Groups.Single(i => i.GroupName == "Test group 1").Items.OfType<UIntPluginFormItem>().Single().Value = 9;
        _formLoader.InfoToForm(_loadedInfo);
        Assert.Equal(9u, _form.TestUIntOption);
    }

    [Fact]
    public void StringOptionsLoadCorrectly()
    {
        _loadedInfo.Groups.Single(i => i.GroupName == "Test group 1").Items.OfType<StringPluginFormItem>().Single().Value = "changed text";
        _formLoader.InfoToForm(_loadedInfo);
        Assert.Equal("changed text", _form.TestStringOption);
    }

    [Fact]
    public void Collection1OptionsLoadCorrectly()
    {
        _loadedInfo.Groups.Single(i => i.GroupName == "collectionGroup").Items.OfType<CollectionPluginFormItem>().First().Value = "hello again";
        _formLoader.InfoToForm(_loadedInfo);
        Assert.Equal("hello again", _form.TestCollectionOption1);
    }
}
