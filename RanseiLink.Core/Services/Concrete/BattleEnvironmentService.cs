using System.Collections.Generic;
using System.IO;

namespace RanseiLink.Core.Services.Concrete;

internal class BattleEnvironmentService : IBattleEnvironmentService
{
    private readonly ModInfo _mod;
    public BattleEnvironmentService(ModInfo mod)
    {
        _mod = mod;
    }

    public ICollection<BattleEnvironment> GetBattleEnvironments()
    {
        var files = Directory.GetFiles(Path.Combine(_mod.FolderPath, Constants.DataFolderPath, "map"));
        List<BattleEnvironment> result = new();
        foreach (var file in files)
        {
            if (BattleEnvironment.TryParse(Path.GetFileName(file), out var environment))
            {
                result.Add(environment);
            }
        }

        return result;
    }
}
