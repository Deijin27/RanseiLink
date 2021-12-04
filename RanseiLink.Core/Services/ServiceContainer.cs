using System;
using System.Collections.Generic;

namespace RanseiLink.Core.Services;

public class ServiceNotRegisteredException : Exception
{
    public ServiceNotRegisteredException(string message) : base(message)
    {
    }
}

public class ServiceContainer : IServiceContainer
{
    private readonly Dictionary<Type, object> Singletons = new Dictionary<Type, object>();
    private readonly Dictionary<Type, Func<object>> SingletonFactories = new Dictionary<Type, Func<object>>();
    private readonly Dictionary<Type, Func<object>> TransientFactories = new Dictionary<Type, Func<object>>();

    public void RegisterSingleton<T>(T instance)
    {
        Singletons[typeof(T)] = instance;
    }

    public void RegisterLazySingleton<T>(Func<T> factory)
    {
        SingletonFactories[typeof(T)] = () => factory();
    }

    public void RegisterTransient<T>(Func<T> factory)
    {
        TransientFactories[typeof(T)] = () => factory();
    }

    public T Resolve<T>()
    {
        if (!TryResolve(out T result))
        {
            throw new ServiceNotRegisteredException($"{GetType().FullName} does not have a registered service of type '{typeof(T).FullName}'");
        }
        return result;
    }

    public bool TryResolve<T>(out T result)
    {
        if (Singletons.TryGetValue(typeof(T), out object value))
        {
            result = (T)value;
            return true;
        }
        if (SingletonFactories.TryGetValue(typeof(T), out Func<object> singletonFactory))
        {
            result = (T)singletonFactory();
            Singletons[typeof(T)] = result;
            return true;
        }
        if (TransientFactories.TryGetValue(typeof(T), out Func<object> factory))
        {
            result = (T)factory();
            return true;
        }
        result = default;
        return false;
    }
}
