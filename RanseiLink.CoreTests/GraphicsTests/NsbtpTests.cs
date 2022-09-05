using FluentAssertions;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Graphics.Conquest;
using System.IO;
using Xunit;

namespace RanseiLink.CoreTests.GraphicsTests;

public class NsbtpTests
{
    [Fact]
    public void TestNonRaw_IdenticalThroughLoadSaveCycle()
    {
        string fileName = "test_leafeon.nsbtp";
        var file = Path.Combine(TestConstants.EmbeddedTestDataFolder, fileName);
        File.Exists(file).Should().BeTrue();

        var btp = new NSBTP(file);

        var mem = new MemoryStream();
        var bw = new BinaryWriter(mem);
        btp.WriteTo(bw);
        var data = mem.ToArray();
        mem.Dispose();

        //File.WriteAllBytes(Path.Combine(Core.FileUtil.DesktopDirectory, fileName + ".debug.bin"), data); // debug

        data.Should().Equal(File.ReadAllBytes(file));
    }

    [Fact]
    public void TestRaw_IdenticalThroughLoadSaveCycle()
    {
        string fileName = "test_leafeon.pat";
        var file = Path.Combine(TestConstants.EmbeddedTestDataFolder, fileName);
        File.Exists(file).Should().BeTrue();

        NSPAT pat;
        using (var br = new BinaryReader(File.OpenRead(file)))
        {
            pat = NSPAT_RAW.Load(br);
        }

        var mem = new MemoryStream();
        var bw = new BinaryWriter(mem);
        NSPAT_RAW.WriteTo(pat, bw);
        var data = mem.ToArray();
        mem.Dispose();

        //File.WriteAllBytes(Path.Combine(Core.FileUtil.DesktopDirectory, fileName + ".debug.bin"), data); // debug

        data.Should().Equal(File.ReadAllBytes(file));
    }
}
