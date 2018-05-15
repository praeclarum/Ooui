using System;
using Ooui;
using Xamarin.Forms;

namespace OouiWXF
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize Xamarin.Forms
            Forms.Init();

            // Create the UI
            var page = new MainPage();


            // Add some logic to it
            //var count = 0;
            //button.Clicked += (s, e) => {
              //  count++;
                //button.Text = $"Clicked {count} times";
            //};

            // Publish a root element to be displayed
            UI.Publish("/", page.GetOouiElement());
        }
    }
}