using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.PluginModule.Api;
using System;
using System.Linq;

namespace MassActionPlugin;

internal class MassAction
{
    private readonly PokemonId[] pokemonIds = EnumUtil.GetValuesExceptDefaults<PokemonId>().ToArray();

    private MassActionOptionForm options;
    private IServiceGetter _services;

    public void Run(IPluginContext context)
    {
        _services = context.Services;
        var optionService = context.Services.Get<IPluginService>();
        options = new();
        if (!optionService.RequestOptions(options))
        {
            return;
        }

        var dialogService = context.Services.Get<IDialogService>();

        dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo(isIndeterminate:true));
            if (options.Target == ConstOptions.MaxLink)
            {
                progress.Report(new ProgressInfo("Setting max link values..."));
                HandleMaxLink();
            }
            else if (options.Target == ConstOptions.IVs)
            {
                progress.Report(new ProgressInfo("Setting IV values..."));
                HandleIVs();
            }
            else if (options.Target == ConstOptions.Capacity)
            {
                progress.Report(new ProgressInfo("Setting capacity values..."));
                HandleCapacity();
            }
            else if (options.Target == ConstOptions.InitLink)
            {
                progress.Report(new ProgressInfo("Setting initial link values..."));
                HandleInitLink();
            }

            progress.Report(new ProgressInfo(progress:100, isIndeterminate:false, statusText:"Done!"));

        });
    }

    private void HandleMaxLink()
    {
        var maxLinkService = _services.Get<IMaxLinkService>();

        foreach (var maxLinkTable in maxLinkService.Enumerate())
        {
            foreach (PokemonId pid in pokemonIds)
            {
                if (options.Mode == ConstOptions.AtLeast)
                {
                    if (maxLinkTable.GetMaxLink(pid) < options.Value)
                    {
                        maxLinkTable.SetMaxLink(pid, options.Value);
                    }
                }
                else if (options.Mode == ConstOptions.Exact)
                {
                    maxLinkTable.SetMaxLink(pid, options.Value);
                }
            }
        }

        maxLinkService.Save();
    }

    private int[] DecideScenarios()
    {
        if (options.Scenario == ConstOptions.AllScenarios)
        {
            return EnumUtil.GetValues<ScenarioId>().Select(i => (int)i).ToArray();
        }
        else
        {
            return new int[] { (int)System.Enum.Parse<ScenarioId>(options.Scenario) };
        }
    }

    private void HandleIVs()
    {
        var scenarioPokemonService = _services.Get<IScenarioPokemonService>();

        foreach (var scenarioId in DecideScenarios())
        {
            var childService = scenarioPokemonService.Retrieve(scenarioId);
            foreach (var sp in childService.Enumerate())
            {
                if (options.Mode == ConstOptions.AtLeast)
                {
                    if (sp.HpIv < options.Value)
                    {
                        sp.HpIv = options.Value;
                    }
                    if (sp.AtkIv < options.Value)
                    {
                        sp.AtkIv = options.Value;
                    }
                    if (sp.DefIv < options.Value)
                    {
                        sp.DefIv = options.Value;
                    }
                    if (sp.SpeIv < options.Value)
                    {
                        sp.SpeIv = options.Value;
                    }
                }
                else if (options.Mode == ConstOptions.Exact)
                {
                    sp.HpIv = options.Value;
                    sp.AtkIv = options.Value;
                    sp.DefIv = options.Value;
                    sp.SpeIv = options.Value;
                }
            }
        }

        scenarioPokemonService.Save();
    }

    private void HandleCapacity()
    {
        var baseWarriorService = _services.Get<IBaseWarriorService>();

        if (options.Mode == ConstOptions.AtLeast)
        {
            foreach (var warrior in baseWarriorService.Enumerate())
            {
                if (warrior.Capacity < options.Value)
                {
                    warrior.Capacity = options.Value;
                }
            }
        }
        else if (options.Mode == ConstOptions.Exact)
        {
            foreach (var warrior in baseWarriorService.Enumerate())
            {
                if (warrior.Capacity != options.Value)
                {
                    warrior.Capacity = options.Value;
                }
            }
        }

        baseWarriorService.Save();
    }

    private void HandleInitLink()
    {
        var scenarioPokemonService = _services.Get<IScenarioPokemonService>();

        var targetExp = LinkCalculator.CalculateExp(options.Value);

        foreach (var scenarioId in DecideScenarios())
        {
            var childService = scenarioPokemonService.Retrieve(scenarioId);
            foreach (var sp in childService.Enumerate())
            {
                // get current link and round
                var currentLink = Convert.ToInt32(LinkCalculator.CalculateLink(sp.Exp));
                
                if (options.Mode == ConstOptions.AtLeast)
                {
                    if (currentLink < options.Value)
                    {
                        sp.Exp = targetExp;
                    }
                }
                else if (options.Mode == ConstOptions.Exact)
                {
                    if (currentLink != options.Value)
                    {
                        sp.Exp = targetExp;
                    }
                }
            }
        }

        scenarioPokemonService.Save();
    }
}

