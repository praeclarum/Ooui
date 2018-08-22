using System;
using Ooui;
using Xamarin.Forms;

namespace Samples
{
    public class DisplayAlertSample : ISample
    {
        public string Title => "Xamarin.Forms DisplayAlert";

        public Ooui.Html.Element CreateElement ()
        {
            var page = new DisplayAlertPage ();
            return page.GetOouiElement ();
        }

        public void Publish ()
        {
            UI.Publish ("/display-alert", CreateElement);
        }
    }
}
