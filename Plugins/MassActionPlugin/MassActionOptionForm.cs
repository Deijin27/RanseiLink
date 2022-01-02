using RanseiLink.PluginModule.Api;

namespace MassActionPlugin;

public static class ConstOptions
{
    public const string MaxLink = "Max links of warriors with all pokemon";
    public const string IVs = "IVs of all warrior's pokemon";
    public const string Capacity = "Capacity of all warriors";

    public const string AtLeast = "At least (anything higher will be retained)";
    public const string Exact = "Exactly (all to this exact value)";
}


public class MassActionOptionForm : IPluginForm
{
    public string Title => "Mass Action Configuration";
    public string ProceedButtonText => "Go!";
    public string CancelButtonText => "Cancel";


    [CollectionOption("Set all", new[] { ConstOptions.MaxLink, ConstOptions.IVs, ConstOptions.Capacity })]
    public string Target { get; set; } = ConstOptions.MaxLink;

    [CollectionOption("To", new[] { ConstOptions.AtLeast, ConstOptions.Exact })]
    public string Mode { get; set; } = ConstOptions.AtLeast;

    [UIntOption("The Value")]
    public uint Value { get; set; } = 0;

    
}
