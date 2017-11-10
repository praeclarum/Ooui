using System;
using System.ComponentModel;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Ooui.Forms
{
    public class VisualElementTracker
    {
        readonly EventHandler<EventArg<VisualElement>> _batchCommittedHandler;

        readonly PropertyChangedEventHandler _propertyChangedHandler;
        readonly EventHandler _sizeChangedEventHandler;
        bool _disposed;
        VisualElement _element;

        // Track these by hand because the calls down into iOS are too expensive
        bool _isInteractive;
        Rectangle _lastBounds;
#if !__MOBILE__
        Rectangle _lastParentBounds;
#endif
        int _updateCount;

        public VisualElementTracker (IVisualElementRenderer renderer)
        {
            if (renderer == null)
                throw new ArgumentNullException (nameof (renderer));

            _propertyChangedHandler = HandlePropertyChanged;
            _sizeChangedEventHandler = HandleSizeChanged;
            _batchCommittedHandler = HandleRedrawNeeded;

            Renderer = renderer;
            renderer.ElementChanged += OnRendererElementChanged;
            SetElement (null, renderer.Element);
        }

        IVisualElementRenderer Renderer { get; set; }

        public void Dispose ()
        {
            Dispose (true);
        }

        public event EventHandler NativeControlUpdated;

        protected virtual void Dispose (bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (disposing) {
                SetElement (_element, null);

                Renderer.ElementChanged -= OnRendererElementChanged;
                Renderer = null;
            }
        }

        void HandlePropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == VisualElement.XProperty.PropertyName || e.PropertyName == VisualElement.YProperty.PropertyName || e.PropertyName == VisualElement.WidthProperty.PropertyName ||
                e.PropertyName == VisualElement.HeightProperty.PropertyName || e.PropertyName == VisualElement.AnchorXProperty.PropertyName || e.PropertyName == VisualElement.AnchorYProperty.PropertyName ||
                e.PropertyName == VisualElement.TranslationXProperty.PropertyName || e.PropertyName == VisualElement.TranslationYProperty.PropertyName || e.PropertyName == VisualElement.ScaleProperty.PropertyName ||
                e.PropertyName == VisualElement.RotationProperty.PropertyName || e.PropertyName == VisualElement.RotationXProperty.PropertyName || e.PropertyName == VisualElement.RotationYProperty.PropertyName ||
                e.PropertyName == VisualElement.IsVisibleProperty.PropertyName || e.PropertyName == VisualElement.IsEnabledProperty.PropertyName ||
                e.PropertyName == VisualElement.InputTransparentProperty.PropertyName || e.PropertyName == VisualElement.OpacityProperty.PropertyName)
                UpdateNativeControl (); // poorly optimized
        }

        void HandleRedrawNeeded (object sender, EventArgs e)
        {
            UpdateNativeControl ();
        }

        void HandleSizeChanged (object sender, EventArgs e)
        {
            UpdateNativeControl ();
        }

        void OnRendererElementChanged (object s, VisualElementChangedEventArgs e)
        {
            if (_element == e.NewElement)
                return;

            SetElement (_element, e.NewElement);
        }

        void OnUpdateNativeControl ()
        {
            var view = Renderer.Element;
            var uiview = Renderer.NativeView;

            if (view == null || view.Batched)
                return;

            var shouldInteract = !view.InputTransparent && view.IsEnabled;
            if (_isInteractive != shouldInteract) {
                _isInteractive = shouldInteract;
            }

            var boundsChanged = _lastBounds != view.Bounds;
            var viewParent = view.RealParent as VisualElement;
            var parentBoundsChanged = _lastParentBounds != (viewParent == null ? Rectangle.Zero : viewParent.Bounds);
            var thread = !boundsChanged;

            var anchorX = (float)view.AnchorX;
            var anchorY = (float)view.AnchorY;
            var translationX = (float)view.TranslationX;
            var translationY = (float)view.TranslationY;
            var rotationX = (float)view.RotationX;
            var rotationY = (float)view.RotationY;
            var rotation = (float)view.Rotation;
            var scale = (float)view.Scale;
            var width = (float)view.Width;
            var height = (float)view.Height;
            var x = (float)view.X;
            var y = (float)view.Y;
            var opacity = (float)view.Opacity;
            var isVisible = view.IsVisible;

            var updateTarget = Interlocked.Increment (ref _updateCount);

            if (updateTarget != _updateCount)
                return;
            var parent = view.RealParent;

            if (isVisible && uiview.IsHidden) {
                uiview.IsHidden = false;
            }

            if (!isVisible && !uiview.IsHidden) {
                uiview.IsHidden = true;
            }

            parentBoundsChanged = true;
            bool shouldUpdate = width > 0 && height > 0 && parent != null && (boundsChanged || parentBoundsChanged);
            if (shouldUpdate) {
                uiview.Style.Position = "absolute";
                uiview.Style.Left = x + "px";
                uiview.Style.Top = y + "px";
                uiview.Style.Width = width + "px";
                uiview.Style.Height = height + "px";
            }
            else if (width <= 0 || height <= 0) {
                return;
            }
            if (opacity >= 1.0f) {
                uiview.Style.Opacity = null;
            }
            else {
                uiview.Style.Opacity = opacity;
            }

            //var transform = 0;
            //const double epsilon = 0.001;
            //caLayer.AnchorPoint = new PointF (anchorX - 0.5f, anchorY - 0.5f);

            //// position is relative to anchor point
            //if (Math.Abs (anchorX - .5) > epsilon)
            //	transform = transform.Translate ((anchorX - .5f) * width, 0, 0);
            //if (Math.Abs (anchorY - .5) > epsilon)
            //	transform = transform.Translate (0, (anchorY - .5f) * height, 0);

            //if (Math.Abs (translationX) > epsilon || Math.Abs (translationY) > epsilon)
            //	transform = transform.Translate (translationX, translationY, 0);

            //if (Math.Abs (scale - 1) > epsilon)
            //	transform = transform.Scale (scale);

            //// not just an optimization, iOS will not "pixel align" a view which has m34 set
            //if (Math.Abs (rotationY % 180) > epsilon || Math.Abs (rotationX % 180) > epsilon)
            //	transform.m34 = 1.0f / -400f;

            //if (Math.Abs (rotationX % 360) > epsilon)
            //	transform = transform.Rotate (rotationX * (float)Math.PI / 180.0f, 1.0f, 0.0f, 0.0f);
            //if (Math.Abs (rotationY % 360) > epsilon)
            //	transform = transform.Rotate (rotationY * (float)Math.PI / 180.0f, 0.0f, 1.0f, 0.0f);

            //transform = transform.Rotate (rotation * (float)Math.PI / 180.0f, 0.0f, 0.0f, 1.0f);
            //caLayer.Transform = transform;

            _lastBounds = view.Bounds;
            _lastParentBounds = viewParent?.Bounds ?? Rectangle.Zero;
        }

        void SetElement (VisualElement oldElement, VisualElement newElement)
        {
            if (oldElement != null) {
                oldElement.PropertyChanged -= _propertyChangedHandler;
                oldElement.SizeChanged -= _sizeChangedEventHandler;
                oldElement.BatchCommitted -= _batchCommittedHandler;
            }

            _element = newElement;

            if (newElement != null) {
                newElement.BatchCommitted += _batchCommittedHandler;
                newElement.SizeChanged += _sizeChangedEventHandler;
                newElement.PropertyChanged += _propertyChangedHandler;

                UpdateNativeControl ();
            }
        }

        void UpdateNativeControl ()
        {
            if (_disposed)
                return;

            OnUpdateNativeControl ();

            NativeControlUpdated?.Invoke (this, EventArgs.Empty);
        }
    }
}
