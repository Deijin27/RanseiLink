#nullable enable

using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;
using Input = Avalonia.Input;

namespace RanseiLink.XP.DragDrop;

public record DragStartResult(IDataObject Data, DragDropEffects AllowedEffects);

public interface IDragSource
{
    // TODO: make our own wrapper for the data
    DragStartResult? StartDrag(PointerEventArgs dragInfo);
}

public interface IDropTarget
{
    // TODO: make our own wrapper for the data
    DragDropEffects DragOver(DragEventArgs dropInfo);

    void Drop(DragEventArgs dropInfo);
}

/// <summary>
/// Container class for attached properties. Must inherit from <see cref="AvaloniaObject"/>.
/// </summary>
public partial class DragDrop : AvaloniaObject
{
    public static readonly AttachedProperty<bool> IsDragSourceProperty =
        AvaloniaProperty.RegisterAttached<DragDrop, InputElement, bool>("IsDragSource", inherits: false);

    public static readonly AttachedProperty<bool> IsDropTargetProperty =
        AvaloniaProperty.RegisterAttached<DragDrop, Interactive, bool>("IsDropTarget", inherits: false);

    public static readonly AttachedProperty<IDragSource?> DragHandlerProperty =
        AvaloniaProperty.RegisterAttached<DragDrop, InputElement, IDragSource?>("DragHandler", inherits: false);

    public static readonly AttachedProperty<IDropTarget?> DropHandlerProperty =
        AvaloniaProperty.RegisterAttached<DragDrop, Interactive, IDropTarget?>("DropHandler", inherits: false);

    public static bool GetIsDragSource(InputElement e) => e.GetValue(IsDragSourceProperty);
    public static void SetIsDragSource(InputElement e, bool value) => e.SetValue(IsDragSourceProperty, value);

    public static bool GetIsDropTarget(Interactive e) => e.GetValue(IsDropTargetProperty);
    public static void SetIsDropTarget(Interactive e, bool value) => e.SetValue(IsDropTargetProperty, value);

    public static IDragSource? GetDragHandler(InputElement e) => e.GetValue(DragHandlerProperty);
    public static void SetDragHandler(InputElement e, IDragSource? value) => e.SetValue(DragHandlerProperty, value);

    public static IDropTarget? GetDropHandler(InputElement e) => e.GetValue(DropHandlerProperty);
    public static void SetDropHandler(InputElement e, IDropTarget? value) => e.SetValue(DropHandlerProperty, value);

    static DragDrop()
    {
        IsDragSourceProperty.Changed.AddClassHandler<InputElement>(OnIsDragSourcePropertyChanged);
        IsDropTargetProperty.Changed.AddClassHandler<Interactive>(OnIsDropTargetPropertyChanged);
    }

    private static void OnIsDragSourcePropertyChanged(InputElement sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.GetNewValue<bool>())
        {
            sender.PointerMoved += DragSource_PointerMoved;
        }
        else
        {
            sender.PointerMoved -= DragSource_PointerMoved;
        }
    }

    private static void DragSource_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is not InputElement inputElement)
        {
            return;
        }

        var properties = e.GetCurrentPoint(inputElement).Properties;
        if (!properties.IsLeftButtonPressed)
        {
            return;
        }

        var handler = GetDragHandler(inputElement);
        if (handler == null)
        {
            return;
        }

        var result = handler.StartDrag(e);
        if (result == null)
        {
            return;
        }

        Input.DragDrop.DoDragDrop(e, result.Data, result.AllowedEffects);
    }

    private static void OnIsDropTargetPropertyChanged(Interactive sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.GetNewValue<bool>())
        {
            sender.SetValue(Input.DragDrop.AllowDropProperty, true);
            sender.AddHandler(Input.DragDrop.DragOverEvent, DragOver);
            sender.AddHandler(Input.DragDrop.DropEvent, Drop);
        }
        else
        {
            sender.SetValue(Input.DragDrop.AllowDropProperty, false);
            sender.RemoveHandler(Input.DragDrop.DragOverEvent, DragOver);
            sender.RemoveHandler(Input.DragDrop.DropEvent, Drop);
        }
    }

    private static void DragOver(object? sender, DragEventArgs e)
    {
        if (sender is not InputElement inputElement)
        {
            return;
        }

        var handler = GetDropHandler(inputElement);
        if (handler == null)
        {
            return;
        }

        handler.DragOver(e);
    }

    private static void Drop(object? sender, DragEventArgs e)
    {
        if (sender is not InputElement inputElement)
        {
            return;
        }

        var handler = GetDropHandler(inputElement);
        if (handler == null)
        {
            return;
        }

        handler.Drop(e);
    }
}