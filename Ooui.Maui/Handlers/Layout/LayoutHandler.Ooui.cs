using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.Maui;

namespace Ooui.Maui.Handlers
{
	public partial class LayoutHandler : ViewHandler<ILayout, Ooui.Div>
	{
		protected override Ooui.Div CreateNativeView() {
			if (VirtualView == null)
			{
				throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a LayoutViewGroup");
			}

			var view = new Ooui.Div();

			return view;
		}

		public override void SetVirtualView(IView view)
		{
			base.SetVirtualView(view);

			_ = NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			// NativeView.View = view;

			//Cleanup the old view when reused
			var oldChildren = NativeView.Children.ToList();
			oldChildren.ForEach(x => NativeView.RemoveChild(x));

			foreach (var child in VirtualView.Children)
			{
				NativeView.AppendChild(child.ToNative(MauiContext));
			}
		}

		public void Add(IView child)
		{
			_ = NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			NativeView.AppendChild(child.ToNative(MauiContext));
		}

		public void Remove(IView child)
		{
			_ = NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");

			if (child?.Handler?.NativeView is Ooui.Div nativeView)
			{
				NativeView.RemoveChild(nativeView);
			}
		}

		protected override void DisconnectHandler(Ooui.Div nativeView)
		{
			base.DisconnectHandler(nativeView);
			var subViews = nativeView.Children.ToList();

			foreach (var subView in subViews)
			{
				nativeView.RemoveChild(subView);
			}
		}
	}
}
