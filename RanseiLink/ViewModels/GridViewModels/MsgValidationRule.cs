using RanseiLink.Core.Text;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using System.IO;
using System;

namespace RanseiLink.ViewModels;

public class MsgValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is not BindingGroup bindingGroup 
            || bindingGroup.Items == null 
            || bindingGroup.Items.Count < 1 
            || bindingGroup.Items[0] is not Message msg)
        {
            return new ValidationResult(false, "Binding error. This is an error with RanseiLink");
        }

        return ValidateMessage(msg);

    }

    public static ValidationResult ValidateMessage(Message msg)
    {
        var stream = new BinaryWriter(new MemoryStream());

        ValidationResult result;

        try
        {
            var pnaWriter = new PnaTextWriter(stream);
            pnaWriter.WriteMessage(msg, false);

            result = ValidationResult.ValidResult;
        }
        catch (Exception e)
        {
            result = new ValidationResult(false, e.Message);
        }
        finally
        {
            stream.Dispose();
        }

        return result;
    }
}
