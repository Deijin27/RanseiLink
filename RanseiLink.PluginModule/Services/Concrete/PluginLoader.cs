using RanseiLink.PluginModule.Api;
using System.Reflection;

namespace RanseiLink.PluginModule.Services.Concrete;

public class PluginLoader : IPluginLoader
{
    public List<Assembly> LoadAssemblies(DirectoryInfo directory, out List<AssemblyLoadFailureInfo> failedToLoad)
    {
        var files = directory.GetFiles("*.dll");

        var assemblies = new List<Assembly>();
        failedToLoad = new List<AssemblyLoadFailureInfo>();

        foreach (var file in files)
        {
            try
            {
                assemblies.Add(Assembly.LoadFile(file.FullName));
            }
            catch (Exception e)
            {
                failedToLoad.Add(new AssemblyLoadFailureInfo(file, e));
            }
        }
        return assemblies;
    }

    public IReadOnlyCollection<TPlugin> GenericLoadPlugins<TPlugin, TPluginAttribute>(DirectoryInfo pluginDirectory, out PluginLoadFailureInfo failures) 
        where TPluginAttribute : Attribute
    {
        var plugins = new List<TPlugin>();
        failures = new PluginLoadFailureInfo();

        if (!pluginDirectory.Exists)
        {
            return plugins;
        }

        List<Assembly> assemblies = LoadAssemblies(pluginDirectory, out var assemblyLoadFailures);
        failures.FailedToLoadAssemblies.AddRange(assemblyLoadFailures);

        List<Type> types = new();

        foreach (var assembly in assemblies)
        {
            try
            {
                Type[] assemblyTypes = assembly.GetTypes();
                types.AddRange(assemblyTypes);
            }
            catch (Exception e)
            {
                failures.FailedToGetTypesFromAssembly.Add(new(assembly, e));
            }
        }

        foreach (var type in types)
        {
            if (type.IsAbstract)
            {
                continue;
            }

            if (typeof(TPlugin).IsAssignableFrom(type) && type.GetCustomAttribute<TPluginAttribute>() != null)
            {
                try
                {
                    plugins.Add((TPlugin)Activator.CreateInstance(type)!);
                }
                catch (Exception e)
                {
                    failures.FailedToActivateTypes.Add(new(type, e));
                }
            }
        }

        return plugins;
    }

    private static readonly DirectoryInfo _pluginDirectory = new(Path.Combine(Environment.CurrentDirectory, "Plugins"));
    private List<PluginInfo>? _cache;
    private PluginLoadFailureInfo? _failureCache;
    public IReadOnlyCollection<PluginInfo> LoadPlugins(out PluginLoadFailureInfo failures)
    {
        if (_cache != null && _failureCache != null)
        {
            failures = _failureCache;
            return _cache;
        }
        _cache = new List<PluginInfo>();
        foreach (var plugin in GenericLoadPlugins<Api.IPlugin, PluginAttribute>(_pluginDirectory, out failures))
        {
            var attribute = plugin.GetType().GetCustomAttribute<PluginAttribute>()!;
            _cache.Add(new PluginInfo(plugin, attribute.DisplayName, attribute.Author, attribute.Version));
        }
        _failureCache = failures;
        return _cache;
    }
}
