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
    private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
    private readonly Dictionary<Type, Func<object>> _singletonFactories = new Dictionary<Type, Func<object>>();
    private readonly Dictionary<Type, Func<object>> _transientFactories = new Dictionary<Type, Func<object>>();

    public void RegisterSingleton<T>(T instance)
    {
        _singletons[typeof(T)] = instance;
    }

    public void RegisterLazySingleton<T>(Func<T> factory)
    {
        _singletonFactories[typeof(T)] = () => factory();
    }

    public void RegisterTransient<T>(Func<T> factory)
    {
        _transientFactories[typeof(T)] = () => factory();
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
        if (_singletons.TryGetValue(typeof(T), out object value))
        {
            result = (T)value;
            return true;
        }
        if (_singletonFactories.TryGetValue(typeof(T), out Func<object> singletonFactory))
        {
            result = (T)singletonFactory();
            _singletons[typeof(T)] = result;
            return true;
        }
        if (_transientFactories.TryGetValue(typeof(T), out Func<object> factory))
        {
            result = (T)factory();
            return true;
        }
        result = default;
        return false;
    }
}
