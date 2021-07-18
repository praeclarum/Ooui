using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Ooui.Maui
{
    public class OouiMauiContext : IMauiContext
    {
        public IServiceProvider? Services { get; }

        readonly IMauiHandlersServiceProvider? _mauiHandlersServiceProvider;
        public IMauiHandlersServiceProvider Handlers =>
            _mauiHandlersServiceProvider ?? throw new InvalidOperationException($"No service provider was specified during construction.");

        public OouiMauiContext()
        {
        }

        public OouiMauiContext(IServiceProvider services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            _mauiHandlersServiceProvider = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<IMauiHandlersServiceProvider>(services);

            if (!Controls.Compatibility.Forms.IsInitialized)
                Controls.Compatibility.Forms.Init(this);
        }

        public OouiMauiContext(IStartup startup)
            : this (GetStartupServices (startup))
        {
        }

        static OouiMauiContext()
        {
            Microsoft.Maui.Controls.DependencyService.Register<Microsoft.Maui.Controls.Internals.ISystemResourcesProvider, Microsoft.Maui.Controls.Compatibility.Platform.Ooui.ResourcesProvider>();
        }

        public static OouiMauiContext FromStartup<TStartup> ()
            where TStartup : IStartup, new()
        {
            var startup = new TStartup();
            return new OouiMauiContext (startup);
        }

        static IServiceProvider GetStartupServices(IStartup startup)
        {
            var host = startup
                .CreateAppHostBuilder()
                .ConfigureServices(ConfigureNativeServices)
                .ConfigureUsing(startup)
                .ConfigureMauiHandlers(ConfigureOouiHandlers)
                .Build();

            var Services = host.Services;
            return Services;
        }

        static void ConfigureOouiHandlers(IMauiHandlersCollection handlers)
        {
            Console.WriteLine("Registering IPage");
            // var previousHandler = handlers.GetHandler<IPage>();
            handlers.AddHandler(typeof(IPage), typeof(Ooui.Maui.Handlers.PageHandler));
            handlers.AddHandler(typeof(Microsoft.Maui.Controls.Page), typeof(Ooui.Maui.Handlers.PageHandler));
            handlers.AddHandler(typeof(Microsoft.Maui.Controls.Layout2.Layout), typeof(Ooui.Maui.Handlers.LayoutHandler));
            handlers.AddHandler(typeof(Microsoft.Maui.Controls.Button), typeof(Ooui.Maui.Handlers.ButtonHandler));
            handlers.AddHandler(typeof(Microsoft.Maui.Controls.Label), typeof(Ooui.Maui.Handlers.LabelHandler));
            // var newHandler = handlers.GetHandler<IPage>();
            // Console.WriteLine($"P = {previousHandler}, N = {newHandler}");
        }

        static void ConfigureNativeServices(HostBuilderContext ctx, IServiceCollection services)
        {
        }
    }
}
