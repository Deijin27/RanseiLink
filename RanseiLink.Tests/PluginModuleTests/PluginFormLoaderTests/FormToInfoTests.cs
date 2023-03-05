using RanseiLink.PluginModule.Services;
using RanseiLink.PluginModule.Services.Concrete;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.Tests.PluginModuleTests.PluginFormLoaderTests;

public class FormToInfoTests
{
    private readonly PluginFormInfo _loadedInfo;
    public FormToInfoTests()
    {
        IPluginFormLoader formLoader = new PluginFormLoader();
        _loadedInfo = formLoader.FormToInfo(new TestPluginForm());
    }

    [Fact]
    public void BoolOptionsLoadCorrectly()
    {
        var options = _loadedInfo.UngroupedItems.OfType<BoolPluginFormItem>().ToList();

        var option = options.Should().ContainSingle().Which;
        option.DisplayName.Should().Be("Test display name");
        option.Description.Should().Be("Test description");
        option.Value.Should().BeTrue();
    }

    [Fact]
    public void IntOptionsLoadCorrectly()
    {
        var options = _loadedInfo.Groups.Single(i => i.GroupName == "Test group 1").Items.OfType<IntPluginFormItem>().ToList();

        var option = options.Should().ContainSingle().Which;
        option.DisplayName.Should().Be("Test int display name");
        option.Description.Should().Be("Test int description");
        option.Value.Should().Be(4);
        option.MinValue.Should().Be(3);
        option.MaxValue.Should().Be(10);
    }

    [Fact]
    public void StringOptionsLoadCorrectly()
    {
        var options = _loadedInfo.Groups.Single(i => i.GroupName == "Test group 1").Items.OfType<StringPluginFormItem>().ToList();

        var option = options.Should().ContainSingle().Which;
        option.DisplayName.Should().Be("Test string display name");
        option.Description.Should().Be("Test string description");
        option.Value.Should().Be("test initial text");
        option.MaxLength.Should().Be(12);
    }

    [Fact]
    public void TextOptionsLoadCorrectly()
    {
        var options = _loadedInfo.Groups.Single(i => i.GroupName == "Test group 2").Items.OfType<TextPluginFormItem>().ToList();

        var option = options.Should().ContainSingle().Which;
        option.Value.Should().Be("test text content");
    }

    [Fact]
    public void Collection1OptionsLoadCorrectly()
    {
        var options = _loadedInfo.Groups.Single(i => i.GroupName == "collectionGroup").Items.OfType<CollectionPluginFormItem>().ToList();
        options.Should().HaveCount(2);

        var option = options[0];
        option.DisplayName.Should().Be("Test collection display name");
        option.Description.Should().Be("Test collection description");
        (option.Values as string[]).Should().Equal(new string[] { "hello", "hello again" });
    }

    [Fact]
    public void Collection2OptionsLoadCorrectly()
    {
        var options = _loadedInfo.Groups.Single(i => i.GroupName == "collectionGroup").Items.OfType<CollectionPluginFormItem>().ToList();
        options.Should().HaveCount(2);

        var option = options[1];
        option.DisplayName.Should().Be("Test collection display name");
        option.Description.Should().Be("Test collection description");
        (option.Values as List<int>).Should().Equal(new List<int> { 1, 2, 3, 4, 5 });
    }
}
