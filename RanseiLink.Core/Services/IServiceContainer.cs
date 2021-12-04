using System;

namespace RanseiLink.Core.Services;

public interface IServiceContainer
{
    /// <summary>
    /// Register a singleton of type <typeparamref name="T"/>. All resolves return the same instance.
    /// </summary>
    /// <param name="instance">Object to register</param>
    void RegisterSingleton<T>(T instance);

    /// <summary>
    /// Register a lazy singleton of type <typeparamref name="T"/>. After the first resolve, subsequent resolves will return the same object.
    /// </summary>
    /// <param name="factory">Factory which produces singleton when invoked</param>
    void RegisterLazySingleton<T>(Func<T> factory);

    /// <summary>
    /// Register a transient object of type <typeparamref name="T"/>. On each resolve an object is created using the factory.
    /// </summary>
    /// <param name="factory"></param>
    void RegisterTransient<T>(Func<T> factory);

    /// <summary>
    /// Resolve a registered object of type <typeparamref name="T"/>
    /// </summary>
    /// <exception cref="ServiceNotRegisteredException"/>
    /// <returns>Instance of object</returns>
    T Resolve<T>();

    /// <summary>
    /// Resolve a registered object of type <typeparamref name="T"/>
    /// </summary>
    /// <param name="result"></param>
    /// <returns>False if service is not registered. True otherwise.</returns>
    bool TryResolve<T>(out T result);
}
