namespace RanseiLink.Core.Services;

[Flags]
public enum PatchOptions
{
    None = 0,
    IncludeSprites = 1
}

public interface IModManager
{
    /// <summary>
    /// Delete mod
    /// </summary>
    /// <param name="modInfo">Mod to delete</param>
    void Delete(ModInfo modInfo);

    /// <summary>
    /// Create a new mod.
    /// </summary>
    /// <param name="baseRomPath">Rom to act as a base for the mod.</param>
    /// <param name="name">Name of the mod</param>
    /// <param name="version">User defined version of the mod</param>
    /// <param name="author">Author of the mod</param>
    /// <returns>Info on created mod</returns>
    ModInfo Create(string baseRomPath, string name = "", string version = "", string author = "");

    /// <summary>
    /// Export current mod to destination folder
    /// </summary>
    /// <param name="destinationFolder">Folder to export into</param>
    /// <returns>Path of exported mod file</returns>
    string Export(ModInfo modInfo, string destinationFolder);

    /// <summary>
    /// Get info on all existing mods of the current version
    /// </summary>
    /// <returns>List of info on mods</returns>
    IList<ModInfo> GetAllModInfo();

    /// <summary>
    /// Import mod
    /// </summary>
    /// <param name="modPath">Path of mod to import</param>
    /// <returns>Info on imported mod</returns>
    ModInfo Import(string modPath);

    /// <summary>
    /// Update the mod with folder stored in <paramref name="modInfo"/> to have the info stored in <paramref name="modInfo"/>
    /// </summary>
    /// <param name="modInfo"></param>
    void Update(ModInfo modInfo);

    /// <summary>
    /// Create a new mod based on an existing, duplicating the data.
    /// </summary>
    /// <param name="baseMod">Mod to copy</param>
    /// <param name="name">Name of new mod</param>
    /// <param name="version">Version of new mod</param>
    /// <param name="author">Author of new mod</param>
    /// <returns></returns>
    ModInfo CreateBasedOn(ModInfo baseMod, string name = "", string version = "", string author = "");

    /// <summary>
    /// Upgrade mods to the latest version.
    /// </summary>
    /// <param name="mods">mods to upgrade</param>
    /// <param name="romPath">Path of rom to use as data source for mod upgrade</param>
    void UpgradeModsToLatestVersion(IEnumerable<ModInfo> mods, string romPath);

    /// <summary>
    /// Get info on all existing mods of previous versions (i.e. can be upgraded to current)
    /// </summary>
    /// <returns></returns>
    IList<ModInfo> GetModInfoPreviousVersions();
}