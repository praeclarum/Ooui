using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Ooui.Forms.Cells
{
    public class CellView : Div, INativeElementView
    {
        private Cell _cell;

        public Action<object, PropertyChangedEventArgs> PropertyChanged;

        public CellView()
        {
            CreateUI();
        }

        public Cell Cell
        {
            get { return _cell; }
            set
            {
                if (_cell == value)
                    return;

                if (_cell != null)
                    Device.BeginInvokeOnMainThread(_cell.SendDisappearing);

                _cell = value;

                if (_cell != null)
                    Device.BeginInvokeOnMainThread(_cell.SendAppearing);
            }
        }

        public Div FirstCol { get; private set; }

        public Div SecondCol { get; private set; }

        public Div ThirdCol { get; private set; }

        public Label TextLabel { get; private set; }

        public Label DetailTextLabel { get; private set; }

        public Image ImageView { get; private set; }

        public Div CustomView { get; private set; }

        public virtual Xamarin.Forms.Element Element => Cell;

        public void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private void CreateUI()
        {
            Style.Width = "100%";
            Style.Display = "flex";

            FirstCol = new Div();
            AppendChild(FirstCol);

            SecondCol = new Div();
            AppendChild(SecondCol);

            ThirdCol = new Div();
            AppendChild(ThirdCol);

            ImageView = new Image();
            FirstCol.AppendChild(ImageView);

            TextLabel = new Label();
            SecondCol.AppendChild(TextLabel);

            DetailTextLabel = new Label();
            SecondCol.AppendChild(DetailTextLabel);

            CustomView = new Div();
            ThirdCol.AppendChild(CustomView);
        }
    }
}
