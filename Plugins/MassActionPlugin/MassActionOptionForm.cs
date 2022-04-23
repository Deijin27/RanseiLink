using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.PluginModule.Api;
using System.Collections.Generic;
using System.Linq;

namespace MassActionPlugin;

public static class ConstOptions
{
    public const string MaxLink = "Max links of warriors with all pokemon";
    public const string IVs = "IVs of all warrior's pokemon";
    public const string Capacity = "Capacity of all warriors";
    public const string InitLink = "Initial links (will be converted to exp value)";

    public const string AtLeast = "At least (anything higher will be retained)";
    public const string Exact = "Exactly (all to this exact value)";

    public const string AllScenarios = "AllScenarios";
}


public class MassActionOptionForm : IPluginForm
{
    public string Title => "Mass Action Configuration";
    public string ProceedButtonText => "Go!";
    public string CancelButtonText => "Cancel";


    [CollectionOption("Set all", new[] { ConstOptions.MaxLink, ConstOptions.IVs, ConstOptions.Capacity, ConstOptions.InitLink })]
    public string Target { get; set; } = ConstOptions.MaxLink;

    [CollectionOption("To", new[] { ConstOptions.AtLeast, ConstOptions.Exact })]
    public string Mode { get; set; } = ConstOptions.AtLeast;

    public List<string> ScenarioOptions { get; } = EnumUtil.GetValues<ScenarioId>().Select(i => i.ToString()).Append(ConstOptions.AllScenarios).ToList();

    [CollectionOption("In Scenario (Where applicable)", nameof(ScenarioOptions))]
    public string Scenario { get; set; } = ConstOptions.AllScenarios;

    [IntOption("The Value")]
    public int Value { get; set; } = 0;

    
}
