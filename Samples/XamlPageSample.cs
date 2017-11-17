using Xamarin.Forms;

namespace Samples
{
    public class XamlPageSample
    {
        public string Title => "Xamarin.Forms Button XAML";

        public Ooui.Element CreateElement (Page page)
        {
            return page.GetOouiElement ();
        }
    }
}
