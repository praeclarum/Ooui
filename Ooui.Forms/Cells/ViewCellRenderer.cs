using Xamarin.Forms;

namespace Ooui.Forms.Cells
{
    public class ViewCellRenderer : CellRenderer
    {
        protected override CellElement CreateCellElement (Cell cell)
        {
            return new ViewCellElement ();
        }
    }
}
