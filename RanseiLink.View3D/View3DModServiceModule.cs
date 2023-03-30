using DryIoc;
using RanseiLink.Core;

namespace RanseiLink.View3D;

public  class View3DModServiceModule : IModule
{
    public void Load(IRegistrator builder)
    {
        builder.Register<ISceneRenderer, SceneRenderer>();
    }
}
