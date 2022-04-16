using Autofac;
using Autofac.Core;
using System.Collections.Generic;

namespace RanseiLink.Core.Services
{
    /// <summary>
    /// Factory to create a kernel for a specific mod.
    /// This is a child kernel of the global kernel, you only need to add mod-specific modules to it
    /// By default it includes the ModInfo instance as a service which can be loaded into constructors
    /// </summary>
    public interface IModServiceGetterFactory
    {
        /// <summary>
        /// Adds a module that should be included in the kernel on creation
        /// </summary>
        /// <param name="module"></param>
        void AddModule(IModule module);
        /// <summary>
        /// Creates a new child kernel of the global kernel, with the mod services included
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        IServiceGetter Create(ModInfo mod);
    }

    public class ModServiceGetterFactory : IModServiceGetterFactory
    {
        private readonly List<IModule> _modules = new List<IModule>();

        public ModServiceGetterFactory()
        {
        }

        public void AddModule(IModule module)
        {
            _modules.Add(module);
        }

        public IServiceGetter Create(ModInfo mod)
        {
            var serviceGetter = new ServiceGetter();
            var scope = ContainerProvider.Container.BeginLifetimeScope(builder =>
            {
                builder.RegisterInstance(mod).As<ModInfo>();
                builder.RegisterInstance(serviceGetter).As<IServiceGetter>();
                foreach (IModule module in _modules)
                {
                    builder.RegisterModule(module);
                }
            });
            serviceGetter.Services = scope;

            return serviceGetter;
        }
    }
}