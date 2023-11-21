using RanseiLink.Core.Archive;
using System.IO;

namespace RanseiLink.Core.Graphics;

/// <summary>
/// Utilities for working G2DR Link Archives
/// </summary>
public static class G2DR
{
    public static (NCGR Ncgr, NCLR Nclr) LoadImgFromFolder(string linkFolder)
    {
        var ncgrPath = Path.Combine(linkFolder, "0003.ncgr");
        if (new FileInfo(ncgrPath).Length == 0)
        {
            ncgrPath = Path.Combine(linkFolder, "0001.ncgr");
        }

        var ncgr = NCGR.Load(ncgrPath);
        var nclr = NCLR.Load(Path.Combine(linkFolder, "0004.nclr"));
        return (ncgr, nclr);
    }

    public static void SaveImgToFolder(string linkFolder, NCGR ncgr, NCLR nclr, bool useNcgrSlot1 = false)
    {
        File.Create("0000.nanr").Dispose();
    }

    public static (NCER Ncer, NCGR Ncgr, NCLR Nclr) LoadCellFromFolder(string linkFolder)
    {
        var (ncgr, nclr) = LoadImgFromFolder(linkFolder);
        var ncer = NCER.Load(Path.Combine(linkFolder, "0002.ncer"));
        return (ncer, ncgr, nclr);
    }

    public static (NANR Nanr, NCER Ncer, NCGR Ncgr, NCLR Nclr) LoadAnimFromFolder(string linkFolder)
    {
        var (ncer, ncgr, nclr) = LoadCellFromFolder(linkFolder);
        var nanr = NANR.Load(Path.Combine(linkFolder, "0000.nanr"));
        return (nanr, ncer, ncgr, nclr);
    }

    public static (NCGR Ncgr, NCLR Nclr) LoadImgFromFile(string linkFilePath)
    {
        var temp = FileUtil.GetTemporaryDirectory();
        try
        {
            LINK.Unpack(linkFilePath, temp);
            return LoadImgFromFolder(temp);
        }
        finally
        {
            Directory.Delete(temp, true);
        }
    }

    public static (NCER Ncer, NCGR Ncgr, NCLR Nclr) LoadCellFromFile(string linkFilePath)
    {
        var temp = FileUtil.GetTemporaryDirectory();
        try
        {
            LINK.Unpack(linkFilePath, temp);
            return LoadCellFromFolder(temp);
        }
        finally
        {
            Directory.Delete(temp, true);
        }
    }

    public static (NANR Nanr, NCER Ncer, NCGR Ncgr, NCLR Nclr) LoadAnimFromFile(string linkFilePath)
    {
        var temp = FileUtil.GetTemporaryDirectory();
        try
        {
            LINK.Unpack(linkFilePath, temp);
            return LoadAnimFromFolder(temp);
        }
        finally
        {
            Directory.Delete(temp, true);
        }
    }
}


