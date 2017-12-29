using System;
using Xamarin.Forms;

namespace Ooui.Forms
{
    public class LinkLabel : Xamarin.Forms.Label
    {
        public static readonly BindableProperty HRefProperty = BindableProperty.Create ("HRef", typeof (string),
            typeof (LinkView), string.Empty, BindingMode.OneWay, null, null, null, null);

        public string HRef {
            get { return (string)base.GetValue (HRefProperty); }
            set { base.SetValue (HRefProperty, value); }
        }

        public LinkLabel ()
        {
        }
    }
}
