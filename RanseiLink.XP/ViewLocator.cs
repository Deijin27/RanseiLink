#nullable enable
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace RanseiLink.XP;
public class ViewLocator : IDataTemplate
{
    private static readonly Dictionary<Type, Type> _registry = [];

    public Control? Build(object? data)
    {
        if (data == null)
        {
            return null;
        }
        var name = data.GetType().FullName!.Replace("ViewModel", "View").Replace("RanseiLink.GuiCore", "RanseiLink.XP");
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }
        else
        {
            return new TextBlock { Text = "Not Found: " + name };
        }
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}