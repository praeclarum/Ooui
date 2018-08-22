using Ooui.Forms.Extensions;
using Ooui.Html;
using System;
using Xamarin.Forms;

namespace Ooui.Forms.Cells
{
    public class CellRenderer : IRegisterable
    {
        EventHandler _onForceUpdateSizeRequested;

        static readonly BindableProperty RealCellProperty =
            BindableProperty.CreateAttached ("RealCell", typeof (CellElement), typeof (Cell), null);

        public virtual CellElement GetCellElement (Cell cell, CellElement reusableElement, List listView)
        {
            var cellElement = reusableElement ?? CreateCellElement (cell);

            cellElement.Cell = cell;

            WireUpForceUpdateSizeRequested (cell, cellElement);
            UpdateBackground (cellElement, cell);

            return cellElement;
        }

        protected static CellElement GetRealCell (BindableObject cell)
        {
            return (CellElement)cell.GetValue (RealCellProperty);
        }

        protected static void SetRealCell (BindableObject cell, CellElement renderer)
        {
            cell.SetValue (RealCellProperty, renderer);
        }

        protected virtual CellElement CreateCellElement (Cell cell)
        {
            return new CellElement ();
        }

        protected virtual void OnForceUpdateSizeRequest (Cell cell, CellElement cellElement)
        {
            cellElement.Style.Height = (int)cell.RenderHeight;
        }

        protected void UpdateBackground (CellElement cellElement, Cell cell)
        {
            var backgroundColor = Xamarin.Forms.Color.Default;

            if (backgroundColor == Xamarin.Forms.Color.Default && cell.RealParent is VisualElement element)
                backgroundColor = element.BackgroundColor;

            cellElement.Style.BackgroundColor = backgroundColor.ToOouiColor (Xamarin.Forms.Color.White);
        }

        protected void WireUpForceUpdateSizeRequested (Cell cell, CellElement cellElement)
        {
            cell.ForceUpdateSizeRequested -= _onForceUpdateSizeRequested;

            _onForceUpdateSizeRequested = (sender, e) => {
                OnForceUpdateSizeRequest (cell, cellElement);
            };

            cell.ForceUpdateSizeRequested += _onForceUpdateSizeRequested;
        }
    }
}
