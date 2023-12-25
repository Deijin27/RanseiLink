using RanseiLink.Core.Archive;

namespace RanseiLink.Core.Graphics;

public enum NcgrSlot
{
    /// <summary>
    /// Use 0001.ncgr
    /// </summary>
    Slot1,
    /// <summary>
    /// Use 0003.ncgr
    /// </summary>
    Slot3,
    /// <summary>
    /// Infer from an folder that already has files. Whichever file is not empty. 
    /// Does not work if both are empty, or neither are empty
    /// </summary>
    Infer
}

/// <summary>
/// Utilities for working G2DR Link Archives
/// </summary>
public static class G2DR
{
    private const string _nanrFile = "0000.nanr";
    private const string _ncgrFile1 = "0001.ncgr";
    private const string _ncerFile = "0002.ncer";
    private const string _ncgrFile3 = "0003.ncgr";
    private const string _nclrFile = "0004.nclr";
    private const string _nscrFile = "0005.nscr";

    public static NCLR LoadPaletteFromFolder(string linkFolder)
    {
        return NCLR.Load(Path.Combine(linkFolder, _nclrFile));
    }

    public static NCGR LoadPixelsFromFolder(string linkFolder, NcgrSlot ncgrSlot = NcgrSlot.Infer)
    {
        var ncgrPath1 = Path.Combine(linkFolder, _ncgrFile1);
        var ncgrPath3 = Path.Combine(linkFolder, _ncgrFile3);
        if (ncgrSlot == NcgrSlot.Infer)
        {
            ncgrSlot = InferSlot(ncgrPath1, ncgrPath3);
        }
        string ncgrPath = ncgrSlot == NcgrSlot.Slot1 ? ncgrPath1 : ncgrPath3;
        return NCGR.Load(ncgrPath);
    }

    public static NCER LoadCellFromFolder(string linkFolder)
    {
        return NCER.Load(Path.Combine(linkFolder, _ncerFile));
    }

    public static NANR LoadAnimFromFolder(string linkFolder)
    {
        return NANR.Load(Path.Combine(linkFolder, _nanrFile));
    }

    public static (NCGR Ncgr, NCLR Nclr) LoadImgFromFolder(string linkFolder, NcgrSlot ncgrSlot = NcgrSlot.Infer)
    {
        var nclr = LoadPaletteFromFolder(linkFolder);
        var ncgr = LoadPixelsFromFolder(linkFolder);
        return (ncgr, nclr);
    }

    public static (NCER Ncer, NCGR Ncgr, NCLR Nclr) LoadCellImgFromFolder(string linkFolder, NcgrSlot ncgrSlot = NcgrSlot.Infer)
    {
        var img = LoadImgFromFolder(linkFolder, ncgrSlot);
        var ncer = LoadCellFromFolder(linkFolder);
        return (ncer, img.Ncgr, img.Nclr);
    }

    public static (NANR Nanr, NCER Ncer, NCGR Ncgr, NCLR Nclr) LoadAnimImgFromFolder(string linkFolder, NcgrSlot ncgrSlot = NcgrSlot.Infer)
    {
        var cell = LoadCellImgFromFolder(linkFolder, ncgrSlot);
        var nanr = LoadAnimFromFolder(linkFolder);
        return (nanr, cell.Ncer, cell.Ncgr, cell.Nclr);
    }

    private static NcgrSlot InferSlot(string ncgrFile1, string ncgrFile3)
    {
        var ncgrInfo1 = new FileInfo(ncgrFile1);
        var ncgrInfo3 = new FileInfo(ncgrFile3);
        if (!ncgrInfo1.Exists || !ncgrInfo3.Exists)
        {
            throw new System.Exception($"Cannot infer location to place ncgr if either of the files don't exist '{ncgrFile1}'");
        }
        if (ncgrInfo1.Length == 0 && ncgrInfo3.Length == 0)
        {
            throw new System.Exception($"Cannot infer location to place ncgr if both are empty. You must explicitly specify which location if this is the case. '{ncgrFile1}'");
        }
        else if (ncgrInfo1.Length == 0)
        {
            return NcgrSlot.Slot3;
        }
        else if (ncgrInfo3.Length == 0)
        {
            return NcgrSlot.Slot1;
        }
        else
        {
            throw new System.Exception($"Cannot infer location to place ncgr if both are non-empty. You must explicitly specify which location if this is the case. '{ncgrFile1}'");
        }
    }

    public static void SavePaletteToFolder(string linkFolder, NCLR nclr)
    {
        var nanrFile = Path.Combine(linkFolder, _nanrFile);
        var ncgrFile1 = Path.Combine(linkFolder, _ncgrFile1);
        var ncerFile = Path.Combine(linkFolder, _ncerFile);
        var ncgrFile3 = Path.Combine(linkFolder, _ncgrFile3);
        var nclrFile = Path.Combine(linkFolder, _nclrFile);
        var nscrFile = Path.Combine(linkFolder, _nscrFile);

        // don't overwrite, this allows this method to be used to modify existing folders
        FileUtil.CreateEmptyFile(nanrFile);
        FileUtil.CreateEmptyFile(ncerFile);
        FileUtil.CreateEmptyFile(nscrFile);
        FileUtil.CreateEmptyFile(ncgrFile1);
        FileUtil.CreateEmptyFile(ncgrFile3);

        nclr.Save(nclrFile);
    }

    public static void SaveImgToFolder(string linkFolder, NCGR ncgr, NCLR nclr, NcgrSlot ncgrSlot)
    {
        var ncgrFile1 = Path.Combine(linkFolder, _ncgrFile1);
        var ncgrFile3 = Path.Combine(linkFolder, _ncgrFile3);

        if (ncgrSlot == NcgrSlot.Infer)
        {
            ncgrSlot = InferSlot(ncgrFile1 , ncgrFile3);
        }

        if (ncgrSlot == NcgrSlot.Slot1)
        {
            ncgr.Save(ncgrFile1);
        }
        else
        {
            ncgr.Save(ncgrFile3);
        }

        SavePaletteToFolder(linkFolder, nclr);
    }

    public static void SaveCellToFolder(string linkFolder, NCER ncer, NCGR ncgr, NCLR nclr, NcgrSlot ncgrSlot)
    {
        ncer.Save(Path.Combine(linkFolder, _ncerFile));
        SaveImgToFolder(linkFolder, ncgr, nclr, ncgrSlot);
    }

    public static void SaveAnimToFolder(string linkFolder, NANR nanr, NCER ncer, NCGR ncgr, NCLR nclr, NcgrSlot ncgrSlot)
    {
        nanr.Save(Path.Combine(linkFolder, _nanrFile));
        SaveCellToFolder(linkFolder, ncer, ncgr, nclr, ncgrSlot);
    }

    public static (NCGR Ncgr, NCLR Nclr) LoadImgFromFile(string linkFilePath, NcgrSlot ncgrSlot = NcgrSlot.Infer)
    {
        var temp = FileUtil.GetTemporaryDirectory();
        try
        {
            LINK.Unpack(linkFilePath, temp);
            return LoadImgFromFolder(temp, ncgrSlot);
        }
        finally
        {
            Directory.Delete(temp, true);
        }
    }

    public static (NCER Ncer, NCGR Ncgr, NCLR Nclr) LoadCellImgFromFile(string linkFilePath, NcgrSlot ncgrSlot = NcgrSlot.Infer)
    {
        var temp = FileUtil.GetTemporaryDirectory();
        try
        {
            LINK.Unpack(linkFilePath, temp);
            return LoadCellImgFromFolder(temp, ncgrSlot);
        }
        finally
        {
            Directory.Delete(temp, true);
        }
    }

    public static (NANR Nanr, NCER Ncer, NCGR Ncgr, NCLR Nclr) LoadAnimFromFile(string linkFilePath, NcgrSlot ncgrSlot = NcgrSlot.Infer)
    {
        var temp = FileUtil.GetTemporaryDirectory();
        try
        {
            LINK.Unpack(linkFilePath, temp);
            return LoadAnimImgFromFolder(temp, ncgrSlot);
        }
        finally
        {
            Directory.Delete(temp, true);
        }
    }
}


