using System;
using System.ComponentModel;
using Ooui.Forms.Extensions;
using Xamarin.Forms;

namespace Ooui.Forms.Renderers
{
    public class ScrollViewRenderer : ViewRenderer<ScrollView, Div>
    {
        protected override void OnElementChanged (ElementChangedEventArgs<ScrollView> e)
        {
            base.OnElementChanged (e);

            this.Style.Overflow = "scroll";
        }

        protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);
        }
    }
}
