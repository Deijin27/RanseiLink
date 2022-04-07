using RanseiLink.Core.Maps;
using System.IO;
using Xunit;

namespace RanseiLink.CoreTests.MapTests;

public class MapLoadTests
{
    [Fact]
    public void IdenticalThroughLoadSaveCycle()
    {
        foreach (var file in Directory.GetFiles(Path.Combine(TestConstants.TestModFolder, RanseiLink.Core.Services.Constants.MapFolderPath)))
        {
            var unchangedBytes = File.ReadAllBytes(file);
            string temp = Path.GetTempFileName();
            PSLM map;
            using (var br = new BinaryReader(File.OpenRead(file)))
            {
                map = new PSLM(br);
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
