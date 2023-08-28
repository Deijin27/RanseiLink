using RanseiLink.Core.Services;
using System;

namespace RanseiLink.XP.ViewModels;

public interface IMainEditorViewModel
{
    void SetMod(ModInfo mod);
    void Deactivate();
    string CurrentModuleId { get; set; }
    bool TryGetModule(string moduleId, out EditorModule module);
}

public class MainEditorViewModel : IMainEditorViewModel
{
    public string CurrentModuleId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Deactivate()
    {
    }

    public void SetMod(ModInfo mod)
    {
    }

    public bool TryGetModule(string moduleId, out EditorModule module)
    {
        throw new NotImplementedException();
    }
}
