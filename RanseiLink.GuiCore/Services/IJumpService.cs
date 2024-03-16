#nullable enable
namespace RanseiLink.GuiCore.Services;

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

    /// <summary>
    /// Jump to MsgGridEditorModule and set the filter text to the provided value
    /// </summary>
    /// <param name="filter">Filter text to set</param>
    /// <param name="regex">Whether the filter text is a regular expression</param>
    void JumpToMessageFilter(string filter, bool regex = false);
}
