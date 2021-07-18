using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility.Platform.Ooui;
using Microsoft.Maui.Controls.Platform;
using Ooui;
using Ooui.Maui.Controls.Compatibility.Platform.iOS;
using ViewHandler = Ooui.Maui.Handlers.ViewHandler<Microsoft.Maui.IView, Ooui.Element>;

namespace Ooui.Maui.Controls.Compatibility
{
	public class RendererToHandlerShim : ViewHandler
	{
		internal IVisualElementRenderer? VisualElementRenderer { get; private set; }

		public static IViewHandler CreateShim(object renderer)
		{
			if (renderer is IViewHandler handler)
				return handler;

			if (renderer is IVisualElementRenderer ivr)
				return new RendererToHandlerShim(ivr);

			return new RendererToHandlerShim();
		}

		public RendererToHandlerShim() : base(ViewHandler.ViewMapper)
		{
		}

		public RendererToHandlerShim(IVisualElementRenderer visualElementRenderer) : this()
		{
			if (visualElementRenderer != null)
				SetupRenderer(visualElementRenderer);
		}

		public void SetupRenderer(IVisualElementRenderer visualElementRenderer)
		{
			VisualElementRenderer = visualElementRenderer;

            // TODO: fak: Handle Forms element changed
			// VisualElementRenderer.ElementChanged += OnElementChanged;

			if (VisualElementRenderer.Element is IView view)
			{
				view.Handler = this;
				SetVirtualView(view);
			}
			else if (VisualElementRenderer.Element != null)
				throw new Exception($"{VisualElementRenderer.Element} must implement: {nameof(IView)}");
		}

		void OnElementChanged(object sender, VisualElementChangedEventArgs e)
		{
			if (e.OldElement is IView view)
				view.Handler = null;

			if (e.NewElement is IView newView)
			{
				newView.Handler = this;
				this.SetVirtualView(newView);
			}
			else if (e.NewElement != null)
				throw new Exception($"{e.NewElement} must implement: {nameof(IView)}");
		}

		protected override Ooui.Element CreateNativeView()
		{
			return VisualElementRenderer.NativeView;
		}

		protected override void ConnectHandler(Ooui.Element nativeView)
		{
			base.ConnectHandler(nativeView);
			VirtualView.Handler = this;
		}

		protected override void DisconnectHandler(Ooui.Element nativeView)
		{
            // TODO: fak: Need to set renderer for native view on DisconnectHandler
			// Ooui.Maui.Controls.Compatibility.Platform.Platform.SetRenderer(
			// 	VisualElementRenderer.Element,
			// 	VisualElementRenderer);

			VisualElementRenderer.SetElement(null);

			base.DisconnectHandler(nativeView);
			VirtualView.Handler = null;
		}

		public override void SetVirtualView(IView view)
		{
            // TODO: fak: Set Compat virtual view
            throw new NotImplementedException("Cannote SetVirtualView");
            
			// if (VisualElementRenderer == null)
			// {
			// 	var renderer = Microsoft.Maui.Controls.Internals.Registrar.Registered.GetHandlerForObject<IVisualElementRenderer>(view)
			// 							   ?? new Microsoft.Maui.Controls.Compatibility.Platform.Ooui.Platform.DefaultRenderer();

			// 	SetupRenderer(renderer);
			// }

			// if (VisualElementRenderer.Element != view)
			// {
			// 	VisualElementRenderer.SetElement((VisualElement)view);
			// }
			// else
			// {
			// 	base.SetVirtualView(view);
			// }

			// Microsoft.Maui.Controls.Compatibility.Platform.Platform.SetRenderer(
			// 	VisualElementRenderer.Element,
			// 	VisualElementRenderer);
		}

		public override void UpdateValue(string property)
		{
			base.UpdateValue(property);
			if (property == "Frame")
			{
				NativeArrange(VisualElementRenderer.Element.Bounds);
			}
		}
	}
}
