using RanseiLink.PluginModule.Api;
using System.Collections;
using System.Reflection;

namespace RanseiLink.PluginModule.Services.Concrete;

public class PluginFormLoader : IPluginFormLoader
{
    public PluginFormInfo FormToInfo(IPluginForm form)
    {
        var items = new List<IPluginFormItem>();

        foreach (var property in form.GetType().GetProperties())
        {
            var boolAttribute = property.GetCustomAttribute<BoolOptionAttribute>();
            if (boolAttribute != null && property.PropertyType == typeof(bool))
            {
                items.Add(new BoolPluginFormItem(
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
                items.Add(new IntPluginFormItem(
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
                items.Add(new StringPluginFormItem(
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

                items.Add(new CollectionPluginFormItem(
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
                items.Add(new TextPluginFormItem(
                    Value: property.GetValue(form) as string
                    ));
            }

            var headerAttribute = property.GetCustomAttribute<HeaderAttribute>();
            if (headerAttribute != null)
            {
                items.Add(new HeaderPluginFormItem(
                    Value: property.GetValue(form) as string
                    ));
            }
        }

        return new(form, items);
    }

    public IPluginForm InfoToForm(PluginFormInfo info)
    {
        var form = info.Form;
        foreach (PluginFormOptionItem item in info.Items.OfType<PluginFormOptionItem>())
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
