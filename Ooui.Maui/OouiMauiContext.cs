using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Ooui.Maui
{
    public class OouiMauiContext : IMauiContext
    {
        public IServiceProvider Services { get; }

        readonly IMauiHandlersServiceProvider? _mauiHandlersServiceProvider;
        public IMauiHandlersServiceProvider Handlers =>
            _mauiHandlersServiceProvider ?? throw new InvalidOperationException($"No service provider was specified during construction.");

        public OouiMauiContext(IServiceProvider services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            _mauiHandlersServiceProvider = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<IMauiHandlersServiceProvider>(services);

            if (!Microsoft.Maui.Controls.Compatibility.Forms.IsInitialized)
                Microsoft.Maui.Controls.Compatibility.Forms.Init(this);
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
                .Build();

            var Services = host.Services;
            return Services;
        }

        static void ConfigureNativeServices(HostBuilderContext ctx, IServiceCollection services)
        {
        }
    }
}
