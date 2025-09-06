using RanseiLink.PluginModule.Services;
using RanseiLink.PluginModule.Services.Concrete;
using System.Linq;

namespace RanseiLink.Windows.Tests.PluginModuleTests.PluginFormLoaderTests;

public class InfoToFormTests
{
    private readonly PluginFormInfo _loadedInfo;
    private readonly TestPluginForm _form;
    private readonly IPluginFormLoader _formLoader;
    public InfoToFormTests()
    {
        _formLoader = new PluginFormLoader();
        _form = new TestPluginForm();
        _loadedInfo = _formLoader.FormToInfo(_form);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void BoolOptionsLoadCorrectly(bool testValue)
    {
        _loadedInfo.Items.OfType<BoolPluginFormItem>().Single().Value = testValue;
        _formLoader.InfoToForm(_loadedInfo);

        _form.TestBoolOption1.Should().Be(testValue);
    }

    [Fact]
    public void IntOptionsLoadCorrectly()
    {
        _loadedInfo.Items.OfType<IntPluginFormItem>().Single().Value = 9;
        _formLoader.InfoToForm(_loadedInfo);

        _form.TestIntOption.Should().Be(9);
    }

    [Fact]
    public void StringOptionsLoadCorrectly()
    {
        _loadedInfo.Items.OfType<StringPluginFormItem>().Single().Value = "changed text";
        _formLoader.InfoToForm(_loadedInfo);

        _form.TestStringOption.Should().Be("changed text");
    }

    [Fact]
    public void Collection1OptionsLoadCorrectly()
    {
        _loadedInfo.Items.OfType<CollectionPluginFormItem>().First().Value = "hello there";
        _formLoader.InfoToForm(_loadedInfo);

        _form.TestCollectionOption1.Should().Be("hello there");
    }
}
