using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;

namespace Ooui.Maui
{
    public class MauiContext : IMauiContext
    {
        readonly IServiceProvider _services;
        readonly IMauiHandlersServiceProvider? _mauiHandlersServiceProvider;

        public MauiContext(IServiceProvider services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _mauiHandlersServiceProvider = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<IMauiHandlersServiceProvider>(Services);
        }

        public IServiceProvider Services =>
            _services ?? throw new InvalidOperationException($"No service provider was specified during construction.");

        public IMauiHandlersServiceProvider Handlers =>
            _mauiHandlersServiceProvider ?? throw new InvalidOperationException($"No service provider was specified during construction.");
    }
}
