using Microsoft.Maui.Hosting;

namespace Microsoft.Maui.Controls.Compatibility
{
    // Startup templates refer to this so it needs to be in the Microsft namespace
    public static class AppHostBuilderExtensions
    {
        public static IAppHostBuilder UseFormsCompatibility(this IAppHostBuilder appBuilder)
        {
            DependencyService.Register<Microsoft.Maui.Controls.Internals.ISystemResourcesProvider, Microsoft.Maui.Controls.Compatibility.Platform.Ooui.ResourcesProvider>();
            return appBuilder;
        }
    }
}
