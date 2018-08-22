using System;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Samples
{
    public class XamlPreviewPageSample : ISample
    {
        public string Title => "Xamarin.Forms XAML Editor";

        public Ooui.Html.Element CreateElement ()
        {
            var page = new XamlEditorPage ();
            return page.GetOouiElement ();
        }
    }

    public partial class XamlEditorPage : ContentPage
    {
        Editor editor;
        ContentView results;

        public XamlEditorPage ()
        {
            InitializeComponent ();

            editor.Text = @"<ContentView
  xmlns=""http://xamarin.com/schemas/2014/forms""
  xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml"">
<Grid>
  <Grid.RowDefinitions>
    <RowDefinition Height=""*"" />
    <RowDefinition Height=""*"" />
  </Grid.RowDefinitions>
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width=""*"" />
    <ColumnDefinition Width=""*"" />
  </Grid.ColumnDefinitions>
  <StackLayout Grid.Row=""0"" Grid.Column=""0"">
    <Label Text=""Top Left"" />
    <Entry Placeholder=""I'm ready for some text"" />
    <Button Text=""I'm a button, but I don't do anything"" />
  </StackLayout>
  <Label Text=""Top Right"" Grid.Row=""0"" Grid.Column=""1"" TextColor=""White"" BackgroundColor=""#c5000b"" />
  <Label Text=""Bottom Left"" Grid.Row=""1"" Grid.Column=""0"" TextColor=""Black"" BackgroundColor=""#ffd320"" />
  <Label Text=""Bottom Right"" Grid.Row=""1"" Grid.Column=""1"" TextColor=""White"" BackgroundColor=""#008000"" />
</Grid>
</ContentView>";
            editor.TextChanged += (sender, e) => DisplayXaml ();
            DisplayXaml ();
        }

        void InitializeComponent ()
        {
            var grid = new Grid ();

            grid.RowDefinitions.Add (new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add (new RowDefinition { Height = GridLength.Star });
            grid.ColumnDefinitions.Add (new ColumnDefinition { Width = GridLength.Star });
            grid.ColumnDefinitions.Add (new ColumnDefinition { Width = GridLength.Star });

            editor = new Editor {
                FontSize = 12,
                FontFamily = "monospace",
            };
            editor.SetValue (Grid.ColumnProperty, 0);
            editor.SetValue (Grid.RowProperty, 1);

            results = new ContentView ();
            results.SetValue (Grid.ColumnProperty, 1);
            results.SetValue (Grid.RowProperty, 1);

            var title = new Label {
                Text = "XAML Editor",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness (8),
            };
            title.SetValue (Grid.ColumnProperty, 0);
            title.SetValue (Grid.RowProperty, 0);

            grid.Children.Add (title);
            grid.Children.Add (editor);
            grid.Children.Add (results);

            Content = grid;
        }

        CancellationTokenSource lastCts = null;

        public void DisplayXaml ()
        {
            try {
                var cts = new CancellationTokenSource ();
                var token = cts.Token;
                lastCts?.Cancel ();
                lastCts = cts;

                var contentView = new ContentView ();
                contentView.LoadFromXaml (editor.Text);

                if (!token.IsCancellationRequested) {
                    results.Content = contentView;
                }
            }
            catch (OperationCanceledException) {
            }
            catch (Exception ex) {
                results.Content = new Label {
                    TextColor = Color.DarkRed,
                    FontSize = 12,
                    Text = ex.ToString (),
                };
            }
        }
    }
}
