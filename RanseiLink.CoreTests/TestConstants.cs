using System;
using System.IO;

namespace RanseiLink.CoreTests;

internal static class TestConstants
{
    public static string TestFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "RanseiLink", "Test");

    public static string TestModFolder => Path.Combine(TestFolder, "TestMod");

    public static string TestTempFolder => Path.Combine(TestFolder, "Temp");

    public static string EmbeddedTestDataFolder { get; } = Path.Combine(Path.GetDirectoryName(new Uri(typeof(TestConstants).Assembly.Location).AbsolutePath)!, "TestData");

}
