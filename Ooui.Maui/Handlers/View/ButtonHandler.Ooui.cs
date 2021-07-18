using System;

using Microsoft.Maui;

namespace Ooui.Maui.Handlers
{
	public partial class ButtonHandler : ViewHandler<IButton, Ooui.Button>
	{
		protected override Ooui.Button CreateNativeView() => throw new NotImplementedException();

		public static void MapText(ButtonHandler handler, IButton button) { }
		public static void MapTextColor(ButtonHandler handler, IButton button) { }
		public static void MapCharacterSpacing(ButtonHandler handler, IButton button) { }
		public static void MapFont(ButtonHandler handler, IButton button) { }
		public static void MapPadding(ButtonHandler handler, IButton button) { }
	}
}