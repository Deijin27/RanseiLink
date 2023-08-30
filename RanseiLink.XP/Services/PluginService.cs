using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using RanseiLink.XP.Views;
using System.Threading.Tasks;

namespace RanseiLink.XP.Services;
public class PluginService : IAsyncPluginService
{
    private readonly IPluginFormLoader _pluginFormLoader;
    public PluginService(IPluginFormLoader pluginFormLoader)
    {
        _pluginFormLoader = pluginFormLoader;
    }
    public async Task<bool> RequestOptions(IPluginForm optionForm)
    {
        PluginFormInfo info = _pluginFormLoader.FormToInfo(optionForm);

        var pluginDialog = new PluginDialog
        {
            DataContext = info,
        };
        await pluginDialog.ShowDialog(App.MainWindow);
        var result = pluginDialog.DialogResult;
        _pluginFormLoader.InfoToForm(info);
        return result;
    }
}
