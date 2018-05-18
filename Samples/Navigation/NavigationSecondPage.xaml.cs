using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Samples.Navigation
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NavigationSecondPage : ContentPage
	{
		public NavigationSecondPage ()
		{
			InitializeComponent ();
		}

        private void Button_Clicked(object sender, EventArgs e)
        {
            (this.Parent as NavigationPage).PushAsync(new Navigation.NavigationThirdPage());
        }

    }
}
