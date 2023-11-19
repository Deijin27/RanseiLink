using RanseiLink.Core.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RanseiLink.CoreTests.GraphicsTests;

/// <summary>
/// Basic tests for nanr loading
/// </summary>
public class NanrTests
{
    [Fact]
    public void LoadKiAnim()
    {
        var nanr = NANR.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_ki2_aurora_anim.nanr"));

        nanr.AnimationBanks.KeyFrameCount.Should().Be(12);

        var banks = nanr.AnimationBanks.Banks;

        var bank0 = banks[0];
        using (new AssertionScope())
        {
            bank0.DataType.Should().Be(0);
            bank0.Unknown1.Should().Be(1);
            bank0.Unknown2.Should().Be(2);
            bank0.Unknown3.Should().Be(0);
            bank0.Frames.Should().HaveCount(4);
        }
        using (new AssertionScope())
        {
            bank0.Frames[0].CellBank.Should().Be(3);
            bank0.Frames[0].Duration.Should().Be(8);
            bank0.Frames[1].CellBank.Should().Be(2);
            bank0.Frames[1].Duration.Should().Be(8);
            bank0.Frames[2].CellBank.Should().Be(1);
            bank0.Frames[2].Duration.Should().Be(8);
            bank0.Frames[3].CellBank.Should().Be(0);
            bank0.Frames[3].Duration.Should().Be(180);
        }

        var bank1 = banks[1];
        using (new AssertionScope())
        {
            bank1.DataType.Should().Be(0);
            bank1.Unknown1.Should().Be(1);
            bank1.Unknown2.Should().Be(2);
            bank1.Unknown3.Should().Be(0);
            bank1.Frames.Should().HaveCount(4);
        }
        using (new AssertionScope())
        {
            bank1.Frames[0].CellBank.Should().Be(4);
            bank1.Frames[0].Duration.Should().Be(12);
            bank1.Frames[1].CellBank.Should().Be(5);
            bank1.Frames[1].Duration.Should().Be(12);
            bank1.Frames[2].CellBank.Should().Be(6);
            bank1.Frames[2].Duration.Should().Be(12);
            bank1.Frames[3].CellBank.Should().Be(7);
            bank1.Frames[3].Duration.Should().Be(12);
        }

        var bank2 = banks[2];
        using (new AssertionScope())
        {
            bank2.DataType.Should().Be(0);
            bank2.Unknown1.Should().Be(1);
            bank2.Unknown2.Should().Be(2);
            bank2.Unknown3.Should().Be(0);
            bank2.Frames.Should().HaveCount(4);
        }
        using (new AssertionScope())
        {
            bank2.Frames[0].CellBank.Should().Be(8);
            bank2.Frames[0].Duration.Should().Be(12);
            bank2.Frames[1].CellBank.Should().Be(9);
            bank2.Frames[1].Duration.Should().Be(12);
            bank2.Frames[2].CellBank.Should().Be(10);
            bank2.Frames[2].Duration.Should().Be(12);
            bank2.Frames[3].CellBank.Should().Be(11);
            bank2.Frames[3].Duration.Should().Be(12);
        }

        nanr.Labels.Names.Should().Equal(new[] { "reflection", "flag_r", "flag_l" });

        nanr.Unknown.Unknown.Should().Be(0);
    }

    [Theory]
    [InlineData("test_ki2_aurora_anim.nanr")]
    public void LoadSaveCycle(string fileName)
    {
        var file = Path.Combine(TestConstants.EmbeddedTestDataFolder, fileName);
        File.Exists(file).Should().BeTrue();

        var ncer = NANR.Load(file);

        var mem = new MemoryStream();
        var bw = new BinaryWriter(mem);
        ncer.WriteTo(bw);
        var data = mem.ToArray();
        mem.Dispose();

        //File.WriteAllBytes(Path.Combine(Core.FileUtil.DesktopDirectory, fileName + ".debug.bin"), data); // debug

        data.Should().Equal(File.ReadAllBytes(file));
    }
}
