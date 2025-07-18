﻿// This file is automatically generated

#nullable enable

using RanseiLink.Core.Enums;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("basewarrior", Description = "Get data on a given BaseWarrior.")]
public class BaseWarriorCommand(ICurrentModService currentModService) : ICommand
{
    [CommandParameter(0, Description = "BaseWarrior ID.", Name = "id")]
    public WarriorId Id { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IBaseWarriorService>();

        var model = service.Retrieve((int)Id);

        console.Render(model, Id);

        return default;
    }
}
