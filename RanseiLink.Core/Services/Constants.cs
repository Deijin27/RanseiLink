using RanseiLink.Core.Enums;
using System;
using System.IO;

namespace RanseiLink.Core.Services;

public static class Constants
{
    static Constants()
    {
        RootFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RanseiLink");
        Directory.CreateDirectory(RootFolder);
    }

    public const string BannerInfoFile = "BannerInfo.xml";
    public const string BannerImageFile = "BannerImage.png";

    /// <summary>
    /// Root folder for application files in local app data
    /// </summary>
    public static readonly string RootFolder;

    public static string DefaultDataFolder(ConquestGameCode gameCode) => Path.Combine(RootFolder, "DefaultData", gameCode.CompatibilitySet().ToString());

    /// <summary>
    /// folder that decrypted blocks are stored in relative to .rlmod file. not in rom.
    /// </summary>
    public const string MsgFolderPath = "msg";
    public static string MsgBlockPathFromId(int id) => Path.Combine(MsgFolderPath, $"block{id}.bin");

    public const string DataFolderPath = "data";

    public const string GraphicsFolderPath = "graphics";

    public static readonly string MapFolderPath = Path.Combine(DataFolderPath, "map");

    public static readonly string AppearPokemonRomPath = Path.Combine(DataFolderPath, "AppearPokemon.dat");
    public static readonly string BaseBushouRomPath = Path.Combine(DataFolderPath, "BaseBushou.dat");
    public static readonly string MaxLinkRomPath = Path.Combine(DataFolderPath, "BaseBushouMaxSyncTable.dat");
    public static readonly string BaseBushouScoutRomPath = Path.Combine(DataFolderPath, "BaseBushouScout.dat");
    public static readonly string BuildingRomPath = Path.Combine(DataFolderPath, "Building.dat");
    public static readonly string BushouRomPath = Path.Combine(DataFolderPath, "Bushou.dat");
    public static readonly string EpisodeRomPath = Path.Combine(DataFolderPath, "Episode.dat");
    public static readonly string EventSpeakerRomPath = Path.Combine(DataFolderPath, "EventSpeaker.dat");
    public static readonly string GimmickRomPath = Path.Combine(DataFolderPath, "Gimmick.dat");
    public static readonly string GimmickObjectRomPath = Path.Combine(DataFolderPath, "GimmickObject.dat");
    public static readonly string GimmickRangeRomPath = Path.Combine(DataFolderPath, "GimmickRange.dat");
    public static readonly string IkusaMap01RomPath = Path.Combine(DataFolderPath, "IkusaMap01.dat");
    public static readonly string ItemRomPath = Path.Combine(DataFolderPath, "Item.dat");
    public static readonly string JinkeiRomPath = Path.Combine(DataFolderPath, "Jinkei.dat");
    public static readonly string KingdomRomPath = Path.Combine(DataFolderPath, "Kuni.dat");
    public static readonly string BattleConfigRomPath = Path.Combine(DataFolderPath, "Map.dat");
    public static readonly string MapPointRomPath = Path.Combine(DataFolderPath, "MapPoint.dat");
    public static readonly string MsgRomPath = Path.Combine(DataFolderPath, "MSG.DAT");
    public static readonly string PasswordRomPath = Path.Combine(DataFolderPath, "password.tbl");
    public static readonly string PokemonRomPath = Path.Combine(DataFolderPath, "Pokemon.dat");
    public static readonly string WarriorSkillRomPath = Path.Combine(DataFolderPath, "Saihai.dat");
    public static readonly string ScenarioRomPath = Path.Combine(DataFolderPath, "Scenario.dat");
    public static readonly string ScenarioBushouRomPath = Path.Combine(DataFolderPath, "ScenarioBushou.dat");
    public static readonly string SeiryokuRomPath = Path.Combine(DataFolderPath, "Seiryoku.dat");
    public static readonly string SkillRomPath = Path.Combine(DataFolderPath, "Skill.dat");
    public static readonly string SpAbilityRomPath = Path.Combine(DataFolderPath, "SpAbility.dat");
    public static readonly string AbilityRomPath = Path.Combine(DataFolderPath, "Tokusei.dat");
    public static readonly string TrainerRomPath = Path.Combine(DataFolderPath, "Trainer.dat");
    public static readonly string TrSkillRomPath = Path.Combine(DataFolderPath, "TrSkill.dat");
    public static readonly string MoveRomPath = Path.Combine(DataFolderPath, "Waza.dat");
    public static readonly string MoveAnimationRomPath = Path.Combine(DataFolderPath, "WazaEffect.dat");
    public static readonly string MoveRangeRomPath = Path.Combine(DataFolderPath, "WazaRange.dat");

    #region Scenario Folder
    public const int ScenarioCount = 11;
    public const int ScenarioPokemonCount = 200;
    public const int ScenarioWarriorCount = 210;
    public static string ScenarioPathFromId(int id) => Path.Combine(DataFolderPath, "Scenario", $"Scenario{(id).ToString().PadLeft(2, '0')}");
    public static string ScenarioPokemonPathFromId(int scenarioId) => Path.Combine(ScenarioPathFromId(scenarioId), "ScenarioPokemon.dat");
    public static string ScenarioWarriorPathFromId(int scenarioId) => Path.Combine(ScenarioPathFromId(scenarioId), "ScenarioBushou.dat");
    public static string ScenarioAppearPokemonPathFromId(int scenarioId) => Path.Combine(ScenarioPathFromId(scenarioId), "ScenarioAppearPokemon.dat");
    public static string ScenarioKingdomPathFromId(int scenarioId) => Path.Combine(ScenarioPathFromId(scenarioId), "ScenarioKuni.dat");
    public static string ScenarioBuildingPathFromId(int scenarioId) => Path.Combine(ScenarioPathFromId(scenarioId), "ScenarioBuilding.dat");
    #endregion

}