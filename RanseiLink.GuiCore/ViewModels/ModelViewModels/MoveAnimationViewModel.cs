using RanseiLink.Core.Enums;
using RanseiLink.Core.Models;

namespace RanseiLink.GuiCore.ViewModels;
public partial class MoveAnimationViewModel()
{
    public void SetModel(MoveAnimationId id, MoveAnimation model)
    {
        _id = id;
        _model = model;
        RaiseAllPropertiesChanged();
    }
}
