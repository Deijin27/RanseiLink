using RanseiLink.PluginModule.Services;
using RanseiLink.PluginModule.Services.Concrete;
using System.Collections.Generic;
using System.Linq;
using Xunit;

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
        Assert.Single(options);
        var option = options.Single();
        Assert.Equal("Test display name", option.DisplayName);
        Assert.Equal("Test description", option.Description);
        Assert.True(option.Value);
    }

    [Fact]
    public void IntOptionsLoadCorrectly()
    {
        var options = _loadedInfo.Groups.Single(i => i.GroupName == "Test group 1").Items.OfType<IntPluginFormItem>().ToList();
        Assert.Single(options);
        var option = options.Single();
        Assert.Equal("Test int display name", option.DisplayName);
        Assert.Equal("Test int description", option.Description);
        Assert.Equal(4, option.Value);
        Assert.Equal(3, option.MinValue);
        Assert.Equal(10, option.MaxValue);
    }

    [Fact]
    public void StringOptionsLoadCorrectly()
    {
        var options = _loadedInfo.Groups.Single(i => i.GroupName == "Test group 1").Items.OfType<StringPluginFormItem>().ToList();
        Assert.Single(options);
        var option = options.Single();
        Assert.Equal("Test string display name", option.DisplayName);
        Assert.Equal("Test string description", option.Description);
        Assert.Equal("test initial text", option.Value);
        Assert.Equal(12, option.MaxLength);
    }

    [Fact]
    public void TextOptionsLoadCorrectly()
    {
        var options = _loadedInfo.Groups.Single(i => i.GroupName == "Test group 2").Items.OfType<TextPluginFormItem>().ToList();
        Assert.Single(options);
        var option = options.Single();
        Assert.Equal("test text content", option.Value);
    }

    [Fact]
    public void Collection1OptionsLoadCorrectly()
    {
        var options = _loadedInfo.Groups.Single(i => i.GroupName == "collectionGroup").Items.OfType<CollectionPluginFormItem>().ToList();
        Assert.Equal(2, options.Count);
        var option = options.First();
        Assert.Equal("Test collection display name", option.DisplayName);
        Assert.Equal("Test collection description", option.Description);
        var items = option.Values as string[];
        Assert.NotNull(items);
        Assert.Equal(new string[] { "hello", "hello again" }, items);
    }

    [Fact]
    public void Collection2OptionsLoadCorrectly()
    {
        var options = _loadedInfo.Groups.Single(i => i.GroupName == "collectionGroup").Items.OfType<CollectionPluginFormItem>().ToList();
        Assert.Equal(2, options.Count);
        var option = options.ElementAt(1);
        Assert.Equal("Test collection display name", option.DisplayName);
        Assert.Equal("Test collection description", option.Description);
        var items = option.Values as List<int>;
        Assert.NotNull(items);
        Assert.Equal(new List<int> { 1, 2, 3, 4, 5 }, items);
    }
}
