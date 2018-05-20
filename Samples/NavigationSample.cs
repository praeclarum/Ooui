﻿
using Ooui;
using Xamarin.Forms;

namespace Samples.Navigation
{
    public class NavigationSample : ISample
    {
        public string Title => "Xamarin.Forms Navigation XAML";

        public Ooui.Element CreateElement()
        {
            var page = new Navigation.NavigationFirstPage();
            var root = new NavigationPage(page);
            return root.GetOouiElement();
        }
    }
}
