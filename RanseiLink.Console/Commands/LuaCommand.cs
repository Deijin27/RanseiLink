namespace RanseiLink.Console.Commands;

[Command("lua", Description = "Run given lua script.")]
public class LuaCommand : ICommand
{
    private readonly ICurrentModService _currentModService;
    public LuaCommand(ICurrentModService currentModService)
    {
        _currentModService = currentModService;
    }

    [CommandParameter(0, Description = "Absolute path to entry point script", Name = "path")]
    public string FilePath { get; set; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (!_currentModService.TryGetCurrentModServiceGetter(out var serviceGetter))
        {
            console.WriteLine("No mod selected");
            return default;
        }

        var luaService = serviceGetter.Get<ILuaService>();

        luaService.RunScript(FilePath);

        console.WriteLine("Script executed successfully.");
        return default;
    }
}