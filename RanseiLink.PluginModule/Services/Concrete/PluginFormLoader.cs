using RanseiLink.PluginModule.Api;
using System.Collections;
using System.Reflection;

namespace RanseiLink.PluginModule.Services.Concrete;

public class PluginFormLoader : IPluginFormLoader
{
    private static PluginFormGroup GetOrAdd(Dictionary<string, PluginFormGroup> dict, string key)
    {
        if (dict.TryGetValue(key, out PluginFormGroup? value))
            return value;

        var newGroup = new PluginFormGroup(key, new());
        dict[key] = newGroup;
        return newGroup;
    }

    public PluginFormInfo FormToInfo(IPluginForm form)
    {
        Dictionary<string, PluginFormGroup> groups = new();

        foreach (var property in form.GetType().GetProperties())
        {
            var boolAttribute = property.GetCustomAttribute<BoolOptionAttribute>();
            if (boolAttribute != null && property.PropertyType == typeof(bool))
            {
                GetOrAdd(groups, boolAttribute.Group).Items.Add(new BoolPluginFormItem(
                    member: property,
                    displayName: boolAttribute.DisplayName,
                    description: boolAttribute.Description,
                    value: (bool)(property.GetValue(form)!)
                    ));
                continue;
            }

            var intAttribute = property.GetCustomAttribute<IntOptionAttribute>();
            if (intAttribute != null && property.PropertyType == typeof(int))
            {
                GetOrAdd(groups, intAttribute.Group).Items.Add(new IntPluginFormItem(
                    member: property,
                    displayName: intAttribute.DisplayName,
                    description: intAttribute.Description,
                    value: (int)property.GetValue(form)!,
                    minValue: intAttribute.MinimumValue,
                    maxValue: intAttribute.MaximumValue
                    ));
                continue;
            }

            var stringAttribute = property.GetCustomAttribute<StringOptionAttribute>();
            if (stringAttribute != null && property.PropertyType == typeof(string))
            {
                GetOrAdd(groups, stringAttribute.Group).Items.Add(new StringPluginFormItem(
                    member: property,
                    displayName: stringAttribute.DisplayName,
                    description: stringAttribute.Description,
                    value: property.GetValue(form) as string,
                    maxLength: stringAttribute.MaxLength
                    ));
                continue;
            }

            var comboAttribute = property.GetCustomAttribute<CollectionOptionAttribute>();
            if (comboAttribute != null)
            {
                ICollection? values = comboAttribute.Mode switch
                {
                    CollectionOptionAttribute.ItemsSourceMode.Collection => comboAttribute.ItemsSource,
                    CollectionOptionAttribute.ItemsSourceMode.MemberName => form.GetType().GetProperty(comboAttribute.ItemsSourcePropertyName!)?.GetValue(form) as ICollection,
                    _ => throw new System.Exception($"Invalid {typeof(CollectionOptionAttribute.ItemsSourceMode).FullName}"),
                };

                GetOrAdd(groups, comboAttribute.Group).Items.Add(new CollectionPluginFormItem(
                    member: property,
                    displayName: comboAttribute.DisplayName,
                    description: comboAttribute.Description,
                    value: property.GetValue(form),
                    values: values
                    ));
            }

            var textAttribute = property.GetCustomAttribute<TextAttribute>();
            if (textAttribute != null)
            {
                GetOrAdd(groups, textAttribute.Group).Items.Add(new TextPluginFormItem(
                    Value: property.GetValue(form) as string
                    ));
            }
        }

        if (groups.TryGetValue("", out var ungrouped))
        {
            return new(form, ungrouped.Items, groups.Values.Where(i => i.GroupName != "").ToList());
        }
        return new(form, new(), groups.Values.ToList());
    }

    public IPluginForm InfoToForm(PluginFormInfo info)
    {
        var form = info.Form;
        foreach (PluginFormOptionItem item in info.Groups.SelectMany(i => i.Items).Concat(info.UngroupedItems).OfType<PluginFormOptionItem>())
        {
            switch (item)
            {
                case BoolPluginFormItem i:
                    item.Member.SetValue(form, i.Value);
                    break;
                case IntPluginFormItem i:
                    item.Member.SetValue(form, i.Value);
                    break;
                case StringPluginFormItem i:
                    item.Member.SetValue(form, i.Value);
                    break;
                case CollectionPluginFormItem i:
                    item.Member.SetValue(form, i.Value);
                    break;
            }
        }
        return form;
    }
}
