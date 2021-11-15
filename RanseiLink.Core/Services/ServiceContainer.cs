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
    private static IServiceContainer _instance;
    public static IServiceContainer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ServiceContainer();
            }
            return _instance;
        }
        set => _instance = value;
    }

    private readonly Dictionary<Type, object> Singletons = new Dictionary<Type, object>();
    private readonly Dictionary<Type, Func<object>> TransientFactories = new Dictionary<Type, Func<object>>();

    public void RegisterSingleton<T>(T instance)
    {
        Singletons[typeof(T)] = instance;
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
        else if (TransientFactories.TryGetValue(typeof(T), out Func<object> factory))
        {
            result = (T)factory();
            return true;
        }
        result = default;
        return false;
    }
}
