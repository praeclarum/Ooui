using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Ooui.Forms
{
    public sealed class RendererPool
    {
        readonly Dictionary<Type, Stack<IVisualElementRenderer>> _freeRenderers =
            new Dictionary<Type, Stack<IVisualElementRenderer>> ();

        readonly VisualElement _oldElement;

        readonly IVisualElementRenderer _parent;

        public RendererPool (IVisualElementRenderer renderer, VisualElement oldElement)
        {
            if (renderer == null)
                throw new ArgumentNullException (nameof (renderer));

            if (oldElement == null)
                throw new ArgumentNullException (nameof (oldElement));

            _oldElement = oldElement;
            _parent = renderer;
        }

        public IVisualElementRenderer GetFreeRenderer (VisualElement view)
        {
            if (view == null)
                throw new ArgumentNullException (nameof (view));

            var rendererType = Xamarin.Forms.Internals.Registrar.Registered.GetHandlerType (view.GetType()) ?? typeof (Renderers.ViewRenderer);

            Stack<IVisualElementRenderer> renderers;
            if (!_freeRenderers.TryGetValue (rendererType, out renderers) || renderers.Count == 0)
                return null;
            
            var renderer = renderers.Pop ();
            renderer.SetElement (view);
            return renderer;
        }

        public void UpdateNewElement (VisualElement newElement)
        {
            if (newElement == null)
                throw new ArgumentNullException ("newElement");

            var sameChildrenTypes = true;

            var oldChildren = ((IElementController)_oldElement).LogicalChildren;
            var newChildren = ((IElementController)newElement).LogicalChildren;

            if (oldChildren.Count == newChildren.Count) {
                for (var i = 0; i < oldChildren.Count; i++) {
                    if (oldChildren[i].GetType () != newChildren[i].GetType ()) {
                        sameChildrenTypes = false;
                        break;
                    }
                }
            }
            else
                sameChildrenTypes = false;

            if (!sameChildrenTypes) {
                ClearRenderers (_parent);
                FillChildrenWithRenderers (newElement);
            }
            else
                UpdateRenderers (newElement);
        }

        void ClearRenderers (IVisualElementRenderer renderer)
        {
            if (renderer == null)
                return;

            var subviews = renderer.NativeView.Children;
            for (var i = 0; i < subviews.Count; i++) {
                var childRenderer = subviews[i] as IVisualElementRenderer;
                if (childRenderer != null) {
                    PushRenderer (childRenderer);

                    // The ListView CalculateHeightForCell method can create renderers and dispose its child renderers before this is called.
                    // Thus, it is possible that this work is already completed.
                    if (childRenderer.Element != null && ReferenceEquals (childRenderer, Platform.GetRenderer (childRenderer.Element)))
                        childRenderer.Element.ClearValue (Platform.RendererProperty);
                }
                renderer.NativeView.RemoveChild (subviews[i]);
            }
        }

        void FillChildrenWithRenderers (VisualElement element)
        {
            foreach (var logicalChild in ((IElementController)element).LogicalChildren) {
                var child = logicalChild as VisualElement;
                if (child != null) {
                    //if (CompressedLayout.GetIsHeadless (child)) {
                    //    child.IsPlatformEnabled = true;
                    //    FillChildrenWithRenderers (child);
                    //}
                    //else {
                        var renderer = GetFreeRenderer (child) ?? Platform.CreateRenderer (child);
                        Platform.SetRenderer (child, renderer);
                        _parent.NativeView.AppendChild (renderer.NativeView);
                    //}
                }
            }
        }

        void PushRenderer (IVisualElementRenderer renderer)
        {
            var reflectableType = renderer as System.Reflection.IReflectableType;
            var rendererType = reflectableType != null ? reflectableType.GetTypeInfo ().AsType () : renderer.GetType ();

            Stack<IVisualElementRenderer> renderers;
            if (!_freeRenderers.TryGetValue (rendererType, out renderers))
                _freeRenderers[rendererType] = renderers = new Stack<IVisualElementRenderer> ();

            renderers.Push (renderer);
        }

        void UpdateRenderers (Xamarin.Forms.Element newElement)
        {
            var newElementController = (IElementController)newElement;

            if (newElementController.LogicalChildren.Count == 0)
                return;

            var subviews = _parent.NativeView.Children;
            for (var i = 0; i < subviews.Count; i++) {
                var childRenderer = subviews[i] as IVisualElementRenderer;
                if (childRenderer == null)
                    continue;

                var x = (int)childRenderer.NativeView.Style.ZIndex / 1000;
                var element = newElementController.LogicalChildren[x] as VisualElement;
                if (element == null)
                    continue;

                if (childRenderer.Element != null && ReferenceEquals (childRenderer, Platform.GetRenderer (childRenderer.Element)))
                    childRenderer.Element.ClearValue (Platform.RendererProperty);

                childRenderer.SetElement (element);
                Platform.SetRenderer (element, childRenderer);
            }
        }
    }
}
