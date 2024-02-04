using RanseiLink.Core.Services;
using System.IO;
using System.Xml.Linq;

namespace RanseiLink.CoreTests.GraphicsTests;

public class CellAnimSerialiseTests
{
    [Fact]
    public void LoadSaveCycle()
    {
        var doc = XDocument.Load(Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_ki2_aurora_anim_serialised.xml"));

        var res = new CellAnimationSerialiser.RLAnimationResource(doc);
        var newDoc = res.Serialise();

        newDoc.Should().BeEquivalentTo(doc);
    }
}
