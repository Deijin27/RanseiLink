﻿using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("scenariowarrior", Description = "Get data on a given ScenarioPokemon for a given Scenario.")]
public class ScenarioWarriorCommand(ICurrentModService currentModService) : ICommand
{
    [CommandParameter(0, Description = "Scenario ID.", Name = "scenarioid")]
    public ScenarioId ScenarioId { get; set; }

    [CommandParameter(1, Description = "ScenarioWarrior ID.", Name = "id")]
    public int ScenarioWarriorId { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IScenarioWarriorService>();

        var model = service.Retrieve((int)ScenarioId).Retrieve(ScenarioWarriorId);

        console.Render(model, $"Scenario = {ScenarioId}, Entry = {ScenarioWarriorId}");

        return default;
    }
}
