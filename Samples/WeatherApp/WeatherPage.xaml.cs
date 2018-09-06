using System;
using Xamarin.Forms;

namespace WeatherApp
{
    public partial class WeatherPage : ContentPage
    {
        public WeatherPage()
        {
            InitializeComponent();
            this.Title = "Sample Weather App";
            getWeatherBtn.Clicked += GetWeatherBtn_Clicked;

            //Set the default binding to a default object for now
            this.BindingContext = Weather;
            unitOfMeasure.ItemsSource = Weather.UnitOfMeasures;
        }

        public Weather Weather { get; } = new Weather();

        private async void GetWeatherBtn_Clicked(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(zipCodeEntry.Text))
            {
                
                Weather weather = await Core.GetWeather(zipCodeEntry.Text, (string)unitOfMeasure.SelectedItem);
                if (weather != null)
                {
                    this.BindingContext = weather;
                    getWeatherBtn.Text = "Search Again";
                }
            }
        }
    }
}
