using System;
using System.ComponentModel;
using Ooui.Forms.Extensions;
using Xamarin.Forms;

namespace Ooui.Forms.Cells
{
    public class TextCellElement : CellElement
    {
        public Ooui.Html.Label TextLabel { get; } = new Ooui.Html.Label ();

        public Ooui.Html.Label DetailTextLabel { get; } = new Ooui.Html.Label ();

        public TextCellElement ()
        {
            AppendChild (TextLabel);
            AppendChild (DetailTextLabel);
        }

        protected override void BindCell ()
        {
            Cell.PropertyChanged += Cell_PropertyChanged;

            if (Cell is TextCell textCell) {
                TextLabel.Text = textCell.Text ?? string.Empty;
                DetailTextLabel.Text = textCell.Detail ?? string.Empty;
                TextLabel.Style.Color = textCell.TextColor.ToOouiColor (OouiTheme.TextColor);
                DetailTextLabel.Style.Color = textCell.DetailColor.ToOouiColor (OouiTheme.SecondaryTextColor);
            }

            base.BindCell ();
        }

        protected override void UnbindCell ()
        {
            Cell.PropertyChanged -= Cell_PropertyChanged;
            base.UnbindCell ();
        }

        void Cell_PropertyChanged (object sender, PropertyChangedEventArgs args)
        {
            if (!(Cell is TextCell textCell))
                return;
            
            if (args.PropertyName == TextCell.TextProperty.PropertyName)
                TextLabel.Text = textCell.Text ?? string.Empty;
            else if (args.PropertyName == TextCell.DetailProperty.PropertyName)
                DetailTextLabel.Text = textCell.Detail ?? string.Empty;
            else if (args.PropertyName == TextCell.TextColorProperty.PropertyName)
                TextLabel.Style.Color = textCell.TextColor.ToOouiColor (OouiTheme.TextColor);
            else if (args.PropertyName == TextCell.DetailColorProperty.PropertyName)
                DetailTextLabel.Style.Color = textCell.DetailColor.ToOouiColor (OouiTheme.SecondaryTextColor);
        }
    }
}
