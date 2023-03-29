using RanseiLink.Windows.Dialogs;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using System.Windows;

namespace RanseiLink.Windows.Services.Concrete;

internal class PluginService : IPluginService
{
    private readonly IPluginFormLoader _pluginFormLoader;
    public PluginService(IPluginFormLoader pluginFormLoader)
    {
        _pluginFormLoader = pluginFormLoader;
    }
    public bool RequestOptions(IPluginForm optionForm)
    {
        PluginFormInfo info = _pluginFormLoader.FormToInfo(optionForm);

        var pluginDialog = new PluginDialog
        {
            DataContext = info,
            Owner = Application.Current.MainWindow,
        };
        var result = pluginDialog.ShowDialog() == true;
        _pluginFormLoader.InfoToForm(info);
        return result;
    }
}
