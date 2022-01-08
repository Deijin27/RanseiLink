using RanseiLink.Core.Map;
using System.IO;
using Xunit;

namespace RanseiLink.Tests.MapTests;

public class MapLoadTests
{
    [Fact]
    public void IdenticalThroughLoadSaveCycle()
    {
        foreach (var file in Directory.GetFiles(Path.Combine(TestConstants.TestModFolder, "data", "map")))
        {
            var unchangedBytes = File.ReadAllBytes(file);
            string temp = Path.GetTempFileName();
            Map map;
            using (var br = new BinaryReader(File.OpenRead(file)))
            {
                map = new Map(br);
            }
            using (var bw = new BinaryWriter(File.Create(temp)))
            {
                map.WriteTo(bw);
            }
            var shouldBeUnchangedBytes = File.ReadAllBytes(temp);
            File.Delete(temp);
            Assert.Equal(unchangedBytes, shouldBeUnchangedBytes);

        }
    }
}
