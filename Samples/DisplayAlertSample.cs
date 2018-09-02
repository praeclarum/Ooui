using System;
using Ooui;
using Xamarin.Forms;

namespace Samples
{
    public class DisplayAlertSample : ISample
    {
        public string Title => "Xamarin.Forms DisplayAlert";
        public string Path => "/display-alert";

        public Ooui.Element CreateElement ()
        {
            var page = new DisplayAlertPage ();
            return page.GetOouiElement ();
        }

        public void Publish ()
        {
            UI.Publish (Path, CreateElement);
        }
    }
}
