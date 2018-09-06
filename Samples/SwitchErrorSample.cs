using Ooui;
using Xamarin.Forms;

namespace Samples
{
    // From https://github.com/praeclarum/Ooui/issues/48
    public class SwitchErrorSample : ISample
    {
        public string Title => "Xamarin.Forms Switch Error";
        public string Path => "/switch";

        public Ooui.Element CreateElement ()
        {
            var layout = new StackLayout();
            var label = new Xamarin.Forms.Label
            {
                Text = "Switch state goes here",
                HorizontalTextAlignment = TextAlignment.Center
            };
            var sw = new Switch
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            sw.Toggled += (sender, args) =>
            {
                label.Text = $"Switch state is: {((Switch)sender).IsToggled}";
            };
            layout.Children.Add(label);
            layout.Children.Add(sw);
            return new ContentPage
            {
                Content = layout
            }.GetOouiElement();
        }

        public void Publish()
        {
            UI.Publish (Path, CreateElement);
        }
    }
}
