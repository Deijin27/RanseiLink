using RanseiLink.Core.Services;
using System.Threading.Tasks;

namespace RanseiLink.Windows.Services;

public interface IAnimGuiManager
{
    public bool IsOverriden(AnimationTypeId type, int id);
    public Task<bool> Export(AnimationTypeId type, int id);
    public Task<bool> Import(AnimationTypeId type, int id);
    public Task<bool> RevertToDefault(AnimationTypeId type, int id);
}