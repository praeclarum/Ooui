
using System;
using System.Diagnostics;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;
using Ooui.Maui;

namespace Microsoft.Maui.Controls.Compatibility
{
    public struct InitializationOptions
	{
		public InitializationFlags Flags;
	}
    
    public static class Forms
    {
		internal static IMauiContext? MauiContext { get; private set; }

		public static bool IsInitialized { get; private set; }

		public static void Init(IMauiContext context) =>
			SetupInit(context);

		static void SetupInit(IMauiContext context, InitializationOptions? maybeOptions = null)
		{
			MauiContext = context;

            // TODO: RendererToHandlerShim
			// Microsoft.Maui.Controls.Internals.Registrar.RegisterRendererToHandlerShim(RendererToHandlerShim.CreateShim);

			Application.AccentColor = Color.FromRgba(50, 79, 133, 255);

			if (!IsInitialized)
			{
				// Only need to do this once
				Log.Listeners.Add(new DelegateLogListener((c, m) => Trace.WriteLine(m, c)));
			}

			Device.SetIdiom(TargetIdiom.Desktop);
            // TODO: fak: Find the correct text flow direction
			// Device.SetFlowDirection(UIApplication.SharedApplication.UserInterfaceLayoutDirection.ToFlowDirection());
			// Device.SetFlags(s_flags);
			var platformServices = new Ooui.Maui.OouiPlatformServices();

			Device.PlatformServices = platformServices;

			// use field and not property to avoid exception in getter
			if (Device.info is IDisposable infoDisposable)
			{
				infoDisposable.Dispose();
				Device.info = null;
			}

			Device.PlatformInvalidator = platformServices;
            // TODO: fak: Device info
			// Device.Info = new IOSDeviceInfo();
			// if (maybeOptions?.Flags.HasFlag(InitializationFlags.SkipRenderers) != true)
			// 	RegisterCompatRenderers();

            // TODO: fak: Expressions?
			// ExpressionSearch.Default = new iOSExpressionSearch();

			IsInitialized = true;
		}
    }
}
