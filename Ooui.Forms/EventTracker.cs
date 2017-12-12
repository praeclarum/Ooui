using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;

using NativeView = Ooui.Element;

namespace Ooui.Forms
{
    public class EventTracker
    {
        readonly NotifyCollectionChangedEventHandler _collectionChangedHandler;

        readonly Dictionary<IGestureRecognizer, NativeGestureRecognizer> _gestureRecognizers = new Dictionary<IGestureRecognizer, NativeGestureRecognizer> ();

        readonly IVisualElementRenderer _renderer;
        bool _disposed;
        NativeView _handler;

        public EventTracker (IVisualElementRenderer renderer)
        {
            if (renderer == null)
                throw new ArgumentNullException (nameof (renderer));

            _collectionChangedHandler = ModelGestureRecognizersOnCollectionChanged;

            _renderer = renderer;
            _renderer.ElementChanged += OnElementChanged;
        }

        ObservableCollection<IGestureRecognizer> ElementGestureRecognizers {
            get {
                if (_renderer?.Element is View)
                    return ((View)_renderer.Element).GestureRecognizers as ObservableCollection<IGestureRecognizer>;
                return null;
            }
        }

        public void Dispose ()
        {
            if (_disposed)
                return;

            _disposed = true;

            foreach (var kvp in _gestureRecognizers) {
                RemoveGestureRecognizer (_handler, kvp.Value);
                kvp.Value.Dispose ();
            }

            _gestureRecognizers.Clear ();

            if (ElementGestureRecognizers != null)
                ElementGestureRecognizers.CollectionChanged -= _collectionChangedHandler;

            _handler = null;
        }

        void ModelGestureRecognizersOnCollectionChanged (object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            LoadRecognizers ();
        }

        void OnElementChanged (object sender, VisualElementChangedEventArgs e)
        {
            if (e.OldElement != null) {
                // unhook
                var oldView = e.OldElement as View;
                if (oldView != null) {
                    var oldRecognizers = (ObservableCollection<IGestureRecognizer>)oldView.GestureRecognizers;
                    oldRecognizers.CollectionChanged -= _collectionChangedHandler;
                }
            }

            if (e.NewElement != null) {
                // hook
                if (ElementGestureRecognizers != null) {
                    ElementGestureRecognizers.CollectionChanged += _collectionChangedHandler;
                    LoadRecognizers ();
                }
            }
        }

        void LoadRecognizers ()
        {
            if (ElementGestureRecognizers == null)
                return;

            foreach (var recognizer in ElementGestureRecognizers) {
                if (_gestureRecognizers.ContainsKey (recognizer))
                    continue;

                var nativeRecognizer = GetNativeRecognizer (recognizer);
                if (nativeRecognizer != null) {
                    AddGestureRecognizer (_handler, nativeRecognizer);

                    _gestureRecognizers[recognizer] = nativeRecognizer;
                }
            }

            var toRemove = _gestureRecognizers.Keys.Where (key => !ElementGestureRecognizers.Contains (key)).ToArray ();
            foreach (var gestureRecognizer in toRemove) {
                var uiRecognizer = _gestureRecognizers[gestureRecognizer];
                _gestureRecognizers.Remove (gestureRecognizer);

                RemoveGestureRecognizer (_handler, uiRecognizer);
                uiRecognizer.Dispose ();
            }
        }

        protected virtual NativeGestureRecognizer GetNativeRecognizer (IGestureRecognizer recognizer)
        {
            if (recognizer == null)
                return null;

            var weakRecognizer = new WeakReference (recognizer);
            var weakEventTracker = new WeakReference (this);

            var tapRecognizer = recognizer as TapGestureRecognizer;
            if (tapRecognizer != null && tapRecognizer.NumberOfTapsRequired == 1) {
                var returnAction = new TargetEventHandler ((s, e) => {
                    var tapGestureRecognizer = weakRecognizer.Target as TapGestureRecognizer;
                    var eventTracker = weakEventTracker.Target as EventTracker;
                    var view = eventTracker?._renderer?.Element as View;

                    if (tapGestureRecognizer != null && view != null)
                        tapGestureRecognizer.SendTapped (view);
                });
                var uiRecognizer = new NativeGestureRecognizer {
                    EventType = "click",
                    Handler = returnAction,
                };
                return uiRecognizer;
            }
            if (tapRecognizer != null && tapRecognizer.NumberOfTapsRequired == 2) {
                var returnAction = new TargetEventHandler ((s, e) => {
                    var tapGestureRecognizer = weakRecognizer.Target as TapGestureRecognizer;
                    var eventTracker = weakEventTracker.Target as EventTracker;
                    var view = eventTracker?._renderer?.Element as View;

                    if (tapGestureRecognizer != null && view != null)
                        tapGestureRecognizer.SendTapped (view);
                });
                var uiRecognizer = new NativeGestureRecognizer {
                    EventType = "dblclick",
                    Handler = returnAction,
                };
                return uiRecognizer;
            }

            return null;
        }

        static void AddGestureRecognizer (Element element, NativeGestureRecognizer recognizer)
        {
            element.AddEventListener (recognizer.EventType, recognizer.Handler);
        }

        static void RemoveGestureRecognizer (Element element, NativeGestureRecognizer recognizer)
        {
            element.RemoveEventListener (recognizer.EventType, recognizer.Handler);
        }

        public void LoadEvents (NativeView handler)
        {
            if (_disposed)
                throw new ObjectDisposedException (null);

            _handler = handler;
            OnElementChanged (this, new VisualElementChangedEventArgs (null, _renderer.Element));
        }

        protected class NativeGestureRecognizer : IDisposable
        {
            public string EventType;
            public TargetEventHandler Handler;
            public void Dispose ()
            {
            }
        }
    }
}
