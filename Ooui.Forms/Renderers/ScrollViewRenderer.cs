using System;
using System.ComponentModel;
using Ooui.Forms.Extensions;
using Xamarin.Forms;

namespace Ooui.Forms.Renderers
{
    public class ScrollViewRenderer : VisualElementRenderer<ScrollView>
    {
        bool disposed = false;

        protected override void OnElementChanged (ElementChangedEventArgs<ScrollView> e)
        {
            if (e.OldElement != null) {
                e.OldElement.ScrollToRequested -= Element_ScrollToRequested;
            }

            if (e.NewElement != null) {
                Style.Overflow = "scroll";

                e.NewElement.ScrollToRequested += Element_ScrollToRequested;
            }

            base.OnElementChanged (e);
        }

        protected override void Dispose (bool disposing)
        {
            base.Dispose (disposing);

            if (disposing && !disposed) {
                if (Element != null) {
                    Element.ScrollToRequested -= Element_ScrollToRequested;
                }
                disposed = true;
            }
        }

        void Element_ScrollToRequested (object sender, ScrollToRequestedEventArgs e)
        {
            var oe = (ITemplatedItemsListScrollToRequestedEventArgs)e;
            var item = oe.Item;
            var group = oe.Group;
            if (e.Mode == ScrollToMode.Position) {
                Send (Ooui.Message.Set (Id, "scrollTop", e.ScrollY));
                Send (Ooui.Message.Set (Id, "scrollLeft", e.ScrollX));
            }
            else {
                switch (e.Position) {
                    case ScrollToPosition.Start:
                        Send (Ooui.Message.Set (Id, "scrollTop", 0));
                        break;
                    case ScrollToPosition.End:
                        Send (Ooui.Message.Set (Id, "scrollTop", new Ooui.Message.PropertyReference { TargetId = Id, Key = "scrollHeight" }));
                        break;
                }
            }
        }
    }
}
