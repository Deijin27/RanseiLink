﻿// This file is automatically generated

#nullable enable

using RanseiLink.Core.Enums;
using RanseiLink.Core.Services.ModelServices;

namespace RanseiLink.Console.ModelCommands;

[Command("eventspeaker", Description = "Get data on a given EventSpeaker.")]
public class EventSpeakerCommand(ICurrentModService currentModService) : ICommand
{
    [CommandParameter(0, Description = "EventSpeaker ID.", Name = "id")]
    public EventSpeakerId Id { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!currentModService.TryGetCurrentModServiceGetter(out var services))
        {
            console.Output.WriteLine("No mod selected");
            return default;
        }

        var service = services.Get<IEventSpeakerService>();

        var model = service.Retrieve((int)Id);

        console.Render(model, Id);

        return default;
    }
}