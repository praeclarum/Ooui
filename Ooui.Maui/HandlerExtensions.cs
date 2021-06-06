using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Ooui;

namespace Ooui.Maui
{
	public static class HandlerExtensions
	{
		public static Element ToOouiElement(this IApplication application, IMauiContext context)
		{
			var window = application.CreateWindow(new ActivationState (context));
			var element = window.View.ToOouiElement(context);
			return element;
		}

		public static Element ToOouiElement(this IView view, IMauiContext context)
		{
			_ = view ?? throw new ArgumentNullException(nameof(view));
			_ = context ?? throw new ArgumentNullException(nameof(context));

			// Maui: This is how MVU works. It collapses views down
			if (view is IReplaceableView ir)
				view = ir.ReplacedView;

			var handler = view.Handler;

			if (handler == null)
			{
				var viewType = view.GetType();
				handler = context.Handlers.GetHandler(viewType);

				if (handler == null)
					throw new Exception($"Handler not found for view {view}");

				handler.SetMauiContext(context);

				view.Handler = handler;
			}

			handler.SetVirtualView(view);

			if (((INativeViewHandler)handler).NativeView is not Element result)
			{
				throw new InvalidOperationException($"Unable to convert {view} to {typeof(Element)}");
			}

			return result;
		}
	}
}
