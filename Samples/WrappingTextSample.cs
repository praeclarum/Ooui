using System;
using Xamarin.Forms;

namespace Samples
{
    public class WrappingTextSample : ISample
    {
        public string Title => "Xamarin.Forms Wrapping Text";
        public string Path => "/wrapping";

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

            var row0s = new StackLayout { Orientation = StackOrientation.Horizontal, BackgroundColor = Color.Azure };
            row0s.Children.Add (new Label { Text = shortText, LineBreakMode = LineBreakMode.WordWrap, WidthRequest = 200 });
            row0s.Children.Add (new Label { Text = mediumText, LineBreakMode = LineBreakMode.WordWrap, WidthRequest = 200 });
            row0s.Children.Add (new Label { Text = longText, LineBreakMode = LineBreakMode.WordWrap, WidthRequest = 200 });
            rows.Children.Add (new ScrollView { Content = row0s });

            var row2 = new StackLayout { Orientation = StackOrientation.Horizontal, BackgroundColor = Color.Azure };
            row2.Children.Add (new Entry { Text = shortText, FontSize = 8, VerticalOptions = LayoutOptions.Center });
            row2.Children.Add (new Entry { Text = shortText, FontSize = 16, VerticalOptions = LayoutOptions.Center });
            row2.Children.Add (new Entry { Text = shortText, FontSize = 32, VerticalOptions = LayoutOptions.Center });
            row2.Children.Add (new Button { Text = shortText, FontSize = 32, VerticalOptions = LayoutOptions.Center });
            rows.Children.Add (row2);

            var row3 = new StackLayout { Orientation = StackOrientation.Horizontal, BackgroundColor = Color.GhostWhite };
            row3.Children.Add (new Entry { Text = shortText, FontSize = 8, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold });
            row3.Children.Add (new Entry { Text = shortText, FontSize = 16, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold });
            row3.Children.Add (new Entry { Text = shortText, FontSize = 32, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold });
            row3.Children.Add (new Button { Text = shortText, FontSize = 32, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold });
            rows.Children.Add (row3);

            var row4 = new StackLayout { Orientation = StackOrientation.Horizontal, BackgroundColor = Color.Azure };
            row4.Children.Add (new Button { Text = shortText, FontSize = 8, VerticalOptions = LayoutOptions.Center });
            row4.Children.Add (new Button { Text = shortText, FontSize = 16, VerticalOptions = LayoutOptions.Center });
            row4.Children.Add (new Button { Text = shortText, FontSize = 32, VerticalOptions = LayoutOptions.Center });
            rows.Children.Add (row4);

            var row5 = new StackLayout { Orientation = StackOrientation.Horizontal, BackgroundColor = Color.GhostWhite };
            row5.Children.Add (new Button { Text = shortText, FontSize = 8, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold });
            row5.Children.Add (new Button { Text = shortText, FontSize = 16, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold });
            row5.Children.Add (new Button { Text = shortText, FontSize = 32, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold });
            rows.Children.Add (row5);

            var row6 = new StackLayout { Orientation = StackOrientation.Horizontal, BackgroundColor = Color.Azure };
            row6.Children.Add (new DatePicker { VerticalOptions = LayoutOptions.Center });
            row6.Children.Add (new TimePicker { VerticalOptions = LayoutOptions.Center });
            rows.Children.Add (row6);

            var row7 = new StackLayout { Orientation = StackOrientation.Horizontal, BackgroundColor = Color.Azure };
            row7.Children.Add (new Label { Text = shortText, FontSize = 32, LineBreakMode = LineBreakMode.WordWrap });
            row7.Children.Add (new Label { Text = mediumText, FontSize = 16, LineBreakMode = LineBreakMode.WordWrap });
            row7.Children.Add (new Label { Text = longText, FontSize = 8, LineBreakMode = LineBreakMode.WordWrap });
            rows.Children.Add (row7);

            var row8 = new StackLayout { Orientation = StackOrientation.Horizontal, BackgroundColor = Color.GhostWhite };
            row8.Children.Add (new Label { Text = shortText, FontSize = 32, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Start });
            row8.Children.Add (new Label { Text = mediumText, FontSize = 16, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.FillAndExpand });
            row8.Children.Add (new Label { Text = longText, FontSize = 8, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End });
            rows.Children.Add (row8);

            var page = new ContentPage
            {
                Content = rows
            };

            return page.GetOouiElement();
        }

        public void Publish()
        {
            Ooui.UI.Publish(Path, CreateElement);
        }

        const string shortText = "Lorem ipsum dolor sit amet.";
        const string mediumText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
        const string longText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
    }
}
