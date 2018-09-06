using Ooui;
using System;
using Xamarin.Forms;

namespace Samples
{
    public class SliderSample : ISample
    {
        private Xamarin.Forms.Label _label;

        public string Title => "Xamarin.Forms Slider Sample";
        public string Path => "/slider";

        public Ooui.Element CreateElement()
        {
            var panel = new StackLayout();

            var titleLabel = new Xamarin.Forms.Label
            {
                Text = "Slider",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold
            };
            panel.Children.Add(titleLabel);

            Slider slider = new Slider
            {
                Minimum = 0,
                Maximum = 100
            };
            panel.Children.Add(slider);

            slider.ValueChanged += OnSliderValueChanged;

            _label = new Xamarin.Forms.Label
            {
                Text = "Slider value is 0",
                HorizontalOptions = LayoutOptions.Center
            };
            panel.Children.Add(_label);

            var page = new ContentPage
            {
                Content = panel
            };

            return page.GetOouiElement();
        }

        void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            _label.Text = String.Format("Slider value is {0:F1}", e.NewValue);
        }

        public void Publish()
        {
            UI.Publish(Path, CreateElement);
        }
    }
}
