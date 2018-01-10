using Ooui.Forms.Renderers;
using System;
using Xamarin.Forms;

namespace Ooui.Forms.Cells
{
    public class ViewCellView : CellView
    {
        private WeakReference<IVisualElementRenderer> _rendererRef;
        private ViewCell _viewCell;
        
        public ViewCell ViewCell
        {
            get { return _viewCell; }
            set
            {
                if (_viewCell == value)
                    return;
                UpdateCell(value);
            }
        }

        private void UpdateCell(ViewCell cell)
        {
            if (_viewCell != null)
                Device.BeginInvokeOnMainThread(_viewCell.SendDisappearing);

            _viewCell = cell;

            Device.BeginInvokeOnMainThread(_viewCell.SendAppearing);

            IVisualElementRenderer renderer;
            if (_rendererRef == null || !_rendererRef.TryGetTarget(out renderer))
                renderer = GetNewRenderer();
            else
            {
                if (renderer.Element != null && renderer == Platform.GetRenderer(renderer.Element))
                    renderer.Element.ClearValue(Platform.RendererProperty);

                var type = Xamarin.Forms.Internals.Registrar.Registered.GetHandlerType(_viewCell.View.GetType());
                var reflectableType = renderer as System.Reflection.IReflectableType;
                var rendererType = reflectableType != null ? reflectableType.GetTypeInfo().AsType() : renderer.GetType();
                if (rendererType == type || (renderer is DefaultRenderer && type == null))
                    renderer.SetElement(_viewCell.View);
                else
                {
                    renderer = GetNewRenderer();
                }
            }

            Platform.SetRenderer(_viewCell.View, renderer);
        }

        private IVisualElementRenderer GetNewRenderer()
        {
            var newRenderer = Platform.CreateRenderer(_viewCell.View);
            _rendererRef = new WeakReference<IVisualElementRenderer>(newRenderer);
            AppendChild(newRenderer.NativeView);

            return newRenderer;
        }
    }
}
