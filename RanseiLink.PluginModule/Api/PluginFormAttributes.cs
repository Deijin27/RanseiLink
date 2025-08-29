using System.Collections;

namespace RanseiLink.PluginModule.Api;

public abstract class BasePluginOptionAttribute : Attribute
{
    public BasePluginOptionAttribute(string displayName, string description)
    {
        DisplayName = displayName;
        Description = description;
    }

    public string DisplayName { get; }
    public string Description { get; }
}

/// <summary>
/// Valid on a <see cref="bool"/> property. Option to be displayed as a checkbox.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class BoolOptionAttribute : BasePluginOptionAttribute
{
    public BoolOptionAttribute(string displayName, string description = "") : base(displayName, description)
    {
    }
}

/// <summary>
/// Valid on a <see cref="int"/> property. Option to be displayed as a number picker.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class IntOptionAttribute : BasePluginOptionAttribute
{
    public IntOptionAttribute(string displayName, string description = "", int minimumValue = 0, int maximumValue = int.MaxValue)
        : base(displayName, description)
    {
        MinimumValue = minimumValue;
        MaximumValue = maximumValue;
    }

    public int MinimumValue { get; }
    public int MaximumValue { get; }
}

/// <summary>
/// Valid on a <see cref="string"/> property. Option to be displayed as a text box, where the user can type a string.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class StringOptionAttribute : BasePluginOptionAttribute
{
    public StringOptionAttribute(string displayName, string description = "", int maxLength = int.MaxValue)
        : base(displayName, description)
    {
        MaxLength = maxLength;
    }

    public int MaxLength { get; }
}

/// <summary>
/// Valid on any property. Option to be displayed as a combobox, providing the user with a list of options from which they pick 1.
/// Provide an <see cref="ICollection"/> (e.g. <see cref="System.Collections.Generic.List{T}"/>) 
/// of options and the value of the property will be set to the users choice, 
/// thus this property's type match or be more generic than the type of the collection items.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class CollectionOptionAttribute : BasePluginOptionAttribute
{
    /// <summary>
    /// Create a combo attribute using the property name of a valid <see cref="ICollection"/> on the same form as this property.
    /// </summary>
    public CollectionOptionAttribute(string displayName, string itemsSourcePropertyName, string description = "")
        : base(displayName, description)
    {
        ItemsSourcePropertyName = itemsSourcePropertyName;
        Mode = ItemsSourceMode.MemberName;
    }

    /// <summary>
    /// Create a collection attribute using a array passed directly to the attribute as <paramref name="itemsSource"/>
    /// </summary>
    public CollectionOptionAttribute(string displayName, object itemsSource, string description = "")
        : base(displayName, description)
    {
        ItemsSource = itemsSource as ICollection;
        Mode = ItemsSourceMode.Collection;
    }

    public string? ItemsSourcePropertyName { get; }
    public ICollection? ItemsSource { get; }
    public ItemsSourceMode Mode { get; }

    public enum ItemsSourceMode
    {
        MemberName,
        Collection
    }
}

/// <summary>
/// Valid on a <see cref="string"/> property. Text to be displayed as a text block inline with the options.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class TextAttribute : Attribute
{
    public TextAttribute()
    {
    }
}

/// <summary>
/// Valid on a <see cref="string"/> property. Displayed as a header.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class HeaderAttribute : Attribute
{
    public HeaderAttribute()
    {
    }
}