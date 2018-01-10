using Xamarin.Forms;

namespace Ooui.Forms.Cells
{
    public class ViewCellRenderer : CellRenderer
    {
        public override CellView GetCell(Cell item, CellView reusableView, List listView)
        {
            var viewCell = (ViewCell)item;

            var nativeViewCell = reusableView as ViewCellView;

            if (nativeViewCell == null)
                nativeViewCell = new ViewCellView();

            nativeViewCell.ViewCell = viewCell;

            SetRealCell(item, nativeViewCell);

            WireUpForceUpdateSizeRequested(item, nativeViewCell);

            return nativeViewCell;
        }
    }
}
