
namespace RanseiLink.GuiCore.Services;

public interface IDispatcherService
{
    void Invoke(Action action);
    T Invoke<T>(Func<T> func);
    Task InvokeAsync(Action action);
    Task<T> InvokeAsync<T>(Func<T> func);
}