namespace Core.Nds
{
    public interface INdsFactory
    {
        INds Create(string ndsFilePath);
    }
}