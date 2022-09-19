using DryIoc;
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
            // RegistrySharing.CloneButKeepCache important to make the child container disposable
            // childDefaultServiceKey=null important because with a value multiple registrations to the same interface dont work
            var builder = ContainerProvider.Container.CreateChild(RegistrySharing.CloneButKeepCache, null);

            builder.RegisterInstance(mod);
            builder.RegisterInstance<IServiceGetter>(serviceGetter);
            foreach (IModule module in _modules)
            {
                builder.RegisterModule(module);
            }
            serviceGetter.Services = builder;

            return serviceGetter;
        }
    }
}