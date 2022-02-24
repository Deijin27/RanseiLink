using RanseiLink.Core;
using RanseiLink.Core.Enums;
using RanseiLink.Core.Models.Interfaces;
using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using RanseiLink.PluginModule.Api;
using System.Linq;

namespace MassActionPlugin;

internal class MassAction
{
    private readonly PokemonId[] pokemonIds = EnumUtil.GetValuesExceptDefaults<PokemonId>().ToArray();
    private readonly WarriorId[] warriorIds = EnumUtil.GetValuesExceptDefaults<WarriorId>().ToArray();
    private readonly ScenarioId[] scenarioIds = EnumUtil.GetValues<ScenarioId>().ToArray();

    private MassActionOptionForm options;
    private IModServiceContainer _dataService;

    public void Run(IPluginContext context)
    {
        var optionService = context.ServiceContainer.Resolve<IPluginService>();
        options = new();
        if (!optionService.RequestOptions(options))
        {
            return;
        }

        var dialogService = context.ServiceContainer.Resolve<IDialogService>();
        _dataService = context.ServiceContainer.Resolve<DataServiceFactory>()(context.ActiveMod);

        dialogService.ProgressDialog(progress =>
        {
            progress.Report(new ProgressInfo(IsIndeterminate:true));
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

            progress.Report(new ProgressInfo(Progress:100, IsIndeterminate:false, StatusText:"Done!"));

        });
    }

    private void HandleMaxLink()
    {
        using IDisposableMaxLinkService maxLinkService = _dataService.MaxLink.Disposable();

        foreach (WarriorId wid in warriorIds)
        {
            IMaxLink maxLinkTable = maxLinkService.Retrieve(wid);
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
            maxLinkService.Save(wid, maxLinkTable);
        }
    }

    private void HandleIVs()
    {
        using IDisposableScenarioPokemonService scenarioPokemonService = _dataService.ScenarioPokemon.Disposable();

        foreach (ScenarioId scenario in scenarioIds)
        {
            for (int i = 0; i < Constants.ScenarioPokemonCount; i++)
            {
                IScenarioPokemon sp = scenarioPokemonService.Retrieve(scenario, i);
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
                scenarioPokemonService.Save(scenario, i, sp);
            }
        }
    }

    private void HandleCapacity()
    {
        using IDisposableBaseWarriorService service = _dataService.BaseWarrior.Disposable();

        if (options.Mode == ConstOptions.AtLeast)
        {
            foreach (WarriorId id in warriorIds)
            {
                var warrior = service.Retrieve(id);
                if (warrior.Capacity < options.Value)
                {
                    warrior.Capacity = options.Value;
                    service.Save(id, warrior);
                }
            }
        }
        else if (options.Mode == ConstOptions.Exact)
        {
            foreach (WarriorId id in warriorIds)
            {
                var warrior = service.Retrieve(id);
                if (warrior.Capacity != options.Value)
                {
                    warrior.Capacity = options.Value;
                    service.Save(id, warrior);
                }
            }
        }
    }
}

