using Ooui.Forms.Extensions;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Ooui.Forms.Cells
{
    public class EntryCellRenderer : CellRenderer
    {
        private static Cell _cell;

        public override CellView GetCell(Cell item, CellView reusableView, List listView)
        {
            TextInput nativeEntry = null;

            var nativeEntryCell = base.GetCell(item, reusableView, listView);

            if (nativeEntryCell == null)
                nativeEntryCell = new CellView();
            else
            {
                nativeEntryCell.Cell.PropertyChanged -= OnCellPropertyChanged;

                nativeEntry = nativeEntryCell.CustomView.FirstChild as TextInput;
                if (nativeEntry != null)
                {
                    nativeEntryCell.CustomView.RemoveChild(nativeEntry);
                    nativeEntry.Change -= OnTextChanged;
                }
            }

            SetRealCell(item, nativeEntryCell);

            if (nativeEntry == null)
                nativeEntryCell.CustomView.AppendChild(nativeEntry = new TextInput());

            var entryCell = (EntryCell)item;

            nativeEntryCell.Cell = item;
            nativeEntryCell.SecondCol.Style.Width = "25%";
            _cell = nativeEntryCell.Cell;

            nativeEntryCell.Cell.PropertyChanged += OnCellPropertyChanged;
            nativeEntry.Change += OnTextChanged;

            WireUpForceUpdateSizeRequested(item, nativeEntryCell);

            UpdateBackground(nativeEntryCell, entryCell);
            UpdateLabel(nativeEntryCell, entryCell);
            UpdateText(nativeEntryCell, entryCell);
            UpdatePlaceholder(nativeEntryCell, entryCell);
            UpdateLabelColor(nativeEntryCell, entryCell);

            return nativeEntryCell;
        }

        private static void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var entryCell = (EntryCell)sender;
            var realCell = (CellView)GetRealCell(entryCell);

            if (e.PropertyName == EntryCell.LabelProperty.PropertyName)
                UpdateLabel(realCell, entryCell);
            else if (e.PropertyName == EntryCell.TextProperty.PropertyName)
                UpdateText(realCell, entryCell);
            else if (e.PropertyName == EntryCell.PlaceholderProperty.PropertyName)
                UpdatePlaceholder(realCell, entryCell);
            else if (e.PropertyName == EntryCell.LabelColorProperty.PropertyName)
                UpdateLabelColor(realCell, entryCell);
        }

        private static void UpdateLabel(CellView cell, EntryCell entryCell)
        {
            cell.TextLabel.Text = entryCell.Label ?? string.Empty;
        }

        private static void UpdateLabelColor(CellView cell, EntryCell entryCell)
        {
            cell.TextLabel.Style.Color = entryCell.LabelColor.ToOouiColor(OouiTheme.TextColor);
        }

        private static void UpdatePlaceholder(CellView cell, EntryCell entryCell)
        {
            if (cell.CustomView.FirstChild is TextInput textInput)
                textInput.Placeholder = entryCell.Placeholder ?? string.Empty;
        }

        private static void UpdateText(CellView cell, EntryCell entryCell)
        {
            if (cell.CustomView.FirstChild is TextInput textInput)
                textInput.Text = entryCell.Text ?? string.Empty;
        }

        private static void OnTextChanged(object sender, EventArgs eventArgs)
        {
            var textInput = (TextInput)sender;

            CellView realCell = GetRealCell(_cell);

            if (realCell != null)
                ((EntryCell)realCell.Cell).Text = textInput.Text;
        }
    }
}
