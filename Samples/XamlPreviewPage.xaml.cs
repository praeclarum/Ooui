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
  <Label Text=""Top Left"" Grid.Row=""0"" Grid.Column=""0"" />
  <Label Text=""Top Right"" Grid.Row=""0"" Grid.Column=""1"" />
  <Label Text=""Bottom Left"" Grid.Row=""1"" Grid.Column=""0"" />
  <Label Text=""Bottom Right"" Grid.Row=""1"" Grid.Column=""1"" />
</Grid>
</ContentView>";
            DisplayXaml ();
        }

        public void DisplayXaml ()
        {
            var asm = typeof (Xamarin.Forms.Xaml.Internals.XamlTypeResolver).Assembly;
            var xamlLoaderType = asm.GetType ("Xamarin.Forms.Xaml.XamlLoader");
            var loadArgTypes = new[] { typeof (object), typeof (string) };
            var loadMethod = xamlLoaderType.GetMethod ("Load", System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public, null, System.Reflection.CallingConventions.Any, loadArgTypes, null);
            var contentView = new ContentView ();
            loadMethod.Invoke (null, new object[] { contentView, editor.Text });
            results.Content = contentView;
        }
    }
}
