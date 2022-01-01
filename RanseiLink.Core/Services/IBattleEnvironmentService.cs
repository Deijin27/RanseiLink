
using System.Collections.Generic;

namespace RanseiLink.Core.Services;

public interface IBattleEnvironmentService
{
    public ICollection<BattleEnvironment> GetBattleEnvironments();
}
