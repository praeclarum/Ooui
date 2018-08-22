using Ooui;
using Xamarin.Forms;

namespace Samples
{
    public class WeatherAppSample : ISample
    {
        public string Title => "Xamarin.Forms WeatherApp";

        public Ooui.Html.Element CreateElement()
        {
            var page = new WeatherApp.WeatherPage();
            return page.GetOouiElement();
        }

        public void Publish()
        {
            UI.Publish("/weatherapp", CreateElement);
        }
    }
}
