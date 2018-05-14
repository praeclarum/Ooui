using Ooui.Forms;
using Ooui.Forms.Maps;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Maps;

[assembly: ExportRenderer(typeof(Map), typeof(MapViewRenderer))]
namespace Xamarin
{
    public static class FormsMaps
    {
        public static string APIKey { get; private set; }
        public static void Init(string apiKey)
        {
            APIKey = apiKey;
            Registrar.RegisterAll(new[] {
                typeof(ExportRendererAttribute),
                typeof(ExportCellAttribute),
                typeof(ExportImageSourceHandlerAttribute),
            });
        }
    }
}
