
namespace RanseiLink.PluginModule.Api;

/// <summary>
/// Provides plugin writers with tools that work trans context
/// I should add/merge the dialog service stuff into this.
/// </summary>
public interface IPluginService
{
    /// <summary>
    /// Displays form to user
    /// </summary>
    /// <param name="optionForm"></param>
    /// <returns>True if proceed button clicked, False if cancel button clicked</returns>
    public bool RequestOptions(IPluginForm optionForm);
}
