#nullable enable
using DryIoc;
using System;

namespace RanseiLink.Core.Services;

/// <summary>
/// Wrapper for whatever IOC is used
/// </summary>
public interface IServiceGetter : IDisposable
{
    T Get<T>();
}

public class ServiceGetter : IServiceGetter
{
    private readonly IContainer _services;
    public ServiceGetter(IContainer services)
    {
        _services = services;
    }

    public void Dispose()
    {
        _services.Dispose();
        GC.SuppressFinalize(this);
    }

    public T Get<T>()
    {
        return _services.Resolve<T>();
    }
}