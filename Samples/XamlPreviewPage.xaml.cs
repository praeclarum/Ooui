using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Samples
{
    public partial class XamlPreviewPage : ContentPage
    {
        public XamlPreviewPage ()
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

        public void DisplayXaml ()
        {
            try {
                var asm = typeof (Xamarin.Forms.Xaml.Internals.XamlTypeResolver).Assembly;
                var xamlLoaderType = asm.GetType ("Xamarin.Forms.Xaml.XamlLoader");
                var loadArgTypes = new[] { typeof (object), typeof (string) };
                var loadMethod = xamlLoaderType.GetMethod ("Load", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, System.Reflection.CallingConventions.Any, loadArgTypes, null);
                var contentView = new ContentView ();
                loadMethod.Invoke (null, new object[] { contentView, editor.Text });
                results.Content = contentView;
            }
            catch (Exception ex) {
                results.Content = new Label {
                    Text = ex.ToString (),
                };
            }
        }
    }
}
