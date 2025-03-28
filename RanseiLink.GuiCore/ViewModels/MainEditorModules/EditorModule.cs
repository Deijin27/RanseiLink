﻿using RanseiLink.Core.Services;
using RanseiLink.Core.Services.ModelServices;
using System.Diagnostics.CodeAnalysis;

namespace RanseiLink.GuiCore.ViewModels;

[AttributeUsage(AttributeTargets.Class)]
public class EditorModuleAttribute : Attribute
{

}

public abstract class EditorModule
{
    public abstract string UniqueId { get; }
    public abstract string ListName { get; }
    public abstract object? ViewModel { get; }

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

public abstract class BaseSelectorEditorModule<TService> : EditorModule, ISelectableModule where TService : IModelService
{
    public override object? ViewModel => _viewModel;

    protected ISelectorViewModelFactory? _selectorVmFactory;
    protected TService? _service;
    protected SelectorViewModel? _viewModel;

    [MemberNotNull(nameof(_service))]
    [MemberNotNull(nameof(_selectorVmFactory))]
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _service = modServices.Get<TService>();
        _selectorVmFactory = modServices.Get<ISelectorViewModelFactory>();
        if (_service == null)
        {
            throw new System.Exception($"Service '{typeof(TService).FullName}' not registered");
        }
    }

    public override void Deactivate()
    {
        _service?.Save();
        _selectorVmFactory = null;
        _service = default;
        _viewModel = null;
    }
    public override void OnPatchingRom() => _service?.Save();

    public virtual void Select(int selectId)
    {
        if (_viewModel == null)
        {
            return;
        }
        _viewModel.Selected = selectId;
    }
}

public abstract class BaseWorkspaceEditorModule<TService> : EditorModule, ISelectableModule where TService : IModelService
{
    public override object? ViewModel => _viewModel;

    protected ISelectorViewModelFactory? _selectorVmFactory;
    protected TService? _service;
    protected WorkspaceViewModel? _viewModel;

    [MemberNotNull(nameof(_service))]
    [MemberNotNull(nameof(_selectorVmFactory))]
    public override void Initialise(IServiceGetter modServices)
    {
        base.Initialise(modServices);
        _service = modServices.Get<TService>();
        _selectorVmFactory = modServices.Get<ISelectorViewModelFactory>();
        if (_service == null)
        {
            throw new System.Exception($"Service '{typeof(TService).FullName}' not registered");
        }
    }

    public override void Deactivate() => _service?.Save();
    public override void OnPatchingRom() => _service?.Save();

    public virtual void Select(int selectId)
    {
        if (_viewModel != null)
        {
            _viewModel.SearchText = null;
            _viewModel.SelectById(selectId);
        }
    }
}

public interface ISelectableModule
{
    void Select(int selectId);
}

