using Ooui.Forms.Extensions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Ooui.Forms.Cells;

namespace Ooui.Forms.Renderers
{
    public class ListViewRenderer : ViewRenderer<ListView, List>
    {
        const int DefaultRowHeight = 44;
        private bool _disposed;
        IVisualElementRenderer _prototype;
        Timer _timer;

        int _rowHeight;

        public int RowHeight
        {
            get => _rowHeight;
            set => _rowHeight = value;
        }

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
                    // Make the list element positioned so child elements will
                    // be positioned relative to it. This will allow the list
                    // to scroll properly.
                    list.Style.Position = "relative";

                    SetNativeControl (list);
                }

                var templatedItems = TemplatedItemsView.TemplatedItems;
                templatedItems.CollectionChanged += OnCollectionChanged;
                e.NewElement.ScrollToRequested += ListView_ScrollToRequested;

                UpdateRowHeight();

                UpdateItems ();
                UpdateSeparator ();
                UpdateBackgroundColor ();
            }

            base.OnElementChanged (e);
        }

        protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);

            if (e.PropertyName == ItemsView<Cell>.ItemsSourceProperty.PropertyName)
                UpdateItems();
            else if (e.PropertyName == Xamarin.Forms.ListView.RowHeightProperty.PropertyName)
            {
                UpdateRowHeight();
                UpdateItems();
            }
            else if (e.PropertyName == VisualElement.WidthProperty.PropertyName)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                }
                else
                {
                    _timer = new Timer();
                    _timer.Interval = 250;
                    _timer.Elapsed += delegate {
                        UpdateItems();
                    };
                    _timer.Enabled = true;
                    _timer.AutoReset = false;
                }
                _timer.Start();
            }
            else if (e.PropertyName == Xamarin.Forms.ListView.SeparatorColorProperty.PropertyName)
                UpdateSeparator ();
            else if (e.PropertyName == Xamarin.Forms.ListView.SeparatorVisibilityProperty.PropertyName)
                UpdateSeparator ();
        }

        protected override void Dispose (bool disposing)
        {
            UnsubscribeCellClicks ();

            base.Dispose (disposing);

            if (disposing && !_disposed) {

                ClearPrototype();

                if (Element != null) {
                    var templatedItems = TemplatedItemsView.TemplatedItems;
                    templatedItems.CollectionChanged -= OnCollectionChanged;
                    Element.ScrollToRequested -= ListView_ScrollToRequested;
                }

                _disposed = true;
            }
        }

        void ClearPrototype()
        {
            if (_prototype != null)
            {
                var element = _prototype.Element;
                element?.ClearValue(Platform.RendererProperty);
                _prototype?.Dispose();
                _prototype = null;
            }
        }

        private void OnCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateItems ();
            UpdateSeparator ();
        }

        private void UnsubscribeCellClicks ()
        {
            if (Control == null)
                return;
            foreach (var c in Control.Children) {
                if (c is Element e)
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
                double offset = 0;
                foreach (var item in items) {
                    var li = listItems[i];
                    var nativeCell = items[i];
                    var children = li.Children;
                    var rv = children.Count > 0 ? children[0] as CellElement : null;
                    var cell = GetCell (item, rv);
                    var height = CalculateHeightForCell(nativeCell);
                    li.Style.Height = height;
                    var viewCell = (ViewCell)cell.Cell;
                    if (viewCell != null && viewCell.View != null)
                    {
                        var rect = new Rectangle(0, offset, Element.Width, height);
                        Layout.LayoutChildIntoBoundingRegion(viewCell.View, rect);
                    }
                    offset += height;
                    if (rv == null) {
                        li.AppendChild (cell);
                    }
                    i++;
                }
            }
        }

        private void UpdateSeparator()
        {
            if (Control == null)
                return;

            var listItems = Control.Children.OfType<ListItem>().ToList();

            foreach (var li in listItems)
            {
                if (Element.SeparatorVisibility == SeparatorVisibility.Default)
                {
                    var color = Element.SeparatorColor.ToOouiColor(Color.FromStyleValue("#999"));
                    li.Style["border-bottom"] = string.Format("{0}px {1} {2}", 1, "solid", color.ToString());
                } 
                else
                {
                    li.Style["border-bottom"] = null;
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

        void UpdateRowHeight()
        {
            var rowHeight = Element.RowHeight;
            if (Element.HasUnevenRows && rowHeight == -1)
                RowHeight = -1;
            else
                RowHeight = rowHeight <= 0 ? DefaultRowHeight : rowHeight;
        }

        internal double CalculateHeightForCell(Cell cell)
        {  
            if (!Element.HasUnevenRows)
            {
                return RowHeight;
            } 
            else 
            {
                var viewCell = cell as ViewCell;
                if (viewCell != null && viewCell.View != null)
                {
                    var target = viewCell.View;
                    if (_prototype == null)
                        _prototype = Platform.CreateRenderer(target);
                    else
                        _prototype.SetElement(target);

                    Platform.SetRenderer(target, _prototype);

                    var req = target.Measure(Element.Width, double.PositiveInfinity, MeasureFlags.IncludeMargins);

                    target.ClearValue(Platform.RendererProperty);
                    foreach (Xamarin.Forms.Element descendant in target.Descendants())
                    {
                        IVisualElementRenderer renderer = Platform.GetRenderer(descendant as VisualElement);

                        // Clear renderer from descendent; this will not happen in Dispose as normal because we need to
                        // unhook the Element from the renderer before disposing it.
                        descendant.ClearValue(Platform.RendererProperty);
                        renderer?.Dispose();
                        renderer = null;
                    }

                    var height = req.Request.Height;
                    return height > 1 ? height : DefaultRowHeight;
                }
                var renderHeight = cell.RenderHeight;
                return renderHeight > 0 ? renderHeight : DefaultRowHeight;
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
