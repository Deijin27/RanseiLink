namespace RanseiLink.Core.Services.ModelServices;
public partial class MoveAnimationService
{
    public override string IdToName(int id)
    {
        return Retrieve(id).Animation.ToString();
    }
}
