using Ooui.Forms.Renderers;
using System;
using Xamarin.Forms;

namespace Ooui.Forms.Cells
{
    public class ViewCellElement : CellElement
    {
        WeakReference<IVisualElementRenderer> _rendererRef;

        protected override void BindCell ()
        {
            var cell = (ViewCell)Cell;

            IVisualElementRenderer renderer;
            if (_rendererRef == null || !_rendererRef.TryGetTarget (out renderer))
                renderer = GetNewRenderer (cell);
            else {
                if (renderer.Element != null && renderer == Platform.GetRenderer (renderer.Element))
                    renderer.Element.ClearValue (Platform.RendererProperty);

                var type = Xamarin.Forms.Internals.Registrar.Registered.GetHandlerTypeForObject (cell.View);
                var reflectableType = renderer as System.Reflection.IReflectableType;
                var rendererType = reflectableType != null ? reflectableType.GetTypeInfo ().AsType () : renderer.GetType ();
                if (rendererType == type || (renderer is DefaultRenderer && type == null)) {
                    renderer.SetElement (cell.View);
                }
                else {
                    renderer = GetNewRenderer (cell);
                }
            }

            Platform.SetRenderer (cell.View, renderer);

            base.BindCell ();
        }

        IVisualElementRenderer GetNewRenderer (ViewCell cell)
        {
            var newRenderer = Platform.CreateRenderer (cell.View);
            _rendererRef = new WeakReference<IVisualElementRenderer> (newRenderer);
            AppendChild (newRenderer.NativeView);

            return newRenderer;
        }
    }
}
