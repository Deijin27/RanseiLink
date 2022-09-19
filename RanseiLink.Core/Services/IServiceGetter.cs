using DryIoc;
using System;

namespace RanseiLink.Core.Services
{
    /// <summary>
    /// Wrapper for whatever IOC is used
    /// </summary>
    public interface IServiceGetter : IDisposable
    {
        T Get<T>();
    }

    public class ServiceGetter : IServiceGetter
    {
        public IContainer Services { get; set; }
        public ServiceGetter()
        {
        }

        public void Dispose()
        {
            Services.Dispose();
            GC.SuppressFinalize(this);
        }

        public T Get<T>()
        {
            return Services.Resolve<T>();
        }
    }
}