#nullable enable
using RanseiLink.Core.Services;

namespace RanseiLink.GuiCore.Services;

public interface IAnimGuiManager
{
    public bool IsOverriden(AnimationTypeId type, int id);
    public Task<bool> Export(AnimationTypeId type, int id);
    public Task<bool> Import(AnimationTypeId type, int id);
    public Task<bool> RevertToDefault(AnimationTypeId type, int id);
}