using System;

namespace RanseiLink.Core.Services;

public interface IServiceContainer
{
    void RegisterSingleton<T>(T instance);
    void RegisterTransient<T>(Func<T> factory);
    T Resolve<T>();
    bool TryResolve<T>(out T result);
}
