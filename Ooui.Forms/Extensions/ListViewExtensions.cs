using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Ooui.Forms.Extensions
{
    public static class ListViewExtensions
    {
        public static void UpdateChildrenSize(this ListView self) {
            var pos = new Point(self.X, self.Y);
            var size = new Size(self.Width, 999999);

            foreach (var cell in self.TemplatedItems) {
                if (!typeof(ViewCell).IsAssignableFrom(cell.GetType()))
                    continue;
                var viewCell = cell as ViewCell;

                if (viewCell.View == null)
                    continue;

                var view = viewCell.View;

                view.VerticalOptions = LayoutOptions.Start;
                Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(
                    view,
                    new Rectangle(pos, size));

                pos.Y += view.Height;
            }

            if (self.Height < pos.Y)
                self.HeightRequest = pos.Y;
        }
    }
}
