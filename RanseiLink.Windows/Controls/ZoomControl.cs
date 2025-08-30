
/*
 * Based on code from
 * https://stackoverflow.com/a/6782715
 * Licence: CC BY-SA 4.0
 */
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RanseiLink.Windows.Controls;
public class ZoomControl : Border
{
    private UIElement _child = null;
    private Point _origin;
    private Point _start;

    private TranslateTransform GetTranslateTransform(UIElement element)
    {
        return (TranslateTransform)((TransformGroup)element.RenderTransform)
          .Children.First(tr => tr is TranslateTransform);
    }

    private ScaleTransform GetScaleTransform(UIElement element)
    {
        return (ScaleTransform)((TransformGroup)element.RenderTransform)
          .Children.First(tr => tr is ScaleTransform);
    }

    public override UIElement Child
    {
        get { return base.Child; }
        set
        {
            if (value != null && value != this.Child)
                this.Initialize(value);
            base.Child = value;
        }
    }

    public void Initialize(UIElement element)
    {
        this._child = element;
        if (_child != null)
        {
            TransformGroup group = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            group.Children.Add(st);
            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);
            _child.RenderTransform = group;
            _child.RenderTransformOrigin = new Point(0.0, 0.0);

            // Hook to events
            this.MouseWheel += ZoomViewEvent;
            this.MouseRightButtonDown += BeginPanningEvent;
            this.MouseRightButtonUp += EndPanningEvent;
            this.MouseMove += PanMoveEvent;
            this.PreviewMouseDown += ResetZoomEvent;
        }
    }

    public void Reset()
    {
        if (_child != null)
        {
            // reset zoom
            var st = GetScaleTransform(_child);
            st.ScaleX = 1.0;
            st.ScaleY = 1.0;

            // reset pan
            var tt = GetTranslateTransform(_child);
            tt.X = 0.0;
            tt.Y = 0.0;
        }
    }

    #region Child Events

    private void ZoomViewEvent(object sender, MouseWheelEventArgs e)
    {
        if (_child != null)
        {
            var st = GetScaleTransform(_child);
            var tt = GetTranslateTransform(_child);

            double zoom = e.Delta > 0 ? .2 : -.2;
            if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                return;

            Point relative = e.GetPosition(_child);
            double absoluteX;
            double absoluteY;

            absoluteX = relative.X * st.ScaleX + tt.X;
            absoluteY = relative.Y * st.ScaleY + tt.Y;

            st.ScaleX += zoom;
            st.ScaleY += zoom;

            tt.X = absoluteX - relative.X * st.ScaleX;
            tt.Y = absoluteY - relative.Y * st.ScaleY;
        }
    }

    private void BeginPanningEvent(object sender, MouseButtonEventArgs e)
    {
        if (_child != null)
        {
            var tt = GetTranslateTransform(_child);
            _start = e.GetPosition(this);
            _origin = new Point(tt.X, tt.Y);
            this.Cursor = Cursors.Hand;
            _child.CaptureMouse();
        }
    }

    private void EndPanningEvent(object sender, MouseButtonEventArgs e)
    {
        if (_child != null)
        {
            _child.ReleaseMouseCapture();
            this.Cursor = Cursors.Arrow;
        }
    }

    void ResetZoomEvent(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
        {
            this.Reset();
        }
    }

    private void PanMoveEvent(object sender, MouseEventArgs e)
    {
        if (_child != null)
        {
            if (_child.IsMouseCaptured)
            {
                var tt = GetTranslateTransform(_child);
                Vector v = _start - e.GetPosition(this);
                tt.X = _origin.X - v.X;
                tt.Y = _origin.Y - v.Y;
            }
        }
    }

    #endregion
}