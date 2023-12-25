using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using RanseiLink.XP.Views;

namespace RanseiLink.XP.Services;
public class PluginService(IPluginFormLoader pluginFormLoader) : IAsyncPluginService
{
    public async Task<bool> RequestOptions(IPluginForm optionForm)
    {
        PluginFormInfo info = pluginFormLoader.FormToInfo(optionForm);

        var pluginDialog = new PluginDialog
        {
            DataContext = info,
        };
        await pluginDialog.ShowDialog(App.MainWindow);
        var result = pluginDialog.DialogResult;
        pluginFormLoader.InfoToForm(info);
        return result;
    }
}
