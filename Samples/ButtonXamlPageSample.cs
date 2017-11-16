using System;
using Ooui;
using Xamarin.Forms;

namespace Samples
{
    public class ButtonXamlPageSample : ISample
    {
        public string Title => "Xamarin.Forms Button XAML";

        public Ooui.Element CreateElement ()
        {
            var page = new ButtonXaml.ButtonXamlPage ();
            return page.GetOouiElement ();
        }
    }
}
