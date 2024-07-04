#nullable enable
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;

namespace RanseiLink.Windows.ViewModels;

/// <summary>
/// Workaround because datagrid doesn't seem to respont do INotifyDataErrorInfo
/// </summary>
public class WorkaroundValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is not BindingGroup bindingGroup 
            || bindingGroup.Items == null 
            || bindingGroup.Items.Count < 1 
            || bindingGroup.Items[0] is not INotifyDataErrorInfo vm)
        {
            return new ValidationResult(false, "Binding error. This is an error with RanseiLink");
        }

        if (vm.HasErrors)
        {
            return new ValidationResult(false, vm.GetErrors(string.Empty));
        }
        return ValidationResult.ValidResult;
    }
}
