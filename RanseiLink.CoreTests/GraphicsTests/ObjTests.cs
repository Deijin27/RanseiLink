using RanseiLink.Core;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Graphics.ExternalFormats;
using SixLabors.ImageSharp;
using System.IO;

namespace RanseiLink.CoreTests.GraphicsTests;

public class ObjTests
{
    [Fact]
    public void IdenticalThroughLoadSaveCycle()
    {
        var tempDir = FileUtil.GetTemporaryDirectory();

        var testMtl = new MTL();
        testMtl.Materials.AddRange(
        [
            new MTL.Material("material1")
            {
                SpecularColor = Color.Red,
                DiffuseColor = Color.Blue,
                AmbientColor = Color.Green,
                Dissolve = 1,
                AmbientTextureMapFile = "texAmbient1.png",
                DissolveTextureMapFile = "texDissolve1.png",
                SpecularTextureMapFile = "texSpecular1.png",
                DiffuseTextureMapFile = "texDiffuse1.png,"
            },
            new MTL.Material("material2")
            {
                SpecularColor = Color.White,
                DiffuseColor = Color.Black,
                AmbientColor = Color.White,
                Dissolve = 0.5f,
                AmbientTextureMapFile = "texAmbient2.png",
                DissolveTextureMapFile = "texDissolve2.png",
                SpecularTextureMapFile = "texSpecular2.png",
                DiffuseTextureMapFile = "texDiffuse2.png,"
            }
        ]);

        var expected = new OBJ
        {
            MaterialLib = testMtl
        };
        expected.Groups.AddRange(
        [
            new OBJ.Group("polygon")
            {
                Vertices = [new(1, 2, 3), new(4, 5, 6), new(1.1f, 1.2f, 1.3f)],
                Normals = [new(7, 8, 9), new(10, 11, 12), new(1.4f, 1.5f, 1.6f)],
                TextureVertices = [new(13, 14), new(16, 17), new(1.7f, 1.8f)],
                Faces =
                [
                    new OBJ.Face("material1")
                    {
                        VertexIndices = [0, 1, 2],
                        NormalIndices = [0, 1, 2],
                        TexCoordIndices = [0, 1, 2],
                    }
                ]
            },
            new OBJ.Group("polygon1")
            {
                Vertices = [new(1, 2, 3), new(4, 5, 6), new(1.1f, 1.2f, 1.3f), new(1, 2, 3), new(4, 5, 6), new(1.1f, 1.2f, 1.3f)],
                Normals = [new(7, 8, 9), new(10, 11, 12), new(1.4f, 1.5f, 1.6f), new(1, 2, 3), new(4, 5, 6), new(1.1f, 1.2f, 1.3f)],
                TextureVertices = [new(13, 14), new(16, 17), new(1.7f, 1.8f), new(13, 14), new(16, 17), new(1.7f, 1.8f)],
                Faces =
                [
                    new OBJ.Face("material2")
                    {
                        VertexIndices = [0, 1, 2],
                        NormalIndices = [0, 1, 2],
                        TexCoordIndices = [0, 1, 2],
                    },
                    new OBJ.Face("material2")
                    {
                        VertexIndices = [3, 4, 5],
                        NormalIndices = [3, 4, 5],
                        TexCoordIndices = [3, 4, 5],
                    }
                ]
            }
        ]);


        string file = Path.Combine(tempDir, "testModel.obj");
        expected.Save(file);

        var actual = new OBJ(file);

        Directory.Delete(tempDir, true);


        AssertMaterialIdentical(expected, actual);
        
        
    }

    private static void AssertMaterialIdentical(OBJ expected, OBJ actual)
    {
        actual.MaterialLib.Should().NotBeNull();
        actual.MaterialLib!.Materials.Should().NotBeNull().And.HaveCount(expected.MaterialLib!.Materials.Count);
        for (int i = 0; i < expected.MaterialLib.Materials.Count; i++)
        {
            var inputMat = expected.MaterialLib.Materials[i];
            var outputMat = actual.MaterialLib.Materials[i];

            outputMat.Name.Should().Be(inputMat.Name);
            outputMat.AmbientColor.Should().Be(inputMat.AmbientColor);
            outputMat.DiffuseColor.Should().Be(inputMat.DiffuseColor);
            outputMat.SpecularColor.Should().Be(inputMat.SpecularColor);
            outputMat.Dissolve.Should().Be(inputMat.Dissolve);
            outputMat.DiffuseTextureMapFile.Should().EndWith(inputMat.DiffuseTextureMapFile);
            outputMat.SpecularTextureMapFile.Should().EndWith(inputMat.SpecularTextureMapFile);
            outputMat.AmbientTextureMapFile.Should().EndWith(inputMat.AmbientTextureMapFile);
            outputMat.DissolveTextureMapFile.Should().EndWith(inputMat.DissolveTextureMapFile);
        }
    }

    private static void AssertGroupsIdentical(OBJ expected, OBJ actual)
    {
        actual.Groups.Should().NotBeNull().And.HaveCount(expected.Groups.Count);
        for (int i = 0; i < expected.Groups.Count; i++)
        {
            var inputGroup = expected.Groups[i];
            var outputGroup = actual.Groups[i];

            outputGroup.Name.Should().Be(inputGroup.Name);
            outputGroup.Vertices.Should().NotBeNull().And.HaveCount(inputGroup.Vertices.Count);
            outputGroup.Normals.Should().NotBeNull().And.HaveCount(inputGroup.Normals.Count);
            outputGroup.TextureVertices.Should().NotBeNull().And.HaveCount(inputGroup.TextureVertices.Count);
            outputGroup.VertexColors.Should().NotBeNull().And.BeEmpty();
            outputGroup.Faces.Should().NotBeNull().And.HaveCount(inputGroup.Faces.Count);

            for (int j = 0; j < inputGroup.Vertices.Count; j++)
            {
                outputGroup.Vertices[j].Should().Be(inputGroup.Vertices[j]);
            }
            for (int j = 0; j < inputGroup.Normals.Count; j++)
            {
                outputGroup.Normals[j].Should().Be(inputGroup.Normals[j]);
            }
            for (int j = 0; j < inputGroup.TextureVertices.Count; j++)
            {
                outputGroup.TextureVertices[j].Should().Be(inputGroup.TextureVertices[j]);
            }
            for (int j = 0; j < inputGroup.Faces.Count; j++)
            {
                var inputFace = inputGroup.Faces[j];
                var outputFace = outputGroup.Faces[j];

                outputFace.MaterialName.Should().Be(inputFace.MaterialName);
                outputFace.VertexIndices.Should().NotBeNull().And.HaveCount(inputFace.VertexIndices.Count);
                outputFace.NormalIndices.Should().NotBeNull().And.HaveCount(inputFace.NormalIndices.Count);
                outputFace.TexCoordIndices.Should().NotBeNull().And.HaveCount(inputFace.TexCoordIndices.Count);
                outputFace.VertexColorIndices.Should().NotBeNull().And.BeEmpty();
                for (int k = 0; k < inputFace.VertexIndices.Count; k++)
                {
                    outputFace.VertexIndices[k].Should().Be(inputFace.VertexIndices[k]);
                }
                for (int k = 0; k < inputFace.NormalIndices.Count; k++)
                {
                    outputFace.NormalIndices[k].Should().Be(inputFace.NormalIndices[k]);
                }
                for (int k = 0; k < inputFace.TexCoordIndices.Count; k++)
                {
                    outputFace.TexCoordIndices[k].Should().Be(inputFace.TexCoordIndices[k]);
                }
            }
        }
    }


    [Fact]
    public void IdenticalThroughConversionToIntermediate()
    {
        var file = Path.Combine(TestConstants.EmbeddedTestDataFolder, "test_map0.obj");
        File.Exists(file).Should().BeTrue();
        File.Exists(Path.ChangeExtension(file, ".mtl")); // mtl is necessary just to load the obj

        var expected = new OBJ(file);

        var intermediate = ConvertModels.ObjToIntermediate(expected);

        var actual = ConvertModels.IntermediateToObj(intermediate);

        AssertGroupsIdentical(expected, actual);
    }
}
