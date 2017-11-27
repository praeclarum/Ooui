using System;

using Xamarin.Forms;

namespace Samples
{
    public class XamlPreviewPageSample : ISample
    {
        public string Title => "Xamarin.Forms XAML Editor";

        public Ooui.Element CreateElement ()
        {
            var page = new XamlPreviewPage ();
            return page.GetOouiElement ();
        }
    }
}
