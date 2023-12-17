using RanseiLink.Windows.Dialogs;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using System.Windows;

namespace RanseiLink.Windows.Services.Concrete;

internal class PluginService(IPluginFormLoader pluginFormLoader) : IPluginService
{
    public bool RequestOptions(IPluginForm optionForm)
    {
        PluginFormInfo info = pluginFormLoader.FormToInfo(optionForm);

        var pluginDialog = new PluginDialog
        {
            DataContext = info,
            Owner = Application.Current.MainWindow,
        };
        var result = pluginDialog.ShowDialog() == true;
        pluginFormLoader.InfoToForm(info);
        return result;
    }
}
