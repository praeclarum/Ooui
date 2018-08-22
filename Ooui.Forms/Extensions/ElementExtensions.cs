using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Ooui.Forms.Extensions
{
    public static class ElementExtensions
    {
        public static SizeRequest GetSizeRequest (this Ooui.Html.Element self, double widthConstraint, double heightConstraint,
            double minimumWidth = -1, double minimumHeight = -1)
        {
            var rw = 0.0;
            var rh = 0.0;
            Size s = new Size (0, 0);
            var measured = false;

            if (self.Style.Width.Equals ("inherit")) {
                s = self.Text.MeasureSize (self.Style, widthConstraint, heightConstraint);
                measured = true;
                rw = double.IsPositiveInfinity (s.Width) ? double.PositiveInfinity : Math.Ceiling (s.Width);
            }
            else {
                rw = self.Style.GetNumberWithUnits ("width", "px", 640);
            }

            if (self.Style.Height.Equals ("inherit")) {
                if (!measured) {
                    s = self.Text.MeasureSize (self.Style, widthConstraint, heightConstraint);
                    measured = true;
                }
                rh = double.IsPositiveInfinity (s.Height) ? double.PositiveInfinity : Math.Ceiling (s.Height * 1.4);
            }
            else {
                rh = self.Style.GetNumberWithUnits ("height", "px", 480);
            }

            var minimum = new Size (minimumWidth < 0 ? rw : minimumWidth,
                minimumHeight < 0 ? rh : minimumHeight);

            return new SizeRequest (new Size (rw, rh), minimum);
        }

        public static IEnumerable<T> CollectElements<T>(this Xamarin.Forms.Element element) where T : Xamarin.Forms.Element {
            if (element == null)
                yield break;

            foreach (var child in element.GetChildrenRecursive())
                if (typeof(T).IsAssignableFrom(child.GetType()))
                    yield return child as T;


            /*var typ = element.GetType();

            if (typeof(Cell).IsAssignableFrom(typeof(T)))
                callHandler(element as T);

            if (typeof(Cell).IsAssignableFrom(typ)) {
                var cell = element as Cell;
                if (typeof(ViewCell).IsAssignableFrom(typ))
                    CollectElements<T>((cell as ViewCell).View, callHandler);
            } else if (typeof(ContentPage).IsAssignableFrom(typ)) {
                var page = element as ContentPage;
                CollectElements<T>(page.Content, callHandler);
            } else if (typeof(Layout<View>).IsAssignableFrom(typ)) {
                var layout = element as Layout<View>;
                foreach (var child in layout.Children)
                    CollectElements<T>(child, callHandler);
            } else if (typeof(Layout).IsAssignableFrom(typ)) {
                var layout = element as Layout;
                foreach (var child in layout.Children)
                    CollectElements<T>(child, callHandler);
            } else if (typeof(ITemplatedItemsView<Cell>).IsAssignableFrom(typ)) {
                var tiv = element as ITemplatedItemsView<Cell>;
                foreach (var cell in tiv.TemplatedItems)
                    CollectElements<T>(cell, callHandler);
            }*/
        }

        public static IEnumerable<Xamarin.Forms.Element> GetChildren(this Xamarin.Forms.Element element) {
            if (element == null)
                yield break;

            var typ = element.GetType();

            if (typeof(Cell).IsAssignableFrom(typ)) {
                var cell = element as Cell;
                if (typeof(ViewCell).IsAssignableFrom(typ))
                    yield return (element as ViewCell).View;
            } else if (typeof(ContentPage).IsAssignableFrom(typ)) {
                var page = element as ContentPage;
                yield return page.Content;
            } else if (typeof(Layout<View>).IsAssignableFrom(typ)) {
                var layout = element as Layout<View>;
                foreach (var child in layout.Children)
                    yield return child;
            } else if (typeof(Layout).IsAssignableFrom(typ)) {
                var layout = element as Layout;
                foreach (var child in layout.Children)
                    yield return child;
            } else if (typeof(ITemplatedItemsView<Cell>).IsAssignableFrom(typ)) {
                var tiv = element as ITemplatedItemsView<Cell>;
                foreach (var cell in tiv.TemplatedItems)
                    yield return cell;
            }
            yield break;
        }

        public static IEnumerable<Xamarin.Forms.Element> GetChildrenRecursive(this Xamarin.Forms.Element element) {
            foreach (var child in element.GetChildren()) {
                yield return child;
                foreach (var gchild in GetChildrenRecursive(child))
                    yield return gchild;
            }
        }
    }
}
