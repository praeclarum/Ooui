using System;
using System.Collections.Generic;
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
                uiview.Style.Left = x;
                uiview.Style.Top = y;
                uiview.Style.Width = width;
                uiview.Style.Height = height;
                Renderer.SetControlSize (new Size (width, height));
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

            var transforms = "";
            var transformOrigin = default (string);
            const double epsilon = 0.001;

            var icult = System.Globalization.CultureInfo.InvariantCulture;

            // position is relative to anchor point
            if ((Math.Abs (anchorX - 0.5) > epsilon) || (Math.Abs (anchorY - 0.5) > epsilon)) {
                transformOrigin = string.Format (icult, "{0:0.######}% {1:0.######}%", anchorX*100, anchorY*100);
            }

            if (Math.Abs (translationX) > epsilon || Math.Abs (translationY) > epsilon)
                transforms = string.Format (icult, "{0} translate({1:0.######}px,{2:0.######}px)", transforms, translationX, translationY);

            if (Math.Abs (scale - 1) > epsilon)
                transforms = string.Format (icult, "{0} scale({1:0.######},{1:0.######})", transforms, scale);

            //if (Math.Abs (rotationX % 360) > epsilon)
            //	RotateX (rotationX);
            //if (Math.Abs (rotationY % 360) > epsilon)
            //RotateY (rotationY);

            if (Math.Abs (rotation % 360) > epsilon)
                transforms = string.Format (icult, "{0} rotate({1:0.######}deg)", transforms, rotation);

            uiview.Style.Transform = transforms.Length > 0 ? transforms : null;
            uiview.Style.TransformOrigin = transforms.Length > 0 ? transformOrigin : null;

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
