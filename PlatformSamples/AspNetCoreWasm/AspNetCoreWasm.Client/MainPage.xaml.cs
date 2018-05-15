using OouiWXF.ViewModels.MainPage;
using Xamarin.Forms;

//
//  2018-05-01  Mark Stega
//              Created
//

namespace OouiWXF
{
    public partial class MainPage : ContentPage
    {
        static public MainPageViewModel pMainPageViewModel;

        public MainPage()
        {
            InitializeComponent();

            pMainPageViewModel = (MainPageViewModel)this.BindingContext;
        }

    }
}

