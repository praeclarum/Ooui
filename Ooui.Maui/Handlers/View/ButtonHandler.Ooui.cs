using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;

namespace Ooui.Maui.Handlers
{
	public partial class ButtonHandler : ViewHandler<IButton, Ooui.Button>
	{
		protected override Ooui.Button CreateNativeView()
		{
			SetControlPropertiesFromProxy();
			return new Ooui.Button();
		}

		protected override void ConnectHandler(Ooui.Button nativeView)
		{
			nativeView.Click += OnClick;

			base.ConnectHandler(nativeView);
		}

		protected override void DisconnectHandler(Ooui.Button nativeView)
		{
			nativeView.Click -= OnClick;

			base.DisconnectHandler(nativeView);
		}

		protected override void SetupDefaults(Ooui.Button nativeView)
		{
			base.SetupDefaults(nativeView);
		}

		public static void MapText(Ooui.Maui.Handlers.ButtonHandler handler, IButton button)
		{
			handler.NativeView?.UpdateText(button);

			// Any text update requires that we update any attributed string formatting
			MapFormatting(handler, button);
		}

		public static void MapTextColor(Ooui.Maui.Handlers.ButtonHandler handler, IButton button)
		{
			handler.NativeView?.UpdateTextColor(button);
		}

		public static void MapCharacterSpacing(Ooui.Maui.Handlers.ButtonHandler handler, IButton button)
		{
			handler.NativeView?.UpdateCharacterSpacing(button);
		}

		public static void MapPadding(Ooui.Maui.Handlers.ButtonHandler handler, IButton button)
		{
			handler.NativeView?.UpdatePadding(button);
		}

		public static void MapFont(Ooui.Maui.Handlers.ButtonHandler handler, IButton button)
		{
			// var fontManager = handler.GetRequiredService<IFontManager>();
			// handler.NativeView?.UpdateFont(button, fontManager);
		}

		public static void MapFormatting(Ooui.Maui.Handlers.ButtonHandler handler, IButton button)
		{
			// Update all of the attributed text formatting properties
			handler.NativeView?.UpdateCharacterSpacing(button);
		}

		void SetControlPropertiesFromProxy()
		{
			if (NativeView == null)
				return;
		}

		void OnClick(object? sender, EventArgs e)
		{
			VirtualView?.Released();
			VirtualView?.Clicked();
		}
	}
}