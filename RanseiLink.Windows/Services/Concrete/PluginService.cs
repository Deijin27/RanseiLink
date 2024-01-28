#nullable enable
using RanseiLink.Windows.Dialogs;
using RanseiLink.PluginModule.Api;
using RanseiLink.PluginModule.Services;
using System.Windows;

namespace RanseiLink.Windows.Services.Concrete;

internal class AsyncPluginService(IPluginFormLoader pluginFormLoader) : IAsyncPluginService
{
    public async Task<bool> RequestOptions(IPluginForm optionForm)
    {
        PluginFormInfo info = pluginFormLoader.FormToInfo(optionForm);

        var result = await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var pluginDialog = new PluginDialog
            {
                DataContext = info,
                Owner = Application.Current.MainWindow,
            };
            return pluginDialog.ShowDialog() == true;
        });
        
        pluginFormLoader.InfoToForm(info);
        return result;
    }
}
