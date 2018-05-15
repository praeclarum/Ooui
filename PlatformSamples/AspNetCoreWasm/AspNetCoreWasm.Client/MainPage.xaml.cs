using OouiWXF.ViewModels.MainPage;

using System;

using Xamarin.Forms;

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

