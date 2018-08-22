using Ooui;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Samples
{
    public class PickerSample : ISample
    {
        private Xamarin.Forms.Label _label;
        private Xamarin.Forms.Picker _picker;

        public string Title => "Xamarin.Forms Picker Sample";

		List<string> myItems = new List<string>
		{
			"red",
			"green",
			"yellow",
			"blue",
			"white",
			"black",
			"purple",
			"orange",
		};

        public Ooui.Html.Element CreateElement()
        {
            var panel = new StackLayout();

            var titleLabel = new Xamarin.Forms.Label
            {
                Text = "Picker",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold
            };
            panel.Children.Add(titleLabel);

            _picker = new Picker
			{
				Title = "Hello",
				//VerticalOptions = LayoutOptions.CenterAndExpand,
				ItemsSource = myItems,
			};

            panel.Children.Add(_picker);

            _picker.SelectedIndexChanged += OnPickerValueChanged;

            _label = new Xamarin.Forms.Label
            {
                Text = "Picker value is",
                HorizontalOptions = LayoutOptions.Center
            };
            panel.Children.Add(_label);

            var page = new ContentPage
            {
                Content = panel
            };

            return page.GetOouiElement();
        }

        void OnPickerValueChanged(object sender, EventArgs e)
        {
            _label.Text = String.Format("Picker value is {0} ({1})", _picker.SelectedIndex, myItems[_picker.SelectedIndex]);
        }

        public void Publish()
        {
            UI.Publish("/picker", CreateElement);
        }
    }
}
