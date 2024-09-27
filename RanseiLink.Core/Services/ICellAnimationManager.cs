using FluentResults;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;

namespace RanseiLink.Core.Services;

public interface ICellAnimationManager
{
    void ClearOverride(AnimationTypeInfo info, int id);
    void Export(AnimationTypeInfo info, int id, string outputFolder, RLAnimationFormat format);
    (DataFile? AnimationLink, DataFile? BackgroundLink) GetDataFile(AnimationTypeInfo info, int id);
    void SetOverride(AnimationTypeInfo info, int id, string? animationLink = null, string? backgroundLink = null);
}


