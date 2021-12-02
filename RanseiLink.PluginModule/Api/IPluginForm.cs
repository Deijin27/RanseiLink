
namespace RanseiLink.PluginModule.Api;

public interface IPluginForm
{
    /// <summary>
    /// Title to be displayed at the top of the form
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Text to be displayed on the proceed button. 
    /// When pressed, the proceed button causes <see cref="IPluginService.RequestOptions(IPluginForm)"/> to return true.
    /// </summary>
    string ProceedButtonText { get; }

    /// <summary>
    /// Text to be displayed on the cancel button. 
    /// When pressed, the cancel button causes <see cref="IPluginService.RequestOptions(IPluginForm)"/> to return false.
    /// </summary>
    string CancelButtonText { get; }
}
