using Ooui;
using Xamarin.Forms;

namespace Samples
{
    public class WebViewSample : ISample
    {
        public string Title => "Xamarin.Forms WebView Sample";

        public Ooui.Html.Element CreateElement()
        {
            var panel = new StackLayout();

            var titleLabel = new Xamarin.Forms.Label
            {
                Text = "WebView",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold
            };
            panel.Children.Add(titleLabel);

            WebView webview = new WebView
            {
                Source = "http://www.xamarin.com"
            };
            panel.Children.Add(webview);

            var page = new ContentPage
            {
                Content = panel
            };

            return page.GetOouiElement();
        }

        public void Publish()
        {
            UI.Publish("/webview", CreateElement);
        }
    }
}
