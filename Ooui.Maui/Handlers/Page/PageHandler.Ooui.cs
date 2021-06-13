using System;
using System.Linq;
using Microsoft.Maui;

namespace Ooui.Maui.Handlers
{
    public partial class PageHandler : ViewHandler<IPage, Ooui.Element>
    {
        protected override Ooui.Element CreateNativeView() => new Ooui.Div();

        void UpdateContent()
		{
			_ = NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			//Cleanup the old view when reused
			var oldChildren = NativeView.Children.ToList();
			oldChildren.ForEach(x => NativeView.RemoveChild(x));

			if (VirtualView.Content != null) {
				NativeView.AppendChild(VirtualView.Content.ToNative(MauiContext));
            }
		}

        public static void MapTitle(PageHandler handler, IPage page)
        {
            // TODO: fak: Map Page.Title
        }

        public static void MapContent(PageHandler handler, IPage page)
        {
            handler.UpdateContent();
        }
    }
}
