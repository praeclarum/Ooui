using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
	}
}
