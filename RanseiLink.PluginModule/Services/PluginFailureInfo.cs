using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace RanseiLink.PluginModule.Services;

public record AssemblyLoadFailureInfo(FileInfo File, Exception Exception);
public record TypeLoadFailureInfo(Assembly Assembly, Exception Exception);
public record TypeActivateFailureInfo(Type Type, Exception Exception);

public class PluginLoadFailureInfo
{
    public List<AssemblyLoadFailureInfo> FailedToLoadAssemblies { get; } = new();
    public List<TypeLoadFailureInfo> FailedToGetTypesFromAssembly { get; } = new();
    public List<TypeActivateFailureInfo> FailedToActivateTypes { get; } = new();
    public bool AnyFailures => FailedToLoadAssemblies.Count > 0 || FailedToGetTypesFromAssembly.Count > 0 || FailedToActivateTypes.Count > 0;

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (FailedToLoadAssemblies.Count > 0)
        {
            sb.AppendLine("\nFailed to load assemblies:\n");
            foreach (var assembly in FailedToLoadAssemblies)
            {
                sb.AppendLine(assembly.File.FullName);
                sb.AppendLine(assembly.Exception.Message);
            }
        }
        if (FailedToGetTypesFromAssembly.Count > 0)
        {
            sb.AppendLine("\nFailed to get types from assembly:\n");
            foreach (var t in FailedToGetTypesFromAssembly)
            {
                sb.AppendLine(t.Assembly.FullName);
                sb.AppendLine(t.Exception.Message);
            }
        }
        if (FailedToActivateTypes.Count > 0)
        {
            sb.AppendLine("\nFailed to activate types:\n");
            foreach (var t in FailedToActivateTypes)
            {
                sb.AppendLine(t.Type.FullName);
                sb.AppendLine(t.Exception.Message);
            }
        }
        return sb.ToString();
    }
}
