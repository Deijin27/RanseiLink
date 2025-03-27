using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using RanseiLink.View3D;
using RanseiLink.Windows.Views.ModelViews.Map;

namespace RanseiLink.Windows.Services.Concrete;

public class MapViewerService(ISceneRenderer sceneRenderer, IAsyncDialogService dialogService) : IMapViewerService
{
    public async Task ShowDialog(MapId id)
    {
        sceneRenderer.Configure(0);

        var window = new Map3DWindow(sceneRenderer);
        var result = sceneRenderer.LoadScene(id);
        if (result.IsFailed)
        {
            await dialogService.ShowMessageBox(MessageBoxSettings.Ok("Failed to load model", result.ToString(), MessageBoxType.Warning));
            return;
        }
        window.ShowDialog();
    }

    public async Task ShowDialog(BattleConfigId id)
    {
        sceneRenderer.Configure(0);

        var window = new Map3DWindow(sceneRenderer);
        var result = sceneRenderer.LoadScene(id);
        if (result.IsFailed)
        {
            await dialogService.ShowMessageBox(MessageBoxSettings.Ok("Failed to load model", result.ToString(), MessageBoxType.Warning));
            return;
        }
        window.ShowDialog();
    }

    public async Task ShowDialog(GimmickObjectId id, int variant)
    {
        sceneRenderer.Configure(SceneRenderOptions.DrawGrid);

        var window = new Map3DWindow(sceneRenderer);
        var result = sceneRenderer.LoadScene(id, variant);
        if (result.IsFailed)
        {
            await dialogService.ShowMessageBox(MessageBoxSettings.Ok("Failed to load model", result.ToString(), MessageBoxType.Warning));
            return;
        }
        window.ShowDialog();
    }
}