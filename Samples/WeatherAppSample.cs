using Ooui;
using Xamarin.Forms;

namespace Samples
{
    public class WeatherAppSample : ISample
    {
        public string Title => "Xamarin.Forms WeatherApp";
        public string Path => "/weatherapp";

        public Ooui.Element CreateElement()
        {
            var page = new WeatherApp.WeatherPage();
            return page.GetOouiElement();
        }

        public void Publish()
        {
            UI.Publish(Path, CreateElement);
        }
    }
}
