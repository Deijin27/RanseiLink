using RanseiLink.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RanseiLink.Services;

public interface IModuleLoadService
{
    List<EditorModule> LoadModules();
}

public class ModuleLoadService : IModuleLoadService
{
    public List<EditorModule> LoadModules()
    {
        var types = System.Reflection.Assembly
                .GetExecutingAssembly()
                .GetTypes();

        IEnumerable<Type> modules = types.Where(i => typeof(EditorModule).IsAssignableFrom(i) && !i.IsAbstract);

        List<EditorModule> result = new();

        foreach (Type t in modules)
        {
            var module = (EditorModule)Activator.CreateInstance(t);
            if (module.UniqueId == null)
            {
                throw new Exception("A Module ID is null");
            }
            result.Add(module);
        }

        return result;
    }
}