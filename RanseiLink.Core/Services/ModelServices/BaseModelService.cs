using System;
using System.IO;

namespace RanseiLink.Core.Services.ModelServices;

public abstract class BaseModelService
{
    private readonly string RomPath;
    private readonly int ItemLength;
    protected readonly ModInfo Mod;
    private readonly int MaxIndex;

    public BaseModelService(ModInfo mod, string romPath, int itemLength, int maxIndex)
    {
        Mod = mod;
        RomPath = romPath;
        ItemLength = itemLength;
        MaxIndex = maxIndex;
    }

    private void ValidateIndex(int index)
    {
        if (index > MaxIndex || index < 0)
        {
            throw new ArgumentOutOfRangeException($"Index in {GetType().Name} should not be greater than {MaxIndex}");
        }
    }

    protected byte[] RetrieveData(int id)
    {
        ValidateIndex(id);
        using (var file = new BinaryReader(File.OpenRead(Path.Combine(Mod.FolderPath, RomPath))))
        {
            file.BaseStream.Position = id * ItemLength;
            return file.ReadBytes(ItemLength);
        }
    }

    protected void SaveData(int id, byte[] data)
    {
        ValidateIndex(id);
        using (var file = new BinaryWriter(File.OpenWrite(Path.Combine(Mod.FolderPath, RomPath))))
        {
            file.BaseStream.Position = id * ItemLength;
            file.Write(data);
        }
    }
}

public abstract class BaseDisposableModelService : IDisposable
{
    private readonly int ItemLength;
    private readonly int MaxIndex;

    protected readonly Stream stream;

    public BaseDisposableModelService(ModInfo mod, string romPath, int itemLength, int maxIndex)
    {
        MaxIndex = maxIndex;
        ItemLength = itemLength;
        stream = File.Open(Path.Combine(mod.FolderPath, romPath), FileMode.Open, FileAccess.ReadWrite);
    }

    public void Dispose()
    {
        stream.Close();
    }

    private void ValidateIndex(int index)
    {
        if (index > MaxIndex || index < 0)
        {
            throw new ArgumentOutOfRangeException($"Index in {GetType().Name} should not be greater than {MaxIndex}");
        }
    }

    protected byte[] RetrieveData(int id)
    {
        ValidateIndex(id);
        stream.Position = id * ItemLength;
        byte[] buffer = new byte[ItemLength];
        stream.Read(buffer, 0, ItemLength);
        return buffer;
    }

    protected void SaveData(int id, byte[] data)
    {
        ValidateIndex(id);
        stream.Position = id * ItemLength;
        stream.Write(data, 0, ItemLength);
    }
}
