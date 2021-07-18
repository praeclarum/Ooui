using System;

using Microsoft.Maui;

namespace Ooui.Maui.Handlers
{
	public partial class LabelHandler : ViewHandler<ILabel, Ooui.Div>
	{
		protected override Ooui.Div CreateNativeView() {
			if (VirtualView == null)
			{
				throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a LayoutViewGroup");
			}

			var view = new Ooui.Div();

			return view;
		}

		public static void MapText(IViewHandler handler, ILabel label) { }
		public static void MapTextColor(IViewHandler handler, ILabel label) { }
		public static void MapCharacterSpacing(IViewHandler handler, ILabel label) { }
		public static void MapFont(LabelHandler handler, ILabel label) { }
		public static void MapHorizontalTextAlignment(LabelHandler handler, ILabel label) { }
		public static void MapLineBreakMode(LabelHandler handler, ILabel label) { }
		public static void MapTextDecorations(LabelHandler handler, ILabel label) { }
		public static void MapMaxLines(IViewHandler handler, ILabel label) { }
		public static void MapPadding(LabelHandler handler, ILabel label) { }
		public static void MapLineHeight(LabelHandler handler, ILabel label) { }
	}
}