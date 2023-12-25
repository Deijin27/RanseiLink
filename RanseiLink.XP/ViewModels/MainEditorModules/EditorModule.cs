using RanseiLink.Core.Services;

namespace RanseiLink.XP.ViewModels;
public class EditorModuleAttribute : Attribute
{

}

public abstract class EditorModule
{
    public abstract string UniqueId { get; }
    public abstract string ListName { get; }
    public abstract object ViewModel { get; }

    /// <summary>
    /// Initialise prior to first view of this module
    /// </summary>
    public virtual void Initialise(IServiceGetter modServices)
    {
    }

    /// <summary>
    /// Do actions prior to the closing of the program, or backing out to the mod selection page
    /// </summary>
    public virtual void Deactivate()
    {
    }

    /// <summary>
    /// Do actions prior to the view model of this module being brought into view
    /// </summary>
    public virtual void OnPageOpening()
    {
    }

    /// <summary>
    /// Do actions prior to the view model of this module being taken out of view
    /// </summary>
    public virtual void OnPageClosing()
    {
    }

    /// <summary>
    /// Do actions prior to patching
    /// </summary>
    public virtual void OnPatchingRom()
    {
    }

}