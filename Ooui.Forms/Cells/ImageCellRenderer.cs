using Ooui.Forms.Renderers;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Ooui.Forms.Cells
{
    public class ImageCellRenderer : TextCellRenderer
    {
        public override CellView GetCell(Cell item, CellView reusableView, List listView)
        {
            var nativeImageCell = reusableView as CellView ?? new CellView();

            var result = (CellView)base.GetCell(item, nativeImageCell, listView);

            var imageCell = (ImageCell)item;

            WireUpForceUpdateSizeRequested(item, result);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            SetImage(imageCell, result);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return result;
        }

        protected override async void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var tvc = (CellView)sender;
            var imageCell = (ImageCell)tvc.Cell;

            base.HandlePropertyChanged(sender, args);

            if (args.PropertyName == ImageCell.ImageSourceProperty.PropertyName)
                await SetImage(imageCell, tvc);
        }

        static async Task SetImage(ImageCell cell, CellView target)
        {
            var source = cell.ImageSource;

            target.ImageView.Source = null;

            IImageSourceHandler handler;

            if (source != null && (handler =   
                Registrar.Registered.GetHandler<Renderers.IImageSourceHandler>(source.GetType())) != null)
            {
                string image;
                try
                {
                    image = await handler.LoadImageAsync(source).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    image = null;
                }

                target.ImageView.Source = image;
            }
            else
                target.ImageView.Source = null;
        }
    }
}
