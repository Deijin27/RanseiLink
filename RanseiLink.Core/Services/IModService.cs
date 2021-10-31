using System.Collections.Generic;

namespace RanseiLink.Core.Services
{
    public interface IModService
    {
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
        /// Get info on all existing mods.
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
        /// Commit current mod to rom
        /// </summary>
        /// <param name="romPath">Path of rom to commit mod to</param>
        void Commit(ModInfo modInfo, string romPath);

        void LoadRom(string path, ModInfo mod);

        /// <summary>
        /// Update the mod with folder stored in <paramref name="modInfo"/> to have the info stored in <paramref name="modInfo"/>
        /// </summary>
        /// <param name="modInfo"></param>
        void Update(ModInfo modInfo);
        ModInfo CreateBasedOn(ModInfo baseMod, string name = "", string version = "", string author = "");
    }
}
