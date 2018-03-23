using Ooui;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Samples
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DisplayAlertPage : ContentPage
	{
		public DisplayAlertPage ()
		{
			InitializeComponent ();
		}

        protected override void OnAppearing ()
        {
            base.OnAppearing ();

            status.Text = "Page appeared";
        }

        public async void OnButtonClicked(object sender, EventArgs args)
        {
            activity.IsRunning = true;
            progress.Progress = 0.5;
            var result = await DisplayAlert($"Alert @ {datePicker.Date}", "This is a test of the dialog. Is it working?", "YES", "NO");
            await DisplayAlert("Alert Response", $"You selected value: {result}", "OK");
            activity.IsRunning = false;
            progress.Progress = 1.0;
        }
    }
}
