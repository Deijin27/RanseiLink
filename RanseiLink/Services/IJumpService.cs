using RanseiLink.Core.Enums;

namespace RanseiLink.Services;

public interface IJumpService
{
    /// <summary>
    /// Jump to selector view model
    /// </summary>
    /// <param name="moduleId">Id of the editor module to choose</param>
    /// <param name="selectId">Id of the item within the selector view to select</param>
    void JumpTo(string moduleId, int selectId);

    /// <summary>
    /// Jump to module
    /// </summary>
    /// <param name="moduleId">Id of the editor module to choose</param>
    void JumpTo(string moduleId);

    void JumpToScenarioWarrior(ScenarioId scenario, uint id);
    void JumpToScenarioPokemon(ScenarioId scenario, uint id);
}
