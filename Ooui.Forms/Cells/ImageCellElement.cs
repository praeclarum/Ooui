using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Ooui.Forms.Extensions;
using Ooui.Forms.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Ooui.Forms.Cells
{
    public class ImageCellElement : CellElement
    {
        public Ooui.Html.Image ImageView { get; } = new Ooui.Html.Image ();

        public Ooui.Html.Label TextLabel { get; } = new Ooui.Html.Label ();

        public Ooui.Html.Label DetailTextLabel { get; } = new Ooui.Html.Label ();

        public ImageCellElement ()
        {
            AppendChild (ImageView);
            AppendChild (TextLabel);
            AppendChild (DetailTextLabel);
        }

        protected override void BindCell ()
        {
            Cell.PropertyChanged += Cell_PropertyChanged;

            if (Cell is ImageCell cell) {
                TextLabel.Text = cell.Text ?? string.Empty;
                DetailTextLabel.Text = cell.Detail ?? string.Empty;
                TextLabel.Style.Color = cell.TextColor.ToOouiColor (OouiTheme.TextColor);
                DetailTextLabel.Style.Color = cell.DetailColor.ToOouiColor (OouiTheme.SecondaryTextColor);
            }

            base.BindCell ();
        }

        protected override void UnbindCell ()
        {
            Cell.PropertyChanged -= Cell_PropertyChanged;

            base.UnbindCell ();
        }

        async void Cell_PropertyChanged (object sender, PropertyChangedEventArgs args)
        {
            if (!(Cell is ImageCell cell))
                return;

            if (args.PropertyName == TextCell.TextProperty.PropertyName)
                TextLabel.Text = cell.Text ?? string.Empty;
            else if (args.PropertyName == TextCell.DetailProperty.PropertyName)
                DetailTextLabel.Text = cell.Detail ?? string.Empty;
            else if (args.PropertyName == TextCell.TextColorProperty.PropertyName)
                TextLabel.Style.Color = cell.TextColor.ToOouiColor (OouiTheme.TextColor);
            else if (args.PropertyName == TextCell.DetailColorProperty.PropertyName)
                DetailTextLabel.Style.Color = cell.DetailColor.ToOouiColor (OouiTheme.SecondaryTextColor);
            else if (args.PropertyName == ImageCell.ImageSourceProperty.PropertyName)
                await SetImage (cell.ImageSource).ConfigureAwait (false);
        }

        async Task SetImage (ImageSource source)
        {
            ImageView.Source = null;

            IImageSourceHandler handler;

            if (source != null && (handler = Registrar.Registered.GetHandlerForObject<Renderers.IImageSourceHandler> (source)) != null) {
                string image;
                try {
                    image = await handler.LoadImageAsync (source).ConfigureAwait (false);
                }
                catch (TaskCanceledException) {
                    image = null;
                }
                ImageView.Source = image;
            }
            else {
                ImageView.Source = null;
            }
        }
    }
}
