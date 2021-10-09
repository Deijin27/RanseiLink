
namespace Core.Nds
{
    public class NdsFactory : INdsFactory
    {
        public INds Create(string ndsFilePath)
        {
            return new Nds(ndsFilePath);
        }
    }
}
