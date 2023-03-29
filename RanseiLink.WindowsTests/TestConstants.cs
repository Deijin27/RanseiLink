using System;
using System.IO;

namespace RanseiLink.Windows.Tests;

internal static class TestConstants
{
    public static readonly string TestModFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "RanseiLink", "Test", "TestMod");
}
