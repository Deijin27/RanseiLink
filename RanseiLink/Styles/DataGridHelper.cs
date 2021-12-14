using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RanseiLink.Styles;

/// <summary>
/// Attached properties that set all columns EditingElementStyle at once.
/// Necessary because these datagrid columns don't correctly use the implicit styles for controls
/// </summary>
public class DataGridHelper : DependencyObject
{
    #region ComboBoxColumn

    private static readonly DependencyProperty ComboBoxColumnEditingStyleProperty = DependencyProperty.RegisterAttached(
        "ComboBoxColumnEditingStyle", 
        typeof(Style), 
        typeof(DataGridHelper), 
        new PropertyMetadata(ComboBoxColumnEditingStylePropertyChangedCallback)
    );

    private static void ComboBoxColumnEditingStylePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var grid = (DataGrid)d;
        if (e.OldValue == null && e.NewValue != null)
        {
            grid.Columns.CollectionChanged += (_, __) => UpdateComboBoxColumnStyles(grid);
        }
    }

    public static void SetComboBoxColumnEditingStyle(DependencyObject element, Style value)
    {
        element.SetValue(ComboBoxColumnEditingStyleProperty, value);
    }
    public static Style GetComboBoxColumnEditingStyle(DependencyObject element)
    {
        return (Style)element.GetValue(ComboBoxColumnEditingStyleProperty);
    }

    private static void UpdateComboBoxColumnStyles(DataGrid grid)
    {
        var style = GetComboBoxColumnEditingStyle(grid);
        foreach (var column in grid.Columns.OfType<DataGridComboBoxColumn>())
        {
            var newStyle = new Style
            {
                BasedOn = column.EditingElementStyle,
                TargetType = style.TargetType
            };

            foreach (var setter in style.Setters.OfType<Setter>())
            {
                newStyle.Setters.Add(setter);
            }

            column.EditingElementStyle = newStyle;
        }
    }

    #endregion

    #region TextColumn

    private static readonly DependencyProperty TextColumnEditingStyleProperty = DependencyProperty.RegisterAttached(
        "TextColumnEditingStyle",
        typeof(Style),
        typeof(DataGridHelper),
        new PropertyMetadata(TextColumnEditingStylePropertyChangedCallback)
    );

    private static void TextColumnEditingStylePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var grid = (DataGrid)d;
        if (e.OldValue == null && e.NewValue != null)
        {
            grid.Columns.CollectionChanged += (_, __) =>
            {
                UpdateTextColumnStyles(grid);
            };
        }
    }

    public static void SetTextColumnEditingStyle(DependencyObject element, Style value)
    {
        element.SetValue(TextColumnEditingStyleProperty, value);
    }
    public static Style GetTextColumnEditingStyle(DependencyObject element)
    {
        return (Style)element.GetValue(TextColumnEditingStyleProperty);
    }

    private static void UpdateTextColumnStyles(DataGrid grid)
    {
        var style = GetTextColumnEditingStyle(grid);
        foreach (var column in grid.Columns.OfType<DataGridTextColumn>())
        {
            var newStyle = new Style
            {
                BasedOn = column.EditingElementStyle,
                TargetType = style.TargetType
            };

            foreach (var setter in style.Setters.OfType<Setter>())
            {
                newStyle.Setters.Add(setter);
            }

            column.EditingElementStyle = newStyle;
        }
    }

    #endregion

}
