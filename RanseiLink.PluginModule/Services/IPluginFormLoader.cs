using RanseiLink.PluginModule.Api;
using System.Collections;
using System.Collections.Generic;
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

public class UIntPluginFormItem : PluginFormOptionItem
{
    public UIntPluginFormItem(PropertyInfo member, string displayName, string description, uint value, uint minValue, uint maxValue) : base(member, displayName, description)
    {
        Value = value;
        MinValue = minValue;
        MaxValue = maxValue;
    }
    public uint Value { get; set; }
    public uint MinValue { get; }
    public uint MaxValue { get; }
}

public class StringPluginFormItem : PluginFormOptionItem
{
    public StringPluginFormItem(PropertyInfo member, string displayName, string description, string value, int maxLength) : base(member, displayName, description)
    {
        Value = value;
        MaxLength = maxLength;
    }
    public string Value { get; set; }
    public int MaxLength { get; }
}

public class CollectionPluginFormItem : PluginFormOptionItem
{
    public CollectionPluginFormItem(PropertyInfo member, string displayName, string description, object value, ICollection values) : base(member, displayName, description)
    {
        Value = value;
        Values = values;
    }
    public object Value { get; set; }
    public ICollection Values { get; }
}

public record TextPluginFormItem(string Value) : IPluginFormItem;
public record PluginFormGroup(string GroupName, List<IPluginFormItem> Items);
public record PluginFormInfo(IPluginForm Form, List<IPluginFormItem> UngroupedItems, List<PluginFormGroup> Groups);