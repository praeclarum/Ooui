using System;
using Xamarin.Forms;

namespace Ooui.Forms
{
    public class LinkLabel : Xamarin.Forms.Label
    {
        public static readonly BindableProperty HRefProperty = BindableProperty.Create ("HRef", typeof (string),
            typeof (LinkLabel), string.Empty, BindingMode.OneWay, null, null, null, null);

        public string HRef {
            get { return (string)base.GetValue (HRefProperty); }
            set { base.SetValue (HRefProperty, value); }
        }

        public static readonly BindableProperty TargetProperty = BindableProperty.Create ("Target", typeof (string),
            typeof (LinkLabel), string.Empty, BindingMode.OneWay, null, null, null, null);

        public string Target {
            get { return (string)base.GetValue (TargetProperty); }
            set { base.SetValue (TargetProperty, value); }
        }

        public LinkLabel ()
        {
        }
    }
}
