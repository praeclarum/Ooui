using Microsoft.Maui;

namespace Ooui.Maui
{
	public static class ButtonExtensions
	{
		public static void UpdateText(this Ooui.Button nativeButton, IButton button) =>
			nativeButton.Text = button.Text;

		public static void UpdateTextColor(this Ooui.Button nativeButton, IButton button)
		{
			// var color = button.TextColor.ToNative();
			// nativeButton.Style.Color = color;
		}

		public static void UpdateCharacterSpacing(this Ooui.Button nativeButton, ITextStyle textStyle)
		{
			// nativeButton.Style.UpdateCharacterSpacing(textStyle);
		}

		public static void UpdateFont(this Ooui.Button nativeButton, ITextStyle textStyle, IFontManager fontManager)
		{
			// nativeButton.Style.UpdateFont(textStyle, fontManager, UIFont.ButtonFontSize);
		}

		public static void UpdatePadding(this Ooui.Button nativeButton, IButton button)
		{
			// nativeButton.Style.UpdatePadding(button.Padding);
		}
	}
}