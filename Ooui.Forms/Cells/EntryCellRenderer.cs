using System;
using Xamarin.Forms;

namespace Ooui.Forms.Cells
{
    public class EntryCellRenderer : TextCellRenderer
    {
        protected override CellElement CreateCellElement (Cell item)
        {
            return new EntryCellElement ();
        }
    }
}
