using RanseiLink.PluginModule.Api;
using System.Collections;
using System.Reflection;

namespace RanseiLink.PluginModule.Services;

public interface IPluginFormItem
{
}

public abstract class PluginFormOptionItem : IPluginFormItem
{
    public PluginFormOptionItem(PropertyInfo member, string displayName, string description)
    {
        Member = member;
        DisplayName = displayName;
        Description = description;
    }
    public PropertyInfo Member { get; }
    public string DisplayName { get; }
    public string Description { get; }
}

public interface IPluginFormLoader
{
    PluginFormInfo FormToInfo(IPluginForm form);
    IPluginForm InfoToForm(PluginFormInfo info);
}


public class BoolPluginFormItem : PluginFormOptionItem 
{ 
    public BoolPluginFormItem(PropertyInfo member, string displayName, string description, bool value) : base(member, displayName, description)
    {
        Value = value;
    }
    public bool Value { get; set; } 
}

public class IntPluginFormItem : PluginFormOptionItem
{
    public IntPluginFormItem(PropertyInfo member, string displayName, string description, int value, int minValue, int maxValue) : base(member, displayName, description)
    {
        Value = value;
        MinValue = minValue;
        MaxValue = maxValue;
    }
    public int Value { get; set; }
    public int MinValue { get; }
    public int MaxValue { get; }
}

public class StringPluginFormItem : PluginFormOptionItem
{
    public StringPluginFormItem(PropertyInfo member, string displayName, string description, string? value, int maxLength) : base(member, displayName, description)
    {
        Value = value;
        MaxLength = maxLength;
    }
    public string? Value { get; set; }
    public int MaxLength { get; }
}

public class CollectionPluginFormItem : PluginFormOptionItem
{
    public CollectionPluginFormItem(PropertyInfo member, string displayName, string description, object? value, ICollection? values) : base(member, displayName, description)
    {
        Value = value;
        Values = values;
    }
    public object? Value { get; set; }
    public ICollection? Values { get; }
}

public record TextPluginFormItem(string? Value) : IPluginFormItem;
public record HeaderPluginFormItem(string? Value) : IPluginFormItem;
public record PluginFormInfo(IPluginForm Form, List<IPluginFormItem> Items);