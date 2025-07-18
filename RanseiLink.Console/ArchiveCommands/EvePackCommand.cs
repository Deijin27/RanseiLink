﻿using RanseiLink.Core.Models;

namespace RanseiLink.Console.ArchiveCommands;

[Command("eve pack")]
public class EvePackCommand : ICommand
{

    [CommandParameter(0, Description = "Path of folder containing necessary files.", Name = "eveFolder")]
    public string EveFolder { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        EVE.Pack(EveFolder);

        console.WriteLine("Complete!");

        return default;
    }
}
