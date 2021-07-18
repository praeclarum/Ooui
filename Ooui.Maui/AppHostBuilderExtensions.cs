#nullable enable
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using System.Collections.Generic;

#if __ANDROID__
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Graphics.Native;
using FrameRenderer = Microsoft.Maui.Controls.Compatibility.Platform.Android.FastRenderers.FrameRenderer;
using LabelRenderer = Microsoft.Maui.Controls.Compatibility.Platform.Android.FastRenderers.LabelRenderer;
using ImageRenderer = Microsoft.Maui.Controls.Compatibility.Platform.Android.FastRenderers.ImageRenderer;
using ButtonRenderer = Microsoft.Maui.Controls.Compatibility.Platform.Android.FastRenderers.ButtonRenderer;
using DefaultRenderer = Microsoft.Maui.Controls.Compatibility.Platform.Android.Platform.DefaultRenderer;
#elif WINDOWS
using Microsoft.Maui.Controls.Compatibility.Platform.UWP;
using Microsoft.Maui.Graphics.Win2D;
using BoxRenderer = Microsoft.Maui.Controls.Compatibility.Platform.UWP.BoxViewBorderRenderer;
using CellRenderer = Microsoft.Maui.Controls.Compatibility.Platform.UWP.TextCellRenderer;
using Deserializer = Microsoft.Maui.Controls.Compatibility.Platform.UWP.WindowsSerializer;
using ResourcesProvider = Microsoft.Maui.Controls.Compatibility.Platform.UWP.WindowsResourcesProvider;
using StreamImagesourceHandler = Microsoft.Maui.Controls.Compatibility.Platform.UWP.StreamImageSourceHandler;
using ImageLoaderSourceHandler = Microsoft.Maui.Controls.Compatibility.Platform.UWP.UriImageSourceHandler;
using DefaultRenderer = Microsoft.Maui.Controls.Compatibility.Platform.UWP.DefaultRenderer;
#elif __IOS__
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Graphics.Native;
using WebViewRenderer = Microsoft.Maui.Controls.Compatibility.Platform.iOS.WkWebViewRenderer;
using NavigationPageRenderer = Microsoft.Maui.Controls.Compatibility.Platform.iOS.NavigationRenderer;
using TabbedPageRenderer = Microsoft.Maui.Controls.Compatibility.Platform.iOS.TabbedRenderer;
using FlyoutPageRenderer = Microsoft.Maui.Controls.Compatibility.Platform.iOS.PhoneFlyoutPageRenderer;
using RadioButtonRenderer = Microsoft.Maui.Controls.Compatibility.Platform.iOS.Platform.DefaultRenderer;
using DefaultRenderer = Microsoft.Maui.Controls.Compatibility.Platform.iOS.Platform.DefaultRenderer;
#elif __OOUI__
using Microsoft.Maui.Controls.Compatibility.Platform.Ooui;
#endif

namespace Microsoft.Maui.Controls.Hosting
{
	public static class AppHostBuilderExtensions
	{
		// static readonly Dictionary<Type, Type> DefaultMauiControlHandlers = new Dictionary<Type, Type>
		// {
// #if WINDOWS
// 			{ typeof(Shell), typeof(ShellHandler) },
// #endif
// 			{ typeof(ActivityIndicator), typeof(ActivityIndicatorHandler) },
			// { typeof(Button), typeof(ButtonHandler) },
// 			{ typeof(CheckBox), typeof(CheckBoxHandler) },
// 			{ typeof(DatePicker), typeof(DatePickerHandler) },
// 			{ typeof(Editor), typeof(EditorHandler) },
// 			{ typeof(Entry), typeof(EntryHandler) },
// 			{ typeof(GraphicsView), typeof(GraphicsViewHandler) },
// 			{ typeof(Image), typeof(ImageHandler) },
			// { typeof(Label), typeof(LabelHandler) },
// 			{ typeof(Layout2.Layout), typeof(LayoutHandler) },
// 			{ typeof(Picker), typeof(PickerHandler) },
// 			{ typeof(ProgressBar), typeof(ProgressBarHandler) },
// 			{ typeof(SearchBar), typeof(SearchBarHandler) },
// 			{ typeof(Slider), typeof(SliderHandler) },
// 			{ typeof(Stepper), typeof(StepperHandler) },
// 			{ typeof(Switch), typeof(SwitchHandler) },
// 			{ typeof(TimePicker), typeof(TimePickerHandler) },
// 			{ typeof(Page), typeof(PageHandler) },
// 			{ typeof(Shapes.Ellipse), typeof(ShapeViewHandler) },
// 			{ typeof(Shapes.Line), typeof(ShapeViewHandler) },
// 			{ typeof(Shapes.Path), typeof(ShapeViewHandler) },
// 			{ typeof(Shapes.Polygon), typeof(ShapeViewHandler) },
// 			{ typeof(Shapes.Polyline), typeof(ShapeViewHandler) },
// 			{ typeof(Shapes.Rectangle), typeof(ShapeViewHandler) },
// 			{ typeof(Layout), typeof(LayoutHandler) },
// 			{ typeof(Window), typeof(WindowHandler) },
		// };

		// public static IMauiHandlersCollection AddMauiControlsHandlers(this IMauiHandlersCollection handlersCollection)
			// => handlersCollection.AddHandlers(DefaultMauiControlHandlers);

		public static IAppHostBuilder UseMauiApp<TApp>(this IAppHostBuilder builder)
			where TApp : class, IApplication
		{
			builder.ConfigureServices((context, collection) =>
			{
				collection.AddSingleton<IApplication, TApp>();
			});

			builder.SetupDefaults();

			return builder;
		}

		public static IAppHostBuilder UseMauiApp<TApp>(this IAppHostBuilder builder, Func<IServiceProvider, TApp> implementationFactory)
			where TApp : class, IApplication
		{
			builder.ConfigureServices((context, collection) =>
			{
				collection.AddSingleton<IApplication>(implementationFactory);
			});

			builder.SetupDefaults();

			return builder;
		}

		static IAppHostBuilder SetupDefaults(this IAppHostBuilder builder)
		{
			builder.ConfigureLifecycleEvents(events =>
			{
#if __ANDROID__
				events.AddAndroid(android => android
					.OnApplicationCreating((app) =>
					{
						// This is the initial Init to set up any system services registered by
						// Forms.Init(). This happens in the Application's OnCreate - before
						// any UI has appeared.
						// This creates a dummy MauiContext that wraps the Application.

						var services = MauiApplication.Current.Services;
						var mauiContext = new MauiContext(services, app);
						var state = new ActivationState(mauiContext);
						Forms.Init(state, new InitializationOptions { Flags = InitializationFlags.SkipRenderers });
					})
					.OnMauiContextCreated((mauiContext) =>
					{
						// This is the final Init that sets up the real context from the activity.

						var state = new ActivationState(mauiContext);
						Forms.Init(state);
					}));
#elif __IOS__
				events.AddiOS(iOS => iOS
					.WillFinishLaunching((app, options) =>
					{
						// This is the initial Init to set up any system services registered by
						// Forms.Init(). This happens before any UI has appeared.
						// This creates a dummy MauiContext.

						var services = MauiUIApplicationDelegate.Current.Services;
						var mauiContext = new MauiContext(services);
						var state = new ActivationState(mauiContext);
						Forms.Init(state, new InitializationOptions { Flags = InitializationFlags.SkipRenderers });
						return true;
					})
					.OnMauiContextCreated((mauiContext) =>
					{
						// This is the final Init that sets up the real context from the application.

						var state = new ActivationState(mauiContext);
						Forms.Init(state);
					}));
#elif WINDOWS
				events.AddWindows(windows => windows
					.OnLaunching((app, args) =>
					{
						// This is the initial Init to set up any system services registered by
						// Forms.Init(). This happens before any UI has appeared.
						// This creates a dummy MauiContext.
						// We need to call this so the Window and Root Page can new up successfully
						// The dispatcher that's inside of Forms.Init needs to be setup before the initial
						// window and root page start creating.
						// Inside OnLaunched we grab the MauiContext that's on the window so we can have the correct
						// MauiContext inside Forms

						var services = MauiWinUIApplication.Current.Services;
						var mauiContext = new MauiContext(services);
						var state = new ActivationState(mauiContext, args);
						Forms.Init(state, new InitializationOptions { Flags = InitializationFlags.SkipRenderers });
					})
					.OnMauiContextCreated((mauiContext) =>
					{
						// This is the final Init that sets up the real context from the application.

						var state = new ActivationState(mauiContext);
						Forms.Init(state);
					}));
#endif
			});

			static void HackSetToInitialized() {
				// DependencyService.SetToInitialized();
				var typ = typeof(DependencyService);
				var field = typ.GetField("s_initialized", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
				field.SetValue(null, true);
			}

			builder
				.ConfigureMauiHandlers(handlers =>
				{
					handlers.AddMauiControlsHandlers();
					HackSetToInitialized();					

#if __ANDROID__ || __IOS__ || WINDOWS || MACCATALYST

					handlers.TryAddCompatibilityRenderer(typeof(BoxView), typeof(BoxRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Entry), typeof(EntryRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Editor), typeof(EditorRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Label), typeof(LabelRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Image), typeof(ImageRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Button), typeof(ButtonRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(ImageButton), typeof(ImageButtonRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(TableView), typeof(TableViewRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(ListView), typeof(ListViewRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(CollectionView), typeof(CollectionViewRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(CarouselView), typeof(CarouselViewRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(IndicatorView), typeof(IndicatorViewRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Path), typeof(PathRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Ellipse), typeof(EllipseRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Line), typeof(LineRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Polyline), typeof(PolylineRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Polygon), typeof(PolygonRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Shapes.Rectangle), typeof(RectangleRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(RadioButton), typeof(RadioButtonRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Slider), typeof(SliderRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(WebView), typeof(WebViewRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(SearchBar), typeof(SearchBarRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Switch), typeof(SwitchRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(SwipeView), typeof(SwipeViewRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(DatePicker), typeof(DatePickerRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(TimePicker), typeof(TimePickerRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Picker), typeof(PickerRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Stepper), typeof(StepperRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(ProgressBar), typeof(ProgressBarRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(ScrollView), typeof(ScrollViewRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(ActivityIndicator), typeof(ActivityIndicatorRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Frame), typeof(FrameRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(CheckBox), typeof(CheckBoxRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(TabbedPage), typeof(TabbedPageRenderer));
#if !WINDOWS
					handlers.TryAddCompatibilityRenderer(typeof(Shell), typeof(ShellRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(OpenGLView), typeof(OpenGLViewRenderer));
#else
					handlers.TryAddCompatibilityRenderer(typeof(Layout), typeof(LayoutRenderer));
#endif
					handlers.TryAddCompatibilityRenderer(typeof(NavigationPage), typeof(NavigationPageRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(CarouselPage), typeof(CarouselPageRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Page), typeof(PageRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(FlyoutPage), typeof(FlyoutPageRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(RefreshView), typeof(RefreshViewRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(NativeViewWrapper), typeof(NativeViewWrapperRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(Cell), typeof(CellRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(ImageCell), typeof(ImageCellRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(EntryCell), typeof(EntryCellRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(TextCell), typeof(TextCellRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(ViewCell), typeof(ViewCellRenderer));
					handlers.TryAddCompatibilityRenderer(typeof(SwitchCell), typeof(SwitchCellRenderer));

					// This is for Layouts that currently don't work when assigned to LayoutHandler
					handlers.TryAddCompatibilityRenderer(typeof(ContentView), typeof(DefaultRenderer));
#if __IOS__
					handlers.TryAddCompatibilityRenderer(typeof(AbsoluteLayout), typeof(DefaultRenderer));
#endif


					DependencyService.Register<Xaml.ResourcesLoader>();
					DependencyService.Register<NativeBindingService>();
					DependencyService.Register<NativeValueConverterService>();
					DependencyService.Register<Deserializer>();
					DependencyService.Register<ResourcesProvider>();
					DependencyService.Register<Xaml.ValueConverterProvider>();

					// Shimmed renderers go directly to the registrar to load Image Handlers
					Internals.Registrar.Registered.Register(typeof(FileImageSource), typeof(FileImageSourceHandler));
					Internals.Registrar.Registered.Register(typeof(StreamImageSource), typeof(StreamImagesourceHandler));
					Internals.Registrar.Registered.Register(typeof(UriImageSource), typeof(ImageLoaderSourceHandler));
					Internals.Registrar.Registered.Register(typeof(FontImageSource), typeof(FontImageSourceHandler));


					Internals.Registrar.Registered.Register(typeof(Microsoft.Maui.EmbeddedFont), typeof(Microsoft.Maui.EmbeddedFontLoader));

#endif

#if __IOS__ || MACCATALYST
					Internals.Registrar.RegisterEffect("Xamarin", "ShadowEffect", typeof(ShadowEffect));
#endif
				})
				.ConfigureServices<MauiCompatBuilder>();

			return builder;
		}

		class MauiCompatBuilder : IMauiServiceBuilder
		{
			public void Configure(HostBuilderContext context, IServiceProvider services)
			{
#if __ANDROID__ || __IOS__ || WINDOWS || MACCATALYST
				CompatServiceProvider.SetServiceProvider(services);
#endif

				if (services.GetService<IGraphicsService>() is IGraphicsService graphicsService)
					GraphicsPlatform.RegisterGlobalService(graphicsService);

#if WINDOWS
				var dictionaries = UI.Xaml.Application.Current?.Resources?.MergedDictionaries;
				if (dictionaries != null)
				{
					// WinUI
					AddLibraryResources<UI.Xaml.Controls.XamlControlsResources>();

					// Microsoft.Maui
					AddLibraryResources("MicrosoftMauiCoreIncluded", "ms-appx:///Microsoft.Maui/Platform/Windows/Styles/Resources.xbf");

					// Microsoft.Maui.Controls
					AddLibraryResources("MicrosoftMauiControlsIncluded", "ms-appx:///Microsoft.Maui.Controls/Platform/Windows/Styles/Resources.xbf");

					// Microsoft.Maui.Controls.Compatibility
					AddLibraryResources("MicrosoftMauiControlsCompatibilityIncluded", "ms-appx:///Microsoft.Maui.Controls.Compatibility/Windows/Resources.xbf");
				}
#endif
			}

			public void ConfigureServices(HostBuilderContext context, IServiceCollection services)
			{
#if __IOS__ || MACCATALYST
				services.AddSingleton<IGraphicsService>(NativeGraphicsService.Instance);
#elif __ANDROID__
				services.AddSingleton<IGraphicsService>(NativeGraphicsService.Instance);
#elif WINDOWS
				services.AddSingleton<IGraphicsService>(W2DGraphicsService.Instance);
#endif
			}

#if WINDOWS
			static void AddLibraryResources(string key, string uri)
			{
				var resources = UI.Xaml.Application.Current?.Resources;
				if (resources == null)
					return;

				var dictionaries = resources.MergedDictionaries;
				if (dictionaries == null)
					return;

				if (!resources.ContainsKey(key))
				{
					dictionaries.Add(new UI.Xaml.ResourceDictionary
					{
						Source = new Uri(uri)
					});
				}
			}

			static void AddLibraryResources<T>()
				where T : UI.Xaml.ResourceDictionary, new()
			{
				var dictionaries = UI.Xaml.Application.Current?.Resources?.MergedDictionaries;
				if (dictionaries == null)
					return;

				var found = false;
				foreach (var dic in dictionaries)
				{
					if (dic is T)
					{
						found = true;
						break;
					}
				}

				if (!found)
				{
					var dic = new T();
					dictionaries.Add(dic);
				}
			}
#endif
		}
	}
}
