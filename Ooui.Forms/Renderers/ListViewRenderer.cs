using Ooui.Forms.Extensions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Ooui.Forms.Cells;
using Ooui.Html;

namespace Ooui.Forms.Renderers
{
    public class ListViewRenderer : ViewRenderer<ListView, List>
    {
        private bool _disposed;

        public ListViewRenderer ()
        {
        }

        ITemplatedItemsView<Cell> TemplatedItemsView => Element;

        protected override void OnElementChanged (ElementChangedEventArgs<ListView> e)
        {
            if (e.OldElement != null) {
                var templatedItems = TemplatedItemsView.TemplatedItems;
                templatedItems.CollectionChanged -= OnCollectionChanged;
                e.OldElement.ScrollToRequested -= ListView_ScrollToRequested;
            }

            if (e.NewElement != null) {
                if (Control == null) {
                    var list = new List ();
                    list.Style.Overflow = "scroll";
                    list.Style.Padding = "0";

                    SetNativeControl (list);
                }

                var templatedItems = TemplatedItemsView.TemplatedItems;
                templatedItems.CollectionChanged += OnCollectionChanged;
                e.NewElement.ScrollToRequested += ListView_ScrollToRequested;

                UpdateItems ();
                UpdateBackgroundColor ();
            }

            base.OnElementChanged (e);
        }

        protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);

            if (e.PropertyName == ItemsView<Cell>.ItemsSourceProperty.PropertyName)
                UpdateItems ();
        }

        protected override void Dispose (bool disposing)
        {
            UnsubscribeCellClicks ();

            base.Dispose (disposing);

            if (disposing && !_disposed) {

                if (Element != null) {
                    var templatedItems = TemplatedItemsView.TemplatedItems;
                    templatedItems.CollectionChanged -= OnCollectionChanged;
                    Element.ScrollToRequested -= ListView_ScrollToRequested;
                }

                _disposed = true;
            }
        }

        private void OnCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateItems ();
        }

        private void UnsubscribeCellClicks ()
        {
            if (Control == null)
                return;
            foreach (var c in Control.Children) {
                if (c is Html.Element e)
                    e.Click -= ListItem_Click;
            }
        }

        private void UpdateItems ()
        {
            if (Control == null)
                return;

            var listItems = Control.Children.OfType<ListItem> ().ToList ();

            var items = TemplatedItemsView.TemplatedItems;

            if (listItems.Count > items.Count) {
                for (var i = items.Count; i < listItems.Count; i++) {
                    listItems[i].Click -= ListItem_Click;
                    Control.RemoveChild (listItems[i]);
                }
                listItems.RemoveRange (items.Count, listItems.Count - items.Count);
            }
            if (listItems.Count < items.Count) {
                for (var i = listItems.Count; i < items.Count; i++) {
                    var li = new ListItem ();
                    li.Style["list-style-type"] = "none";
                    li.Click += ListItem_Click;
                    Control.AppendChild (li);
                    listItems.Add (li);
                }
            }

            bool grouping = Element.IsGroupingEnabled;

            if (grouping) {
                // Not Implemented
            }
            else {
                var i = 0;
                foreach (var item in items) {
                    var li = listItems[i];
                    var children = li.Children;
                    var rv = children.Count > 0 ? children[0] as CellElement : null;
                    var cell = GetCell (item, rv);
                    if (rv == null) {
                        li.AppendChild (cell);
                    }
                    i++;
                }
            }
        }

        void ListItem_Click (object sender, TargetEventArgs e)
        {
            if (Control == null)
                return;
            var it = (ListItem)sender;
            var ndx = Control.Children.IndexOf (it);
            Element.NotifyRowTapped (ndx, null);
        }

        void ListView_ScrollToRequested (object sender, ScrollToRequestedEventArgs e)
        {
            if (Control == null)
                return;

            var oe = (ITemplatedItemsListScrollToRequestedEventArgs)e;
            var item = oe.Item;
            var group = oe.Group;
            switch (e.Position) {
                case ScrollToPosition.Start:
                    Control.Send (Ooui.Message.Set (Control.Id, "scrollTop", 0));
                    break;
                case ScrollToPosition.End:
                    Control.Send (Ooui.Message.Set (Control.Id, "scrollTop", new Ooui.Message.PropertyReference { TargetId = Control.Id, Key = "scrollHeight" }));
                    break;
            }
        }

        void UpdateBackgroundColor ()
        {
            if (Control == null)
                return;
            
            var backgroundColor = Element.BackgroundColor.ToOouiColor (Xamarin.Forms.Color.White);

            Control.Style.BackgroundColor = backgroundColor;
        }

        CellElement GetCell (Cell cell, CellElement reusableView)
        {
            var renderer = (Cells.CellRenderer)Registrar.Registered.GetHandlerForObject<IRegisterable> (cell);

            var realCell = renderer.GetCellElement (cell, reusableView, Control);

            return realCell;
        }
    }
}
