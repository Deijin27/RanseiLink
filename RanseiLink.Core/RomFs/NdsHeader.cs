#nullable enable
using System;
using System.IO;
using System.Text;

namespace RanseiLink.Core.RomFs;

public enum Unitcode : byte
{
    Nds,
    Dsi_0,
    NdsPlusDsi,
    Dsi_1
}

[Flags]
public enum InternalFlags : byte
{
    Autostart = 0b_0000_0010
}

public readonly struct NdsHeader
{
    public NdsHeader(BinaryReader br)
    {
        GameTitle = Encoding.UTF8.GetString(br.ReadBytes(12));
        GameCode = Encoding.UTF8.GetString(br.ReadBytes(4));
        MakerCode = Encoding.UTF8.GetString(br.ReadBytes(2));
        Unitcode = (Unitcode)br.ReadByte();
        EncryptionSeedSelect = br.ReadByte();
        DeviceCapacity = br.ReadByte();
        Reserved_0 = br.ReadBytes(7);
        GameRevision = br.ReadUInt16();
        RomVersion = br.ReadByte();
        InternalFlags = (InternalFlags)br.ReadByte();
        Arm9RomOffset = br.ReadUInt32();
        Arm9LoadAddress = br.ReadUInt32();
        Arm9EntryAddress = br.ReadUInt32();
        Arm9Size = br.ReadUInt32();
        Arm7RomOffset = br.ReadUInt32();
        Arm7LoadAddress = br.ReadUInt32();
        Arm7EntryAddress = br.ReadUInt32();
        Arm7Size = br.ReadUInt32();
        FileNameTableOffset = br.ReadUInt32();
        FileNameTableLength = br.ReadUInt32();
        FileAllocationTableOffset = br.ReadUInt32();
        FileAllocationTableLength = br.ReadUInt32();
        Arm9OverlayOffset = br.ReadUInt32();
        Arm9OverlayLength = br.ReadUInt32();
        Arm7OverlayOffset = br.ReadUInt32();
        Arm7OverlayLength = br.ReadUInt32();
        NormalCardControlRegisterSettings = br.ReadUInt32();
        SecureCardControlRegisterSettings = br.ReadUInt32();
        IconBannerOffset = br.ReadUInt32();
        SecureAreaCrc = br.ReadUInt16();
        SecureTransferTimeout = br.ReadUInt16();
        Arm9Autoload = br.ReadUInt32();
        Arm7Autoload = br.ReadUInt32();
        SecureDisable = br.ReadUInt64();
        NtrRegionRomSize = br.ReadUInt32();
        HeaderSize = br.ReadUInt32();
        Reserved_1 = br.ReadBytes(56);
        NintendoLogo = br.ReadBytes(156);
        NintendoLogoCrc = br.ReadUInt16();
        HeaderCrc = br.ReadUInt16();
        DebuggerReserved = br.ReadBytes(32);

    }
    public readonly string GameTitle;
    public readonly string GameCode;
    public readonly string MakerCode;
    public readonly Unitcode Unitcode;
    public readonly byte EncryptionSeedSelect;
    public readonly byte DeviceCapacity;
    public readonly byte[] Reserved_0;
    public readonly ushort GameRevision; // Used by Dsi Titles
    public readonly byte RomVersion;
    public readonly InternalFlags InternalFlags;
    public readonly uint Arm9RomOffset;
    public readonly uint Arm9EntryAddress;
    public readonly uint Arm9LoadAddress;
    public readonly uint Arm9Size;
    public readonly uint Arm7RomOffset;
    public readonly uint Arm7EntryAddress;
    public readonly uint Arm7LoadAddress;
    public readonly uint Arm7Size;
    public readonly uint FileNameTableOffset;
    public readonly uint FileNameTableLength;
    public readonly uint FileAllocationTableOffset;
    public readonly uint FileAllocationTableLength;
    public readonly uint Arm9OverlayOffset;
    public readonly uint Arm9OverlayLength;
    public readonly uint Arm7OverlayOffset;
    public readonly uint Arm7OverlayLength;
    public readonly uint NormalCardControlRegisterSettings;
    public readonly uint SecureCardControlRegisterSettings;
    public readonly uint IconBannerOffset;
    public readonly ushort SecureAreaCrc; // 2K
    public readonly ushort SecureTransferTimeout;
    public readonly uint Arm9Autoload;
    public readonly uint Arm7Autoload;
    public readonly ulong SecureDisable;
    public readonly uint NtrRegionRomSize; // excluding dsi area
    public readonly uint HeaderSize;
    public readonly byte[] Reserved_1; // 56
    public readonly byte[] NintendoLogo; // 156
    public readonly ushort NintendoLogoCrc;
    public readonly ushort HeaderCrc;
    public readonly byte[] DebuggerReserved; // 32

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{nameof(GameTitle)}: {GameTitle}");
        sb.AppendLine($"{nameof(GameCode)}: {GameCode}");
        sb.AppendLine($"{nameof(MakerCode)}: {MakerCode}");
        sb.AppendLine($"{nameof(Unitcode)}: {Unitcode}");
        sb.AppendLine($"{nameof(EncryptionSeedSelect)}: 0x{EncryptionSeedSelect:x}");
        sb.AppendLine($"{nameof(DeviceCapacity)}: 0x{EncryptionSeedSelect:x}");

        sb.AppendLine($"{nameof(GameRevision)}: {GameRevision}");
        sb.AppendLine($"{nameof(RomVersion)}: {RomVersion}");
        sb.AppendLine($"{nameof(InternalFlags)}: {InternalFlags}");
        sb.AppendLine($"{nameof(Arm9RomOffset)}: 0x{Arm9RomOffset:x}");
        sb.AppendLine($"{nameof(Arm9EntryAddress)}: 0x{Arm9EntryAddress:x}");
        sb.AppendLine($"{nameof(Arm9LoadAddress)}: 0x{Arm9LoadAddress:x}");
        sb.AppendLine($"{nameof(Arm9Size)}: 0x{Arm9Size:x}");
        sb.AppendLine($"{nameof(Arm7RomOffset)}: 0x{Arm7RomOffset:x}");
        sb.AppendLine($"{nameof(Arm7EntryAddress)}: 0x{Arm7EntryAddress:x}");
        sb.AppendLine($"{nameof(Arm7LoadAddress)}: 0x{Arm7LoadAddress:x}");
        sb.AppendLine($"{nameof(Arm7Size)}: 0x{Arm7Size:x}");
        sb.AppendLine($"{nameof(FileNameTableOffset)}: 0x{FileNameTableOffset:x}");
        sb.AppendLine($"{nameof(FileNameTableLength)}: 0x{FileNameTableLength:x}");
        sb.AppendLine($"{nameof(FileAllocationTableOffset)}: 0x{FileAllocationTableOffset:x}");
        sb.AppendLine($"{nameof(FileAllocationTableLength)}: 0x{FileAllocationTableLength:x}");
        sb.AppendLine($"{nameof(Arm9OverlayOffset)}: 0x{Arm9OverlayOffset:x}");
        sb.AppendLine($"{nameof(Arm9OverlayLength)}: 0x{Arm9OverlayLength:x}");
        sb.AppendLine($"{nameof(Arm7OverlayOffset)}: 0x{Arm7OverlayOffset:x}");
        sb.AppendLine($"{nameof(Arm7OverlayLength)}: 0x{Arm7OverlayLength:x}");
        sb.AppendLine($"{nameof(NormalCardControlRegisterSettings)}: 0x{NormalCardControlRegisterSettings:x}");
        sb.AppendLine($"{nameof(SecureCardControlRegisterSettings)}: 0x{SecureCardControlRegisterSettings:x}");
        sb.AppendLine($"{nameof(IconBannerOffset)}: 0x{IconBannerOffset:x}");
        sb.AppendLine($"{nameof(SecureAreaCrc)}: 0x{SecureAreaCrc:x}");
        sb.AppendLine($"{nameof(SecureTransferTimeout)}: 0x{SecureTransferTimeout:x}");
        sb.AppendLine($"{nameof(Arm9Autoload)}: 0x{Arm9Autoload:x}");
        sb.AppendLine($"{nameof(Arm7Autoload)}: 0x{Arm7Autoload:x}");
        sb.AppendLine($"{nameof(SecureDisable)}: 0x{SecureDisable:x}");
        sb.AppendLine($"{nameof(NtrRegionRomSize)}: 0x{NtrRegionRomSize:x}");
        sb.AppendLine($"{nameof(HeaderSize)}: 0x{HeaderSize:x}");


        sb.AppendLine($"{nameof(NintendoLogoCrc)}: 0x{NintendoLogoCrc:x}");
        sb.AppendLine($"{nameof(HeaderCrc)}: 0x{HeaderCrc:x}");

        return sb.ToString();
    }
}