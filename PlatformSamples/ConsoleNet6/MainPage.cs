using Microsoft.Maui;
using Microsoft.Maui.Controls;

class MainPage : ContentPage {
    public MainPage () : base () {
        Title = "Maui";
        Content = new StackLayout {
            VerticalOptions = LayoutOptions.FillAndExpand,
            Children = {
                new Label {
                    Text = "Hello Chat Room!",
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                },
                new Button {
                    Text = "Start Chat",
                    Command = new Command (() => {
                    })
                }
            }
        };
    }
} 

