using Microsoft.Maui;
using Microsoft.Maui.Controls;

class MainPage : ContentPage {

    VerticalStackLayout layout = new VerticalStackLayout();

    int counter = 0;

    public MainPage () : base () {
        Title = "Maui";

        var label = new Label { Text = "Hello Chat Room!!!!" };
        layout.Add(label);
        layout.Add(new Button {
                    Text = "Increase Counter",
                    Command = new Command (() => {
                        counter++;
                        label.Text = "Count = " + counter + "...";
                    })
                });
        Content = layout;
    }
} 

