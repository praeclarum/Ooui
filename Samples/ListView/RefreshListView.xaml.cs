using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;

namespace Samples
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RefreshListView : ContentPage
	{
		public RefreshListView ()
		{
			InitializeComponent ();

            BindingContext = new RefreshListViewModel ();
        }

        void Handle_Clicked (object sender, System.EventArgs e)
        {
            string item = ((RefreshListViewModel)BindingContext).Data.LastOrDefault ();
            list.ScrollTo (item, ScrollToPosition.End, true);
        }
	}
}
