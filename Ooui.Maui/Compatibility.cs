using Microsoft.Maui.Hosting;

namespace Microsoft.Maui.Controls.Compatibility
{
    public static class AppHostBuilderExtensions
    {
        public static IAppHostBuilder UseFormsCompatibility(this IAppHostBuilder appBuilder)
        {
            return appBuilder;
        }
    }
}