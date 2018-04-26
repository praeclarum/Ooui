using Ooui.Forms.Extensions;
using System.ComponentModel;
using Xamarin.Forms;

namespace Ooui.Forms.Cells
{
    public class TextCellRenderer : CellRenderer
    {
        public override CellView GetCell(Cell item, CellView reusableView, List listView)
        {
            var nativeTextCell = base.GetCell(item, reusableView, listView);
            var textCell = (TextCell)item;

            if (nativeTextCell.Cell != null)
                nativeTextCell.Cell.PropertyChanged -= nativeTextCell.HandlePropertyChanged;

            nativeTextCell.Cell = textCell;
            textCell.PropertyChanged += nativeTextCell.HandlePropertyChanged;
            nativeTextCell.ForwardPropertyChanged = HandlePropertyChanged;

            nativeTextCell.TextLabel.Text = textCell.Text ?? string.Empty;
            nativeTextCell.DetailTextLabel.Text = textCell.Detail ?? string.Empty;
            nativeTextCell.TextLabel.Style.Color = textCell.TextColor.ToOouiColor(OouiTheme.TextColor);
            nativeTextCell.DetailTextLabel.Style.Color = textCell.DetailColor.ToOouiColor(OouiTheme.SecondaryTextColor);

            WireUpForceUpdateSizeRequested(item, nativeTextCell);

            UpdateBackground(nativeTextCell, item);

            return nativeTextCell;
        }

        protected virtual void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var tvc = (CellView)sender;
            var textCell = (TextCell)tvc.Cell;

            if (args.PropertyName == TextCell.TextProperty.PropertyName)
                tvc.TextLabel.Text = textCell.Text ?? string.Empty;
            else if (args.PropertyName == TextCell.DetailProperty.PropertyName)
                tvc.DetailTextLabel.Text = textCell.Detail ?? string.Empty;
            else if (args.PropertyName == TextCell.TextColorProperty.PropertyName)
                tvc.TextLabel.Style.Color = textCell.TextColor.ToOouiColor(OouiTheme.TextColor);
            else if (args.PropertyName == TextCell.DetailColorProperty.PropertyName)
                tvc.DetailTextLabel.Style.Color = textCell.DetailColor.ToOouiColor(OouiTheme.SecondaryTextColor);
        }
    }
}
