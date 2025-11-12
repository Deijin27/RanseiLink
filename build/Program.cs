namespace RanseiLink.Build;

internal class Program
{
    public static int Main(string[] args)
    {
        var build = new Build();
        build.Run(args);
        return 0;
    }
}
