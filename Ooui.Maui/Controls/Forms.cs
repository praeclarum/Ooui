
using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;

namespace Ooui.Maui.Controls.Compatibility
{
    static class Forms
    {
        internal static IMauiContext? MauiContext { get; private set; }

        public static bool IsInitialized { get; private set; }

        public static bool IsInitializedRenderers { get; private set; }

        public static void Init(IMauiContext context) => SetupInit(context, 0);

        public static void Init(Microsoft.Maui.Controls.Compatibility.InitializationOptions options) =>
			SetupInit(new OouiMauiContext(), options.Flags);

        static void SetupInit(IMauiContext context, InitializationFlags initFlags)
        {
            MauiContext = context;

            // TODO: RendererToHandlerShim
            // Microsoft.Maui.Controls.Internals.Registrar.RegisterRendererToHandlerShim(RendererToHandlerShim.CreateShim);

            Application.AccentColor = Microsoft.Maui.Graphics.Color.FromRgba(50, 79, 133, 255);

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
            Device.PlatformInvalidator = platformServices;

            // use field and not property to avoid exception in getter
            if (Device.info is IDisposable infoDisposable)
            {
                infoDisposable.Dispose();
                Device.info = null;
            }
            Device.Info = new OouiDeviceInfo();

            if (initFlags.HasFlag(InitializationFlags.SkipRenderers) != true) {
                RegisterCompatRenderers();
            }

            // TODO: fak: Expressions?
            // ExpressionSearch.Default = new iOSExpressionSearch();

            IsInitialized = true;
        }

        internal static void RegisterCompatRenderers()
        {
            if (!IsInitializedRenderers)
            {
                IsInitializedRenderers = true;

                // Only need to do this once
                Microsoft.Maui.Controls.Internals.Registrar.RegisterAll(new[]
                {
                    // TODO: fak: do we need these?
                    // typeof(ExportRendererAttribute),
                    // typeof(ExportCellAttribute),
                    // typeof(ExportImageSourceHandlerAttribute),
                    typeof(ExportFontAttribute)
                });
            }
        }

        internal static void RegisterCompatRenderers(
			Assembly[] assemblies,
			Assembly defaultRendererAssembly,
			Action<Type> viewRegistered)
		{
			if (IsInitializedRenderers)
				return;

			IsInitializedRenderers = true;

			// Only need to do this once
			Microsoft.Maui.Controls.Internals.Registrar.RegisterAll(
				assemblies,
				defaultRendererAssembly,
				new[] {
						// typeof(ExportRendererAttribute),
						// typeof(ExportCellAttribute),
						// typeof(ExportImageSourceHandlerAttribute),
						typeof(ExportFontAttribute)
					}, default(InitializationFlags),
				viewRegistered);
		}
    }
}
