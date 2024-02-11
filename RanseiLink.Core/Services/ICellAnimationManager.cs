using FluentResults;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Resources;

namespace RanseiLink.Core.Services;

public interface ICellAnimationManager
{
    void ClearOverride(AnimationTypeId type, int id);
    void Export(AnimationTypeId type, int id, string outputFolder, RLAnimationFormat format);
    (DataFile? AnimationLink, DataFile? BackgroundLink) GetDataFile(AnimationTypeId type, int id);
    Result ImportBackgroundOnly(AnimationTypeId type, int id, string backgroundImg);
    Result ImportAnimAndBackground(AnimationTypeId type, int id, string animationXml);
    void SetOverride(AnimationTypeId type, int id, string? animationLink = null, string? backgroundLink = null);
}


