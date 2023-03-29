using System;
using System.Collections.Generic;
using RanseiLink.PluginModule.Api;

namespace RanseiLink.Windows.Tests.PluginModuleTests.PluginFormLoaderTests;

internal class TestPluginForm : IPluginForm
{
    public string Title => throw new NotImplementedException();
    public string ProceedButtonText => throw new NotImplementedException();
    public string CancelButtonText => throw new NotImplementedException();

    [BoolOption("Test display name", "Test description")]
    public bool TestBoolOption1 { get; set; } = true;

    [IntOption("Test int display name", "Test int description", "Test group 1", 3, 10)]
    public int TestIntOption { get; set; } = 4;

    [StringOption("Test string display name", "Test string description", "Test group 1", 12)]
    public string TestStringOption { get; set; } = "test initial text";

    [Text("Test group 2")]
    public string TestTextOption => "test text content";

    [CollectionOption("Test collection display name", new string[] { "hello", "hello again" }, "Test collection description", "collectionGroup")]
    public string TestCollectionOption1 { get; set; }

    public List<int> Collection2Items => new() { 1, 2, 3, 4, 5 };

    [CollectionOption("Test collection display name", nameof(Collection2Items), "Test collection description", "collectionGroup")]
    public string TestCollectionOption2 { get; set; }

}
