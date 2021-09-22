using System.IO;

namespace Core.Services
{
    internal static class Constants
    {
        public const string DataFolderPath = "data";

        public static readonly string AppearPokemonRomPath = Path.Combine(DataFolderPath, "AppearPokemon.dat");
        public static readonly string BaseBushouRomPath = Path.Combine(DataFolderPath, "BaseBushou.dat");
        public static readonly string BaseBushouMaxSyncTableRomPath = Path.Combine(DataFolderPath, "BaseBushouMaxSyncTable.dat");
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
        public static readonly string MapRomPath = Path.Combine(DataFolderPath, "Map.dat");
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
        public static readonly string MoveEffectRomPath = Path.Combine(DataFolderPath, "WazaEffect.dat");
        public static readonly string MoveRangeRomPath = Path.Combine(DataFolderPath, "WazaRange.dat");

        #region Scenario Folder
        public const int ScenarioCount = 11;
        public const int ScenarioPokemonCount = 200;
        public static string ScenarioPathFromId(int id) => Path.Combine(DataFolderPath, "Scenario", $"Scenario{id.ToString().PadLeft(2, '0')}");
        public static string ScenarioPokemonPathFromId(int scenarioId) => Path.Combine(ScenarioPathFromId(scenarioId), "ScenarioPokemon.dat");
        #endregion

    }
}
