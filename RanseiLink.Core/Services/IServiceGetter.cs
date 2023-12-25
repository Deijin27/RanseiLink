using DryIoc;

namespace RanseiLink.Core.Services;

/// <summary>
/// Wrapper for whatever IOC is used
/// </summary>
public interface IServiceGetter : IDisposable
{
    T Get<T>();
}

public class ServiceGetter(IContainer services) : IServiceGetter
{
    public void Dispose()
    {
        services.Dispose();
        GC.SuppressFinalize(this);
    }

    public T Get<T>()
    {
        return services.Resolve<T>();
    }
}