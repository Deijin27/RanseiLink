#nullable enable
using Avalonia.Threading;

namespace RanseiLink.XP.Services;

public class DispatcherService : IDispatcherService
{
    public void Invoke(Action action)
    {
        Dispatcher.UIThread.Invoke(action);
    }

    public T Invoke<T>(Func<T> func)
    {
        return Dispatcher.UIThread.Invoke(func);
    }

    public async Task InvokeAsync(Action action)
    {
        await Dispatcher.UIThread.InvokeAsync(action);
    }

    public async Task<T> InvokeAsync<T>(Func<T> func)
    {
        return await Dispatcher.UIThread.InvokeAsync(func);
    }
}
