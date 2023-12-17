using RanseiLink.Core.Services;

namespace RanseiLink.Windows.Services;

public interface IAnimGuiManager
{
    public bool IsOverriden(AnimationTypeId type, int id);
    public bool Export(AnimationTypeId type, int id);
    public bool Import(AnimationTypeId type, int id);
    public bool RevertToDefault(AnimationTypeId type, int id);
}