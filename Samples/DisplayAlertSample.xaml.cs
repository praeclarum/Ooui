using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Samples
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DisplayAlertSample : ContentPage
	{
		public DisplayAlertSample ()
		{
			InitializeComponent ();
		}

        public async void OnButtonClicked(object sender, EventArgs args)
        {
            var result = await DisplayAlert("Alert Message", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer nec odio. Praesent libero. Sed cursus ante dapibus diam. Sed nisi. Nulla quis sem at nibh elementum imperdiet. Duis sagittis ipsum. Praesent mauris. Fusce nec tellus sed augue semper porta. Mauris massa.", "YES", "NO");
            await DisplayAlert("Alert Response", $"You selected value: {result}", "OK");
        }
	}
}
