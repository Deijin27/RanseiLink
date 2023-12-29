#nullable enable
using System.Windows;

namespace RanseiLink.Windows.Services.Concrete;

public class DispatcherService : IDispatcherService
{
    public void Invoke(Action action)
    {
        Application.Current.Dispatcher.Invoke(action);
    }

    public T Invoke<T>(Func<T> func)
    {
        return Application.Current.Dispatcher.Invoke(func);
    }

    public async Task InvokeAsync(Action action)
    {
        await Application.Current.Dispatcher.InvokeAsync(action);
    }

    public async Task<T> InvokeAsync<T>(Func<T> func)
    {
        return await Application.Current.Dispatcher.InvokeAsync(func);
    }
}
