using Ooui.Forms.Extensions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Ooui.Forms.Renderers
{
    public class ListViewRenderer : ViewRenderer<ListView, List>
    {
        private bool _disposed;
        private List _listView;
        private List<ListItem> _cells;

        public ListViewRenderer()
        {
            _cells = new List<ListItem>();
        }

        ITemplatedItemsView<Cell> TemplatedItemsView => Element;

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            if (e.OldElement != null) 
            {
                var templatedItems = TemplatedItemsView.TemplatedItems;
                templatedItems.CollectionChanged -= OnCollectionChanged;
                e.OldElement.ScrollToRequested -= ListView_ScrollToRequested;
            }

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    _listView = new List();
                    _listView.Style.Overflow = "scroll";
                    _listView.Style.Padding = "0";

                    SetNativeControl(_listView);
                }

                var templatedItems = TemplatedItemsView.TemplatedItems;
                templatedItems.CollectionChanged += OnCollectionChanged;
                e.NewElement.ScrollToRequested += ListView_ScrollToRequested;

                UpdateItems ();
                UpdateBackgroundColor();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ListView.ItemsSourceProperty.PropertyName)
                UpdateItems();
        }

        protected override void Dispose(bool disposing)
        {
			UnsubscribeCellClicks();

            base.Dispose(disposing);

            if (disposing && !_disposed) 
            {

                if (Element != null) 
                {
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

        private void UnsubscribeCellClicks()
		{
			foreach (var c in _cells)
			{
				c.Click -= ListItem_Click;
			}
		}

        private void UpdateItems()
        {
			UnsubscribeCellClicks();
            _cells.Clear();

            foreach (var child in _listView.Children) 
            {
                _listView.RemoveChild (child);
            }

            var items = TemplatedItemsView.TemplatedItems;

            if (!items.Any())
            {
                return;
            }

            bool grouping = Element.IsGroupingEnabled;

            if (grouping)
            {
                return;
            }

            foreach (var item in items)
            {
                var cell = GetCell(item);

                var listItem = new ListItem();
                listItem.Style["list-style-type"] = "none";

                listItem.AppendChild(cell);
                listItem.Click += ListItem_Click;

                _cells.Add(listItem);
            }

            foreach (var cell in _cells)
            {
                _listView.AppendChild(cell);
            }
        }

        private void ListItem_Click(object sender, TargetEventArgs e)
        {
            var it = (ListItem)sender;
            var ndx = _cells.IndexOf(it);
            Element.NotifyRowTapped(ndx, null);
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

        private void UpdateBackgroundColor()
        {
            var backgroundColor = Element.BackgroundColor.ToOouiColor();

            _listView.Style.BackgroundColor = backgroundColor;
        }

        private Div GetCell(Cell cell)
        {
            var renderer =
                (Cells.CellRenderer)Registrar.Registered.GetHandler<IRegisterable>(cell.GetType());

            var realCell = renderer.GetCell(cell, null, _listView);

            return realCell;
        }
    }
}
