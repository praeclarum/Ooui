using Ooui;
using Xamarin.Forms;

namespace Samples
{
    public class ButtonXamlSample : ISample
    {
        public string Title => "Xamarin.Forms Button XAML";
        public string Path => "buttons";

        public Ooui.Element CreateElement ()
        {
            var page = new ButtonXaml.ButtonXamlPage ();
            return page.GetOouiElement ();
        }

        public void Publish()
        {
            UI.Publish(Path, CreateElement);
        }
    }
}
