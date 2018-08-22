using Ooui.Html;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Ooui.Forms.Cells
{
    public class CellElement : Div, INativeElementView
    {
        Cell cell;

        public Cell Cell {
            get => cell;
            set {
                if (cell == value)
                    return;

                if (cell != null)
                    UnbindCell ();

                cell = value;

                if (cell != null)
                    BindCell ();
            }
        }

        public virtual Xamarin.Forms.Element Element => Cell;

        public CellElement ()
        {
            Style.Width = "100%";
        }

        protected virtual void UnbindCell ()
        {
            Device.BeginInvokeOnMainThread (Cell.SendDisappearing);
        }

        protected virtual void BindCell ()
        {
            Device.BeginInvokeOnMainThread (cell.SendAppearing);
        }
    }
}
