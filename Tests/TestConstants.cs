using System;
using System.IO;

namespace Tests
{
    internal static class TestConstants
    {
        public static readonly string TestModFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RanseiLink", "Test", "TestMod");
    }
}
