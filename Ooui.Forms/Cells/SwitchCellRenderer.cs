using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Ooui.Forms.Cells
{
    public class SwitchCellRenderer : CellRenderer
    {
        private static Cell _cell;

        public override CellView GetCell(Cell item, CellView reusableView, List listView)
        {
            var nativeSwitchCell = reusableView as CellView;
            Input oouiSwitch = null;

            if (nativeSwitchCell == null)
                nativeSwitchCell = new CellView();
            else
            {
                oouiSwitch = nativeSwitchCell.CustomView.FirstChild as Input;

                if (oouiSwitch != null)
                {
                    nativeSwitchCell.CustomView.RemoveChild(oouiSwitch);
                    oouiSwitch.Click -= OnSwitchClick;
                }
                nativeSwitchCell.Cell.PropertyChanged -= OnCellPropertyChanged;
            }

            SetRealCell(item, nativeSwitchCell);

            if (oouiSwitch == null)
            {
                oouiSwitch = new Input(InputType.Checkbox);
                oouiSwitch.SetAttribute("data-toggle", "toggle");
            }

            var switchCell = (SwitchCell)item;

            nativeSwitchCell.Cell = item;
            nativeSwitchCell.SecondCol.Style.Width = "25%";
            _cell = nativeSwitchCell.Cell;

            nativeSwitchCell.Cell.PropertyChanged += OnCellPropertyChanged;
            nativeSwitchCell.CustomView.AppendChild(oouiSwitch);
            nativeSwitchCell.TextLabel.Text = switchCell.Text ?? string.Empty;

            oouiSwitch.IsChecked = switchCell.On;
            oouiSwitch.Click += OnSwitchClick;

            WireUpForceUpdateSizeRequested(item, nativeSwitchCell);

            UpdateBackground(nativeSwitchCell, item);

            return nativeSwitchCell;
        }

        private void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var switchCell = (SwitchCell)sender;
            var nativeSwitchCell = (CellView)GetRealCell(switchCell);

            if (e.PropertyName == SwitchCell.OnProperty.PropertyName)
                ((Input)nativeSwitchCell.CustomView.FirstChild).IsChecked = switchCell.On;
            else if (e.PropertyName == SwitchCell.TextProperty.PropertyName)
                nativeSwitchCell.TextLabel.Text = switchCell.Text ?? string.Empty;
        }

        private void OnSwitchClick(object sender, EventArgs eventArgs)
        {
            var switchInput = (Input)sender;

            CellView realCell = GetRealCell(_cell);

            if (realCell != null)
                ((SwitchCell)realCell.Cell).On = switchInput.IsChecked;
        }
    }
}
