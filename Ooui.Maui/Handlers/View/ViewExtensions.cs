using NativeView = Ooui.Element;

using Microsoft.Maui;

namespace Ooui.Maui
{
	public static class ViewExtensions
	{
		public static void UpdateIsEnabled(this NativeView nativeView, IView view) { }

		public static void UpdateVisibility(this NativeView nativeView, IView view) { }

		public static void UpdateBackground(this NativeView nativeView, IView view) { }

		public static void UpdateAutomationId(this NativeView nativeView, IView view) { }

		public static void UpdateOpacity(this NativeView nativeView, IView view) { }

		public static void UpdateSemantics(this NativeView nativeView, IView view) { }

		public static void UpdateTranslationX(this NativeView nativeView, IView view) { }

		public static void UpdateTranslationY(this NativeView nativeView, IView view) { }

		public static void UpdateScale(this NativeView nativeView, IView view) { }

		public static void UpdateRotation(this NativeView nativeView, IView view) { }

		public static void UpdateRotationX(this NativeView nativeView, IView view) { }

		public static void UpdateRotationY(this NativeView nativeView, IView view) { }

		public static void UpdateAnchorX(this NativeView nativeView, IView view) { }

		public static void UpdateAnchorY(this NativeView nativeView, IView view) { }

		public static void InvalidateMeasure(this NativeView nativeView, IView view) { }

		public static void UpdateWidth(this NativeView nativeView, IView view) { }

		public static void UpdateHeight(this NativeView nativeView, IView view) { }

		public static void UpdateText(this NativeView nativeView, ILabel view) {
			nativeView.Text = view.Text;
		}

		public static void UpdateTextColor(this NativeView nativeView, IView view) { }

		public static void UpdateCharacterSpacing(this NativeView nativeView, IView view) { }

		public static void UpdateHorizontalTextAlignment(this NativeView nativeView, IView view) { }

		public static void UpdatePadding(this NativeView nativeView, IView view) { }

		public static void UpdateTextDecorations(this NativeView nativeView, IView view) { }

		public static void UpdateLineHeight(this NativeView nativeView, IView view) { }

		public static void UpdateMaxLines(this NativeView nativeView, IView view) { }

		public static void UpdateLineBreakMode(this NativeView nativeView, IView view) { }
	}
}
