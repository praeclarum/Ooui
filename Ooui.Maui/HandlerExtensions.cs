using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Ooui;

namespace Ooui.Maui
{
	public static class HandlerExtensions
	{
		public static Element ToOouiElement(this IView view, IMauiContext context)
		{
			var nativeView = view.ToNative(context);
			if (view?.Handler is INativeViewHandler nvh && nvh.ViewController != null)
				return nvh.ViewController;

			throw new NotImplementedException ();
		}

		public static Element ToNative(this IView view, IMauiContext context)
		{
			_ = view ?? throw new ArgumentNullException(nameof(view));
			_ = context ?? throw new ArgumentNullException(nameof(context));

			//This is how MVU works. It collapses views down
			if (view is IReplaceableView ir)
				view = ir.ReplacedView;

			var handler = view.Handler;

			if (handler == null)
			{
				handler = context.Handlers.GetHandler(view.GetType());

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
