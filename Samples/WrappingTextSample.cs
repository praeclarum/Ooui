using System;
using Xamarin.Forms;

namespace Samples
{
    public class WrappingTextSample : ISample
    {
        public string Title => "Xamarin.Forms Wrapping Text";

        public Ooui.Element CreateElement()
        {
            var rows = new StackLayout { Orientation = StackOrientation.Vertical };

            var row0 = new StackLayout { Orientation = StackOrientation.Horizontal, BackgroundColor = Color.Azure };
            row0.Children.Add (new Label { Text = shortText, LineBreakMode = LineBreakMode.WordWrap });
            row0.Children.Add (new Label { Text = mediumText, LineBreakMode = LineBreakMode.WordWrap });
            row0.Children.Add (new Label { Text = longText, LineBreakMode = LineBreakMode.WordWrap });
            rows.Children.Add (row0);

            var row1 = new StackLayout { Orientation = StackOrientation.Horizontal, BackgroundColor = Color.GhostWhite };
            row1.Children.Add (new Label { Text = shortText, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Start });
            row1.Children.Add (new Label { Text = mediumText, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.FillAndExpand });
            row1.Children.Add (new Label { Text = longText, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End });
            rows.Children.Add (row1);

            var page = new ContentPage
            {
                Content = rows
            };

            return page.GetOouiElement();
        }

        public void Publish()
        {
            Ooui.UI.Publish("/wrapping", CreateElement);
        }

        const string shortText = "Lorem ipsum dolor sit amet.";
        const string mediumText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
        const string longText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
    }
}
