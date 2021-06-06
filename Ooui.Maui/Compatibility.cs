using Microsoft.Maui.Hosting;

namespace Microsoft.Maui.Controls.Compatibility
{
    // Startup templates refer to this so it needs to be in the Microsft namespace
    public static class AppHostBuilderExtensions
    {
        public static IAppHostBuilder UseFormsCompatibility(this IAppHostBuilder appBuilder)
        {
            return appBuilder;
        }
    }
}
