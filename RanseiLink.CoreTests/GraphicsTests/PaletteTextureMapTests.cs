using RanseiLink.Core.Graphics;
using RanseiLink.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RanseiLink.CoreTests.GraphicsTests;

public class PaletteTextureMapTests
{
    [Fact]
    public void IdenticalThroughLoadSaveCycle()
    {
        var testFile = Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_palette_texture_map.xml");
        var result = PaletteTextureMap.Load(testFile);
        if (!result.IsSuccess)
        {
            Assert.Fail(result.ToString());
        }
        var temp = Path.GetTempFileName();
        result.Value.Save(temp);

        var before = File.ReadAllText(testFile);
        var after = File.ReadAllText(temp);
        Assert.Equal(before, after);
        File.Delete(temp);
    }
}
